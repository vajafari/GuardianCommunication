using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.Suprema
{
    public class SupremaSdk1Server : IDisposable
    {
        private SupremaSdk1ServerConfig _config;
        private CancellationTokenSource _serverCts;
        private readonly List<int> _disconnectedDeviceNumbers = new List<int>();
        private readonly List<DtoDevice> _currentDeviceList = new List<DtoDevice>();
        private readonly ConcurrentDictionary<uint, SupremaSdk1OnDemandAdapter> _connectedDeviceAdapters = new ConcurrentDictionary<uint, SupremaSdk1OnDemandAdapter>();
        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;
        private Thread _deviceCommandLoop;

        private readonly bool _useFunctionLock;
        private readonly bool _useAutoResponse;
        private readonly bool _useLock;
        //private bool _matchingFail;
        private BSSDK.BS_ConnectionProc _onDeviceConnected;
        private BSSDK.BS_DisconnectedProc _onDeviceDisconnected;
        private BSSDK.BS_RequestStartProc _onRequestStart;
        private BSSDK.BS_LogProc _onLogReceived;
        private readonly List<OperationResultEnumeration> _connectionError = new List<OperationResultEnumeration>
        {
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative100,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative101,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative102,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative103,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative104,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative105,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative106,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative107,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative200,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative201,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative202,
            OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative203,
        };

        // ReSharper disable CommentTypo
        //BSSDK.BS_ImageLogProc fnCallbackImageLog;
        //BSSDK.BS_RequestMatchingProc fnCallbackRequestMatching;
        //BSSDK.BS_RequestUserInfoProc fnCallbackRequestUserInfo;
        // ReSharper restore CommentTypo



        #region Singleton

        public static SupremaSdk1Server Instance { get; }

        private SupremaSdk1Server()
        {

            _useFunctionLock = false;
            _useAutoResponse = true;
            _useLock = false;
            //_connectionCount = 0;
            //_matchingFail = false;

        }

        static SupremaSdk1Server()
        {
            Instance = new SupremaSdk1Server();
        }

        #endregion


        public void StartServer(SupremaSdk1ServerConfig serverConfig
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , List<DtoDevice> deviceInfos)
        {
            _config = serverConfig;
            _actionToGetCommands = actionToGetCommands;
            LoggingSystem.LogInfo("Suprema SDK 1 config", serverConfig);
            var thread = new Thread(() => DelayedStartServer(deviceInfos)) { IsBackground = true };
            thread.Start();
        }

        public void StopServer()
        {
            BSSDK.BS_StopServerApp();
        }

        public void SetDeviceList(List<DtoDevice> deviceInfos)
        {
            lock (_currentDeviceList)
            {
                if (deviceInfos == null)
                {
                    deviceInfos = new List<DtoDevice>();
                }

                var forAdd = new List<DtoDevice>();
                var forUpdate = new List<DtoDevice>();
                var forDelete = new List<DtoDevice>();
                var pushDeviceInfos = deviceInfos.Where
                (row =>
                    row.ProducerNumber == ProducerEnumeration.Suprema
                    && row.SdkVersion == SdkVersionEnumeration.SdkVersion1
                    && row.ConnectionMode == DeviceConnectionModeEnumeration.Push).ToList();
                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerSetDeviceList))
                {
                    LoggingSystem.LogInfo("Suprema 1 Server device list for push", pushDeviceInfos);
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

        public List<Guid> GetConnectedDeviceNumbers()
        {
            var connectedDeviceSerialNumbers = _connectedDeviceAdapters.Keys;
            lock (_currentDeviceList)
            {
                lock (_disconnectedDeviceNumbers)
                {
                    return _currentDeviceList
                        .Where(d => connectedDeviceSerialNumbers.Contains(d.SerialNumber.ToUInt32())
                                    && !_disconnectedDeviceNumbers.Contains(d.DeviceNumber))
                        .Select(d => d.Id).ToList();
                }
            }
        }

        public SupremaSdk1OnDemandAdapter GetDeviceAdapter(Guid deviceId)
        {
            DtoDevice device;
            lock (_currentDeviceList)
            {
                device = _currentDeviceList.FirstOrDefault(d => d.Id == deviceId);

            }
            if (device != null)
            {
                _connectedDeviceAdapters.TryGetValue(device.SerialNumber.ToUInt32(), out var deviceAdapter);
                return deviceAdapter;
            }
            return null;
        }


        #region Private Methods

        private void DelayedStartServer(List<DtoDevice> deviceInfos)
        {
            SetDeviceList(deviceInfos);

            // به دلیل اینکه سرعت اتصال دستگاه ها بسیار بالا می باشد
            // می بایست یک مکس بکنیم و سپس سرور را اسارت نماییم
            Thread.Sleep(_config.StartDelayInSecond * 1000);
            try
            {

                //Set event procedure. 
                _onDeviceConnected = DeviceConnected;
                BSSDK.BS_SetConnectedCallback(_onDeviceConnected, _useFunctionLock, _useAutoResponse);

                _onDeviceDisconnected = DeviceDisconnected;
                BSSDK.BS_SetDisconnectedCallback(_onDeviceDisconnected, _useFunctionLock);

                _onRequestStart = RequestStart;
                BSSDK.BS_SetRequestStartedCallback(_onRequestStart, _useFunctionLock, _useAutoResponse);

                _onLogReceived = RealtimeLogReceived;
                BSSDK.BS_SetLogCallback(_onLogReceived, _useFunctionLock, _useAutoResponse);

                //fnCallbackImageLog = new BSSDK.BS_ImageLogProc(ImageLogProc);
                //BSSDK.BS_SetImageLogCallback(fnCallbackImageLog, m_UseFunctionLock, (bool)m_UseAutoResponse);

                //fnCallbackRequestUserInfo = new BSSDK.BS_RequestUserInfoProc(RequestUserInfoProc);
                //BSSDK.BS_SetRequestUserInfoCallback(fnCallbackRequestUserInfo, (bool)m_UseFunctionLock);

                //fnCallbackRequestMatching = new BSSDK.BS_RequestMatchingProc(RequestMatchingProc);
                //BSSDK.BS_SetRequestMatchingCallback(fnCallbackRequestMatching, (bool)m_UseFunctionLock);

                BSSDK.BS_SetSynchronousOperation(_useLock);

                var result = BSSDK.BS_StartServerApp(_config.ServerPort, _config.MaxConnections, "", "", BSSDK.KEEP_ALIVE_INTERVAL);
                if (result == BSSDK.BS_SUCCESS)
                {
                    LoggingSystem.LogInfo($"Suprema 1 Server  started Successfully at port {_config.ServerPort}");
                    _serverCts = new CancellationTokenSource();
                    _deviceCommandLoop = new Thread(DeviceCommandLoop) { IsBackground = true };
                    _deviceCommandLoop.Start();
                }
                else
                {
                    LoggingSystem.LogError($"Suprema Sdk1 server failed", $"Error code is {result}");
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }


        #region Device Events

        private int DeviceConnected(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            try
            {
                // ReSharper disable CommentTypo
                // این ویت به خاطر اینست که گاهی اوقات رویداد کانکت و دیسکانکت تقریبا همزمان با هم دریافت می شود
                // در کانکت ما صبر می کنیم تا در صورتی که رویداد دیسکانکت همزمان امده است اول پراسس بشود بعد رویداد کانکت پردازش شود
                // ReSharper restore CommentTypo

                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerConnection))
                {
                    LoggingSystem.LogInfo("Suprema 1 Server Device Connected", new
                    {
                        Handle = handle,
                        DeviceId = deviceId,
                        DeviceType = deviceType,
                        ConnectionType = connectionType,
                        FunctionType = functionType,
                        IpAddress = ipAddress
                    });
                }
                DtoDevice deviceInfo;
                lock (_currentDeviceList)
                {
                    deviceInfo = _currentDeviceList.FirstOrDefault(row => row.SerialNumber == deviceId.ToString());
                    if (deviceInfo == null)
                    {
                        if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerConnection))
                        {
                            LoggingSystem.LogInfo("Suprema 1 Server Invalid device connected", new { handle, deviceId, deviceType, connectionType, functionType, ipAddress });
                        }
                        return BSSDK.BS_ERR_TERMINAL_NOT_AUTHORIZED;
                    }
                }

                Thread.Sleep(1000);
                var result = BSSDK.BS_StartRequest(handle, deviceType, _config.ServerPort);
                if (result != BSSDK.BS_SUCCESS)
                {
                    if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerConnection))
                    {
                        LoggingSystem.LogInfo("Suprema 1 Server Cannot start monitoring", new
                        {
                            Result = result,
                            Handle = handle,
                            DeviceId = deviceId,
                            DeviceType = deviceType,
                            ConnectionType = connectionType,
                            FunctionType = functionType,
                            IpAddress = ipAddress
                        });
                    }
                    return BSSDK.BS_ERR_TERMINAL_NOT_AUTHORIZED;
                }

                if (_connectedDeviceAdapters.TryRemove(deviceId, out var deviceAdapter))
                {
                    deviceAdapter.Dispose();
                }
                deviceAdapter = new SupremaSdk1OnDemandAdapter(deviceInfo, handle, deviceId, (uint)deviceType);
                _connectedDeviceAdapters.TryAdd(deviceId, deviceAdapter);
                lock (_disconnectedDeviceNumbers)
                {
                    _disconnectedDeviceNumbers.RemoveAll(d => d == deviceInfo.DeviceNumber);
                }
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceId = deviceInfo.Id,
                        IsConnected = true
                    }
                });
                return BSSDK.BS_SUCCESS;
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on DeviceAccepted For SupremaSdk1Server",
                    $"Additional Data is {ObjectHelper.SerializeAsJson(new { deviceId })}");
            }
            return BSSDK.BS_SUCCESS;
        }

        private int DeviceDisconnected(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerConnection))
            {
                LoggingSystem.LogInfo("Suprema 1 Server Device removed from connected device", new
                {
                    Handle = handle,
                    DeviceId = deviceId,
                    DeviceType = deviceType,
                    ConnectionType = connectionType,
                    FunctionType = functionType,
                    IpAddress = ipAddress
                });
            }

            DtoDevice device;
            lock (_currentDeviceList)
            {
                device = _currentDeviceList.FirstOrDefault(row => row.SerialNumber == deviceId.ToString());
                if (device == null)
                {
                    if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerConnection))
                    {
                        LoggingSystem.LogInfo("Suprema 1 Server Invalid device disconnected",
                            new { handle, deviceId, deviceType, connectionType, functionType, ipAddress });
                    }
                    return BSSDK.BS_ERR_TERMINAL_NOT_AUTHORIZED;
                }
            }

            lock (_disconnectedDeviceNumbers)
            {
                _disconnectedDeviceNumbers.Add(device.DeviceNumber);
            }

            return BSSDK.BS_SUCCESS;
        }

        private int RequestStart(int handle, uint deviceId, int deviceType, int connectionType, int functionType, string ipAddress)
        {
            return BSSDK.BS_SUCCESS;
        }

        private int RealtimeLogReceived(int handle, uint deviceId, int deviceType, int connectionType, IntPtr data)
        {
            try
            {
                if (_connectedDeviceAdapters.TryGetValue(deviceId, out var deviceAdapter))
                {
                    var deviceInfo = deviceAdapter.DeviceInfo;
                    DtoAttendance attendanceRecord;
                    if (deviceType == BSSDK.BS_DEVICE_FSTATION ||
                        deviceType == BSSDK.BS_DEVICE_BIOSTATION2 ||
                        deviceType == BSSDK.BS_DEVICE_DSTATION ||
                        deviceType == BSSDK.BS_DEVICE_XSTATION)
                    {
                        var record = (BSLogRecordEx)Marshal.PtrToStructure(data, typeof(BSLogRecordEx));
                        var eventTime = DateTimeHelper.ConvertUnixTimestampToUtc((uint)record.eventTime);
                        switch (record.eventType)
                        {
                            case BSSDK.BE_EVENT_IDENTIFY_SUCCESS:
                            case BSSDK.BE_EVENT_VERIFY_SUCCESS:
                                if (record.userID <= 0)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveAttendance)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                attendanceRecord = new DtoAttendance
                                {
                                    LogIdOnDevice = 0,
                                    UserIdOnDevice = record.userID,
                                    AttendanceSource = AttendanceSourceEnumeration.Device,
                                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                                    AttendanceDateTime = eventTime,
                                    VerificationStyle = record.subEvent,
                                    DeviceId = deviceInfo.Id,
                                    LocationId = deviceInfo.LocationId,
                                    CameraId = null,
                                    StatusCode = record.tnaEvent,
                                    RfCardNumber = null,
                                    IsSentToGuardian = false,
                                };
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerRealTimeAttendance))
                                {
                                    LoggingSystem.LogInfo("Suprema 1 Server Attendance Record", attendanceRecord);
                                }
                                HardwareEventPublisher.Instance.PublishAttendance(attendanceRecord);
                                break;

                            //case BSSDK.BE_EVENT_ENROLL_SUCCESS:
                            default:
                                //HardwareEventPublisher.Instance.PublishUserChangedReceived(deviceInfo.DeviceNumber, record.userID);
                                if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveEvents)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                var eventLog = new DtoDeviceEventLog
                                {
                                    DeviceId = deviceInfo.Id,
                                    Id = 0,
                                    EventCode = record.eventType,
                                    EventDateTime = eventTime,
                                    UserIdOnDevice = record.userID > 0 ? (long?)record.userID : null,
                                    Producer = deviceInfo.ProducerNumber,
                                    SdkVersion = deviceInfo.SdkVersion,
                                    IsFromDevice = false
                                };
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerRealTimeLog))
                                {
                                    LoggingSystem.LogInfo("Suprema 1 Server Log Record", eventLog);
                                }
                                HardwareEventPublisher.Instance.PublishDeviceEventLogData(eventLog);
                                break;
                        }
                    }
                    else
                    {
                        var record = (BSLogRecord)Marshal.PtrToStructure(data, typeof(BSLogRecord));
                        var eventTime = DateTimeHelper.ConvertUnixTimestampToUtc((uint)record.eventTime);
                        switch (record.eventType)
                        {
                            case BSSDK.BE_EVENT_IDENTIFY_SUCCESS:
                            case BSSDK.BE_EVENT_VERIFY_SUCCESS:
                                if (record.userID <= 0)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveAttendance)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                attendanceRecord = new DtoAttendance
                                {
                                    LogIdOnDevice = 0,
                                    UserIdOnDevice = record.userID,
                                    CameraId = null,
                                    AttendanceSource = AttendanceSourceEnumeration.Device,
                                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                                    AttendanceDateTime = eventTime,
                                    VerificationStyle = record.subEvent,
                                    DeviceId = deviceInfo.Id,
                                    LocationId = deviceInfo.LocationId,
                                    StatusCode = record.tnaEvent,
                                    RfCardNumber = null,
                                };
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerRealTimeAttendance))
                                {
                                    LoggingSystem.LogInfo("Suprema 1 Server Attendance Record", attendanceRecord);
                                }
                                HardwareEventPublisher.Instance.PublishAttendance(attendanceRecord);
                                break;
                            //case BSSDK.BE_EVENT_ENROLL_SUCCESS:
                            default:
                                //HardwareEventPublisher.Instance.PublishUserChangedReceived(deviceInfo.DeviceNumber, record.userID);
                                if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveEvents)
                                {
                                    return BSSDK.BS_SUCCESS;
                                }
                                var eventLog = new DtoDeviceEventLog
                                {
                                    DeviceId = deviceInfo.Id,
                                    Id = 0,
                                    EventCode = record.eventType,
                                    EventDateTime = eventTime,
                                    UserIdOnDevice = record.userID > 0 ? (long?)record.userID : null,
                                    Producer = deviceInfo.ProducerNumber,
                                    SdkVersion = deviceInfo.SdkVersion,
                                    IsFromDevice = false
                                };
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerRealTimeLog))
                                {
                                    LoggingSystem.LogInfo("Suprema 1 Server Log Record", eventLog);
                                }
                                HardwareEventPublisher.Instance.PublishDeviceEventLogData(eventLog);
                                break;
                        }
                    }

                    lock (_disconnectedDeviceNumbers)
                    {
                        _disconnectedDeviceNumbers.RemoveAll(d => d == deviceInfo.DeviceNumber);
                    }
                }
                else
                {
                    if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerRealTimeAttendance))
                    {
                        LoggingSystem.LogInfo("Suprema 1 Server Log received from invalid device",
                            new
                            {
                                Handle = handle,
                                DeviceId = deviceId,
                                DeviceType = deviceType,
                                ConnectionType = connectionType,
                            });
                    }
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on RealtimeLogReceived for SupremaSdk1Server",
                    $"Additional Data is {ObjectHelper.SerializeAsJson(new { handle, deviceId, deviceType, connectionType, data })}");
            }
            return BSSDK.BS_SUCCESS;
        }

        #endregion


        #region Commands


        private void DeviceCommandLoop()
        {
            var token = _serverCts.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {

                    var deviceNumbers = GetConnectedDeviceNumbers();
                    var allCommands = new List<DtoDeviceUnsentCommand>();
                    if (deviceNumbers.IsCollectionNotNullOrEmpty())
                    {
                        var commandParams = new DeviceNotSentCommandsFilter
                        {
                            Count = 1,
                            DeviceNumbers = deviceNumbers,
                            Producer = ProducerEnumeration.Suprema,
                            SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        };
                        if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerCommandParams))
                        {
                            LoggingSystem.LogInfo("Suprema 1 Server Command Params", commandParams);
                        }
                        allCommands = _actionToGetCommands(commandParams);
                    }
                    if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerCommandResult))
                    {
                        LoggingSystem.LogInfo("Suprema 1 Server Command Result", allCommands);
                    }
                    if (allCommands.IsCollectionNullOrEmpty())
                    {
                        token.WaitHandle.WaitOne(_config.CommandSetting.SleepBetweenSendsIfCommandNotExistsInMilliSeconds);
                        continue;
                    }
                    foreach (var command in allCommands)
                    {
                        if (_connectedDeviceAdapters.TryGetValue(command.DeviceSerialNumber.ToUInt32(), out var deviceAdapter))
                        {
                            Thread.Sleep(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds);
                            try
                            {
                                var deviceInfo = deviceAdapter.DeviceInfo;
                                switch (command.CommandType)
                                {
                                    case DeviceCommandTypeEnumeration.SetUserInfo:
                                        deviceAdapter.SetUserInfo(ObjectHelper.DeserializeAsJson<DtoUserDeviceRelatedData>(command.CommandContent));
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.EnrollUserWithTemplate:
                                        deviceAdapter.SetUserInfoWithTemplate(ObjectHelper.DeserializeAsJson<DtoUserDeviceRelatedData>(command.CommandContent));
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.DeleteUser:
                                        deviceAdapter.DeleteUserById(ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadUser:
                                        var commandRead = ObjectHelper.DeserializeAsJson<CommandReadUser>(command.CommandContent);
                                        var user = deviceAdapter.GetUserInfoByUserId(commandRead.UserId, commandRead.TemplateType);
                                        if (user != null)
                                        {
                                            HardwareEventPublisher.Instance.PublishNewUserEnrolled(user, deviceAdapter.DeviceInfo.DeviceNumber, DtoUserEnrolledSetting.GetAllSettingInstance());
                                        }
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ClearUser:
                                        deviceAdapter.DeleteAllUsers();
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ClearData:
                                        deviceAdapter.ClearData();
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.Reboot:
                                        deviceAdapter.RebootDevice();
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadAttendance:
                                        {
                                            if (deviceAdapter.DeviceInfo == null || !deviceAdapter.DeviceInfo.DeviceSettings.DontSaveAttendance)
                                            {
                                                var startAndEnd =
                                                    ObjectHelper.DeserializeAsJson<CommandStartAndEndDate>(
                                                        command.CommandContent);
                                                var attendances = deviceAdapter.GetData(startAndEnd.StartDate,
                                                    startAndEnd.EndDate);
                                                if (attendances.IsCollectionNotNullOrEmpty())
                                                {
                                                    foreach (var att in attendances)
                                                    {
                                                        HardwareEventPublisher.Instance.PublishAttendance(att);
                                                    }
                                                }
                                            }
                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                NumericId = command.NumericId
                                            });
                                        }
                                        break;
                                    case DeviceCommandTypeEnumeration.ReadoutAttendance:
                                        {
                                            if (deviceAdapter.DeviceInfo == null || !deviceAdapter.DeviceInfo.DeviceSettings.DontSaveAttendance)
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

                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                NumericId = command.NumericId
                                            });
                                        }
                                        break;
                                    case DeviceCommandTypeEnumeration.AttendanceLogCount:
                                        {
                                            var startAndEnd = ObjectHelper.DeserializeAsJson<CommandStartAndEndDate>(command.CommandContent);
                                            var attendanceCount = deviceAdapter.GetRecordCount(startAndEnd.StartDate, startAndEnd.EndDate);
                                            HardwareEventPublisher.Instance.PublishAccessLogCountReceived(deviceAdapter.DeviceInfo.DeviceNumber, attendanceCount);
                                            HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                            {
                                                CommandResponseResult = "SUCCESS",
                                                CommandResponseTime = DateTime.Now,
                                                NumericId = command.NumericId
                                            });
                                        }
                                        break;
                                    case DeviceCommandTypeEnumeration.FaceCount:
                                        var faceCount = deviceAdapter.GetFaceCount();
                                        HardwareEventPublisher.Instance.PublishFaceCountReceived(deviceAdapter.DeviceInfo.DeviceNumber, faceCount);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.FingerCount:
                                        var fingerCount = deviceAdapter.GetFingerCount();
                                        HardwareEventPublisher.Instance.PublishFingerCountReceived(deviceAdapter.DeviceInfo.DeviceNumber, fingerCount);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.UserCount:
                                        var userCount = deviceAdapter.GetUserCount();
                                        HardwareEventPublisher.Instance.PublishUserCountReceived(deviceAdapter.DeviceInfo.DeviceNumber, userCount);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.SetDateAndTime:
                                        deviceAdapter.SetDateTime();
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanFace:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                        var face = deviceAdapter.ScanFace(ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face, deviceAdapter.DeviceInfo.DeviceNumber);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanFinger:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                        var scanFingerParams = ObjectHelper.DeserializeAsJson<CommandScanFinger>(command.CommandContent);
                                        var finger = deviceAdapter.ScanFinger(scanFingerParams.UserId, scanFingerParams.FingerIndex);
                                        HardwareEventPublisher.Instance.PublishNewFingerEnrolled(finger, deviceAdapter.DeviceInfo.DeviceNumber);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    case DeviceCommandTypeEnumeration.ScanCard:
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                        var scanCardParams = ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        var cardNumber = deviceAdapter.ScanCard();
                                        HardwareEventPublisher.Instance.PublishNewCardEnrolled(cardNumber, deviceAdapter.DeviceInfo.DeviceNumber, scanCardParams.UserId);
                                        ReportCommandSuccess("SUCCESS", command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                    default:
                                        ReportCommandSuccess(ServiceConstants.NotSupportedCommandText, command.NumericId, deviceInfo.DeviceNumber);
                                        break;
                                }
                            }
                            catch (ThreadAbortException)
                            {

                            }
                            catch (OperationCannotBeDoneException exp)
                            {
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerCommandException))
                                {
                                    LoggingSystem.LogInfo("Suprema 1 Server Command send exception", new
                                    {
                                        command.NumericId,
                                        command.DeviceNumber,
                                        command.CommandType,
                                        Exception = exp
                                    });
                                }
                                if (exp.OperationResult.Errors.IsCollectionNotNullOrEmpty())
                                {
                                    var error = exp.OperationResult.Errors.First();
                                    if (error == OperationResultEnumeration.CommunicationStatusNotSupport)
                                    {
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                        {
                                            CommandResponseResult = ServiceConstants.NotSupportedCommandText,
                                            CommandResponseTime = DateTime.Now,
                                            NumericId = command.NumericId,
                                        });
                                    }
                                    else if (command.CommandType == DeviceCommandTypeEnumeration.ReadUser
                                             && error == OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorCodeNegative306)
                                    {
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                        {
                                            CommandResponseResult = ServiceConstants.UserNotFoundCommandText,
                                            CommandResponseTime = DateTime.Now,
                                            NumericId = command.NumericId,
                                        });
                                    }
                                    else if (_connectionError.Contains(error))
                                    {
                                        if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ServerCommandConnectionException))
                                        {
                                            LoggingSystem.LogInfo("Suprema 1 Server Command connection exception", new
                                            {
                                                command.NumericId,
                                                command.DeviceNumber,
                                                command.CommandType,
                                                Exception = exp
                                            });
                                        }
                                        lock (_disconnectedDeviceNumbers)
                                        {
                                            _disconnectedDeviceNumbers.Add(deviceAdapter.DeviceInfo.DeviceNumber);
                                        }
                                    }
                                    else
                                    {
                                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                    }
                                }
                                else
                                {
                                    // No error detail: consume the command (with a recorded result) so it isn't
                                    // re-fetched and re-executed on every loop iteration (stuck command).
                                    HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.NumericId });
                                    HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                                    {
                                        CommandResponseResult = "Command failed without error detail",
                                        CommandResponseTime = DateTime.Now,
                                        NumericId = command.NumericId,
                                    });
                                }
                            }
                            catch (Exception exp)
                            {
                                LoggingSystem.LogError(exp);
                            }

                        }
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                }
            }
        }


        #endregion


        private void ReportCommandSuccess(string text, int commandId, int deviceNumber)
        {
            HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
            {
                CommandResponseResult = text,
                CommandResponseTime = DateTime.Now,
                NumericId = commandId,
            });
            lock (_disconnectedDeviceNumbers)
            {
                _disconnectedDeviceNumbers.RemoveAll(d => d == deviceNumber);
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
                // Managed teardown only on explicit Dispose — never on the finalizer thread.
                // Cooperative shutdown (replaces the no-op Thread.Abort of an always-null field):
                // stop the real command loop before tearing down adapters and the SDK server.
                //TODO: Ask about it
                _serverCts?.Cancel();
                if (!(_deviceCommandLoop?.Join(10000) ?? true))
                {
                    LoggingSystem.LogInfo("SupremaSdk1Server Dispose: command loop did not stop within timeout; continuing (background thread)");
                }

                lock (_connectedDeviceAdapters)
                {
                    foreach (var device in _connectedDeviceAdapters)
                    {
                        device.Value.Dispose();
                    }
                }
                StopServer();
                _serverCts?.Dispose();
                _serverCts = null;
            }
            _disposed = true;
        }

        #endregion


    }



}
