using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AccessControl.TimeHandling;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Virdi.VirdiConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;
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
        private readonly List<DtoDevice> _deviceList = new List<DtoDevice>();
        private Thread _deviceCommandsThread;
        private CancellationTokenSource _serverCts;
        private readonly List<Guid> _connectedDeviceIds = new List<Guid>();

        private UCSAPI _ucsApi;
        private ITerminalUserData _terminalUserData;
        private IAccessLogData _accessLogData;
        private IServerUserData _serverUserData;
        private IServerAuthentication _serverAuthentication;
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
            , List<DtoDevice> deviceInfos)
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
                _ucsApi.EventGetTerminalTime -= ucsAPI_OnEventGetTerminalTime;

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

        public void SetDeviceList(List<DtoDevice> deviceInfos)
        {

            try
            {
                if (deviceInfos.IsCollectionNullOrEmpty())
                {
                    return;
                }

                var pushDeviceInfos = deviceInfos.Where
                    (row =>
                    row.ProducerNumber == ProducerEnumeration.Virdi
                    && row.SdkVersion == SdkVersionEnumeration.SdkVersion1).ToList();
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerSetDeviceList))
                {
                    LoggingSystem.LogInfo("Virdi Server set device list", pushDeviceInfos);
                }

                lock (_deviceList)
                {
                    _deviceList.Clear();
                    _deviceList.AddRange(pushDeviceInfos);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SetDeviceOnPushModeList");
            }
        }

        private void DelayedStartServer(List<DtoDevice> deviceInfos)
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

                }


                _ucsApi.ServerStart(9999, _config.ServerPort);
                if ((VirdiErrorEnum)_ucsApi.ErrorCode != VirdiErrorEnum.Success)
                {
                    LoggingSystem.LogError("Error On Virdi Service Start", $"Error code = {_ucsApi.ErrorCode}");
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
                    _ucsApi.EventGetTerminalTime += ucsAPI_OnEventGetTerminalTime;

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

        public List<Guid> GetConnectedDeviceNumbers()
        {
            lock (_connectedDeviceIds)
            {
                // Return a snapshot copy, never the live list (callers enumerate off-lock).
                return _connectedDeviceIds.ToList();
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


                lock (_connectedDeviceIds)
                {
                    if (_connectedDeviceIds.Contains(device.Id))
                    {
                        _connectedDeviceIds.Remove(device.Id);
                    }
                    _connectedDeviceIds.Add(device.Id);
                    if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerDeviceConnection))
                    {
                        LoggingSystem.LogInfo($"Virdi Server Terminal is {terminalId} & Connected with ip {terminalIp} connected successfully");
                    }
                }
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceId = device.Id,
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
                var device = GetDeviceByTerminalId(terminalId);
                if (device == null)
                {
                    return;
                }

                lock (_connectedDeviceIds)
                {
                    if (_connectedDeviceIds.Contains(device.Id))
                    {
                        _connectedDeviceIds.Remove(device.Id);
                    }
                }
                HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                {
                    new DtoDeviceConnectionStatus
                    {
                        DeviceId = device.Id,
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

            var deviceInfo = GetDeviceByTerminalId(terminalId);
            if (deviceInfo == null)
            {
                return;
            }

            var timeService = new DeviceTimeService();
            var deviceTime = timeService.UtcToDeviceTime(DateTime.UtcNow, deviceInfo.IanaTimeZoneId);
            _ucsApi.SetTerminalTime((short)deviceTime.Year
                , (byte)deviceTime.Month
                , (byte)deviceTime.Day
                , (byte)deviceTime.Hour
                , (byte)deviceTime.Minute
                , (byte)deviceTime.Second);
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
                    List<Guid> connectedDeviceIds;
                    lock (_connectedDeviceIds)
                    {
                        connectedDeviceIds = _connectedDeviceIds.ToList();
                    }

                    if (!connectedDeviceIds.IsCollectionNotNullOrEmpty())
                    {
                        token.WaitHandle.WaitOne(_config.CommandSetting.SleepBetweenSendsIfCommandNotExistsInMilliSeconds);
                        continue;
                    }
                    var commandFetchParams = new DeviceNotSentCommandsFilter
                    {
                        Count = 1,
                        DeviceIds = connectedDeviceIds,
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
                        LoggingSystem.LogInfo("Virdi Server Command Fetch result", allCommands.Select(c => new { c.DeviceId, c.DeviceNumber, c.CommandType }));
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
                                        virdiError = AddUserSync(command.NumericId, command.DeviceNumber, device,
                                            ObjectHelper.DeserializeAsJson<DtoUserDeviceRelatedData>(
                                                command.CommandContent));
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.DeleteUser:
                                    {
                                        virdiError = DeleteUserByIdAsync(command.NumericId, command.DeviceNumber,
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ReadUser:
                                    {
                                        virdiError = GetUserDataAsync
                                        (command.NumericId, command.DeviceNumber,
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent).UserId);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ClearUser:
                                    {
                                        DeleteAllUserAsync(command.NumericId, command.DeviceNumber);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ScanFace:
                                    {

                                        var dbCommand =
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        virdiError = device.HasVisiblelight
                                            ? ScanVisiblelightFaceAsync(command.NumericId, command.DeviceNumber, dbCommand.UserId)
                                            : ScanFaceAsync(command.NumericId, command.DeviceNumber, dbCommand.UserId);
                                        Thread.Sleep(_config.CommandSetting.WaitBetweenCommandSendInMilliseconds * 3);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ScanIris:
                                    {
                                        var dbCommand =
                                            ObjectHelper.DeserializeAsJson<CommandUserId>(command.CommandContent);
                                        virdiError = ScanIrisAsync(command.NumericId, command.DeviceNumber, dbCommand.UserId);
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
                                //        PublishCommandSendEvent(command.NumericId, result);
                                //        if (resultOfScan.Item2 == VirdiErrorEnum.Success)
                                //        {
                                //            HardwareEventPublisher.Instance.PublishNewFingerEnrolled(resultOfScan.Item1, command.DeviceNumber);
                                //        }
                                //    }
                                //    break;
                                // ReSharper restore CommentTypo
                                case DeviceCommandTypeEnumeration.ReadAttendance:
                                    {
                                        if (device.DeviceSettings == null || !device.DeviceSettings.DontSaveAttendance)
                                        {
                                            virdiError = GetLogAsync(command.NumericId, command.DeviceNumber,
                                                ObjectHelper.DeserializeAsJson<VirdiGetDataCommand>(command.CommandContent)
                                                    .LogType);
                                        }
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.ReadoutAttendance:
                                    {
                                        if (device.DeviceSettings == null || !device.DeviceSettings.DontSaveAttendance)
                                        {
                                            var dbCommand =
                                                ObjectHelper.DeserializeAsJson<CommandStartAndEndDate>(
                                                    command.CommandContent);
                                            virdiError = GetDataByRangeDateAsync
                                            (command.NumericId, command.DeviceNumber, dbCommand.StartDate, dbCommand.EndDate,
                                                VirdiDeviceLogTypeEnum.Period);
                                        }
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.UserCount:
                                    {
                                        virdiError = GetUserCountAsync(command.NumericId, command.DeviceNumber);
                                    }
                                    break;
                                case DeviceCommandTypeEnumeration.AttendanceLogCount:
                                    {
                                        virdiError = GetLogCountAsync
                                            (command.NumericId, command.DeviceNumber, VirdiDeviceLogTypeEnum.New);
                                    }
                                    break;
                                default:
                                    PublishResponseReceivedEvent(command.NumericId, "NOT SUPPORTED");
                                    break;
                            }

                            if (virdiError != VirdiErrorEnum.NotConnected &&
                                virdiError != VirdiErrorEnum.InvalidTerminal)
                            {
                                PublishMessageSentEvent(command.NumericId);
                            }

                            if (virdiError != VirdiErrorEnum.Success)
                            {
                                // دستگاه اعلام کرده است که عملیات ناموفق بوده پس باشد در توضیخات کامند ذکر شود
                                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription()
                                {
                                    NumericId = command.NumericId,
                                    Description = $"Error Code is = {virdiError.ToString()}",
                                });
                            }

                        }
                        catch (Exception exp)
                        {
                            PublishMessageSentEvent(command.NumericId);
                            // Record the failure instead of silently reporting the command as delivered.
                            HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription
                            {
                                NumericId = command.NumericId,
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

            var deviceInfo = GetDeviceByTerminalId(terminalId);
            if (deviceInfo == null)
            {
                return VirdiErrorEnum.NotConnected;
            }
            var timeService = new DeviceTimeService();
            var startDateProcessed = timeService.UtcToDeviceTime
                (startDate.ToUniversalTime(), deviceInfo.IanaTimeZoneId);
            var endDateProcessed = timeService.UtcToDeviceTime
                (endDate.ToUniversalTime(), deviceInfo.IanaTimeZoneId);

            _accessLogData.SetPeriod(startDateProcessed.Year, startDateProcessed.Month, startDateProcessed.Day, endDateProcessed.Year, endDateProcessed.Month, endDateProcessed.Day);
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
                    var deviceInList = GetDeviceByTerminalId(terminalId);
                    if (deviceInList == null)
                    {
                        return;
                    }

                    if (_accessLogData.IsAuthorized != 1)
                    {
                        // تردد نامجاز

                        if (deviceInList.DeviceSettings != null && deviceInList.DeviceSettings.DontSaveInvalidAttendance)
                        {
                            return;
                        }
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var timeService = new DeviceTimeService();
                        var attendanceDateProcessed = timeService.DeviceTimeToUtc(attendanceDate, deviceInList.IanaTimeZoneId);

                        var currentRecord = new DtoInvalidAttendance
                        {
                            AttendanceDateTime = attendanceDateProcessed,
                            UserIdOnDevice = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceId = deviceInList.Id,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            RfCardNumber = _accessLogData.RFID,
                            Reason = GetAuthFailReason(_accessLogData.AuthResult),
                            Image = _accessLogData.PictureDataLength > 0 ? _accessLogData.PictureData as byte[] : null,
                        };
                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log invalid received", currentRecord);
                        }
                        HardwareEventPublisher.Instance.PublishInvalidAttendance(currentRecord);

                    }
                    else
                    {
                        if (deviceInList.DeviceSettings != null && deviceInList.DeviceSettings.DontSaveAttendance)
                        {
                            return;
                        }
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var timeService = new DeviceTimeService();
                        var attendanceDateProcessed = timeService.DeviceTimeToUtc(attendanceDate, deviceInList.IanaTimeZoneId);
                        var currentRecord = new DtoAttendance
                        {
                            AttendanceDateTime = attendanceDateProcessed,
                            UserIdOnDevice = _accessLogData.UserID,
                            CameraId = null,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceId = deviceInList.Id,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            IsSentToGuardian = false,
                            RfCardNumber = _accessLogData.RFID,
                            IoType = deviceInList.IoType,
                            ModuleId = deviceInList.ModuleId,
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
                                DeviceId = deviceInList.Id,
                                UserIdOnDevice = currentRecord.UserIdOnDevice,
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
                    var deviceInList = GetDeviceByTerminalId(terminalId);
                    if (deviceInList == null)
                    {
                        return;
                    }
                    if (_accessLogData.IsAuthorized != 1)
                    {
                        // تردد نامجاز

                        if (deviceInList.DeviceSettings != null && deviceInList.DeviceSettings.DontSaveInvalidAttendance)
                        {
                            return;
                        }
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var timeService = new DeviceTimeService();
                        var attendanceDateProcessed = timeService.DeviceTimeToUtc(attendanceDate, deviceInList.IanaTimeZoneId);
                        var currentRecord = new DtoInvalidAttendance
                        {
                            AttendanceDateTime = attendanceDateProcessed,
                            UserIdOnDevice = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceId = deviceInList.Id,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            RfCardNumber = _accessLogData.RFID,
                            Reason = GetAuthFailReason(_accessLogData.AuthResult),
                            DoorId = null,
                            Image = _accessLogData.PictureDataLength > 0 ? _accessLogData.PictureData as byte[] : null,
                        };
                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log invalid received", currentRecord);
                        }

                    }
                    else
                    {
                        // تردد مجاز
                        if (deviceInList.DeviceSettings != null && deviceInList.DeviceSettings.DontSaveAttendance)
                        {
                            return;
                        }
                        var attendanceDate = DateTime.Parse(_accessLogData.DateTime, new CultureInfo("en-US"));
                        var timeService = new DeviceTimeService();
                        var attendanceDateProcessed = timeService.DeviceTimeToUtc(attendanceDate, deviceInList.IanaTimeZoneId);
                        var currentRecord = new DtoAttendance
                        {
                            AttendanceDateTime = attendanceDateProcessed,
                            UserIdOnDevice = _accessLogData.UserID,
                            StatusCode = _accessLogData.AuthMode,
                            DeviceId = deviceInList.Id,
                            CameraId = null,
                            VerificationStyle = (int)GetVerificationStyle(_accessLogData.AuthType),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            IsSentToGuardian = false,
                            RfCardNumber = _accessLogData.RFID,
                            IoType = deviceInList.IoType,
                            ModuleId = deviceInList.ModuleId,

                        };
                        if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerRealTimeLog))
                        {
                            LoggingSystem.LogInfo("Virdi real time Access log valid received", currentRecord);
                        }
                        HardwareEventPublisher.Instance.PublishAttendance(currentRecord);

                        if (_accessLogData.PictureDataLength > 0)
                        {
                            HardwareEventPublisher.Instance.PublishAttendanceImage(new DtoDeviceAttendanceImage
                            {
                                DeviceId = deviceInList.Id,
                                UserIdOnDevice = currentRecord.UserIdOnDevice,
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
            HardwareEventPublisher.Instance.PublishNewUserEnrolled(user, terminalId, DtoUserEnrolledSetting.GetAllSettingInstance());
        }
        private DtoUserDeviceRelatedData GetUserInfo(ITerminalUserData terminalUserData, bool addTemplateInfos)
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
                if (_terminalUserData.IsIris != 0)
                {
                    verificationStyle |= VirdiVerificationStyleEnumeration.IsIris;
                }

                var currentUser = new DtoUserDeviceRelatedData()
                {
                    UserIdOnDevice = _terminalUserData.UserID,
                    Password = _terminalUserData.Password,
                    Privilege = _terminalUserData.IsAdmin,
                    UserName = _terminalUserData.UserName,
                    //IsEnable = _terminalUserData.IsIdentify == 1,
                    IsEnable = true,
                    VerificationStyle = (int)verificationStyle,
                    FingerDataList = new List<DtoUserFinger>(),
                    FaceDataList = new List<DtoUserFace>(),
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
                        var fingerData = new DtoUserFinger
                        {
                            FingerIndex = fingerIndex,
                            TemplateData = allTemplate,
                            UserIdOnDevice = terminalUserData.UserID,
                            //CheckSum = (uint)((NTemplateType400 * 100000)  + NTemplateType400)
                        };
                        currentUser.FingerDataList.Add(fingerData);
                    }

                    currentUser.FaceDataList = new List<DtoUserFace>();
                    if (_terminalUserData.FaceNumber > 0)
                    {
                        var biFaceData = (byte[])terminalUserData.FaceData;
                        var currentFace = new DtoUserFace
                        {
                            UserIdOnDevice = terminalUserData.UserID,
                            Length = biFaceData.Length,
                            TemplateData = biFaceData,
                            FaceIndex = terminalUserData.FaceNumber,
                        };
                        currentUser.FaceDataList.Add(currentFace);
                    }

                    currentUser.IrisDataList = new List<DtoUserIris>();
                    if (_terminalUserData.IrisDataLength > 0)
                    {
                        var biIrisData = (byte[])terminalUserData.IrisData;
                        var currentIris = new DtoUserIris
                        {
                            UserIdOnDevice = terminalUserData.UserID,
                            Length = biIrisData.Length,
                            TemplateData = biIrisData,
                        };
                        currentUser.IrisDataList.Add(currentIris);
                    }

                    if (_terminalUserData.WalkThroughLength > 0)
                    {
                        if (_terminalUserData.WalkThroughType == WalkThroughTemplateType)
                        {
                            currentUser.FaceDataList = new List<DtoUserFace>
                            {
                                new DtoUserFace
                                {
                                    Length = _terminalUserData.WalkThroughLength,
                                    UserIdOnDevice =  terminalUserData.UserID,
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

        public VirdiErrorEnum AddUserSync(int clientId, int terminalId, DtoDevice device, DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerSetUser))
            {
                LoggingSystem.LogInfo("Virdi Server all user command", new { ClientId = clientId, TerminalId = terminalId, DeviceNumber = device.DeviceNumber, User = userInfo });
            }
            var userInfoForDevice = userInfo.WithDeviceLocalDates(device);
            lock (_serverUserData)
            {
                _serverUserData.InitUserData();
                _serverUserData.UserID = (int)userInfoForDevice.UserIdOnDevice;
                _serverUserData.UniqueID = userInfoForDevice.UserIdOnDevice.ToString();
                _serverUserData.UserName = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(userInfoForDevice.UserName));
                _serverUserData.IsAdmin = userInfoForDevice.Privilege;
                _serverUserData.IsIdentify = 1;
                _serverUserData.IsFace1toN = 1;
                _serverUserData.AuthType = 0;
                _serverUserData.IsBlacklist = Convert.ToInt32(!userInfoForDevice.IsEnable);
                // ReSharper disable PossibleInvalidOperationException
                _serverUserData.SetAccessDate(1,
                    userInfoForDevice.StartDateTime.Value.Year
                    , userInfoForDevice.StartDateTime.Value.Month
                    , userInfoForDevice.StartDateTime.Value.Day
                    ,userInfoForDevice.EndDateTime.Value.Year
                    , userInfoForDevice.EndDateTime.Value.Month
                    , userInfoForDevice.EndDateTime.Value.Day
                    );
                // ReSharper restore PossibleInvalidOperationException

                var verificationStyleEnum = (VirdiVerificationStyleEnumeration)userInfoForDevice.VerificationStyle;
                _serverUserData.SetAuthType(
                    Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsAndOperation)),
                    device.HasFingerPrint ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsFinger)) : 0,
                    0,
                    Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsPassword)),
                    device.HasRfReader ? Convert.ToInt32(verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsCard)) : 0,
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
                if (device.HasRfReader && userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                {
                    var rfCardNumbers = userInfoForDevice.RfCardNumbers.Distinct().ToList();
                    for (var i = 0; i < rfCardNumbers.Count; i++)
                    {
                        _serverUserData.SetCardData(i == 0 ? 1 : 0, rfCardNumbers[i]);
                    }
                }
                if (verificationStyleEnum.HasFlag(VirdiVerificationStyleEnumeration.IsPassword)
                    && userInfoForDevice.Password.IsNotNullOrEmpty())
                {
                    _serverUserData.Password = userInfoForDevice.Password;
                }

                // Set Iris data
                if (device.HasIris)
                {
                    if (userInfoForDevice.IrisDataList.IsCollectionNotNullOrEmpty())
                    {
                        _serverUserData.IsIris1toN = 1;
                        var iris = userInfoForDevice.IrisDataList.First();
                        _serverUserData.SetIrisData(iris.TemplateData.Length, iris.TemplateData);
                    }
                }

                // Set Finger data
                if (device.HasFingerPrint)
                {
                    if (userInfoForDevice.FingerDataList.IsCollectionNotNullOrEmpty())
                    {

                        _serverUserData.IsCheckSimilarFinger = 0;
                        foreach (var currentFingerPrint in userInfoForDevice.FingerDataList)
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
                    if (device.HasVisiblelight)
                    {
                        var walkThroughTemplate =
                            userInfoForDevice.FaceDataList.FirstOrDefault(f => f.FaceIndex == WalkThroughFaceIndex);
                        if (walkThroughTemplate != null)
                        {
                            _serverUserData.SetWalkThroughData
                                (WalkThroughTemplateType, walkThroughTemplate.Length, walkThroughTemplate.TemplateData);
                        }
                        else if (userInfoForDevice.VisibleLightImage.IsCollectionNotNullOrEmpty())
                        {
                            var imageForSend = userInfoForDevice.VisibleLightImage;
                            imageForSend = ImageHelper.ResizeImageByDimensions(imageForSend, _config.MaxVisibleLightImageSizeWidth, _config.MaxVisibleLightImageSizeHeight);
                            imageForSend = ImageHelper.ReduceImageSize(imageForSend, _config.MaxVisibleLightImageSizeInKb);
                            _serverUserData.SetWalkThroughData
                                (WalkThroughImageType, imageForSend.Length, imageForSend);
                        }
                    }
                    else
                    {
                        if (userInfoForDevice.FaceDataList.IsCollectionNotNullOrEmpty())
                        {
                            var firstFace =
                                userInfoForDevice.FaceDataList.FirstOrDefault(f => f.FaceIndex != WalkThroughFaceIndex);
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

                if ((device.DeviceSettings == null || device.DeviceSettings.IsSendProfileImageActive)
                    && userInfoForDevice.HardwareProfileImage.IsCollectionNotNullOrEmpty()
                    && userInfoForDevice.HardwareProfileImage.Length <= MaxSizeForProfileImage)
                {
                    var picture = new byte[MaxSizeForProfileImage];
                    for (var i = 0; i < userInfoForDevice.HardwareProfileImage.Length; i++)
                    {
                        picture[i] = userInfoForDevice.HardwareProfileImage[i];
                    }
                    _serverUserData.SetPictureData(userInfoForDevice.HardwareProfileImage.Length, "JPG", picture);
                }

                const int isOverwrite = 1;
                _serverUserData.AddUserToTerminal(ProcessClientIdBeforeSend(clientId), terminalId, isOverwrite);
                var resultOfSetUser = (VirdiErrorEnum)_ucsApi.ErrorCode;
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
                    NumericId = clientId,
                    Mode = CommandMode,
                    Description = $"Error Code is = {_ucsApi.EventError.ToString()}",
                });
            }

        }

        public Tuple<DtoUserFinger, VirdiErrorEnum> ScanFingerSync(int terminalId, long userId, int fingerIndex)
        {
            _ucsApi.EnrollFromTerminal(0, terminalId);

            if (_ucsApi.ErrorCode == (int)VirdiErrorEnum.Success)
            {
                var templateIndex = 0;
                var nFingerId = _ucsApi.get_FingerID(fingerIndex);
                var fingerTemplate1 = _ucsApi.get_FPSampleData(nFingerId, templateIndex) as byte[];
                var fingerTemplate2 = _ucsApi.get_FPSampleData(nFingerId, templateIndex + 1) as byte[];

                var allTemplate = new byte[NTemplateType400 + NTemplateType400];
                Buffer.BlockCopy(fingerTemplate1, 0, allTemplate, 0, NTemplateType400);
                Buffer.BlockCopy(fingerTemplate2, 0, allTemplate, NTemplateType400, NTemplateType400);
                return new Tuple<DtoUserFinger, VirdiErrorEnum>(new DtoUserFinger
                {
                    UserIdOnDevice = userId,
                    FingerIndex = fingerIndex,
                    TemplateData = allTemplate,
                    CheckSum = 0,
                }, VirdiErrorEnum.Success);
            }
            else
            {
                return new Tuple<DtoUserFinger, VirdiErrorEnum>(null, (VirdiErrorEnum)_ucsApi.ErrorCode);
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
                Date = DateTime.UtcNow,
                UserIdOnDevice = userId,
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
                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoUserFace
                        {
                            UserIdOnDevice = scanWalkThroughData.UserIdOnDevice,
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
                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoUserFace
                        {
                            UserIdOnDevice = scanWalkThroughData.UserIdOnDevice,
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
                _scanFaceCommands.RemoveAll(row => row.Date <= DateTime.UtcNow.AddHours(-6));
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
                Date = DateTime.UtcNow,
                UserIdOnDevice = userId,
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
                            HardwareEventPublisher.Instance.PublishNewFaceEnrolled(new DtoUserFace
                            {
                                UserIdOnDevice = scanFaceData.UserIdOnDevice,
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
                _scanFaceCommands.RemoveAll(row => row.Date <= DateTime.UtcNow.AddHours(-6));
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
                Date = DateTime.UtcNow,
                UserIdOnDevice = userId,
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
                    HardwareEventPublisher.Instance.PublishNewIrisEnrolled(new DtoUserIris()
                    {
                        UserIdOnDevice = irisData.UserIdOnDevice,
                        Length = walkThroughData.Length,
                        TemplateData = walkThroughData
                    }, terminalId);
                    _scanIrisCommands.RemoveAll(row => row.ClientId == clientId);
                }
            }
            finally
            {
                _scanIrisCommands.RemoveAll(row => row.Date <= DateTime.UtcNow.AddHours(-6));
            }
        }


        private readonly AutoResetEvent _userDataListWaitHandle = new AutoResetEvent(false);
        private List<DtoUserDeviceRelatedData> _userDataListSync = new List<DtoUserDeviceRelatedData>();
        public List<DtoUserDeviceRelatedData> GetDeviceUserIdsSync(int clientId, int terminalId)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.ServerGetUserData))
            {
                LoggingSystem.LogInfo("Virdi Server get user ids command", new { ClientId = clientId, TerminalId = terminalId });
            }

            var deviceInList = GetDeviceByTerminalId(terminalId);
            if (deviceInList == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }
            lock (_connectedDeviceIds)
            {
                if (!_connectedDeviceIds.Contains(deviceInList.Id))
                {
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
                }
            }
            lock (_userDataListSync)
            {
                _userDataListSync = new List<DtoUserDeviceRelatedData>();
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

        #region Door controll


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


        #endregion

        #region Match On Server


        private void ucsAPI_EventVerifyCard(int terminalId, int authMode, int antiPassBackLevel, string rfidNumber)
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.MatchOnServer))
            {
                LoggingSystem.LogInfo("Virdi Server Match on server card received", new { RfidNumber = rfidNumber, TerminalId = terminalId, AuthMode = authMode, AntiPassBackLevel = antiPassBackLevel });
            }

            var deviceInList = GetDeviceByTerminalId(terminalId);
            if (deviceInList == null)
            {
                return;
            }
            var now = DateTime.UtcNow;

            var authorizationResult = _serverMatchProcessor(new DtoServerMatchData
            {
                DeviceId = deviceInList.Id,
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
            _serverAuthentication.SendAuthResultToTerminal(terminalId, (int)authorizationResult.UserIdOnDevice, 1, 0, isAuthorized, now.ToString("yyyy-MM-dd hh:mm:ss"), 0);
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
                CommandResponseTime = DateTime.UtcNow,
                NumericId = commandId,
                Mode = CommandMode,
            });
        }

        public static int ProcessClientIdBeforeSend(int clientId)
        {
            return clientId % CommandMode;
        }

        public DtoDevice GetDeviceByTerminalId(int terminalId)
        {
            lock (_deviceList)
            {
                return _deviceList.FirstOrDefault(d => d.DeviceNumber == terminalId);
            }

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
            public long UserIdOnDevice { get; set; }
            public DateTime Date { get; set; }
            public int ClientId { get; set; }
        }

        private class ScanFaceData
        {
            public FaceData Face { get; set; }
            public long UserIdOnDevice { get; set; }
            public DateTime Date { get; set; }
            public int ClientId { get; set; }
        }

        private class ScanIrisData
        {
            public long UserIdOnDevice { get; set; }
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