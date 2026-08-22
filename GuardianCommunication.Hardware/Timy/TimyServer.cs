using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;
using Newtonsoft.Json.Linq;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using SuperWebSocket;
using Timer = System.Timers.Timer;

namespace GuardianCommunication.Hardware.Timy
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class TimyServer : IDisposable
    {
        private IBootstrap _bootstrap;
        private WebSocketServer _server;
        private Timer _getCommandTimers;
        private TimyPushConfig _pushConfig;
        // لیست دستگاه‌هایی که اجازه‌ی push/online monitoring دارند (پیکربندی، نه اتصال زنده)
        private readonly ConcurrentDictionary<string, DtoDevice> _pushDevices =
            new ConcurrentDictionary<string, DtoDevice>();

        // SessionId → TimyDeviceAgent  (اتصال‌های زنده‌ی فعلی)
        private readonly ConcurrentDictionary<string, TimyDeviceAgent> _connectedAgents =
            new ConcurrentDictionary<string, TimyDeviceAgent>();

        //// SerialNumber → SessionId  (برای دسترسی سریع و جلوگیری از دو اتصال هم‌زمان یک دستگاه)
        private readonly ConcurrentDictionary<string, string> _sessionIdBySerialNumber =
            new ConcurrentDictionary<string, string>();

        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;

        #region Singleton

        public static TimyServer Instance { get; }

        private TimyServer()
        {
        }

        static TimyServer()
        {
            Instance = new TimyServer();
        }

        #endregion


        public void StartTimyServer(List<DtoDevice> deviceInfos
            , TimyPushConfig pushConfig
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands)
        {
            _actionToGetCommands = actionToGetCommands;
            _pushConfig = pushConfig;
            var thread = new Thread(() => DoStartServerProcess(deviceInfos)) { IsBackground = true };
            thread.Start();

            // این تایمر هر بار یک Fetch دسته‌ای از دیتابیس می‌زند (فقط برای دستگاه‌های آزاد)
            // و نتیجه را بین Session‌های مربوطه Dispatch می‌کند.
            _getCommandTimers = new Timer(pushConfig.GetCommandTimerIntervalInMillisecond);
            _getCommandTimers.Elapsed += GetCommandTimersOnElapsed;
            _getCommandTimers.Start();

            LoggingSystem.LogInfo("TimyServer Started");
        }


        public void DoStartServerProcess(List<DtoDevice> deviceInfos)
        {
            var setOnlineMonitoringMode = new Thread
                (() => SetDeviceList(deviceInfos))
                { IsBackground = true };
            setOnlineMonitoringMode.Start();
            
            _bootstrap = BootstrapFactory.CreateBootstrap();
            if (_bootstrap.Initialize())
            {
                var result = _bootstrap.Start();
                if (result == StartResult.Success)
                {
                    var server = _bootstrap.AppServers.FirstOrDefault();
                    if (server != null)
                    {
                        if (server.State == ServerState.Running)
                        {
                            _server = server as WebSocketServer;
                            // ReSharper disable once PossibleNullReferenceException
                            _server.NewMessageReceived += TimyServer_NewMessageReceived;
                            _server.NewSessionConnected += TimyServer_NewSessionConnected;
                            _server.SessionClosed += TimyServer_SessionClosed;
                            LoggingSystem.LogInfo("Timy Server started");
                        }
                        else
                        {
                            LoggingSystem.LogInfo("Timy Server failed to start");
                        }
                    }
                }
                else
                {
                    LoggingSystem.LogInfo("Timy Server failed to bootstrap");
                }
            }
            else
            {
                LoggingSystem.LogInfo("Timy Server failed to initialize");
            }
        }

        public void SetDeviceList(List<DtoDevice> deviceInfos)
        {
            var timyPushDevicesInfo = deviceInfos.Where
                (row => row.ProducerNumber == ProducerEnumeration.Timy
                && row.ConnectionMode == DeviceConnectionModeEnumeration.Push).ToList();

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerSetDeviceList))
            {
                LoggingSystem.LogInfo("Timy Server set device list", timyPushDevicesInfo);
            }

            try
            {
                var agentsForRemove = new List<KeyValuePair<string, TimyDeviceAgent>>();
                foreach (var agent in _connectedAgents)
                {
                    var currentDeviceInfo = timyPushDevicesInfo.FirstOrDefault
                        (row => row.DeviceNumber == agent.Value.DeviceInfo.DeviceNumber);
                    if (currentDeviceInfo == null)
                    {
                        agentsForRemove.Add(agent);
                    }
                    else
                    {
                        // مشخصات جدید دستگاه را برای Agent هایی که تا کنون وصل شده اند ست می کنیم
                        agent.Value.DeviceInfo = currentDeviceInfo;
                    }
                }

                if (agentsForRemove.Any())
                {
                    foreach (var agent in agentsForRemove)
                    {
                        RemoveDeviceAgentBySession(agent.Key);
                    }
                }

                _pushDevices.Clear();
                foreach (var item in timyPushDevicesInfo)
                {
                    _pushDevices.TryAdd(item.SerialNumber, item);
                }


            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp
                    , "ZkServer Error on SetOnlineMonitoringModeDeviceList");
            }



        }

        public List<Guid> GetConnectedDeviceNumbers()
        {
            var connectedSerials = _sessionIdBySerialNumber.Keys;
            return _pushDevices.Values
                .Where(d => d.SerialNumber.IsNotNullOrEmpty() && connectedSerials.Contains(d.SerialNumber))
                .Select(d => d.Id)
                .ToList();
        }

        // ───────────────────────────────────────────────
        //  Timer دوره‌ای: یک Fetch برای همه دستگاه‌های آزاد، سپس Dispatch
        // ───────────────────────────────────────────────

        // Monitor.TryEnter/Exit با await ناسازگار است (چون thread-affinity دارد و await می‌تواند
        // ادامه‌ی کار را روی Thread دیگری برگرداند). SemaphoreSlim برای این منظور در کد async صحیح است.
        private readonly SemaphoreSlim _dispatchCycleGate = new SemaphoreSlim(1, 1);

        private void GetCommandTimersOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogGetCommandTimersElapsed))
            {
                LoggingSystem.LogInfo("GetCommandTimersOnElapsed Process is calling");
            }

            if (!_dispatchCycleGate.Wait(0))
                return;

            _ = RunDispatchCycleAsync();
        }

        private async Task RunDispatchCycleAsync()
        {
            try
            {
                await DispatchCommandsToConnectedDevicesAsync();
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
            finally
            {
                _dispatchCycleGate.Release();
            }
        }

        /// <summary>
        /// فقط یک بار به دیتابیس مراجعه می‌کند: برای تمام دستگاه‌های متصل و آزاد (IsIdle)،
        /// دستورات بدون‌ارسال را در یک Query واحد می‌خواند و هرکدام را به Session مربوطه می‌دهد.
        /// هر دستگاه کاملاً مستقل است؛ TrySendCommandAsync به صورت fire-and-forget صدا زده می‌شود
        /// تا دستگاه‌های سریع منتظر دستگاه‌های کند نمانند.
        /// دستگاه‌های مشغول (IsIdle == false) حتی در فیلتر Query هم شرکت نمی‌کنند.
        /// </summary>
        private Task DispatchCommandsToConnectedDevicesAsync()
        {
            // فقط دستگاه‌هایی که هم وصل‌اند هم آزادند وارد Query می‌شوند
            var idleAgents = _connectedAgents.Values
                .Where(s => s.IsIdle)
                .ToDictionary(s => s.DeviceInfo.SerialNumber);

            if (idleAgents.Count == 0)
                return Task.CompletedTask; // همه مشغول‌اند یا هیچ دستگاهی وصل نیست؛ هیچ Query‌ای زده نمی‌شود

            List<DtoDeviceUnsentCommand> commands;
            try
            {
                commands = _actionToGetCommands(new DeviceNotSentCommandsFilter
                {
                    Count = 1,
                    Producer = ProducerEnumeration.Timy,
                    SdkVersion = SdkVersionEnumeration.SdkVersion1,
                    DeviceSerialNumbers = idleAgents.Keys.ToList(),
                });
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error fetching commands batch from database");
                return Task.CompletedTask;
            }

            if (commands.IsCollectionNullOrEmpty())
            {
                return Task.CompletedTask;
            }

            foreach (var command in commands.Where(c => idleAgents.ContainsKey(c.DeviceSerialNumber)))
            {
                _ = idleAgents[command.DeviceSerialNumber].TrySendCommandAsync(command);
                Thread.Sleep(_pushConfig.WaitBetweenCommandSendInMilliseconds);
            }

            return Task.CompletedTask;
        }

        // ───────────────────────────────────────────────
        //  Routing: هر پیام را به DeviceSession مربوطه پاس می‌دهد
        // ───────────────────────────────────────────────

        private static void TimyServer_NewSessionConnected(WebSocketSession session)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerDeviceConnection))
            {
                LoggingSystem.LogInfo("Timy Server new device connected", session.RemoteEndPoint);
            }
            // در این مرحله هنوز نمی‌دانیم SerialNumber چیست (دستگاه باید "reg" بفرستد)
            // پس TimyDeviceSession اینجا ساخته نمی‌شود؛ در HandleRegistration ساخته می‌شود.
        }

        private void TimyServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerDeviceConnection))
            {
                LoggingSystem.LogInfo("Timy Server device Disconnted", $"SessionClose[{session.RemoteEndPoint}] for reason {reason}");
            }

            RemoveDeviceAgentBySession(session.SessionID);
        }

        private void TimyServer_NewMessageReceived(WebSocketSession session, string message)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerMessage))
            {
                LoggingSystem.LogInfo($"Timy Server MessageReceived[{session.RemoteEndPoint}], Message:" + message);
            }

            try
            {
                var jsonMessage = JObject.Parse(message);
                var cmd = jsonMessage.Value<string>("cmd");

                // "reg" تنها پیامی است که TimyServer مستقیماً پردازش می‌کند،
                // چون باعث ساخت/جایگزینی TimyDeviceSession می‌شود.
                if (cmd == "reg")
                {
                    ProcessDeviceRegistration(session, jsonMessage);
                    return;
                }

                // هر پیام دیگر مستقیم به DeviceSession مربوط به همین Socket پاس می‌شود
                if (_connectedAgents.TryGetValue(session.SessionID, out var deviceSession))
                {
                    deviceSession.HandleMessage(jsonMessage);
                }
                else
                {
                    LoggingSystem.LogInfo($"Message received from unregistered session {session.SessionID}, ignored.");
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        // ساخت یا جایگزینی TimyDeviceSession هنگام "reg"
        // طبق تصمیم: در reconnect یک Session کاملاً تازه ساخته می‌شود؛ state قبلی دور ریخته می‌شود.
        private void ProcessDeviceRegistration(WebSocketSession session, JObject jsonMessage)
        {
            var sn = jsonMessage.Value<string>("sn");
            var jsonObjectDeviceInfo = jsonMessage["devinfo"];
            var useduser = jsonObjectDeviceInfo.Value<int>("useduser");
            var usedfp = jsonObjectDeviceInfo.Value<int>("usedfp");
            var usednewlog = jsonObjectDeviceInfo.Value<int>("usednewlog");

            var serverTime = DateTime.Now;
            var responseMessage = "{\"ret\":\"reg\",\"result\":true,\"cloudtime\":\"" + serverTime.ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
            session.Send(responseMessage);

            if (!_pushDevices.TryGetValue(sn, out var deviceData))
            {
                LoggingSystem.LogInfo($"Registration from unknown device serial {sn}, ignored.");
                return;
            }

            // اگر این دستگاه قبلاً (با Session/Socket دیگری) متصل بوده، آن Session قدیمی را ببند
            if (_sessionIdBySerialNumber.TryRemove(sn, out var oldSessionId))
            {
                RemoveDeviceAgentBySession(oldSessionId);
            }

            // ساخت Session تازه؛ state قبلی (صف pending، busy/idle) عمداً دور ریخته می‌شود
            var deviceSession = new TimyDeviceAgent(session, deviceData, _pushConfig);
            _connectedAgents[session.SessionID] = deviceSession;
            _sessionIdBySerialNumber[sn] = session.SessionID;

            HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(
                new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceId = deviceData.Id,
                        IsConnected = true
                    }
                });
            HardwareEventPublisher.Instance.PublishAccessLogCountReceived(deviceData.DeviceNumber, usednewlog);
            HardwareEventPublisher.Instance.PublishFingerCountReceived(deviceData.DeviceNumber, usedfp);
            HardwareEventPublisher.Instance.PublishUserCountReceived(deviceData.DeviceNumber, useduser);
        }

        private void RemoveDeviceAgentBySession(string sessionId)
        {
            if (!_connectedAgents.TryRemove(sessionId, out var deviceAgent))
                return;

            _sessionIdBySerialNumber.TryRemove(deviceAgent.DeviceInfo.SerialNumber, out _);
            deviceAgent.Dispose();

            HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(
                new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceId = deviceAgent.DeviceInfo.Id,
                        IsConnected = false
                    }
                });
        }

        public void StopServer()
        {
            Dispose(true);
        }

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
            if (disposing)
            {
                foreach (var session in _connectedAgents.Values)
                {
                    session.Dispose();
                }
                _connectedAgents.Clear();
                _sessionIdBySerialNumber.Clear();
                _dispatchCycleGate.Dispose();
            }

            _server.NewMessageReceived -= TimyServer_NewMessageReceived;
            _server.NewSessionConnected -= TimyServer_NewSessionConnected;
            _server.SessionClosed -= TimyServer_SessionClosed;

            foreach (var server in _bootstrap.AppServers)
            {
                server.Stop();
            }
            _bootstrap.Stop();

            if (_getCommandTimers != null)
            {
                _getCommandTimers.Stop();
                _getCommandTimers.Elapsed -= GetCommandTimersOnElapsed;
            }
            _disposed = true;
        }

        ~TimyServer()
        {
            Dispose(false);
        }

        #endregion
    }
}