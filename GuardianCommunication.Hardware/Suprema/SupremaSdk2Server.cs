using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;

namespace GuardianCommunication.Hardware.Suprema
{
    public class SupremaSdk2Server : IDisposable
    {

        #region Variables


        private IntPtr _sdkContext = IntPtr.Zero;
        private SupremaSdk2ServerConfig _config;

        private readonly List<DtoCommunicationDeviceData> _currentDeviceList = new List<DtoCommunicationDeviceData>();
        private readonly ConcurrentDictionary<uint, SupremaSdk2OnDemandAdapter> _connectedDeviceAdapters = new ConcurrentDictionary<uint, SupremaSdk2OnDemandAdapter>();
        private readonly ConcurrentQueue<uint> _disconnectedDevices = new ConcurrentQueue<uint>();
        private Thread _deviceCommandsThread;
        private Thread _reconnectDeviceThread;
        private CancellationTokenSource _serverCts;

        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;

        private ApiV2.OnLogReceived _onLogReceived;
        private ApiV2.OnDeviceFound _onDeviceFound;
        private ApiV2.OnDeviceAccepted _onDeviceAccepted;
        private ApiV2.OnDeviceConnected _onDeviceConnected;
        private ApiV2.OnDeviceDisconnected _onDeviceDisconnected;

        #endregion


        #region Singleton

        public static SupremaSdk2Server Instance { get; }

        private SupremaSdk2Server()
        {
        }

        static SupremaSdk2Server()
        {
            Instance = new SupremaSdk2Server();
        }

        #endregion


        public void StartServer(
            SupremaSdk2ServerConfig serverConfig,
            Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , List<DtoCommunicationDeviceData> deviceInfos)
        {
            _config = serverConfig;
            _actionToGetCommands = actionToGetCommands;

            LoggingSystem.LogInfo("Suprema SDK 2 config", serverConfig);
            var thread = new Thread(() => DelayedStartServer(deviceInfos));
            thread.Start();

        }


        public void SetDeviceList(List<DtoCommunicationDeviceData> deviceInfos)
        {
            lock (_currentDeviceList)
            {
                if (deviceInfos == null)
                {
                    deviceInfos = new List<DtoCommunicationDeviceData>();
                }

                var forAdd = new List<DtoCommunicationDeviceData>();
                var forUpdate = new List<DtoCommunicationDeviceData>();
                var forDelete = new List<DtoCommunicationDeviceData>();
                var pushDeviceInfos = deviceInfos.Where
                (row =>
                    row.ProducerEnum == ProducerEnumeration.Suprema
                    && row.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2
                    && row.ConnectionMode == DeviceConnectionModeEnumeration.Push).ToList();
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerSetDeviceList))
                {
                    LoggingSystem.LogInfo("Suprema 2 Server device list for push", pushDeviceInfos);
                }

