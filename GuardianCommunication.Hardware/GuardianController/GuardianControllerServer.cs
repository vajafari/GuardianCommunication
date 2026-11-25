using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Hardware.GuardianController.Model;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;
using Grpc.Core;
using Guardian.Controller;
using Timer = System.Timers.Timer;

namespace GuardianCommunication.Hardware.GuardianController
{
    /// <summary>
    /// سرور gRPC بوردهای کنترلر گاردین؛ بورد کلاینت است و با یک استریم دوطرفه وصل می‌شود.
    /// نام سرویس روی سیم guardian.controller.GuardianControllerGrpcService است و فریمور برد گاردین باید همین را داشته باشد.
    /// </summary>
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    public class GuardianControllerServer : GuardianControllerGrpcService.GuardianControllerGrpcServiceBase, IDisposable
    {
        private GuardianControllerServerConfig _serverConfig;
        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;
        private Func<DtoServerMatchData, DtoServerMatchResult> _serverMatchProcessor;

        private Server _grpcServer;

        // دستگاه‌هایی که اجازه‌ی اتصال دارند (پیکربندی، نه اتصال زنده) به کلید شماره سریال
        private readonly ConcurrentDictionary<string, DtoDevice> _onlineMonitoringModeDevices
            = new ConcurrentDictionary<string, DtoDevice>(StringComparer.OrdinalIgnoreCase);

        // اتصال‌های زنده‌ی فعلی به کلید شماره سریال
        private readonly ConcurrentDictionary<string, GuardianControllerAgent> _onlineMonitoringAgents
            = new ConcurrentDictionary<string, GuardianControllerAgent>(StringComparer.OrdinalIgnoreCase);

        // Timer دوره‌ای: هر بار یک Fetch دسته‌ای برای بوردهای «آزاد و متصل» و سپس Dispatch مستقل هر دستور.
        // gate با Wait(0) تضمین می‌کند دو چرخه‌ی Dispatch هم‌پوشانی نکنند (هم‌سان با TimyServer).
        private Timer _grpcDispatchTimer;
        private readonly SemaphoreSlim _grpcDispatchGate = new SemaphoreSlim(1, 1);


        #region Singleton

        public static GuardianControllerServer Instance { get; }

        private GuardianControllerServer()
        {
        }

        static GuardianControllerServer()
        {
            Instance = new GuardianControllerServer();
        }

        #endregion


        public void StartGuardianControllerServer(
            GuardianControllerServerConfig serverConfig
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , Func<DtoServerMatchData, DtoServerMatchResult> serverMatchProcessor
            , List<DtoDevice> deviceInfos)
        {
            _actionToGetCommands = actionToGetCommands;
            _serverMatchProcessor = serverMatchProcessor;
            _serverConfig = serverConfig;
            LoggingSystem.LogInfo("GuardianControllerServer Starting", serverConfig);

            var thread = new Thread(() => DoStartServerProcess(deviceInfos)) { IsBackground = true };
            thread.Start();
        }

        public void StopGuardianControllerServer()
        {
            Dispose(true);
        }

        /// <summary>
        /// شناسه بوردهایی که الان اتصال زنده دارند
        /// </summary>
        public List<Guid> GetConnectedDeviceIds()
        {
            return _onlineMonitoringAgents.Values
                .Select(a => a.DeviceInfo.Id)
                .ToList();
        }

        private void DoStartServerProcess(List<DtoDevice> deviceInfos)
        {
            SetDeviceList(deviceInfos);
            var threadOnlineMonitoring = new Thread(StartOnlineMonitoringServer) { IsBackground = true };
            threadOnlineMonitoring.Start();
        }


        #region GRPC SERVER Online Monitoring

        /// <summary>
        /// بوردهای مجاز به اتصال؛ اتصال بوردی که دیگر در فهرست نیست قطع می‌شود و مشخصات بقیه به‌روز می‌شود (هم‌سان با TimyServer.SetDeviceList)
        /// </summary>
        public void SetDeviceList(List<DtoDevice> deviceInfos)
        {
            try
            {
                var guardianControllerDevices = deviceInfos
                    .Where(d => d.ProducerNumber == ProducerEnumeration.Guardian
                                && d.SdkVersion == SdkVersionEnumeration.SdkVersion1
                                && d.ConnectionMode == DeviceConnectionModeEnumeration.Push
                                && d.SerialNumber.IsNotNullOrEmpty())
                    .ToList();

                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ServerSetDeviceList))
                {
                    LoggingSystem.LogInfo("Guardian controller server set device list", guardianControllerDevices);
                }

                foreach (var agent in _onlineMonitoringAgents.Values.ToList())
                {
                    var currentDeviceInfo = guardianControllerDevices.FirstOrDefault(d => d.Id == agent.DeviceInfo.Id);
                    if (currentDeviceInfo == null)
                    {
                        RemoveAgent(agent.DeviceInfo.SerialNumber);
                    }
                    else
                    {
                        // مشخصات جدید بورد را برای Agent هایی که تا کنون وصل شده‌اند ست می‌کنیم
                        agent.DeviceInfo = currentDeviceInfo;
                    }
                }

                _onlineMonitoringModeDevices.Clear();
                foreach (var deviceInfo in guardianControllerDevices)
                {
                    _onlineMonitoringModeDevices.TryAdd(deviceInfo.SerialNumber, deviceInfo);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "GuardianControllerServer Error on SetDeviceList");
            }
        }

        private void StartOnlineMonitoringServer()
        {
            try
            {
                _grpcServer = new Server { Services = { GuardianControllerGrpcService.BindService(this) } };
                _grpcServer.Ports.Add(new ServerPort("0.0.0.0", _serverConfig.GrpcServerPort, ServerCredentials.Insecure));
                _grpcServer.Start();
                LoggingSystem.LogInfo("GuardianControllerServer gRPC server started", _serverConfig.GrpcServerPort);

                // به‌جای یک Thread دائمی با while(true)، از یک Timer + gate استفاده می‌کنیم (هم‌سان با TimyServer)
                var dispatchInterval = Math.Max(100, _serverConfig.GrpcCommandTimerIntervalInMillisecond);
                _grpcDispatchTimer = new Timer(dispatchInterval);
                _grpcDispatchTimer.Elapsed += GrpcDispatchTimerOnElapsed;
                _grpcDispatchTimer.Start();
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "GuardianControllerServer Error on StartOnlineMonitoringServer");
            }
        }

        private void StopOnlineMonitoringServer()
        {
            if (_grpcDispatchTimer != null)
            {
                _grpcDispatchTimer.Stop();
                _grpcDispatchTimer.Elapsed -= GrpcDispatchTimerOnElapsed;
                _grpcDispatchTimer.Dispose();
                _grpcDispatchTimer = null;
            }

            foreach (var agent in _onlineMonitoringAgents.Values.ToList())
            {
                agent.Dispose();
            }
            _onlineMonitoringAgents.Clear();

            if (_grpcServer != null)
            {
                _grpcServer.ShutdownAsync().Wait(10000);
            }
        }

        // ───────────────────────────────────────────────
        //  Timer دوره‌ای: یک Fetch برای همه‌ی بوردهای آزاد، سپس Dispatch مستقل
        // ───────────────────────────────────────────────

        private void GrpcDispatchTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGetCommandTimersElapsed))
            {
                LoggingSystem.LogInfo("GuardianControllerServer GrpcDispatchTimerOnElapsed is calling");
            }

            // اگر چرخه‌ی قبلی هنوز در حال اجراست، این تیک را نادیده می‌گیریم (بدون بلاک‌شدن)
            if (!_grpcDispatchGate.Wait(0))
            {
                return;
            }
            _ = RunGrpcDispatchCycleAsync();
        }

        private async Task RunGrpcDispatchCycleAsync()
        {
            try
            {
                await DispatchCommandsToOnlineMonitoringDevicesAsync().ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on GuardianControllerServer.RunGrpcDispatchCycleAsync");
            }
            finally
            {
                _grpcDispatchGate.Release();
            }
        }

        /// <summary>
        /// فقط یک بار به دیتابیس مراجعه می‌کند: برای تمام بوردهای متصل و آزاد (IsIdle)،
        /// دستورات بدون‌ارسال را در یک Query واحد می‌خواند و برای هر بورد یک دستور را
        /// به‌صورت fire-and-forget ارسال می‌کند تا بوردهای سریع منتظر بوردهای کند نمانند.
        /// </summary>
        private Task DispatchCommandsToOnlineMonitoringDevicesAsync()
        {
            // فقط بوردهایی که هم وصل‌اند هم آزادند وارد Query می‌شوند
            var idleAgents = _onlineMonitoringAgents.Values
                .Where(a => a.IsIdle && a.DeviceInfo.SerialNumber.IsNotNullOrEmpty())
                .GroupBy(a => a.DeviceInfo.SerialNumber, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            if (idleAgents.Count == 0)
            {
                return Task.CompletedTask;
            }

            var commandFetchParams = new DeviceNotSentCommandsFilter
            {
                Count = _serverConfig.GrpcServerCommandCount,
                DeviceSerialNumbers = idleAgents.Keys.ToList(),
                Producer = ProducerEnumeration.Guardian,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
            };

            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerCommandFetch))
            {
                LoggingSystem.LogInfo("Guardian controller server command fetch params", commandFetchParams);
            }

            List<DtoDeviceUnsentCommand> allCommands;
            try
            {
                allCommands = _actionToGetCommands(commandFetchParams);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error fetching commands batch from database");
                return Task.CompletedTask;
            }

            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerCommandFetchResult))
            {
                LoggingSystem.LogInfo("Guardian controller server command fetch result"
                    , allCommands?.Select(c => new { c.NumericId, c.DeviceId, c.CommandType }));
            }

            if (allCommands.IsCollectionNullOrEmpty())
            {
                return Task.CompletedTask;
            }

            // در هر چرخه فقط یک دستور برای هر بورد ارسال می‌شود (single in-flight per device)
            var dispatchedSerials = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var command in allCommands)
            {
                if (!idleAgents.TryGetValue(command.DeviceSerialNumber, out var agent))
                {
                    continue;
                }
                if (!dispatchedSerials.Add(command.DeviceSerialNumber))
                {
                    continue;
                }
                _ = SendCommandToDeviceAsync(agent, command);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// ارسال یک دستور به یک برد و ثبت نتیجه در دیتابیس:
        ///   موفق → CommandResponseReceived(SUCCESS)
        ///   خطای واقعی (نه Timeout و نه LineIsBusy) → CommandSentToDevice + CommandDescriptionReceived
        ///   Timeout یا LineIsBusy → هیچ (دستور ارسال‌نشده می‌ماند تا دور بعد دوباره تلاش شود)
        /// </summary>
        private async Task SendCommandToDeviceAsync(GuardianControllerAgent agent, DtoDeviceUnsentCommand command)
        {
            try
            {
                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerCommandBeforeSend))
                {
                    LoggingSystem.LogInfo("Guardian controller server command before send",
                        new { command.NumericId, command.DeviceId, command.CommandType });
                }

                var timeoutInMillisecond = GetCommandTimeoutInMillisecond(command);
                var resultOfSendCommand = await agent.SendCommandAsync(command, timeoutInMillisecond).ConfigureAwait(false);

                if (resultOfSendCommand.ErrorCode == GuardianControllerErrorEnumeration.Success)
                {
                    HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                        new DtoDeviceCommandProcessingResult
                        {
                            CommandResponseResult = "SUCCESS",
                            CommandResponseTime = DateTime.UtcNow,
                            NumericId = command.NumericId
                        });
                }
                else if (resultOfSendCommand.ErrorCode != GuardianControllerErrorEnumeration.MessageTimeout
                         && resultOfSendCommand.ErrorCode != GuardianControllerErrorEnumeration.LineIsBusy)
                {
                    HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<long> { command.NumericId });
                    HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(
                        new DtoDeviceCommandProcessingDescription
                        {
                            Description = $"Error code is {resultOfSendCommand.ErrorCode} and description is {resultOfSendCommand.Message}",
                            NumericId = command.NumericId,
                        });
                }
            }
            catch (Exception exp)
            {
                HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<long> { command.NumericId });
                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(
                    new DtoDeviceCommandProcessingDescription
                    {
                        Description = $"Unknown Error. Message is {exp.GetFullExceptionMessage()}",
                        NumericId = command.NumericId,
                    });
                LoggingSystem.LogError(exp, "Error on GuardianControllerServer.SendCommandToDeviceAsync");
            }
        }

        private int GetCommandTimeoutInMillisecond(DtoDeviceUnsentCommand command)
        {
            switch (command.CommandType)
            {
                case DeviceCommandTypeEnumeration.SetUserInfo:
                case DeviceCommandTypeEnumeration.DeleteUser:
                case DeviceCommandTypeEnumeration.GuardianControllerSetCalendar:
                case DeviceCommandTypeEnumeration.GuardianControllerSetCalendarTemplate:
                case DeviceCommandTypeEnumeration.GuardianControllerSetHardwareConfig:
                case DeviceCommandTypeEnumeration.GuardianControllerKeepOnly:
                    return _serverConfig.GrpcServerTimeoutLongInMillisecond;
                case DeviceCommandTypeEnumeration.ClearData:
                case DeviceCommandTypeEnumeration.ClearUser:
                    return _serverConfig.GrpcServerTimeoutVeryLongInMillisecond;
                default:
                    return _serverConfig.GrpcServerTimeoutShortInMillisecond;
            }
        }

        public override async Task Stream(
            IAsyncStreamReader<GuardianControllerGrpcMessage> requestStream
            , IServerStreamWriter<GuardianControllerGrpcMessage> responseStream
            , ServerCallContext context)
        {
            // 1. Handshake: خواندن سریال از اولین پیام
            if (!await requestStream.MoveNext(context.CancellationToken).ConfigureAwait(false))
            {
                context.Status = new Status(StatusCode.InvalidArgument, "Handshake missing."); return;
            }

            var firstMsg = requestStream.Current;
            var deviceSerialNumber = firstMsg.DeviceSerialNumber;
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerConnect))
            {
                LoggingSystem.LogInfo("Guardian controller grpc server device connect", new
                {
                    DeviceSerialNumber = deviceSerialNumber,
                    context.Peer,
                });
            }

            if (string.IsNullOrWhiteSpace(deviceSerialNumber) ||
                !_onlineMonitoringModeDevices.TryGetValue(deviceSerialNumber, out var deviceInfo))
            {
                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerConnect))
                {
                    LoggingSystem.LogInfo("Guardian controller grpc server device is unauthorized", new
                    {
                        DeviceSerialNumber = deviceSerialNumber,
                        context.Peer,
                    });
                }
                context.Status = new Status(StatusCode.PermissionDenied, "Unauthorized device."); return;
            }

            // 2. اگر همین برد قبلاً اتصالی داشته، اتصال قدیمی را کامل ببند (reconnect → state تازه)
            RemoveAgent(deviceSerialNumber);

            // 3. ساخت Agent جدید و ثبت آن *قبل* از شروع پردازش
            var newAgent = new GuardianControllerAgent(deviceInfo, responseStream, requestStream, context.CancellationToken, _serverMatchProcessor);
            _onlineMonitoringAgents[deviceSerialNumber] = newAgent;
            HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
            {
                new DtoDeviceConnectionStatus
                {
                    DeviceId = deviceInfo.Id,
                    IsConnected = true
                }
            });

            try
            {
                // 4. واگذاری کامل کنترل ارتباط به Agent؛ این متد تا زمان قطع اتصال برد بلوک می‌ماند.
                // firstMsg از استریم مصرف شده و دیگر با MoveNext در دسترس نیست، پس صریحاً به Agent داده می‌شود تا Route شود
                await newAgent.StartProcessingAsync(firstMsg).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on StartProcessingAsync of guardian controller agent", new { deviceInfo.Id, deviceInfo.SerialNumber });
            }
            finally
            {
                // فقط اگر همچنان همین Agent ثبت است پاک می‌شود (اگر reconnect جای آن را گرفته، دست نمی‌خورد)
                RemoveAgent(deviceSerialNumber, newAgent);
            }
        }

        #endregion


        #region Utilities

        private void RemoveAgent(string deviceSerialNumber)
        {
            if (_onlineMonitoringAgents.TryRemove(deviceSerialNumber, out var oldAgent))
            {
                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcServerConnect))
                {
                    LoggingSystem.LogInfo("Guardian controller grpc server connection exists and must be disposed", new
                    {
                        DeviceSerialNumber = deviceSerialNumber
                    });
                }
                oldAgent.Dispose();
                PublishDisconnected(oldAgent);
            }
        }

        // حذف اتمیک: فقط وقتی همان نمونه‌ی موردانتظار هنوز ثبت است (جلوگیری از حذف اتصال جدیدِ reconnect)
        private void RemoveAgent(string deviceSerialNumber, GuardianControllerAgent expectedAgent)
        {
            if (((ICollection<KeyValuePair<string, GuardianControllerAgent>>)_onlineMonitoringAgents)
                .Remove(new KeyValuePair<string, GuardianControllerAgent>(deviceSerialNumber, expectedAgent)))
            {
                expectedAgent.Dispose();
                PublishDisconnected(expectedAgent);
            }
        }

        private static void PublishDisconnected(GuardianControllerAgent agent)
        {
            HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
            {
                new DtoDeviceConnectionStatus
                {
                    DeviceId = agent.DeviceInfo.Id,
                    IsConnected = false
                }
            });
        }

        #endregion


        #region Implementation of IDisposable

        private bool _disposed;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            StopOnlineMonitoringServer();
            _grpcDispatchGate.Dispose();
            _disposed = true;
        }

        ~GuardianControllerServer()
        {
            Dispose(false);
        }

        #endregion
    }
}
