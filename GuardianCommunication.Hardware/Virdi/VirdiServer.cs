using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Virdi.VirdiConcepts;
using UCSAPICOMLib;

namespace GuardianCommunication.Hardware.Virdi
{

    [SuppressMessage("ReSharper", "UseIndexedProperty")]
    public class VirdiServer : IDisposable
    {
        // terminalId = DeviceNumber
        public const int CommandMode = 65000;
        private const int WalkThroughFaceIndex = 999;
        private const int WalkThroughImageType = 12;
        private const int WalkThroughTemplateType = 13;
        private const int MaxSizeForProfileImage = 10 * 1024;

        #region Variable
        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;
        private Func<DtoServerMatchData, DtoServerMatchResult> _serverMatchProcessor;
        private readonly ConcurrentDictionary<int, DtoCommunicationDeviceData> _deviceList = new ConcurrentDictionary<int, DtoCommunicationDeviceData>();
        private Thread _deviceCommandsThread;
        private CancellationTokenSource _serverCts;
        private readonly List<int> _connectedDevices = new List<int>();

        private UCSAPI _ucsApi;
        private ITerminalUserData _terminalUserData;
        private IAccessLogData _accessLogData;
        private IServerUserData _serverUserData;
        private IServerAuthentication _serverAuthentication;
        private IAccessControlData _accessControlData;
        private const int NTemplateType400 = 400;
        private bool _isVirdiServerStarted;
        private VirdiServerConfig _config;

        #endregion

        #region Singleton

        public static VirdiServer Instance { get; }

        private VirdiServer()
        {
        }

        static VirdiServer()
        {
            Instance = new VirdiServer();
        }

        #endregion


        public void StartVirdiServer(VirdiServerConfig config
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , Func<DtoServerMatchData, DtoServerMatchResult> serverMatchProcessor
            , List<DtoCommunicationDeviceData> deviceInfos)
        {

            if (_isVirdiServerStarted)
            {
                return;
            }
            LoggingSystem.LogInfo("Virdi Server started with config", config);
            _config = config;
            _actionToGetCommands = actionToGetCommands;
            _serverMatchProcessor = serverMatchProcessor;
            var thread = new Thread(() => DelayedStartServer(deviceInfos)) { IsBackground = true };
            thread.Start();
        }

        public void StopVirdiServer()
        {
            if (Instance == null) return;
            if (_ucsApi == null) return;
            try
            {
                // Cooperative shutdown (replaces Thread.Abort): signal the command loop first so it
                // stops touching the SDK, then join it after ServerStop below.
                _serverCts?.Cancel();
                _ucsApi.EventTerminalConnected -= uCSCOMObj_EventTerminalConnected;
                _ucsApi.EventTerminalDisconnected -= uCSCOMObj_EventTerminalDisconnected;
                _ucsApi.EventGetAccessLogCount -= ucsAPI_EventGetAccessLogCount;
                _ucsApi.EventGetAccessLog -= ucsAPI_EventGetAccessLog;
                _ucsApi.EventRealTimeAccessLog -= ucsAPI_EventRealTimeAccessLog;
                _ucsApi.EventGetUserInfoList -= ucsAPI_EventGetUserData;
                _ucsApi.EventGetUserData -= ucsAPI_EventGetUserData;
                _ucsApi.EventGetUserCount -= ucsAPI_EventGetUserCount;
                _ucsApi.EventDeleteAllUser -= ucsAPI_EventDeleteAllUser;
                _ucsApi.EventDeleteUser -= ucsAPI_EventDeleteUser;
                _ucsApi.EventAddUser -= _ucsApi_EventAddUser;
                _ucsApi.EventOpenDoor -= ucsAPI_EventOpenDoor;
                _ucsApi.EventRegistFace -= ucsAPI_EventRegistFace;
                _ucsApi.EventWalkThroughData -= ucsAPI_EventWalkThroughData;
                _ucsApi.EventRegistIris -= ucsAPI_EventRegistIris;
                //_ucsApi.EventGetUserInfoList -= ucsAPI_EventGetUserInfoList;
                _ucsApi.EventVerifyCard -= ucsAPI_EventVerifyCard;
                _ucsApi.EventVerifyPassword -= ucsAPI_EventVerifyPassword;
                _ucsApi.EventGetTerminalTime -= ucsAPI_OnEventGetTerminalTime;
                _ucsApi.EventSetAccessControlData -= ucsApi_OnEventSetAccessControlData;

                _ucsApi.ServerStop();
                if ((VirdiErrorEnum)_ucsApi.ErrorCode != VirdiErrorEnum.Success)
                {
                    LoggingSystem.LogError($"Error On Virdi Service Stop", $"Error code = {_ucsApi.ErrorCode}");
                }
                _isVirdiServerStarted = false;
                LoggingSystem.LogInfo("VirdiServer stopped");
                if (!(_deviceCommandsThread?.Join(10000) ?? true))
                {
                    LoggingSystem.LogInfo("VirdiServer StopVirdiServer: command loop did not stop within timeout; continuing (background thread)");
                }
                _serverCts?.Dispose();
                _serverCts = null;
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error On Virdi Service Stop");
            }
        }

        public void SetDeviceList(List<DtoCommunicationDeviceData> deviceInfos)
        {

            try
            {
                if (deviceInfos.IsCollectionNullOrEmpty())
                {
                    return;
                }

                var pushDeviceInfos = deviceInfos.Where
                    (row =>
                    row.ProducerEnum == ProducerEnumeration.Virdi
                    && row.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1).ToList();
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerSetDeviceList))
                {
                    LoggingSystem.LogInfo("Virdi Server set device list", pushDeviceInfos);
                }

                _deviceList.Clear();
                foreach (var device in pushDeviceInfos)
                {
                    _deviceList.TryAdd(device.DeviceNumber, device);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SetDeviceOnPushModeList");
            }
        }

