using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Grpc.Core;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.PadisController.Definition;
using GuardianCommunication.Hardware.PadisController.Model;
using GuardianCommunication.Hardware.Shared;
using Padis.Controller;
using Timer = System.Timers.Timer;

namespace GuardianCommunication.Hardware.PadisController
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    public class PadisControllerServer : PadisControllerGrpcService.PadisControllerGrpcServiceBase, IDisposable
    {
        private PadisControllerServerConfig _serverConfig;
        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;
        private Func<DeviceNotSentCommandsFilter, List<DtoUnsentCommandCountByDeviceSerialNumber>> _actionToGetCommandsCountByDeviceSerialNumber;
        private Func<DtoServerMatchData, DtoServerMatchResult> _serverMatchProcessor;
        private Timer _getCommandTimers;
        private readonly List<DtoUnsentCommandCountByDeviceSerialNumber> _deviceCurrentUnsentCommandsCountByDeviceSerialNumber = 
            new List<DtoUnsentCommandCountByDeviceSerialNumber>();


        #region Singleton

        public static PadisControllerServer Instance { get; }

        private PadisControllerServer()
        {
        }

        static PadisControllerServer()
        {
            Instance = new PadisControllerServer();
        }

        #endregion


        public void StartPadisControllerServer(
            PadisControllerServerConfig serverConfig
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , Func<DeviceNotSentCommandsFilter, List<DtoUnsentCommandCountByDeviceSerialNumber>> actionToGetCommandsCountByDeviceSerialNumber
            , Func<DtoServerMatchData, DtoServerMatchResult> serverMatchProcessor
            , List<DtoCommunicationDeviceData> deviceInfos)
        {
            _actionToGetCommands = actionToGetCommands;
            _actionToGetCommandsCountByDeviceSerialNumber = actionToGetCommandsCountByDeviceSerialNumber;
            _serverMatchProcessor = serverMatchProcessor;
            LoggingSystem.LogInfo("PadisControllerServer Starting", new
            {
                PushServerConfig = serverConfig
            });
            _serverConfig = serverConfig;
            _getCommandTimers = new Timer(_serverConfig.GetCommandTimerIntervalInMillisecond);
            _getCommandTimers.Elapsed += GetCommandTimersOnElapsed;
            _getCommandTimers.Start();

            var thread = new Thread(() => DoStartServerProcess(deviceInfos)) { IsBackground = true };
            thread.Start();
        }

        public void StopPadisControllerServer()
        {
            Dispose(true);
        }

        public List<int> GetConnectedDeviceNumbers()
        {
            var deviceNumbers = new List<int>();
            deviceNumbers.AddRange(_pushDevicesConnectionInfo.Values
                .Where(d => Math.Abs(d.ConnectionDateTime.Subtract(DateTime.Now).TotalSeconds) <=
                            _serverConfig.PushServerIntervalForConsiderDeviceOnlineInSecond)
                .Select(d => d.DeviceNumber));
            deviceNumbers.AddRange(_onlineMonitoringAgents.Select(d => d.Value.DeviceInfo.DeviceNumber).ToList());
            return deviceNumbers;
        }

        private void DoStartServerProcess(List<DtoCommunicationDeviceData> deviceInfos)
        {
            //SetDeviceOnPushMode(deviceInfos);
            //var threadPush = new Thread(StartPushServer) { IsBackground = true };
            //threadPush.Start();

            SetDeviceOnSocketCommunicationMode(deviceInfos);
            var threadOnlineMonitoring = new Thread(StartOnlineMonitoringServer) { IsBackground = true };
            threadOnlineMonitoring.Start();

        }

        private readonly object _lockGetCommandObject = new object();
        private void GetCommandTimersOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGetCommandTimersElapsed))
            {
                LoggingSystem.LogInfo("GetCommandTimersOnElapsed Process is calling");
            }

            if (!Monitor.TryEnter(_lockGetCommandObject))
            {
                return;
            }

            try
            {
                var deviceSerialNumbers = _pushDevices.Where(d => d.SerialNumber.IsNotNullOrEmpty())
                    .Select(d => d.SerialNumber).ToList();
                var commandCountInfo =
                    _actionToGetCommandsCountByDeviceSerialNumber(new DeviceNotSentCommandsFilter
                    {
                        Producer = ProducerEnumeration.Padis,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        DeviceSerialNumbers = deviceSerialNumbers,
                    });
                //_commandCountDictionary.Clear();
                lock (_deviceCurrentUnsentCommandsCountByDeviceSerialNumber)
                {
                    _deviceCurrentUnsentCommandsCountByDeviceSerialNumber.Clear();
                    _deviceCurrentUnsentCommandsCountByDeviceSerialNumber.AddRange(commandCountInfo);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on PadisControllerServer.GetCommandTimersOnElapsed");
            }
            finally
            {
                Monitor.Exit(_lockGetCommandObject);
            }
        }


        #region PUSH SERVER

        private HttpListener _listener;
        private CancellationTokenSource _cts;
        private readonly List<DtoCommunicationDeviceData> _pushDevices = new List<DtoCommunicationDeviceData>();
        private readonly ConcurrentDictionary<int, PushServerDeviceConnectionInfo> _pushDevicesConnectionInfo =
            new ConcurrentDictionary<int, PushServerDeviceConnectionInfo>();

        public void SetDeviceOnPushMode(List<DtoCommunicationDeviceData> deviceInfos)
        {
            if (deviceInfos.IsCollectionNullOrEmpty())
            {
                return;
            }

            var pushDeviceInfos = deviceInfos.Where
            (row => row.ProducerEnum == ProducerEnumeration.Padis
                    && row.ConnectionMode == DeviceConnectionModeEnumeration.Push
                    && !row.OnlineMonitoringMode).ToList();
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ServerSetDeviceList))
            {
                LoggingSystem.LogInfo("PadisServer Set push device list", pushDeviceInfos);
            }

            lock (_pushDevices)
            {
                try
                {
                    _pushDevices.Clear();
                    _pushDevices.AddRange(pushDeviceInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp
                        , "PadisServer Error on SetDeviceOnPushModeList");
                }
            }

        }

        public void StartPushServer()
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("HttpListener is not supported on this OS.");

            _listener = new HttpListener();
            _listener.Prefixes.Add(_serverConfig.PushServerPushAddress);
            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => PushServerListenLoop(_cts.Token));
        }

        private async Task PushServerListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();

                    // هر درخواست در ترد خودش هندل میشه
                    _ = Task.Run(() => PushServerHandleRequest(context, token), token);
                }
                catch (HttpListenerException) when (token.IsCancellationRequested)
                {
                    // expected shutdown
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on PadisControllerServer.ListenLoop");
                }
            }
        }

        private async Task PushServerHandleRequest(HttpListenerContext context, CancellationToken token)
        {
            try
            {
                var result = new PushServerCommandsProcessResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ResponseBody = string.Empty
                };
                string requestText;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    requestText = await reader.ReadToEndAsync();

                }
                var deviceCommunicationData = ObjectHelper.DeserializeAsJson<PadisControllerCommunicationModel>(requestText);
                if (deviceCommunicationData != null && deviceCommunicationData.DeviceSerialNumber.IsNullOrEmpty())
                {
                    var deviceInfo =
                        _pushDevices.FirstOrDefault(d => d.SerialNumber == deviceCommunicationData.DeviceSerialNumber);
                    if (deviceInfo != null)
                    {
                        _pushDevicesConnectionInfo[deviceInfo.DeviceNumber] = new PushServerDeviceConnectionInfo
                        {
                            DeviceNumber = deviceInfo.DeviceNumber,
                            ConnectionDateTime = DateTime.Now,
                        };

                        switch (deviceCommunicationData.ContentType)
                        {
                            case PadisControllerContentTypeEnumeration.GetConfigs:
                                result = PushServerProcessGetControllerConfigsCommands(deviceInfo);
                                break;
                            case PadisControllerContentTypeEnumeration.GetNewCommands:
                                result = PushServerProcessGetNewCommands(deviceInfo);
                                break;
                            case PadisControllerContentTypeEnumeration.GetDateAndTime:
                                result = PushServerProcessGetDateAndTime();
                                break;
                            case PadisControllerContentTypeEnumeration.ProcessedCommandId:
                                {
                                    if (deviceCommunicationData.ContentInJsonFormat is PadisControllerCommandResponseModel[] models)
                                    {
                                        result = PushServerProcessCommandResponse(models);
                                    }
                                    else
                                    {
                                        LoggingSystem.LogError("PadisControllerPushServer ProcessedCommandId content", requestText);
                                        result.StatusCode = (int)HttpStatusCode.BadRequest;
                                    }
                                }
                                break;
                            case PadisControllerContentTypeEnumeration.Attendance:
                                {
                                    if (deviceCommunicationData.ContentInJsonFormat is PadisControllerAttendanceCommunicationModel[] models)
                                    {
                                        result = PushServerProcessAttendance(deviceInfo, models);
                                    }
                                    else
                                    {
                                        LoggingSystem.LogError("PadisControllerPushServer Attendance content", requestText);
                                        result.StatusCode = (int)HttpStatusCode.BadRequest;
                                    }
                                }
                                break;
                            case PadisControllerContentTypeEnumeration.HardwareEvent:
                                {
                                    if (deviceCommunicationData.ContentInJsonFormat is PadisControllerOperationLogCommunicationModel[] models)
                                    {
                                        result = PushServerProcessOperationLog(deviceInfo, models);
                                    }
                                    else
                                    {
                                        LoggingSystem.LogError("PadisControllerPushServer HaardwareEvent content", requestText);
                                        result.StatusCode = (int)HttpStatusCode.BadRequest;
                                    }
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration
                                .PushLogInvalidDevice))
                    {
                        LoggingSystem.LogInfo("PadisControllerServer INVALID DEVICE", new
                        {
                            CommandContent = requestText,
                            SerialNumber = deviceCommunicationData.DeviceSerialNumber
                        });
                        result.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }

                }
                else
                {
                    LoggingSystem.LogError("Invalid PadisControllerPushServer communication data", requestText);
                    result.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.PushLogCommandContent))
                {
                    LoggingSystem.LogInfo("PadisControllerPushServer Command log", new
                    {
                        RequestContent = requestText,
                        ResponseContent = result.ResponseBody,
                    });
                }

                var buffer = Encoding.UTF8.GetBytes(result.ResponseBody);
                context.Response.ContentType = "application/json";
                context.Response.ContentEncoding = Encoding.UTF8;
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = result.StatusCode;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length, token);

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on PadisControllerServer.HandleRequest");
            }
            finally
            {
                try
                {
                    context.Response.Close();

                }
                catch
                {
                    //Ignore 
                }
            }
        }

        private void PushServerStop()
        {
            // Push server ممکن است اصلاً استارت نشده باشد (در DoStartServerProcess کامنت است)؛
            // پس null-guard لازم است تا Dispose هنگام خاموش‌کردن سرور crash نکند.
            _cts?.Cancel();
            if (_listener != null)
            {
                _listener.Stop();
            }
        }


        private static PushServerCommandsProcessResult PushServerProcessGetControllerConfigsCommands(DtoCommunicationDeviceData deviceInfo)
        {
            var result = new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = ObjectHelper.SerializeAsJson(new PadisControllerDeviceConfigModel
                {
                    ServerDateTime = DateTime.Now.ToEpochMillisecondsTime(),
                    Timezone = (int?)deviceInfo.TimeSetting?.TimeZone ?? 330
                })
            };
            return result;
        }

        private PushServerCommandsProcessResult PushServerProcessGetNewCommands(DtoCommunicationDeviceData deviceInfo)
        {
            var result = new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = string.Empty
            };
            var commandCount = 0;
            lock (_deviceCurrentUnsentCommandsCountByDeviceSerialNumber)
            {
                var currentDeviceCommandCountInfo = _deviceCurrentUnsentCommandsCountByDeviceSerialNumber
                    .FirstOrDefault(d => d.DeviceSerialNumber == deviceInfo.SerialNumber);
                if (currentDeviceCommandCountInfo != null)
                {
                    commandCount = currentDeviceCommandCountInfo.CommandCount;
                }
            }
            if (commandCount > 0)
            {
                var currentDeviceCommands =
                    _actionToGetCommands(new DeviceNotSentCommandsFilter
                    {
                        Producer = ProducerEnumeration.Zk,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        DeviceSerialNumbers = new List<string> { deviceInfo.SerialNumber },
                        Count = _serverConfig.PushServerMaxCommandCount
                    });
                if (currentDeviceCommands.IsCollectionNotNullOrEmpty())
                {
                    var commandBuilder = new StringBuilder();
                    var ids = new List<int>();
                    foreach (var cmd in currentDeviceCommands)
                    {
                        var currentCommandObject = new PadisControllerCommandModel
                        {
                            CommandId = cmd.Id,
                            Command = cmd.CommandType,
                            CommandContent = cmd.CommandContent,
                        };
                        var currentCommand = ObjectHelper.SerializeAsJson(currentCommandObject);
                        if (commandBuilder.Length + currentCommand.Length > _serverConfig.PushServerMaxCommandLength)
                        {
                            break;
                        }

                        commandBuilder.Append(currentCommand);
                        ids.Add(cmd.Id);
                    }

                    HardwareEventPublisher.Instance.PublishCommandSentToDevice(ids);
                    result.ResponseBody = commandBuilder.ToString();
                }
            }

            return result;
        }

        private static PushServerCommandsProcessResult PushServerProcessCommandResponse(PadisControllerCommandResponseModel[] commandResults)
        {
            var result = new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = string.Empty
            };

            if (commandResults.IsCollectionNotNullOrEmpty())
            {
                foreach (var currentResult in commandResults)
                {
                    if (currentResult.IsSuccessFull)
                    {
                        var commandResult = new DtoDeviceCommandProcessingResult
                        {
                            Id = currentResult.CommandId,
                            CommandResponseTime = DateTime.Now,
                            CommandResponseResult = currentResult.ProcessResultText,
                            Mode = null,
                        };
                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(commandResult);
                    }
                    else
                    {
                        // دستگاه اعلام کرده است که عملیات ناموفق بوده پس باشد در توضیخات کامند ذکر شود
                        HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription
                        {
                            Id = currentResult.CommandId,
                            Description = currentResult.ProcessResultText,
                            Mode = null,
                        });
                    }
                }
            }
            return result;
        }

        private static PushServerCommandsProcessResult PushServerProcessOperationLog(DtoCommunicationDeviceData deviceInfo, PadisControllerOperationLogCommunicationModel[] models)
        {
            var result = new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = string.Empty
            };
            PadisControllerUtils.ProcessOperationLog(deviceInfo, models);
            return result;
        }

        private static PushServerCommandsProcessResult PushServerProcessGetDateAndTime()
        {
            return new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = ObjectHelper.SerializeAsJson(new PadisControllerDateAndTimeCommunicationModel
                {
                    DateTimeEpoch = DateTime.Now.ToEpochMillisecondsTime()
                })
            };
        }

        private static PushServerCommandsProcessResult PushServerProcessAttendance
            (DtoCommunicationDeviceData deviceInfo, PadisControllerAttendanceCommunicationModel[] attendanceModel)
        {
            var result = new PushServerCommandsProcessResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                ResponseBody = string.Empty
            };
            PadisControllerUtils.ProcessAttendance(deviceInfo, attendanceModel);
            return result;
        }

        #region Type

        public class PushServerDeviceConnectionInfo
        {
            public int DeviceNumber { get; set; }

            public DateTime ConnectionDateTime { get; set; }
        }

        public class PushServerCommandsProcessResult
        {
            public int StatusCode { get; set; }

            public string ResponseBody { get; set; }
        }


        #endregion

        #endregion


        #region GRPC SERVER Online Monitoring

        private Server _grpcServer;
        private readonly ConcurrentDictionary<string, DtoCommunicationDeviceData> _onlineMonitoringModeDevices
            = new ConcurrentDictionary<string, DtoCommunicationDeviceData>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, PadisControllerAgent> _onlineMonitoringAgents
            = new ConcurrentDictionary<string, PadisControllerAgent>(StringComparer.OrdinalIgnoreCase);

        // Timer دوره‌ای: هر بار یک Fetch دسته‌ای برای دستگاه‌های «آزاد و متصل» و سپس Dispatch مستقل هر دستور.
        // gate با Wait(0) تضمین می‌کند دو چرخه‌ی Dispatch هم‌پوشانی نکنند (هم‌سان با TimyServer).
        private Timer _grpcDispatchTimer;
        private readonly SemaphoreSlim _grpcDispatchGate = new SemaphoreSlim(1, 1);

        public void SetDeviceOnSocketCommunicationMode(List<DtoCommunicationDeviceData> deviceInfos)
        {
            try
            {
                // Agent هایی که دیگر در لیست دستگاه‌ها نیستند باید قطع و پاک شوند (هم‌سان با TimyServer.SetDeviceList)
                var agentsForRemove = _onlineMonitoringAgents.Values
                    .Where(agent => deviceInfos.All(row => row.DeviceNumber != agent.DeviceInfo.DeviceNumber))
                    .ToList();

                foreach (var agent in agentsForRemove)
                {
                    RemoveAgent(agent.DeviceInfo.SerialNumber);
                }

                _onlineMonitoringModeDevices.Clear();
                foreach (var deviceInfo in deviceInfos)
                {
                    if (deviceInfo.ProducerEnum == ProducerEnumeration.Padis
                        && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1
                        && deviceInfo.SerialNumber.IsCollectionNotNullOrEmpty()
                        && deviceInfo.OnlineMonitoringMode)
                    {
                        _onlineMonitoringModeDevices.TryAdd(deviceInfo.SerialNumber, deviceInfo);
                    }
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp
                    , "PadisControllerServer Error on SetDeviceOnSocketCommunicationMode");
            }

        }

        public void StartOnlineMonitoringServer()
        {
            _grpcServer = new Server { Services = { PadisControllerGrpcService.BindService(this) } };
            _grpcServer.Ports.Add(new ServerPort("0.0.0.0", _serverConfig.GrpcServerPort, ServerCredentials.Insecure));
            _grpcServer.Start();

            // به‌جای یک Thread دائمی با while(true) و Thread.Abort، از یک Timer + gate استفاده می‌کنیم (هم‌سان با TimyServer)
            var dispatchInterval = Math.Max(100, _serverConfig.GrpcCommandTimerIntervalInMillisecond);
            _grpcDispatchTimer = new Timer(dispatchInterval);
            _grpcDispatchTimer.Elapsed += GrpcDispatchTimerOnElapsed;
            _grpcDispatchTimer.Start();
        }

        public void StopOnlineMonitoringServer()
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
        //  Timer دوره‌ای: یک Fetch برای همه‌ی دستگاه‌های آزاد، سپس Dispatch مستقل
        // ───────────────────────────────────────────────

        private void GrpcDispatchTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
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
                LoggingSystem.LogError(exp, "Error on PadisControllerServer.RunGrpcDispatchCycleAsync");
            }
            finally
            {
                _grpcDispatchGate.Release();
            }
        }

        /// <summary>
        /// فقط یک بار به دیتابیس مراجعه می‌کند: برای تمام دستگاه‌های متصل و آزاد (IsIdle)،
        /// دستورات بدون‌ارسال را در یک Query واحد می‌خواند و برای هر دستگاه یک دستور را
        /// به‌صورت fire-and-forget ارسال می‌کند تا دستگاه‌های سریع منتظر دستگاه‌های کند نمانند.
        /// دستگاه‌های مشغول (IsIdle == false) اصلاً وارد Query نمی‌شوند (هم‌سان با TimyServer).
        /// </summary>
        private Task DispatchCommandsToOnlineMonitoringDevicesAsync()
        {
            // فقط دستگاه‌هایی که هم وصل‌اند هم آزادند وارد Query می‌شوند
            var idleAgents = _onlineMonitoringAgents.Values
                .Where(a => a.IsIdle && a.DeviceInfo.SerialNumber.IsNotNullOrEmpty())
                .GroupBy(a => a.DeviceInfo.SerialNumber)
                .ToDictionary(g => g.Key, g => g.First());

            if (idleAgents.Count == 0)
            {
                return Task.CompletedTask;
            }

            var commandFetchParams = new DeviceNotSentCommandsFilter
            {
                Count = _serverConfig.GrpcServerCommandCount,
                DeviceSerialNumbers = idleAgents.Keys.ToList(),
                Producer = ProducerEnumeration.Padis,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
            };

            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerCommandFetch))
            {
                LoggingSystem.LogInfo("Padis Controller Server Command Fetch params", commandFetchParams);
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

            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerCommandFetchResult))
            {
                LoggingSystem.LogInfo("Padis Controller Server Command Fetch result"
                    , allCommands.Select(c => new { c.Id, c.DeviceNumber, c.CommandType }));
            }

            if (allCommands.IsCollectionNullOrEmpty())
            {
                return Task.CompletedTask;
            }

            // در هر چرخه فقط یک دستور برای هر دستگاه ارسال می‌شود (single in-flight per device)
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
        /// ارسال یک دستور به یک دستگاه و ثبت نتیجه در دیتابیس.
        /// منطق publish دقیقاً مانند نسخه‌ی قبلی حفظ شده است:
        ///   موفق → CommandResponseReceived(SUCCESS)
        ///   خطای واقعی (نه Timeout و نه LineIsBusy) → CommandSentToDevice + CommandDescriptionReceived
        ///   Timeout یا LineIsBusy → هیچ (دستور ارسال‌نشده می‌ماند تا دور بعد دوباره تلاش شود)
        /// </summary>
        private async Task SendCommandToDeviceAsync(PadisControllerAgent agent, DtoDeviceUnsentCommand command)
        {
            try
            {
                if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerCommandBeforeSend))
                {
                    LoggingSystem.LogInfo("Padis Controller Server Command before send",
                        new { command.DeviceNumber, command.CommandType });
                }

                var timeoutInMillisecond = GetCommandTimeoutInMillisecond(command);
                var resultOfSendCommand = await agent.SendCommandAsync(command, timeoutInMillisecond).ConfigureAwait(false);

                if (resultOfSendCommand.ErrorCode == PadisControllerErrorEnumeration.Success)
                {
                    HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                        new DtoDeviceCommandProcessingResult
                        {
                            CommandResponseResult = "SUCCESS",
                            CommandResponseTime = DateTime.Now,
                            Id = command.Id
                        });
                }
                else if (resultOfSendCommand.ErrorCode != PadisControllerErrorEnumeration.MessageTimeout
                         && resultOfSendCommand.ErrorCode != PadisControllerErrorEnumeration.LineIsBusy)
                {
                    HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.Id });
                    HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(
                        new DtoDeviceCommandProcessingDescription
                        {
                            Description = $"Error code is {(int)resultOfSendCommand.ErrorCode}",
                            Id = command.Id,
                        });
                }
            }
            catch (Exception exp)
            {
                HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.Id });
                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(
                    new DtoDeviceCommandProcessingDescription
                    {
                        Description = $"Unknown Error. Message is {exp.GetFullExceptionMessage()}",
                        Id = command.Id,
                    });
                LoggingSystem.LogError(exp, "Error on PadisControllerServer.SendCommandToDeviceAsync");
            }
        }

        private int GetCommandTimeoutInMillisecond(DtoDeviceUnsentCommand command)
        {
            switch (command.CommandType)
            {
                case DeviceCommandTypeEnumeration.SetUserInfo:
                case DeviceCommandTypeEnumeration.EnrollUserWithTemplate:
                case DeviceCommandTypeEnumeration.DeleteUser:
                case DeviceCommandTypeEnumeration.PadisControllerSetRelay:
                case DeviceCommandTypeEnumeration.PadisControllerSetIoPort:
                case DeviceCommandTypeEnumeration.PadisControllerSetWiegand:
                case DeviceCommandTypeEnumeration.PadisControllerSetCalendar:
                    return _serverConfig.GrpcServerTimeoutLongInMillisecond;
                case DeviceCommandTypeEnumeration.ReadoutAttendance:
                case DeviceCommandTypeEnumeration.ReadAttendance:
                case DeviceCommandTypeEnumeration.ClearData:
                    return _serverConfig.GrpcServerTimeoutVeryLongInMillisecond;
                default:
                    return _serverConfig.GrpcServerTimeoutShortInMillisecond;
            }
        }

        public override async Task Stream(
            IAsyncStreamReader<PadisControllerGrpcMessage> requestStream
            , IServerStreamWriter<PadisControllerGrpcMessage> responseStream
            , ServerCallContext context)
        {
            // 1. Handshake: خواندن سریال از اولین پیام


            if (!await requestStream.MoveNext(context.CancellationToken).ConfigureAwait(false))
            {
                context.Status = new Status(StatusCode.InvalidArgument, "Handshake missing."); return;
            }

            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerConnect))
            {
                LoggingSystem.LogInfo("Padis controller grpc server device", new
                {
                    Stream = requestStream
                });
            }

            var firstMsg = requestStream.Current;
            var deviceSerialNumber = firstMsg.DeviceSerialNumber; // فرض: پیام اول حاوی SerialNumber است
            if (string.IsNullOrWhiteSpace(deviceSerialNumber) ||
                !_onlineMonitoringModeDevices.TryGetValue(deviceSerialNumber, out var deviceInfo))
            {
                if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerConnect))
                {
                    LoggingSystem.LogInfo("Padis controller grpc server device is unauthorized", new
                    {
                        Stream = requestStream
                    });
                }
                context.Status = new Status(StatusCode.PermissionDenied, "Unauthorized device."); return;
            }

            // 2. اگر همین دستگاه قبلاً اتصالی داشته، اتصال قدیمی را کامل ببند (reconnect → state تازه)
            RemoveAgent(deviceSerialNumber);

            // 3. ساخت Agent جدید و ثبت آن *قبل* از شروع پردازش.
            //    نکته: در نسخه‌ی قبلی ثبت قفل دستور بعد از await StartProcessingAsync انجام می‌شد؛ چون آن await
            //    تا لحظه‌ی قطع اتصال بلاک است، عملاً قفل هیچ‌وقت در طول اتصال وجود نداشت و هیچ دستوری به دستگاه نمی‌رسید.
            var newAgent = new PadisControllerAgent(deviceInfo, responseStream, requestStream, context.CancellationToken, _serverMatchProcessor);
            _onlineMonitoringAgents[deviceSerialNumber] = newAgent;

            try
            {
                // 4. واگذاری کامل کنترل ارتباط به Agent
                // این متد تا زمان قطع اتصال دستگاه بلوک می‌ماند
                await newAgent.StartProcessingAsync().ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on StartProcessingAsync of agent", new
                {
                    newAgent.DeviceInfo.DeviceNumber
                });
            }
            finally
            {
                // فقط اگر همچنان همین Agent ثبت‌شده است پاکش کن (اگر reconnect جای آن را گرفته، دست نزن)
                RemoveAgent(deviceSerialNumber, newAgent);
            }
        }

        #endregion


        #region Utilities

        private void RemoveAgent(string deviceSerialNumber)
        {
            if (_onlineMonitoringAgents.TryRemove(deviceSerialNumber, out var oldAgent))
            {
                if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcServerConnect))
                {
                    LoggingSystem.LogInfo("Padis controller grpc server connection exists and must be disposed", new
                    {
                        DeviceSerialNumber = deviceSerialNumber
                    });
                }
                oldAgent.Dispose();
            }
        }

        // حذف اتمیک: فقط وقتی همان نمونه‌ی موردانتظار هنوز ثبت است (جلوگیری از حذف اتصال جدیدِ reconnect)
        private void RemoveAgent(string deviceSerialNumber, PadisControllerAgent expectedAgent)
        {
            if (((ICollection<KeyValuePair<string, PadisControllerAgent>>)_onlineMonitoringAgents)
                .Remove(new KeyValuePair<string, PadisControllerAgent>(deviceSerialNumber, expectedAgent)))
            {
                expectedAgent.Dispose();
            }
        }


        #endregion

        
        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Free managed resources
            }
            if (_getCommandTimers != null)
            {
                _getCommandTimers.Stop();
                _getCommandTimers.Elapsed -= GetCommandTimersOnElapsed;
                _getCommandTimers.Dispose();
                _getCommandTimers = null;
            }
            PushServerStop();
            StopOnlineMonitoringServer();
            _grpcDispatchGate.Dispose();

            _disposed = true;
        }

        ~PadisControllerServer()
        {
            Dispose(false);
        }

        #endregion


    }


}