                foreach (var device in pushDeviceInfos)
                {
                    var deviceInCurrentList = _currentDeviceList.FirstOrDefault(a => a.DeviceNumber == device.DeviceNumber);
                    if (deviceInCurrentList != null)
                    {
                        forUpdate.Add(device);
                    }
                    else
                    {
                        forAdd.Add(device);
                    }
                }
                foreach (var device in _currentDeviceList)
                {
                    var deviceInNewList = pushDeviceInfos.FirstOrDefault(a => a.DeviceNumber == device.DeviceNumber);
                    if (deviceInNewList == null)
                    {
                        forDelete.Add(device);
                    }
                }
                foreach (var device in forAdd)
                {
                    _currentDeviceList.Add(device);
                }
                foreach (var device in forUpdate)
                {
                    _currentDeviceList.RemoveAll(d => d.DeviceNumber == device.DeviceNumber);
                    _currentDeviceList.Add(device);
                }
                foreach (var device in forDelete)
                {
                    try
                    {

                        _connectedDeviceAdapters.TryRemove(device.SerialNumber.ToUInt32(), out var deviceAdapter);
                        if (deviceAdapter != null)
                        {
                            try
                            {
                                ApiV2.BS2_StopMonitoringLog(_sdkContext, deviceAdapter.DeviceId);
                                deviceAdapter.Dispose();
                            }
                            catch (Exception exp)
                            {
                                LoggingSystem.LogError(exp, "Error on stop server", device);
                            }
                        }

                        _currentDeviceList.RemoveAll(d => d.DeviceNumber == device.DeviceNumber);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on remove device from push device on suprema 2", device);
                    }
                }
            }
        }

        public List<int> GetConnectedDeviceNumbers()
        {
            return _connectedDeviceAdapters.Values.Select(r => r.DeviceInfo.DeviceNumber).ToList();
            //var connectedDeviceSerialNumbers = _connectedDeviceAdapters.Keys;
            //lock (_currentDeviceList)
            //{
            //    return _currentDeviceList
            //        .Where(d => connectedDeviceSerialNumbers.Contains(d.SerialNumber.ToUInt32()))
            //        .Select(d => d.DeviceNumber).ToList();
            //}
        }

        public SupremaSdk2OnDemandAdapter GetDeviceAdapter(int deviceNumber)
        {
            DtoCommunicationDeviceData device;
            lock (_currentDeviceList)
            {
                device = _currentDeviceList.FirstOrDefault(d => d.DeviceNumber == deviceNumber);
            }

            if (device != null)
            {
                _connectedDeviceAdapters.TryGetValue(device.SerialNumber.ToUInt32(), out var deviceAdapter);
                return deviceAdapter;
            }
            return null;
        }

        #region Private Methods

        private void DelayedStartServer(List<DtoCommunicationDeviceData> deviceInfos)
        {

            SetDeviceList(deviceInfos);

            // ReSharper disable CommentTypo
            // به دلیل اینکه سرعت اتصال دستگاه ها بسیار بالا می باشد
            // می بایست یک مکس بکنیم و سپس سرور را اسارت نماییم
            // ReSharper restore CommentTypo
            Thread.Sleep(_config.StartDelayInSecond * 1000);

            _onDeviceFound = DeviceFound;
            _onDeviceAccepted = DeviceAccepted;
            _onDeviceConnected = DeviceConnected;
            _onDeviceDisconnected = DeviceDisconnected;
            _onLogReceived = RealtimeLogReceived;

            try
            {
                _sdkContext = ApiV2.BS2_AllocateContext();
                if (_sdkContext == IntPtr.Zero)
                {
                    throw new Exception("Sdk Suprema Version 2 cannot be initialized");
                }

                var result = (BS2ErrorCode)ApiV2.BS2_Initialize(_sdkContext);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    ApiV2.BS2_ReleaseContext(_sdkContext);
                    _sdkContext = IntPtr.Zero;
                    throw new Exception($"Sdk Initialization field with result ${result} For SupremaSdk2Server");
                }

                result = (BS2ErrorCode)ApiV2.BS2_SetServerPort(_sdkContext, (ushort)_config.ServerPort);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    ApiV2.BS2_ReleaseContext(_sdkContext);
                    _sdkContext = IntPtr.Zero;
                    throw new Exception($"Can't set server port {_config.ServerPort} For SupremaSdk2Server");
                }


                result = (BS2ErrorCode)ApiV2.BS2_SetDeviceEventListener(_sdkContext,
                    _onDeviceFound,
                    _onDeviceAccepted,
                    _onDeviceConnected,
                    _onDeviceDisconnected);
                if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    ApiV2.BS2_ReleaseContext(_sdkContext);
                    _sdkContext = IntPtr.Zero;
                    throw new Exception($"Can't register a callback function/method to a sdk ${result} For SupremaSdk2Server");
                }

                LoggingSystem.LogInfo($"SupremaSdk2Server started Successfully at port {_config.ServerPort}");

                _serverCts = new CancellationTokenSource();

                _deviceCommandsThread = new Thread(DeviceCommand) { IsBackground = true };
                _deviceCommandsThread.Start();

                _reconnectDeviceThread = new Thread(ReconnectDevice) { IsBackground = true };
                _reconnectDeviceThread.Start();



            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void StopServer()
        {
            // Cooperative shutdown (replaces Thread.Abort): signal cancellation and wait for the
            // worker threads to actually exit BEFORE releasing the native context. This removes the
            // use-after-free where BS2_ReleaseContext ran while a worker was still inside a BS2_* call.
            //TODO: Ask about it
            _serverCts?.Cancel();
            var commandStopped = _deviceCommandsThread?.Join(10000) ?? true;
            var reconnectStopped = _reconnectDeviceThread?.Join(10000) ?? true;
            var threadsStopped = commandStopped && reconnectStopped;

            lock (_currentDeviceList)
            {
                foreach (var device in _currentDeviceList)
                {
                    if (_connectedDeviceAdapters.TryGetValue(device.SerialNumber.ToUInt32(), out var deviceAdapter))
                    {
                        try
                        {
                            ApiV2.BS2_StopMonitoringLog(_sdkContext, deviceAdapter.DeviceId);
                            deviceAdapter.Dispose();
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "Error on stop server", device);
                        }
                    }
                }
            }

            if (threadsStopped)
            {
                ApiV2.BS2_ReleaseContext(_sdkContext);
                _sdkContext = IntPtr.Zero;
            }
            else
            {
                // A worker did not stop in time; skip ReleaseContext to avoid freeing a context that a
                // native call may still be using. The context is reclaimed by the OS at process exit.
                LoggingSystem.LogInfo("SupremaSdk2Server StopServer: worker thread(s) did not stop in time; skipping BS2_ReleaseContext");
            }

            _serverCts?.Dispose();
            _serverCts = null;
        }

        private static void DeviceFound(uint deviceId)
        {
        }

        private void DeviceAccepted(uint deviceId)
        {

            try
            {
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerConnection))
                {
                    LoggingSystem.LogInfo("Suprema 2 Server Device Connected", new
                    {
                        DeviceId = deviceId,
                    });
                }

                DtoCommunicationDeviceData device;
                lock (_currentDeviceList)
                {
                    device = _currentDeviceList.FirstOrDefault(row => row.SerialNumber == deviceId.ToString());
                    if (device == null)
                    {
                        if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerConnection))
                        {
                            LoggingSystem.LogInfo("Suprema 2 Server Invalid device connected", new
                            {
                                DeviceId = deviceId,
                            });
                        }
                        return;
                    }
                }

                var result = ApiV2.BS2_StartMonitoringLog(_sdkContext, deviceId, _onLogReceived);
                if (result != (int)BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerConnection))
                    {
                        LoggingSystem.LogInfo("Suprema 2 Server Error on StartMonitoringLog", new
                        {
                            ErrorCode = result,
                            Device = device,
                        });
                    }
                    return;
                }

                if (_connectedDeviceAdapters.TryRemove(deviceId, out var deviceAdapter))
                {
                    deviceAdapter.Dispose();
                }
                deviceAdapter = new SupremaSdk2OnDemandAdapter(device, _sdkContext, deviceId);
                _connectedDeviceAdapters.TryAdd(deviceId, deviceAdapter);

                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceNumber = device.DeviceNumber,
                        IsConnected = true
                    }
                });
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on DeviceAccepted For SupremaSdk2Server",
                    $"Additional Data is {deviceId}");
            }
        }

        private static void DeviceConnected(uint deviceId)
        {
        }

        private void DeviceDisconnected(uint deviceId)
        {
            try
            {
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerConnection))
                {
                    LoggingSystem.LogInfo("Suprema 2 Server Device Disconnected", new
                    {
                        DeviceID = deviceId,
                    });
                }

                DtoCommunicationDeviceData device;
                lock (_currentDeviceList)
                {
                    device = _currentDeviceList.FirstOrDefault(row => row.SerialNumber == deviceId.ToString());
                    if (device == null)
                    {
                        if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerConnection))
                        {
                            LoggingSystem.LogInfo("Suprema 2 Server Invalid device connected", new
                            {
                                DeviceID = deviceId,
                            });
                        }
                        return;
                    }
                }
                _disconnectedDevices.Enqueue(deviceId);
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceNumber = device.DeviceNumber,
                        IsConnected = false
                    }
                });
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on DeviceDisconnected  in SupremaSdk2Server",
                    $"Additional Data is {ObjectHelper.SerializeAsJsonFormatted(new { deviceId })}");
            }
        }

        private void DeviceCommand()
        {
            var token = _serverCts.Token;
            while (!token.IsCancellationRequested)
            {

                try
                {
                    if (!_connectedDeviceAdapters.Any())
                    {
                        token.WaitHandle.WaitOne(_config.CommandSetting.SleepBetweenSendsIfCommandNotExistsInMilliSeconds);
                        continue;
                    }

                    var deviceCommandParams = new DeviceNotSentCommandsFilter
                    {
                        Count = 1,
                        DeviceSerialNumbers = _connectedDeviceAdapters.Select(d => d.Key.ToString()).ToList(),
                        Producer = ProducerEnumeration.Suprema,
                        SdkVersion = SdkVersionEnumeration.SdkVersion2,
                    };
                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerCommandParams))
                    {
                        LoggingSystem.LogInfo("Suprema 2 Server Command Params", deviceCommandParams);
                    }
                    var allCommands = _actionToGetCommands(deviceCommandParams);
                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerCommandResult))
                    {
                        LoggingSystem.LogInfo("Suprema 2 Server All Commands", allCommands);
                    }
                    if (allCommands.IsCollectionNullOrEmpty())
                    {
                        token.WaitHandle.WaitOne(_config.CommandSetting.SleepBetweenSendsIfCommandNotExistsInMilliSeconds);
                        continue;
                    }

                    foreach (var command in allCommands)
                    {
                        try
                        {
                            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerCommandSend))
                            {
                                LoggingSystem.LogInfo("Suprema 2 Server Command for send",
                                    new
                                    {
                                        command.DeviceNumber,
                                        command.DeviceSerialNumber,
                                        command.CommandType,
                                        command.Id
                                    });
                            }

                            if (_connectedDeviceAdapters.TryGetValue(command.DeviceSerialNumber.ToUInt32(), out var deviceAdapter))
                            {
                                Thread.Sleep(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds);
                                switch (command.CommandType)
                                {
                                    case DeviceCommandTypeEnumeration.SetUserInfo:
                                        deviceAdapter.SetUserInfo(
                                            ObjectHelper.DeserializeAsJson<DtoEmployeeDeviceRelatedData>(
                                                command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.EnrollUserWithTemplate:
                                        deviceAdapter.SetUserInfoWithTemplate(
                                            ObjectHelper.DeserializeAsJson<DtoEmployeeDeviceRelatedData>(
                                                command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.DeleteUser:
                                        deviceAdapter.DeleteUserById(ObjectHelper
                                            .DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadUser:
                                        var commandRead =
                                            ObjectHelper.DeserializeAsJson<CommandReadUser>(command.CommandContent);
                                        var user = (deviceAdapter.GetUserById(commandRead.UserId,
                                            commandRead.TemplateType));

                                        if (user != null)
                                        {
                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                                new DtoDeviceCommandProcessingResult
                                                {
                                                    CommandResponseResult = "SUCCESS",
                                                    CommandResponseTime = DateTime.Now,
                                                    Id = command.Id
                                                });
                                            HardwareEventPublisher.Instance.PublishNewUserEnrolled(user,
                                                deviceAdapter.DeviceInfo.DeviceNumber,
                                                DtoEmployeeEnrolledSetting.GetAllSettingInstance());
                                        }
                                        else
                                        {
                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                                new DtoDeviceCommandProcessingResult
                                                {
                                                    CommandResponseResult = ServiceConstants.UserNotFoundCommandText,
                                                    CommandResponseTime = DateTime.Now,
                                                    Id = command.Id
                                                });
                                        }

                                        break;
                                    case DeviceCommandTypeEnumeration.ClearUser:
                                        deviceAdapter.DeleteAllUsers();
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ClearData:
                                        deviceAdapter.ClearData();
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.Reboot:
                                        deviceAdapter.RebootDevice();
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadAttendance:
                                        {
                                            if (!deviceAdapter.DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration
                                                    .DontSaveAttendance))
                                            {
                                                var attendances = deviceAdapter.GetData
                                                ((uint)ObjectHelper
                                                    .DeserializeAsJson<CommandLastLogId>(command.CommandContent).LastLogId);
                                                if (attendances.IsCollectionNotNullOrEmpty())
                                                {
                                                    foreach (var att in attendances)
                                                    {
                                                        HardwareEventPublisher.Instance.PublishAttendance(att);
                                                    }
                                                }
                                            }

                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                                new DtoDeviceCommandProcessingResult
                                                {
                                                    CommandResponseResult = "SUCCESS",
                                                    CommandResponseTime = DateTime.Now,
                                                    Id = command.Id
                                                });
                                        }
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadoutAttendance:
                                        {
                                            if (!deviceAdapter.DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration
                                                    .DontSaveAttendance))
                                            {
                                                var dateInterval =
                                                    ObjectHelper.DeserializeAsJson<CommandStartAndEndDate>(
                                                        command.CommandContent);
                                                var attendances = deviceAdapter.Readout(dateInterval.StartDate,
                                                    dateInterval.EndDate);
                                                if (attendances.IsCollectionNotNullOrEmpty())
                                                {
                                                    foreach (var att in attendances)
                                                    {
                                                        HardwareEventPublisher.Instance.PublishAttendance(att);
                                                    }
                                                }
                                            }

                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                                new DtoDeviceCommandProcessingResult
                                                {
                                                    CommandResponseResult = "SUCCESS",
                                                    CommandResponseTime = DateTime.Now,
                                                    Id = command.Id
                                                });
                                        }
                                        break;
                                    case DeviceCommandTypeEnumeration.AttendanceLogCount:
                                        var attendanceCount = deviceAdapter.GetRecordCount();
                                        HardwareEventPublisher.Instance.PublishAccessLogCountReceived(
                                            deviceAdapter.DeviceInfo.DeviceNumber, attendanceCount);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.FaceCount:
                                        var faceCount = deviceAdapter.GetFaceCount();
                                        HardwareEventPublisher.Instance.PublishFaceCountReceived(
                                            deviceAdapter.DeviceInfo.DeviceNumber, faceCount);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.FingerCount:
                                        var fingerCount = deviceAdapter.GetFingerCount();
                                        HardwareEventPublisher.Instance.PublishFingerCountReceived(
                                            deviceAdapter.DeviceInfo.DeviceNumber, fingerCount);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.UserCount:
                                        var userCount = deviceAdapter.GetUserCount();
                                        HardwareEventPublisher.Instance.PublishUserCountReceived(
                                            deviceAdapter.DeviceInfo.DeviceNumber, userCount);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SetDateAndTime:
                                        deviceAdapter.SetDateTime();
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanFace:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int>
                                            { command.Id });
                                        var face = deviceAdapter.ScanFace(ObjectHelper
                                            .DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face,
                                            deviceAdapter.DeviceInfo.DeviceNumber);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanFinger:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int>
                                            { command.Id });
                                        var scanFingerParams =
                                            ObjectHelper.DeserializeAsJson<CommandScanFinger>(
                                                command.CommandContent);
                                        var finger = deviceAdapter.ScanFinger(scanFingerParams.UserId,
                                            scanFingerParams.FingerIndex);
                                        HardwareEventPublisher.Instance.PublishNewFingerEnrolled(finger,
                                            deviceAdapter.DeviceInfo.DeviceNumber);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanCard:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int>
                                            { command.Id });
                                        var scanCardParams =
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        var cardNumber = deviceAdapter.ScanCard();
                                        HardwareEventPublisher.Instance.PublishNewCardEnrolled
                                        (cardNumber, deviceAdapter.DeviceInfo.DeviceNumber,
                                            scanCardParams.UserId);
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SupremaSdk2HolidayGroup:
                                        deviceAdapter.SendHolidays(ObjectHelper
                                            .DeserializeAsJson<List<DtoSupremaSdk2DeviceHolidayGroup>>(command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SupremaSdk2AccessSchedule:
                                        deviceAdapter.SetAccessSchedules(ObjectHelper
                                            .DeserializeAsJson<List<DtoSupremaSdk2AccessSchedule>>(command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SupremaSdk2AccessLevel:
                                        deviceAdapter.SetAccessLevels(ObjectHelper
                                            .DeserializeAsJson<List<DtoSupremaSdk2AccessLevel>>(command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SupremaSdk2AccessGroup:
                                        deviceAdapter.SetAccessGroups(ObjectHelper
                                            .DeserializeAsJson<List<DtoSupremaSdk2AccessGroup>>(command.CommandContent));
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    case DeviceCommandTypeEnumeration.SupremaSdk2DoorInfo:
                                        var doorInfo = ObjectHelper
                                            .DeserializeAsJson<DtoSupremaSdk2DeviceDoor>(command.CommandContent);
                                        deviceAdapter.SetDoorInfo(new List<DtoSupremaSdk2DeviceDoor> { doorInfo });
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                    default:
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(
                                            new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = ServiceConstants.NotSupportedCommandText,
                                                CommandResponseTime = DateTime.Now,
                                                Id = command.Id
                                            });
                                        break;
                                }
                            }
                            else
                            {
                                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerCommandSend))
                                {
                                    LoggingSystem.LogInfo("Suprema 2 Server Command fetched for device but not found for",
                                        new
                                        {
                                            ConnectedDeviceIds = _connectedDeviceAdapters.Keys.ToList(),
                                            command.DeviceNumber,
                                            command.DeviceSerialNumber,
                                            command.CommandType,
                                            command.Id
                                        });
                                }
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            return;
                        }
                        catch (OperationCannotBeDoneException exp)
                        {
                            if (exp.OperationResult.Errors.IsCollectionNotNullOrEmpty())
                            {
                                var error = exp.OperationResult.Errors.First();
                                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration
                                        .ServerCommandSendException))
                                {
                                    LoggingSystem.LogInfo("Suprema 2 Server Command Exception",
                                        new
                                        {
                                            command.DeviceNumber,
                                            command.DeviceSerialNumber,
                                            command.CommandType,
                                            command.Id
                                        });
                                }

                                if (error != OperationResultEnumeration.CommunicationStatusCannotConnect
                                    && error != OperationResultEnumeration
                                        .CommunicationStatusSupremaSdk2CannotFindDevice
                                    && error != OperationResultEnumeration
                                        .CommunicationStatusSupremaSdk2CannotConnectSocket
                                    && error != OperationResultEnumeration.CommunicationStatusSupremaSdk2Timeout
                                   )
                                {
                                    HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int>
                                        { command.Id });
                                }
                            }
                            else
                            {
                                // No error detail: consume the command (with a recorded description) so it isn't
                                // re-fetched and re-executed on every loop iteration (stuck command).
                                HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.Id });
                                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription
                                {
                                    Id = command.Id,
                                    Description = "Command failed without error detail",
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                }
            }
        }

        private void ReconnectDevice()
        {
            var token = _serverCts.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_disconnectedDevices.TryDequeue(out var deviceSerialNumber))
                    {
                        if (_connectedDeviceAdapters.TryGetValue(deviceSerialNumber, out var deviceAdapter))
                        {
                            var result = (BS2ErrorCode)ApiV2.BS2_ConnectDevice(_sdkContext, deviceAdapter.DeviceId);
                            if (result != BS2ErrorCode.BS_SDK_SUCCESS)
                            {
                                if (result != BS2ErrorCode.BS_SDK_ERROR_CANNOT_CONNECT_SOCKET)
                                {
                                    _connectedDeviceAdapters.TryRemove(deviceSerialNumber, out _);
                                    ApiV2.BS2_StopMonitoringLog(_sdkContext, deviceAdapter.DeviceId);
                                    deviceAdapter.Dispose();
                                }
                                else
                                {
                                    _disconnectedDevices.Enqueue(deviceSerialNumber);
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerReconnectError))
                    {
                        LoggingSystem.LogError(exp, "Suprema 2 Server Reconnect Exception");
                    }
                }
            }
        }

        private void RealtimeLogReceived(uint deviceId, IntPtr log)
        {
            try
            {
                if (log != IntPtr.Zero)
                {
                    var deviceInfo = _currentDeviceList.FirstOrDefault(d => d.SerialNumber == deviceId.ToString());
                    if (deviceInfo == null)
                    {
                        return;
                    }
                    var eventLog = (BS2Event)Marshal.PtrToStructure(log, typeof(BS2Event));
                    var eventType = SupremaV2Utility.GetEventType(eventLog.code);
                    if (eventType == SupremaSdk2EventTypeEnumeration.VerifySuccess)
                    {
                        if (deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                        {
                            return;
                        }
                        var att = SupremaV2Utility.ConvertBs2EventToDtoAttendance(
                            eventLog
                            , deviceInfo.DeviceNumber
                            , deviceInfo.TimeSetting
                            , DeviceAttendanceIoRetrieveTypeEnumeration.Push);
                        if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerRealTimeAttendance))
                        {
                            LoggingSystem.LogInfo("Suprema 2 Server realtime attendance", att);
                        }
                        HardwareEventPublisher.Instance.PublishAttendance(att);
                        return;
                    }
                    if (eventType == SupremaSdk2EventTypeEnumeration.UserChanged)
                    {
                        if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Suprema 2 Server realtime user changes", new { deviceInfo.DeviceNumber, UserId = Encoding.ASCII.GetString(eventLog.userID).ToInt64() });
                        }
                        //Convert.ToBoolean(eventLog.param) ? "Device" : "Server"
                        if (Convert.ToBoolean(eventLog.param))
                        {
                            HardwareEventPublisher.Instance.PublishUserChangedReceived(deviceInfo.DeviceNumber, Encoding.ASCII.GetString(eventLog.userID).ToInt64());
                        }
                    }

                    if (deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
                    {
                        return;
                    }
                    var operationLog = SupremaV2Utility.ConvertBs2EventToDtoDeviceEventLog(
                        eventLog, deviceInfo.DeviceNumber, deviceInfo.TimeSetting);
                    if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ServerRealTimeLog))
                    {
                        LoggingSystem.LogInfo("Suprema 2 Server realtime log", operationLog);
                    }
                    HardwareEventPublisher.Instance.PublishDeviceEventLogData(operationLog);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on RealtimeLogReceived  in SupremaSdk2Server",
                    $"Additional Data is {ObjectHelper.SerializeAsJsonFormatted(new { deviceId })}");
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
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Managed + native teardown only on explicit Dispose. A finalizer can't safely
                // release _sdkContext (it would require joining worker threads and risks the C1
                // use-after-free); as a process-lifetime singleton the OS reclaims it at exit.
                StopServer();
            }
            _disposed = true;
        }

        #endregion


    }

}