        private void DelayedStartServer(List<DtoCommunicationDeviceData> deviceInfos)
        {
            SetDeviceList(deviceInfos);

            // به دلیل اینکه سرعت اتصال دستگاه ها بسیار بالا می باشد
            // می بایست یک مکس بکنیم و سپس سرور را اسارت نماییم
            Thread.Sleep(_config.StartDelayInSecond * 1000);

            try
            {
                _serverCts = new CancellationTokenSource();
                _deviceCommandsThread = new Thread(DeviceCommandLoop) { IsBackground = true };
                _deviceCommandsThread.Start();

                if (_ucsApi == null)
                {
                    _ucsApi = new UCSAPIClass();
                    _terminalUserData = _ucsApi.TerminalUserData as ITerminalUserData;
                    _accessLogData = _ucsApi.AccessLogData as IAccessLogData;
                    _serverUserData = _ucsApi.ServerUserData as IServerUserData;
                    _serverAuthentication = _ucsApi.ServerAuthentication as IServerAuthentication;
                    _accessControlData = _ucsApi.AccessControlData as IAccessControlData;

                }


                _ucsApi.ServerStart(9999, _config.ServerPort);
                if ((VirdiErrorEnum)_ucsApi.ErrorCode != VirdiErrorEnum.Success)
                {
                    LoggingSystem.LogError($"Error On Virdi Service Start", $"Error code = {_ucsApi.ErrorCode}");
                }
                else
                {
                    _ucsApi.EventTerminalConnected += uCSCOMObj_EventTerminalConnected;
                    _ucsApi.EventTerminalDisconnected += uCSCOMObj_EventTerminalDisconnected;
                    _ucsApi.EventGetAccessLogCount += ucsAPI_EventGetAccessLogCount;
                    _ucsApi.EventGetAccessLog += ucsAPI_EventGetAccessLog;
                    _ucsApi.EventRealTimeAccessLog += ucsAPI_EventRealTimeAccessLog;
                    _ucsApi.EventGetUserInfoList += ucsAPI_EventGetUserData;
                    _ucsApi.EventGetUserData += ucsAPI_EventGetUserData;
                    _ucsApi.EventGetUserCount += ucsAPI_EventGetUserCount;
                    _ucsApi.EventDeleteAllUser += ucsAPI_EventDeleteAllUser;
                    _ucsApi.EventDeleteUser += ucsAPI_EventDeleteUser;
                    _ucsApi.EventAddUser += _ucsApi_EventAddUser;
                    _ucsApi.EventOpenDoor += ucsAPI_EventOpenDoor;
                    _ucsApi.EventRegistFace += ucsAPI_EventRegistFace;
                    _ucsApi.EventWalkThroughData += ucsAPI_EventWalkThroughData;
                    _ucsApi.EventRegistIris += ucsAPI_EventRegistIris;
                    //_ucsApi.EventGetUserInfoList += ucsAPI_EventGetUserInfoList;
                    _ucsApi.EventVerifyCard += ucsAPI_EventVerifyCard;
                    _ucsApi.EventVerifyPassword += ucsAPI_EventVerifyPassword;
                    _ucsApi.EventGetTerminalTime += ucsAPI_OnEventGetTerminalTime;
                    _ucsApi.EventSetAccessControlData += ucsApi_OnEventSetAccessControlData;




                    _isVirdiServerStarted = true;
                    LoggingSystem.LogInfo($"Virdi Service start successfully at port {_config.ServerPort}");
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        #region Connection

        public List<int> GetConnectedDeviceNumbers()
        {
            lock (_connectedDevices)
            {
                // Return a snapshot copy, never the live list (callers enumerate off-lock).
                return _connectedDevices.ToList();
            }
        }

        private void uCSCOMObj_EventTerminalConnected(int terminalId, string terminalIp)
        {
            try
            {
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeviceConnection))
                {
                    LoggingSystem.LogInfo($"Virdi Server Terminal is {terminalId} ip {terminalIp} requested for connection");
                }

                var device = GetDeviceByTerminalId(terminalId);
                if (device == null)
                {
                    return;
                }
                lock (_connectedDevices)
                {
                    if (_connectedDevices.Contains(terminalId))
                    {
                        _connectedDevices.Remove(terminalId);
                    }
                    _connectedDevices.Add(terminalId);
                    if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeviceConnection))
                    {
                        LoggingSystem.LogInfo($"Virdi Server Terminal is {terminalId} & Connected with ip {terminalIp} connected successfully");
                    }
                }
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceNumber = terminalId,
                        IsConnected = true
                    }
                });
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Virdi Server Error on EventTerminalConnected Virdi Devices");
            }


        }

        private void uCSCOMObj_EventTerminalDisconnected(int terminalId)
        {
            try
            {
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeviceConnection))
                {
                    LoggingSystem.LogInfo($"Virdi Server Terminal {terminalId} is disconnected");
                }
                lock (_connectedDevices)
                {
                    if (_connectedDevices.Contains(terminalId))
                    {
                        _connectedDevices.Remove(terminalId);
                    }
                }
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceNumber = terminalId,
                        IsConnected = false
                    }
                });
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on EventTerminalDisconnected Virdi Devices");
            }
        }

        private void ucsAPI_OnEventGetTerminalTime(int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetTime))
            {
                LoggingSystem.LogInfo($"Virdi Server Terminal {terminalId} get time");
            }
            var now = DateTime.Now;
            _ucsApi.SetTerminalTime((short)now.Year
                , (byte)now.Month
                , (byte)now.Day
                , (byte)now.Hour
                , (byte)now.Minute
                , (byte)now.Second);
        }

        #endregion

        #region Commands

        private void DeviceCommandLoop()
        {
            var token = _serverCts.Token;
            while (!token.IsCancellationRequested)
            {
                // Cancelable pacing wait; exit promptly if a stop was requested.
                if (token.WaitHandle.WaitOne(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds))
                {
                    break;
                }
                try
                {
                    List<int> connectedDeviceNumbers;
                    lock (_connectedDevices)
                    {
                        // Snapshot once under the lock; used for both the emptiness check and the filter below.
                        connectedDeviceNumbers = _connectedDevices.ToList();
                    }

                    if (!connectedDeviceNumbers.IsCollectionNotNullOrEmpty())
                    {
                        token.WaitHandle.WaitOne(_config.CommandSetting.SleepBetweenSendsIfCommandNotExistsInMilliSeconds);
                        continue;
                    }
                    var commandFetchParams = new DeviceNotSentCommandsFilter
                    {
                        Count = 1,
                        DeviceNumbers = connectedDeviceNumbers,
                        Producer = ProducerEnumeration.Virdi,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                    };
                    if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerCommandFetch))
                    {
                        LoggingSystem.LogInfo("Virdi Server Command Fetch params", commandFetchParams);
                    }
                    var allCommands = _actionToGetCommands(commandFetchParams);
                    if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerCommandFetchResult))
                    {
                        LoggingSystem.LogInfo("Virdi Server Command Fetch result", allCommands.Select(c => new { c.Id, c.DeviceNumber, c.CommandType }));
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
                            var device = GetDeviceByTerminalId(command.DeviceNumber);
                            if (device == null)
                            {
                                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerCommandFetchStartSend))
                                {
                                    LoggingSystem.LogInfo("Virdi Server Command exists but no terminal found", new { command.DeviceNumber });
                                }
                                continue;
                            }
                            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerCommandFetchStartSend))
                            {
                                LoggingSystem.LogInfo("Virdi Server Command exists but no terminal found", new { device.DeviceNumber, command.CommandType });
                            }
                            var virdiError = VirdiErrorEnum.Success;
                            switch (command.CommandType)
                            {
                                case DeviceCommandTypeEnumeration.SetUserInfo:
                                case DeviceCommandTypeEnumeration.EnrollUserWithTemplate:
                                    {
                                        virdiError = AddUserSync(command.Id, command.DeviceNumber, device,
                                            ObjectHelper.DeserializeAsJson<DtoEmployeeDeviceRelatedData>(
                                                command.CommandContent));
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.DeleteUser:
                                    {
                                        virdiError = DeleteUserByIdAsync(command.Id, command.DeviceNumber,
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ReadUser:
                                    {
                                        virdiError = GetUserDataAsync
                                        (command.Id, command.DeviceNumber,
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ClearUser:
                                    {
                                        DeleteAllUserAsync(command.Id, command.DeviceNumber);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ScanFace:
                                    {

                                        var dbCommand =
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        virdiError = device.HasVisibleLight
                                            ? ScanVisiblelightFaceAsync(command.Id, command.DeviceNumber, dbCommand.UserId)
                                            : ScanFaceAsync(command.Id, command.DeviceNumber, dbCommand.UserId);
                                        Thread.Sleep(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds * 3);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ScanIris:
                                    {
                                        var dbCommand =
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        virdiError = ScanIrisAsync(command.Id, command.DeviceNumber, dbCommand.UserId);
                                        Thread.Sleep(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds * 3);
                                    }
                                    break;
                                // ReSharper disable CommentTypo
                                //case DeviceCommandTypeEnumeration.ScanFinger:
                                //    {
                                //        var dbCommand = ObjectHelper.DeserializeAsJson<CommandScanFinger>(command.CommandContent);
                                // اول ارسال کامند فرستاده می شود
                                // ولی باید چک کنیم ببینیم وصل هست یا خیر
                                // HardwareEventPublisher.Instance.PublishNewFingerEnrolled(resultOfScan.Item1, command.DeviceNumber);
                                //        var resultOfScan = ScanFingerSync(command.DeviceNumber, dbCommand.UserId, dbCommand.FingerIndex);
                                //        PublishCommandSendEvent(command.Id, result);
                                //        if (resultOfScan.Item2 == VirdiErrorEnum.Success)
                                //        {
                                //            HardwareEventPublisher.Instance.PublishNewFingerEnrolled(resultOfScan.Item1, command.DeviceNumber);
                                //        }
                                //    }
                                //    break;
                                // ReSharper restore CommentTypo
                                case DeviceCommandTypeEnumeration.ReadAttendance:
                                    {
                                        if (!device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                                        {
                                            virdiError = GetLogAsync(command.Id, command.DeviceNumber,
                                                ObjectHelper.DeserializeAsJson<VirdiGetDataCommand>(command.CommandContent)
                                                    .LogType);
                                        }
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ReadoutAttendance:
                                    {
                                        if (!device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                                        {
                                            var dbCommand =
                                                ObjectHelper.DeserializeAsJson<CommandStartAndEndDate>(
                                                    command.CommandContent);
                                            virdiError = GetDataByRangeDateAsync
                                            (command.Id, command.DeviceNumber, dbCommand.StartDate, dbCommand.EndDate,
                                                VirdiDeviceLogTypeEnum.Period);
                                        }
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.UserCount:
                                    {
                                        virdiError = GetUserCountAsync(command.Id, command.DeviceNumber);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.AttendanceLogCount:
                                    {
                                        virdiError = GetLogCountAsync
                                            (command.Id, command.DeviceNumber, VirdiDeviceLogTypeEnum.New);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.VirdiAccessControlData:
                                    {
                                        virdiError = SendAccessControlData(command.Id, command.DeviceNumber,
                                            ObjectHelper.DeserializeAsJson<DtoVirdiAccessControlData>(command.CommandContent));
                                    }
                                    break;
                                default:
                                    PublishResponseReceivedEvent(command.Id, "NOT SUPPORTED");
                                    break;
                            }

                            if (virdiError != VirdiErrorEnum.NotConnected &&
                                virdiError != VirdiErrorEnum.InvalidTerminal)
                            {
                                PublishMessageSentEvent(command.Id);
                            }

                            if (virdiError != VirdiErrorEnum.Success)
                            {
                                // دستگاه اعلام کرده است که عملیات ناموفق بوده پس باشد در توضیخات کامند ذکر شود

                                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription()
                                {
                                    Id = command.Id,
                                    Description = $"Error Code is = {virdiError.ToString()}",
                                });
                            }

                        }
                        catch (Exception exp)
                        {
                            PublishMessageSentEvent(command.Id);
                            // Record the failure instead of silently reporting the command as delivered.
                            HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription
                            {
                                Id = command.Id,
                                Description = $"Unknown Error. Message is {exp.GetFullExceptionMessage()}",
                            });
                            LoggingSystem.LogError(exp);
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

        #region Attendance

        public VirdiErrorEnum GetLogCountAsync(int clientId, int terminalId, VirdiDeviceLogTypeEnum logType)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetStatistics))
            {
                LoggingSystem.LogInfo("Virdi Server device statistics command", new { ClientId = clientId, TerminalId = terminalId, LogType = logType });
            }
            _accessLogData.GetAccessLogCountFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId, (int)logType);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventGetAccessLogCount(int clientId, int terminalId, int logCount)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetStatistics))
            {
                LoggingSystem.LogInfo("Virdi Server device statistics result", new { ClientId = clientId, TerminalId = terminalId, LogCount = logCount });
            }
            HardwareEventPublisher.Instance.PublishAccessLogCountReceived(terminalId, logCount);
            PublishResponseReceivedEvent(clientId);
        }

        public VirdiErrorEnum GetLogAsync(int clientId, int terminalId, VirdiDeviceLogTypeEnum logType)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetData))
            {
                LoggingSystem.LogInfo("Virdi Server device get data command", new { ClientId = clientId, TerminalId = terminalId, LogType = logType });
            }
            _accessLogData.GetAccessLogFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId, (int)logType);
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetData))
            {
                LoggingSystem.LogInfo("Virdi Server device get data result", new { ClientId = clientId, TerminalId = terminalId, Result = _ucsApi.ErrorCode });
            }
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        public VirdiErrorEnum GetDataByRangeDateAsync(int clientId, int terminalId, DateTime startDate, DateTime endDate, VirdiDeviceLogTypeEnum logType)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetData))
            {
                LoggingSystem.LogInfo("Virdi Server device get data by date range", new { ClientId = clientId, TerminalId = terminalId, LogType = logType, StartDate = startDate, EndDate = endDate });
            }
            _accessLogData.SetPeriod(startDate.Year, startDate.Month, startDate.Day, endDate.Year, endDate.Month, endDate.Day);
            _accessLogData.GetAccessLogFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId, (int)logType);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventGetAccessLog(int clientId, int terminalId)
        {

            lock (_accessLogData)
            {

                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetData))
                {
                    LoggingSystem.LogInfo("Virdi Server device get data device sent data", new { ClientId = clientId, TerminalId = terminalId, _accessLogData.UserID });
                }
                PublishResponseReceivedEvent(clientId);
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetData))
                {
                    LoggingSystem.LogInfo("Virdi Server device publish response to server", new { ClientId = clientId, TerminalId = terminalId, _accessLogData.UserID });
                }
                if (_accessLogData.UserID <= 0) return;
                try
                {
                    

                    if (_accessLogData.IsAuthorized != 1)
                    {
                        // تردد نامجاز
                        if (!IsInvalidSaveAttendanceActive(terminalId)) return;
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var currentRecord = new DtoInvalidAttendance
                        {
                            AttendanceDateTime = attendanceDate,
                            EmployeeNumber = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceNumber = terminalId,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            RfCardNumber = _accessLogData.RFID,
                            Reason = GetAuthFailReason(_accessLogData.AuthResult),
                            DoorId = null,
                        };
                        if (_accessLogData.PictureDataLength > 0)
                        {
                            currentRecord.Image = _accessLogData.PictureData as byte[];
                        }

                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log invalid received", currentRecord);
                        }

                        var deviceInList = GetDeviceByTerminalId(terminalId);
                        if (deviceInList == null ||
                            !deviceInList.DeviceSettings.HasFlag(DeviceSettingsEnumeration.ServerMatch))
                        {
                            HardwareEventPublisher.Instance.PublishInvalidAttendance(currentRecord);
                        }


                    }
                    else
                    {
                        if (!IsSaveAttendanceActive(terminalId)) return;
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var currentRecord = new DtoAttendance
                        {
                            AttendanceDateTime = attendanceDate,
                            EmployeeNumber = _accessLogData.UserID,
                            CameraId = null,
                            StatusCode = _accessLogData.AuthMode,
                            Id = 0,
                            DeviceNumber = terminalId,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            IsInvalid = false,
                            IsSent = false,
                            RfCardNumber = _accessLogData.RFID,
                        };
                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerAccessLog))
                        {
                            LoggingSystem.LogInfo("Virdi Server Virdi Access log received", currentRecord);
                        }

                        HardwareEventPublisher.Instance.PublishAttendance(currentRecord);

                        if (_accessLogData.PictureDataLength > 0)
                        {
                            HardwareEventPublisher.Instance.PublishAttendanceImage(new DtoDeviceAttendanceImage
                            {
                                DeviceNumber = terminalId,
                                EmployeeNumber = currentRecord.EmployeeNumber,
                                AttendanceDateTime = currentRecord.AttendanceDateTime,
                                Image = _accessLogData.PictureData as byte[]
                            });
                        }
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Virdi Server Error EventGetAccessLog");
                }
            }
        }


        private void ucsAPI_EventRealTimeAccessLog(int terminalId)
        {
            lock (_accessLogData)
            {
                if (_accessLogData.UserID <= 0) return;
                try
                {
                    if (_accessLogData.IsAuthorized != 1)
                    {
                        if (!IsInvalidSaveAttendanceActive(terminalId)) return;
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        
                        var currentRecord = new DtoInvalidAttendance
                        {
                            AttendanceDateTime = attendanceDate,
                            EmployeeNumber = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceNumber = terminalId,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            RfCardNumber = _accessLogData.RFID,
                            Reason = GetAuthFailReason(_accessLogData.AuthResult),
                            DoorId = null,
                        };
                        if (_accessLogData.PictureDataLength > 0)
                        {
                            currentRecord.Image = _accessLogData.PictureData as byte[];
                        }

                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log invalid received", currentRecord);
                        }

                        var deviceInList = GetDeviceByTerminalId(terminalId);
                        if (deviceInList == null ||
                            !deviceInList.DeviceSettings.HasFlag(DeviceSettingsEnumeration.ServerMatch))
                        {
                            HardwareEventPublisher.Instance.PublishInvalidAttendance(currentRecord);
                        }
                    }
                    else
                    {
                        // تردد مجاز
                        if (!IsSaveAttendanceActive(terminalId)) return;
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var currentRecord = new DtoAttendance
                        {
                            AttendanceDateTime = attendanceDate,
                            EmployeeNumber = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceNumber = terminalId,
                            CameraId = null,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            IsInvalid = false,
                            IsSent = false,
                            RfCardNumber = _accessLogData.RFID,

                        };
                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log valid received", currentRecord);
                        }

                        var deviceInList = GetDeviceByTerminalId(terminalId);
                        if (deviceInList == null ||
                            !deviceInList.DeviceSettings.HasFlag(DeviceSettingsEnumeration.ServerMatch))
                        {
                            HardwareEventPublisher.Instance.PublishAttendance(currentRecord);
                        }

                        if (_accessLogData.PictureDataLength > 0)
                        {
                            HardwareEventPublisher.Instance.PublishAttendanceImage(new DtoDeviceAttendanceImage
                            {
                                DeviceNumber = terminalId,
                                EmployeeNumber = currentRecord.EmployeeNumber,
                                AttendanceDateTime = currentRecord.AttendanceDateTime,
                                Image = _accessLogData.PictureData as byte[]
                            });
                        }
                    }


                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Virdi Server Error EventGetAccessLog");
                }
            }
        }

        private static AttendanceVerificationStyleEnumeration GetVerificationStyle(int code)
        {
            switch (code)
            {
                case 0:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case 1:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case 2:
                    return AttendanceVerificationStyleEnumeration.CardAndFinger;
                case 3:
                    return AttendanceVerificationStyleEnumeration.Card;
                case 4:
                    return AttendanceVerificationStyleEnumeration.IdAndPassword;
                case 5:
                    return AttendanceVerificationStyleEnumeration.Face;
                case 6:
                    return AttendanceVerificationStyleEnumeration.Face;
                case 8:
                    return AttendanceVerificationStyleEnumeration.Iris;
                case 9:
                    return AttendanceVerificationStyleEnumeration.IrisAndCard;
                default:
                    return AttendanceVerificationStyleEnumeration.Unknown;
            }
        }

        #endregion

        #region User

        public VirdiErrorEnum GetUserCountAsync(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetStatistics))
            {
                LoggingSystem.LogInfo("Virdi Server get user count command", new { ClientId = clientId, TerminalId = terminalId });
            }
            _terminalUserData.GetUserCountFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventGetUserCount(int clientId, int terminalId, int adminCount, int userCount)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetStatistics))
            {
                LoggingSystem.LogInfo("Virdi Server get user count result", new { ClientId = clientId, TerminalId = terminalId, AdminCount = adminCount, UserCount = userCount });
            }
            HardwareEventPublisher.Instance.PublishUserCountReceived(terminalId, userCount);
            PublishResponseReceivedEvent(clientId);
        }

        public VirdiErrorEnum GetUserDataAsync(int clientId, int terminalId, long userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
            {
                LoggingSystem.LogInfo("Virdi Server  get user data command", new { ClientId = clientId, TerminalId = terminalId, UserId = userId });
            }
            _terminalUserData.GetUserDataFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId, (int)userId);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventGetUserData(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
            {
                LoggingSystem.LogInfo("Virdi Server get user data result received", new { ClientId = clientId, TerminalId = terminalId });
            }
            PublishResponseReceivedEvent(clientId);
            var user = GetUserInfo(_terminalUserData, true);
            HardwareEventPublisher.Instance.PublishNewUserEnrolled(user, terminalId, DtoEmployeeEnrolledSetting.GetAllSettingInstance());
        }
        private DtoEmployeeDeviceRelatedData GetUserInfo(ITerminalUserData terminalUserData, bool addTemplateInfos)
        {

            lock (_terminalUserData)
            {
                var verificationStyle = VirdiVerificationStyleEnumeration.None;
                if (_terminalUserData.IsCard != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsCard;
                }
                if (_terminalUserData.IsFinger != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsFinger;
                }
                if (_terminalUserData.IsPassword != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsPassword;
                }
                if (_terminalUserData.IsAndOperation != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsAndOperation;
                }
                if (_terminalUserData.IsFace != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsFace;
                }
                if (_terminalUserData.IsIris  != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsIris;
                }
                
                var currentUser = new DtoEmployeeDeviceRelatedData
                {
                    EmployeeNumber = _terminalUserData.UserID,
                    Password = _terminalUserData.Password,
                    Privilege = _terminalUserData.IsAdmin,
                    UserName = _terminalUserData.UserName,
                    //IsEnable = _terminalUserData.IsIdentify == 1,
                    IsEnable = true,
                    VerificationStyle = (int)verificationStyle,
                    FingerDataList = new List<DtoEmployeeFinger>(),
                    FaceDataList = new List<DtoEmployeeFace>(),
                    RfCardNumbers = new List<string>(),
                };
                if (addTemplateInfos)
                {
                    var rfCardNumbers = new List<string>();
                    if (terminalUserData.CardNumber > 0)
                    {
                        for (var i = 0; i < terminalUserData.CardNumber; i++)
                        {
                            rfCardNumbers.Add(terminalUserData.get_RFID(i));
                        }
                    }
                    currentUser.RfCardNumbers = rfCardNumbers;

                    for (var i = 0; i < terminalUserData.TotalFingerCount; i++)
                    {
                        var fingerIndex = terminalUserData.get_FingerID(i);
                        //int fingerPrintDataSize1 = terminalUserData.get_FPSampleDataLength(fingerIndex, 0);
                        var fingerPrintData1 = terminalUserData.get_FPSampleData(fingerIndex, 0) as byte[];
                        //int fingerPrintDataSize2 = terminalUserData.get_FPSampleDataLength(fingerIndex, 1);
                        var fingerPrintData2 = terminalUserData.get_FPSampleData(fingerIndex, 1) as byte[];

                        var allTemplate = new byte[NTemplateType400 + NTemplateType400];
                        Buffer.BlockCopy(fingerPrintData1, 0, allTemplate, 0, NTemplateType400);
                        Buffer.BlockCopy(fingerPrintData2, 0, allTemplate, NTemplateType400, NTemplateType400);
                        var fingerData = new DtoEmployeeFinger
                        {
                            FingerIndex = fingerIndex,
                            TemplateData = allTemplate,
                            EmployeeNumber = terminalUserData.UserID,
                            //CheckSum = (uint)((NTemplateType400 * 100000)  + NTemplateType400)
                        };
                        currentUser.FingerDataList.Add(fingerData);
                    }

                    currentUser.FaceDataList = new List<DtoEmployeeFace>();
                    if (_terminalUserData.FaceNumber > 0)
                    {
                        var biFaceData = (byte[])terminalUserData.FaceData;
                        var currentFace = new DtoEmployeeFace
                        {
                            EmployeeNumber = terminalUserData.UserID,
                            Length = biFaceData.Length,
                            TemplateData = biFaceData,
                            FaceIndex = terminalUserData.FaceNumber,
                        };
                        currentUser.FaceDataList.Add(currentFace);
                    }

                    currentUser.IrisDataList = new List<DtoEmployeeIris>();
                    if (_terminalUserData.IrisDataLength > 0)
                    {
                        var biIrisData = (byte[])terminalUserData.IrisData;
                        var currentIris = new DtoEmployeeIris
                        {
                            EmployeeNumber = terminalUserData.UserID,
                            Length = biIrisData.Length,
                            TemplateData = biIrisData,
                        };
                        currentUser.IrisDataList.Add(currentIris);
                    }

                    if (_terminalUserData.WalkThroughLength > 0)
                    {
                        if (_terminalUserData.WalkThroughType == WalkThroughTemplateType)
                        {
                            currentUser.FaceDataList = new List<DtoEmployeeFace>
                            {
                                new DtoEmployeeFace
                                {
                                    Length = _terminalUserData.WalkThroughLength,
                                    EmployeeNumber =  terminalUserData.UserID,
                                    FaceIndex = WalkThroughFaceIndex,
                                    TemplateData = (byte[])_terminalUserData.WalkThroughData,
                                }
                            };
                            if (_terminalUserData.PictureDataLength > 0)
                            {
                                currentUser.VisibleLightImage = (byte[])_terminalUserData.PictureData;
                            }
                        }
                        else
                        {
                            if (_terminalUserData.WalkThroughType == WalkThroughImageType)
                            {
                                // This means template is JPG
                                currentUser.VisibleLightImage = (byte[])_terminalUserData.WalkThroughData;
                            }
                        }

                    }
                    if (_terminalUserData.PictureDataLength > 0)
                    {
                        currentUser.HardwareProfileImage = (byte[])_terminalUserData.PictureData;
                    }
                }
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
                {
                    LoggingSystem.LogInfo("Virdi Server get user data result", currentUser);
                }
                return currentUser;
            }
        }

        public VirdiErrorEnum DeleteUserByIdAsync(int clientId, int terminalId, long userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeleteUser))
            {
                LoggingSystem.LogInfo("Virdi Server delete user command", new { ClientId = clientId, TerminalId = terminalId, UserID = userId });
            }
            _terminalUserData.DeleteUserFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId, (int)userId);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventDeleteUser(int clientId, int terminalId, int userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeleteUser))
            {
                LoggingSystem.LogInfo("Virdi Server delete user result", new { ClientId = clientId, TerminalId = terminalId, UserID = userId });
            }
            PublishResponseReceivedEvent(clientId);
        }

        public VirdiErrorEnum DeleteAllUserAsync(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeleteUser))
            {
                LoggingSystem.LogInfo("Virdi Server delete all users command", new { ClientId = clientId, TerminalId = terminalId });
            }
            _terminalUserData.DeleteAllUserFromTerminal(ProcessClientIdBeforeSend(clientId), terminalId);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        private void ucsAPI_EventDeleteAllUser(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeleteUser))
            {
                LoggingSystem.LogInfo("Virdi Server delete all users result received", new { ClientId = clientId, TerminalId = terminalId });
            }
            PublishResponseReceivedEvent(clientId);
        }

        public VirdiErrorEnum AddUserSync(int clientId, int terminalId, DtoCommunicationDeviceData device, DtoEmployeeDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerSetUser))
            {
                LoggingSystem.LogInfo("Virdi Server all user command", new { ClientId = clientId, TerminalId = terminalId, DeviceNumber = device.DeviceNumber, User = userInfo });
            }
            lock (_serverUserData)
            {
                _serverUserData.InitUserData();
                _serverUserData.UserID = (int)userInfo.EmployeeNumber;
                _serverUserData.UniqueID = userInfo.EmployeeNumber.ToString();
                _serverUserData.UserName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(userInfo.UserName));
                _serverUserData.IsAdmin = userInfo.Privilege;
                _serverUserData.IsIdentify = 1;
                _serverUserData.IsFace1toN = 1;
                _serverUserData.AuthType = 0;
                _serverUserData.IsBlacklist = Convert.ToInt32(!userInfo.IsEnable);
                var startDate = userInfo.StartTime;
                var endDate = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime, ProducerEnumeration.Virdi, SdkVersionEnumeration.SdkVersion1);
                _serverUserData.SetAccessDate(1,
                    startDate.Year, startDate.Month, startDate.Day,
                    endDate.Year, endDate.Month, endDate.Day);
                if (userInfo.VirdiAccessGroupCode.IsNotNullOrEmpty())
                {
                    _serverUserData.AccessGroup = userInfo.VirdiAccessGroupCode;
                }
                var verificationStyleEnum = (VirdiVerificationStyleEnumeration)userInfo.VerificationStyle;
                _serverUserData.SetAuthType(
                    Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsAndOperation)),
                    device.HasFinger ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsFinger)) : 0,
                    0,
                    Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsPassword)),
                    device.HasRfCard ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsCard)) : 0,
                    0);
                _serverUserData.SetAuthTypeEx(
                    device.HasFace ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsFace)) : 0
                    , 0
                    , 0
                    , device.HasIris ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsIris)) : 0
                    , 0
                    , 0
                    , 0
                    , 0);
                if (device.HasRfCard && userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                {
                    var rfCardNumbers = userInfo.RfCardNumbers.Distinct().ToList();
                    for (var i = 0; i < rfCardNumbers.Count; i++)
                    {
                        _serverUserData.SetCardData(i == 0 ? 1 : 0, rfCardNumbers[i]);
                    }
                }
                if (verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsPassword)
                    && userInfo.Password.IsNotNullOrEmpty())
                {
                    _serverUserData.Password = userInfo.Password;
                }

                // Set Iris data
                if (device.HasIris)
                {
                    if (userInfo.IrisDataList.IsCollectionNotNullOrEmpty())
                    {
                        _serverUserData.IsIris1toN = 1;
                        var iris = userInfo.IrisDataList.First();
                        _serverUserData.SetIrisData(iris.TemplateData.Length, iris.TemplateData);
                    }
                }

                // Set Finger data
                if (device.HasFinger)
                {
                    if (userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
                    {

                        _serverUserData.IsCheckSimilarFinger = 0;
                        foreach (var currentFingerPrint in userInfo.FingerDataList)
                        {
                            var biFpData1 = new byte[NTemplateType400];
                            var biFpData2 = new byte[NTemplateType400];
                            Buffer.BlockCopy(currentFingerPrint.TemplateData, 0, biFpData1, 0, NTemplateType400);
                            Buffer.BlockCopy(currentFingerPrint.TemplateData, NTemplateType400, biFpData2, 0, NTemplateType400);
                            _serverUserData.AddFingerData(currentFingerPrint.FingerIndex, NTemplateType400, biFpData1, biFpData2);
                        }
                        //_serverUserData.SetDuressFinger(1, 1);
                    }
                }

                // Set face data
                if (device.HasFace)
                {
                    if (device.HasVisibleLight)
                    {
                        var walkThroughTemplate =
                            userInfo.FaceDataList.FirstOrDefault(f => f.FaceIndex == WalkThroughFaceIndex);
                        if (walkThroughTemplate != null)
                        {
                            _serverUserData.SetWalkThroughData
                                (WalkThroughTemplateType, walkThroughTemplate.Length, walkThroughTemplate.TemplateData);
                        }
                        else if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                        {
                            var imageForSend = userInfo.VisibleLightImage;
                            imageForSend = ImageHelper.ResizeImageByDimensions(imageForSend, _config.MaxVisibleLightImageSizeWidth, _config.MaxVisibleLightImageSizeHeight);
                            imageForSend = ImageHelper.ReduceImageSize(imageForSend, _config.MaxVisibleLightImageSizeInKb);
                            _serverUserData.SetWalkThroughData
                                (WalkThroughImageType, imageForSend.Length, imageForSend);
                        }
                    }
                    else
                    {
                        if (userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                        {
                            var firstFace =
                                userInfo.FaceDataList.FirstOrDefault(f => f.FaceIndex != WalkThroughFaceIndex);
                            if (firstFace != null)
                            {
                                _serverUserData.FaceNumber = firstFace.FaceIndex;
                                _serverUserData.FaceData = firstFace.TemplateData;
                            }
                        }
                    }
                }



                //// Set profile image data
                //// برای دستگاه های ویزیبل عکس کاربری با عکس ویزیبل یکسان است
                //if (device.HasFace
                //    && device.HasVisibleLight
                //    && userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty()
                //    && userInfo.VisibleLightImage.Length < MaxSizeForProfileImage)
                //{
                //    var picture = new byte[MaxSizeForProfileImage];
                //    for (var i = 0; i < userInfo.VisibleLightImage.Length; i++)
                //    {
                //        picture[i] = userInfo.VisibleLightImage[i];
                //    }
                //    _serverUserData.SetPictureData(userInfo.VisibleLightImage.Length, "JPG", picture);
                //}
                //else 

                if (device.SendProfileImage
                    && userInfo.HardwareProfileImage.IsCollectionNotNullOrEmpty()
                    && userInfo.HardwareProfileImage.Length <= MaxSizeForProfileImage)
                {
                    var picture = new byte[MaxSizeForProfileImage];
                    for (var i = 0; i < userInfo.HardwareProfileImage.Length; i++)
                    {
                        picture[i] = userInfo.HardwareProfileImage[i];
                    }
                    _serverUserData.SetPictureData(userInfo.HardwareProfileImage.Length, "JPG", picture);
                }

                const int isOverwrite = 1;
                _serverUserData.AddUserToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, isOverwrite);
                var resultOfSetUser = (VirdiErrorEnum)_ucsApi.ErrorCode;
                if (resultOfSetUser != VirdiErrorEnum.Success)
                {
                    return resultOfSetUser;
                }

                return resultOfSetUser;
            }
        }
        private void _ucsApi_EventAddUser(int clientId, int terminalId, int userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerSetUser))
            {
                LoggingSystem.LogInfo("Virdi Server add user result", new { ClientId = clientId, TerminalId = terminalId, UserId = userId, Error = _ucsApi.EventError.ToString() });
            }
            if ((VirdiErrorEnum)_ucsApi.EventError == VirdiErrorEnum.Success)
            {
                PublishResponseReceivedEvent(clientId);
            }
            else
            {
                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription()
                {
                    Id = clientId,
                    Mode = CommandMode,
                    Description = $"Error Code is = {_ucsApi.EventError.ToString()}",
                });
            }

        }

        public Tuple<DtoEmployeeFinger, VirdiErrorEnum> ScanFingerSync(int terminalId, long userId, int fingerIndex)
        {
            _ucsApi.EnrollFromTerminal(0, terminalId);

            if (_ucsApi.ErrorCode == (int)VirdiErrorEnum.Success)
            {
                var templateIndex = 0;
                var nFingerId = _ucsApi.get_FingerID(fingerIndex);
                var fingerTemplate1 = _ucsApi.get_FPSampleData(nFingerId, (int)templateIndex) as byte[];
                var fingerTemplate2 = _ucsApi.get_FPSampleData(nFingerId, (int)templateIndex + 1) as byte[];

                var allTemplate = new byte[NTemplateType400 + NTemplateType400];
                Buffer.BlockCopy(fingerTemplate1, 0, allTemplate, 0, NTemplateType400);
                Buffer.BlockCopy(fingerTemplate2, 0, allTemplate, NTemplateType400, NTemplateType400);
                return new Tuple<DtoEmployeeFinger, VirdiErrorEnum>(new DtoEmployeeFinger
                {
                    EmployeeNumber = userId,
                    FingerIndex = fingerIndex,
                    TemplateData = allTemplate,
                    CheckSum = 0,
                }, VirdiErrorEnum.Success);
            }
            else
            {
                return new Tuple<DtoEmployeeFinger, VirdiErrorEnum>(null, (VirdiErrorEnum)_ucsApi.ErrorCode);
            }
        }

        readonly List<ScanWalkThroughData> _scanWalkThroughData = new List<ScanWalkThroughData>();
        public VirdiErrorEnum ScanVisiblelightFaceAsync(int clientId, int terminalId, long userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
            {
                LoggingSystem.LogInfo("Virdi Server scan visiblelight face command", new { ClientId = clientId, TerminalId = terminalId, UserId = userId });
            }
            var newClientId = ProcessClientIdBeforeSend(clientId);
            _scanWalkThroughData.Add(new ScanWalkThroughData
            {
                Date = DateTime.Now,
                EmployeeNumber = userId,
                ClientId = newClientId,
            });
            _terminalUserData.RegistWalkThroughFaceFromTerminal(newClientId, terminalId, 0); //opt: 0=jpg & template, 1=jpg, 2=template
            return VirdiErrorEnum.Success;
        }
        void ucsAPI_EventWalkThroughData(int clientId, int terminalId, int userId, int dataType, int dataLength, object eventData)
        {
            try
            {
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
                {
                    LoggingSystem.LogInfo("Virdi Server scan visiblelight face result", new { ClientId = clientId, TerminalId = terminalId, UserId = userId, DataType = dataType, DataLength = dataLength });
                }
                PublishResponseReceivedEvent(clientId);
                if (dataType == WalkThroughImageType)
                {
                    var walkThroughData = (byte[])eventData;
                    if (walkThroughData.IsCollectionNullOrEmpty())
                    {
                        return;
                    }

                    var scanWalkThroughData = _scanWalkThroughData.FirstOrDefault(row => row.ClientId == clientId);
                    if (scanWalkThroughData != null)
                    {
                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoEmployeeFace
                        {
                            EmployeeNumber = scanWalkThroughData.EmployeeNumber,
                            FaceIndex = 1,
                            Length = walkThroughData.Length,
                            TemplateData = walkThroughData
                        }, terminalId);
                        _scanFaceCommands.RemoveAll(row => row.ClientId == clientId);
                    }
                }
                else if (dataType == WalkThroughTemplateType)
                {
                    var walkThroughData = (byte[])eventData;
                    if (walkThroughData.IsCollectionNullOrEmpty())
                    {
                        return;
                    }

                    var scanWalkThroughData = _scanWalkThroughData.FirstOrDefault(row => row.ClientId == clientId);
                    if (scanWalkThroughData != null)
                    {
                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoEmployeeFace
                        {
                            EmployeeNumber = scanWalkThroughData.EmployeeNumber,
                            FaceIndex = WalkThroughFaceIndex,
                            Length = walkThroughData.Length,
                            TemplateData = walkThroughData
                        }, terminalId);
                        _scanFaceCommands.RemoveAll(row => row.ClientId == clientId);
                    }
                }


            }
            finally
            {
                _scanFaceCommands.RemoveAll(row => row.Date <= DateTime.Now.AddHours(-6));
            }

        }

        readonly List<ScanFaceData> _scanFaceCommands = new List<ScanFaceData>();
        public VirdiErrorEnum ScanFaceAsync(int clientId, int terminalId, long userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
            {
                LoggingSystem.LogInfo("Virdi Server scan face command", new { ClientId = clientId, TerminalId = terminalId, UserId = userId });
            }
            var newClientId = ProcessClientIdBeforeSend(clientId);
            _scanFaceCommands.Add(new ScanFaceData
            {
                Date = DateTime.Now,
                EmployeeNumber = userId,
                ClientId = newClientId,
            });
            _terminalUserData.RegistFaceFromTerminal(newClientId, terminalId, 0);
            return VirdiErrorEnum.Success;
        }
        void ucsAPI_EventRegistFace(int clientId, int terminalId, int currentIndex, int totalNumber, object eventData)
        {
            try
            {
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
                {
                    LoggingSystem.LogInfo("Virdi Server scan face result", new { ClientId = clientId, TerminalId = terminalId, CurrentIndex = currentIndex, TotalNumber = totalNumber });
                }
                PublishResponseReceivedEvent(clientId);
                var regFaceData = (byte[])eventData;
                if (currentIndex == 0 && totalNumber == 0)
                {
                    return;
                }
                var scanFaceData = _scanFaceCommands.FirstOrDefault(row => row.ClientId == clientId);
                if (scanFaceData != null)
                {
                    if (currentIndex == 1)
                    {
                        scanFaceData.Face = new FaceData
                        {
                            FaceNum = totalNumber,
                            FaceBlock = new FaceBlock[totalNumber]
                        };
                    }
                    scanFaceData.Face.FaceBlock[currentIndex - 1] = new FaceBlock
                    {
                        Length = regFaceData.Length,
                        Data = new byte[regFaceData.Length]
                    };
                    scanFaceData.Face.FaceBlock[currentIndex - 1].Data = regFaceData;

                    if (currentIndex == totalNumber)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var binaryWriter = new BinaryWriter(memoryStream))
                            {
                                for (var i = 0; i < totalNumber; i++)
                                {
                                    binaryWriter.Write(scanFaceData.Face.FaceBlock[i].Length);
                                    binaryWriter.Write(scanFaceData.Face.FaceBlock[i].Data);
                                }
                            }
                            var bytes = memoryStream.ToArray();
                            HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoEmployeeFace
                            {
                                EmployeeNumber = scanFaceData.EmployeeNumber,
                                FaceIndex = totalNumber,
                                Length = bytes.Length,
                                TemplateData = bytes
                            }, terminalId);
                        }
                        _scanFaceCommands.RemoveAll(row => row.ClientId == clientId);
                    }
                }
            }
            finally
            {
                _scanFaceCommands.RemoveAll(row => row.Date <= DateTime.Now.AddHours(-6));
            }
        }


        readonly List<ScanIrisData> _scanIrisCommands = new List<ScanIrisData>();
        public VirdiErrorEnum ScanIrisAsync(int clientId, int terminalId, long userId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
            {
                LoggingSystem.LogInfo("Virdi Server scan Iris command", new { ClientId = clientId, TerminalId = terminalId, UserId = userId });
            }
            var newClientId = ProcessClientIdBeforeSend(clientId);
            _scanIrisCommands.Add(new ScanIrisData
            {
                Date = DateTime.Now,
                EmployeeNumber = userId,
                ClientId = newClientId,
            });
            _terminalUserData.RegistIrisFromTerminal(newClientId, terminalId, 0);
            return VirdiErrorEnum.Success;
        }
        void ucsAPI_EventRegistIris(int clientId, int terminalId, int currentIndex, int totalNumber, object eventData)
        {
            try
            {
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerScan))
                {
                    LoggingSystem.LogInfo("Virdi Server scan Iris result", new { ClientId = clientId, TerminalId = terminalId, CurrentIndex = currentIndex, TotalNumber = totalNumber, EventData = eventData });
                }
                PublishResponseReceivedEvent(clientId);
                var walkThroughData = (byte[])eventData;
                if (walkThroughData.IsCollectionNullOrEmpty())
                {
                    return;
                }

                var irisData = _scanIrisCommands.FirstOrDefault(row => row.ClientId == clientId);
                if (irisData != null)
                {
                    HardwareEventPublisher.Instance.PublishNewIrisEnrolled(new DtoEmployeeIris()
                    {
                        EmployeeNumber = irisData.EmployeeNumber,
                        Length = walkThroughData.Length,
                        TemplateData = walkThroughData
                    }, terminalId);
                    _scanIrisCommands.RemoveAll(row => row.ClientId == clientId);
                }
            }
            finally
            {
                _scanIrisCommands.RemoveAll(row => row.Date <= DateTime.Now.AddHours(-6));
            }
        }

        
        private readonly AutoResetEvent _userDataListWaitHandle = new AutoResetEvent(false);
        private List<DtoEmployeeDeviceRelatedData> _userDataListSync = new List<DtoEmployeeDeviceRelatedData>();
        public List<DtoEmployeeDeviceRelatedData> GetDeviceUserIdsSync(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
            {
                LoggingSystem.LogInfo("Virdi Server get user ids command", new { ClientId = clientId, TerminalId = terminalId });
            }
            lock (_connectedDevices)
            {
                if (!_connectedDevices.Contains(terminalId))
                {
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
                }
            }
            lock (_userDataListSync)
            {
                _userDataListSync = new List<DtoEmployeeDeviceRelatedData>();
                _terminalUserData.GetUserInfoListFromTerminal(clientId, terminalId);
                _userDataListWaitHandle.WaitOne(new TimeSpan(0, 0, 0, _config.SyncOperationTimeout));
                var resultOfLogCount = (VirdiErrorEnum)_ucsApi.ErrorCode;
                if (resultOfLogCount != VirdiErrorEnum.Success)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult
                        ((int)resultOfLogCount, ProducerEnumeration.Virdi, SdkVersionEnumeration.SdkVersion1));
                }
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
                {
                    LoggingSystem.LogInfo("Virdi Server get user ids result", _userDataListSync);
                }
                return _userDataListSync;
            }
        }

        private void ucsAPI_EventGetUserInfoList(int clientId, int terminalId)
        {

            _userDataListSync.Add(GetUserInfo(_terminalUserData, false));
            if (_terminalUserData.CurrentIndex == _terminalUserData.TotalNumber)
            {
                _userDataListWaitHandle.Set();
            }
        }

        #endregion

        #region Access control

        public VirdiErrorEnum OpenDoor(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.DoorControl))
            {
                LoggingSystem.LogInfo("Virdi Server open door command", new { ClientId = clientId, TerminalId = terminalId });
            }
            _ucsApi.OpenDoorToTerminal(ProcessClientIdBeforeSend(clientId), terminalId);
            return (VirdiErrorEnum)_ucsApi.ErrorCode;
        }
        void ucsAPI_EventOpenDoor(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.DoorControl))
            {
                LoggingSystem.LogInfo("Virdi Server open door result", new { ClientId = clientId, TerminalId = terminalId });
            }
        }


        public VirdiErrorEnum SendAccessControlData(int clientId, int terminalId, DtoVirdiAccessControlData accessControlData)
        {
            lock (_accessControlData)
            {
                _accessControlData.InitData();

                #region Holiday

                if (accessControlData.AccessDataType.HasFlag(VirdiAccessControlDataTypeEnumeration.Holiday))
                {
                    if (accessControlData.Holidays.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var item in accessControlData.Holidays)
                        {
                            _accessControlData.SetHoliday(item.GroupCode, item.Index, item.Month, item.Day);
                        }
                    }
                }

                #endregion

                #region TimeZone

                if (accessControlData.AccessDataType.HasFlag(VirdiAccessControlDataTypeEnumeration.TimeZone))
                {
                    if (accessControlData.Timezones.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var item in accessControlData.Timezones)
                        {
                            _accessControlData.SetTimeZone(item.Code, item.Index, item.StartHour, item.StartMinute,
                                item.EndHour, item.EndMinute);
                        }
                    }
                }

                #endregion

                #region AccessTimes

                if (accessControlData.AccessDataType.HasFlag(VirdiAccessControlDataTypeEnumeration.AccessTimes))
                {
                    if (accessControlData.AccessTimes.IsCollectionNotNullOrEmpty())
                    {
                        var codes = accessControlData.AccessTimes.Select(at => at.Code).Distinct().ToList();
                        foreach (var code in codes)
                        {
                            var currentCodeAccessTimes =
                                accessControlData.AccessTimes.Where(at => at.Code == code).ToList();
                            var sunday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Sunday);
                            var monday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Monday);
                            var tuesday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Tuesday);
                            var wednesday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Wednesday);
                            var thursday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Thursday);
                            var friday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Friday);
                            var saturday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Saturday);
                            var holiday =
                                currentCodeAccessTimes.FirstOrDefault(i =>
                                    i.DayOfWeekEnum == VirdiDayOfWeekEnumeration.Holiday);
                            _accessControlData.SetAccessTime(code
                                , sunday?.TimezoneCode ?? null
                                , monday?.TimezoneCode ?? null
                                , tuesday?.TimezoneCode ?? null
                                , wednesday?.TimezoneCode ?? null
                                , thursday?.TimezoneCode ?? null
                                , friday?.TimezoneCode ?? null
                                , saturday?.TimezoneCode ?? null
                                , holiday?.TimezoneCode ?? null
                                , holiday?.HolidayCode ?? null
                            );
                        }
                    }
                }

                #endregion

                #region AccessGroup

                if (accessControlData.AccessDataType.HasFlag(VirdiAccessControlDataTypeEnumeration.AccessGroup))
                {
                    if (accessControlData.AccessGroups.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var item in accessControlData.AccessGroups)
                        {
                            _accessControlData.SetAccessGroup(item.Code, item.Index, item.AccessTimeCode);
                        }
                    }
                }

                #endregion
                _accessControlData.SetAccessControlDataToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, 0);
                _accessControlData.SetAccessControlDataToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, 1);
                _accessControlData.SetAccessControlDataToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, 2);
                _accessControlData.SetAccessControlDataToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, 3);

                return (VirdiErrorEnum)_ucsApi.ErrorCode;
            }

        }

        private void ucsApi_OnEventSetAccessControlData(int clientId, int terminalId, int datatype)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.SendAccessControlData))
            {
                LoggingSystem.LogInfo("Virdi send access control data", new { ClientId = clientId, TerminalId = terminalId, DataType = datatype });
            }
            PublishResponseReceivedEvent(clientId);
        }


        #endregion

        #region Match On Server

        private void ucsAPI_EventVerifyPassword(int terminalId, int userId, int authMode, int antiPassBackLevel, string password)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.MatchOnServer))
            {
                LoggingSystem.LogInfo("Virdi Server Match on server password received", new { UserId = userId, TerminalId = terminalId, AuthMode = authMode, AntiPassBackLevel = antiPassBackLevel, Password = password });
            }
            var now = DateTime.Now;
            var authorizationResult = _serverMatchProcessor(new DtoServerMatchData
            {
                MatchType = ServerMatchingTypeEnumeration.Password,
                DeviceNumber = terminalId,
                UserId = userId,
                Password = password,
                EventDateTime = now,
            });
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.MatchOnServer))
            {
                LoggingSystem.LogInfo("Virdi Server Match on server password result", new
                {
                    AuthResult = authorizationResult,
                    Password = password,
                    TerminalId = terminalId,
                    AuthMode = authMode,
                    AntiPassBackLevel = antiPassBackLevel
                });
            }
            var isAuthorized = authorizationResult.IsSuccessfullyProcessed ? 1 : 0;
            // ReSharper disable PossibleInvalidOperationException
            _serverAuthentication.SendAuthResultToTerminal(terminalId, (int)authorizationResult.UserId, 1, 0, isAuthorized, now.ToString("yyyy-MM-dd hh:mm:ss"), 0);
            // ReSharper restore PossibleInvalidOperationException

        }

        private void ucsAPI_EventVerifyCard(int terminalId, int authMode, int antiPassBackLevel, string rfidNumber)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.MatchOnServer))
            {
                LoggingSystem.LogInfo("Virdi Server Match on server card received", new { RfidNumber = rfidNumber, TerminalId = terminalId, AuthMode = authMode, AntiPassBackLevel = antiPassBackLevel });
            }

            var now = DateTime.Now;

            var authorizationResult = _serverMatchProcessor(new DtoServerMatchData
            {
                DeviceNumber = terminalId,
                RfCardNumber = rfidNumber,
                MatchType = ServerMatchingTypeEnumeration.Card,
                EventDateTime = now,
            });
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.MatchOnServer))
            {
                LoggingSystem.LogInfo("Virdi Server Match on server card result", new
                {
                    AuthResult = authorizationResult,
                    RfidNumber = rfidNumber,
                    TerminalId = terminalId,
                    AuthMode = authMode,
                    AntiPassBackLevel = antiPassBackLevel
                });
            }

            var isAuthorized = authorizationResult.IsSuccessfullyProcessed ? 1 : 0;
            _serverAuthentication.SetAuthType(1, 0, 0, 0, 1, 0);
            _serverAuthentication.SetAuthTypeEx(0, 0, 0, 0, 0, 0, 0, 0);
            // ReSharper disable PossibleInvalidOperationException
            _serverAuthentication.SendAuthResultToTerminal(terminalId, (int)authorizationResult.UserId, 1, 0, isAuthorized, now.ToString("yyyy-MM-dd hh:mm:ss"), 0);
            // ReSharper restore PossibleInvalidOperationException
        }

        #endregion

        #region Utilities

        //private bool IsDeviceConnected(int terminalId)
        //{
        //    return FindDevice(terminalId) != null;
        //}

        public void PublishMessageSentEvent(int commandId)
        {
            HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { commandId });
        }

        public void PublishResponseReceivedEvent(int commandId, string commandResponse = "SUCCESS")
        {
            HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
            {
                CommandResponseResult = commandResponse,
                CommandResponseTime = DateTime.Now,
                Id = commandId,
                Mode = CommandMode,
            });
        }

        public static int ProcessClientIdBeforeSend(int clientId)
        {
            return clientId % CommandMode;
        }

        public DtoCommunicationDeviceData GetDeviceByTerminalId(int terminalId)
        {
            _deviceList.TryGetValue(terminalId, out var deviceAdapter);
            return deviceAdapter;
        }


        public bool IsSaveAttendanceActive(int terminalId)
        {
            var deviceInfo = GetDeviceByTerminalId(terminalId);
            if (deviceInfo == null)
            {
                return true;
            }
            return !deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance);
        }

        public bool IsInvalidSaveAttendanceActive(int terminalId)
        {
            var deviceInfo = GetDeviceByTerminalId(terminalId);
            if (deviceInfo == null)
            {
                return true;
            }
            return !deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveInvalidAttendance);
        }


        private InvalidAttendanceReasonEnumeration GetAuthFailReason(int authResult)
        {
            switch (authResult)
            {
                case 1:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfInvalidUser;
                case 3:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfPermission;
                case 6:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfAntiPassback;
                case 10:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfBlacklist;
                case 14:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfExpired;
                default:
                    return InvalidAttendanceReasonEnumeration.UnAuthorize;


            }
        }

        #endregion

        #region Types

        private class ScanWalkThroughData
        {
            public long EmployeeNumber { get; set; }
            public DateTime Date { get; set; }
            public int ClientId { get; set; }
        }

        private class ScanFaceData
        {
            public FaceData Face { get; set; }
            public long EmployeeNumber { get; set; }
            public DateTime Date { get; set; }
            public int ClientId { get; set; }
        }

        private class ScanIrisData
        {
            public long EmployeeNumber { get; set; }
            public DateTime Date { get; set; }
            public int ClientId { get; set; }
        }

        private class FaceBlock
        {
            public int Length;
            public byte[] Data;
        }

        private class FaceData
        {
            public int FaceNum;
            public FaceBlock[] FaceBlock;
        }

        #endregion

        #region Implementation of IDisposable

        private bool _disposed;

        /// <inheritdoc />
        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public void Dispose()
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
                // Managed/COM teardown only on explicit Dispose — never on the finalizer thread.
                StopVirdiServer();
                _userDataListWaitHandle?.Dispose();
            }
            _disposed = true;
        }

        #endregion

    }
}