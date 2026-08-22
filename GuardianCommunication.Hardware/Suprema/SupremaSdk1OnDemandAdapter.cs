using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.Suprema
{
    /// <summary>
    /// Superma BioLite Sdk 1.8
    /// </summary>
    public class SupremaSdk1OnDemandAdapter : IDisposable
    {
        private const int TemplateSize = 384;
        private const int FaceTemplateSize = 2284;
        private const int BsMaxFingerPerUser = 2;
        private const int BsMaxImageSize = 100 * 1024;
        private const int BsFstFaceTemplateSize = 2000;
        private const int BsFstMaxFaceType = 5;
        private const int BsFstMaxFaceTemplate = 25;
        private int _deviceHandle;

        public int DeviceHandle => _deviceHandle;
        public bool IsDeviceConnected { get; set; }
        public DtoDevice DeviceInfo { get; }
        public bool IsInPushMode { get; }
        public uint DeviceId { get; private set; }
        public uint ProductCode { get; private set; }

        public SupremaSdk1OnDemandAdapter(DtoDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;
            ProductCode = (uint)DeviceInfo.DeviceTypeCode;
            //var resultInit = BSSDK.BS_InitSDK();
            //RaiseErrorIfRequired(resultInit);
            var resultOpen = BSSDK.BS_OpenInternalUDP(ref _deviceHandle);
            RaiseErrorIfRequired(resultOpen);

        }

        internal SupremaSdk1OnDemandAdapter(DtoDevice deviceInfo, int handle, uint deviceId, uint productCode)
        {
            IsDeviceConnected = true;
            IsInPushMode = true;
            DeviceInfo = deviceInfo;
            ProductCode = productCode;
            _deviceHandle = handle;
            DeviceId = deviceId;
        }


        #region Private Methods

        //private T[] AllocateStructureArray<T>(int count)
        //{
        //	var result = new T[count];
        //	var structSize = Marshal.SizeOf(typeof(T));
        //	var buffer = Marshal.AllocHGlobal(structSize * count);
        //	var curBuffer = buffer;
        //	for (var idx = 0; idx < count; idx++)
        //	{
        //		result[idx] = (T)Marshal.PtrToStructure(curBuffer, typeof(T));
        //		curBuffer = (IntPtr)((long)curBuffer + structSize);
        //	}
        //	Marshal.FreeHGlobal(buffer);
        //	return result;
        //}

        //private object AllocateStructure(Type t)
        //{
        //	var structSize = Marshal.SizeOf(t);
        //	var buffer = Marshal.AllocHGlobal(structSize);
        //	var instance = Marshal.PtrToStructure(buffer, t);
        //	Marshal.FreeHGlobal(buffer);
        //	return instance;
        //}

        private void DisableClock(int inactivationTime)
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        for (var nIndex = 0; nIndex < 2; nIndex++)
                        {
                            var result = BSSDK.BS_Disable(DeviceHandle, inactivationTime);
                            if (result == BSSDK.BS_SUCCESS)
                                break;
                        }
                    }
                    break;
            }

        }

        private void EnableClock()
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    for (var nIndex = 0; nIndex < 2; nIndex++)
                    {
                        var result = BSSDK.BS_Enable(DeviceHandle);
                        if (result == BSSDK.BS_SUCCESS)
                            break;
                    }
                    break;
            }
        }

        private static byte[] FingerDivider(int index, byte[] templateData)
        {
            byte[] buffer = null;
            if (index == 0)
            {
                buffer = templateData.ToList().GetRange(0, 768).ToArray();
            }
            if (index == 1)
            {
                buffer = templateData.ToList().GetRange(768, 768).ToArray();
            }
            return buffer;
        }

        #endregion


        #region Public Methods

        #region Other

        public bool SetDateTime()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 1 SetDateTime is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var timestamp = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(DateTime.UtcNow);
            DisableClock(10);
            var result = BSSDK.BS_SetTime(DeviceHandle, timestamp);
            EnableClock();
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 1 SetDateTime is calling", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            RaiseErrorIfRequired(result);

            return true;
        }

        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 1 GetDateTime is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var timestamp = 0;
            var result = BSSDK.BS_GetTime(DeviceHandle, ref timestamp);
            RaiseErrorIfRequired(result);
            var resultFinal = DateTimeHelper.ConvertUnixTimestampToUtc((uint)timestamp);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 1 GetDateTime result", new
                {
                    DeviceInfo,
                    Result = resultFinal,
                });
            }
            return resultFinal;

        }

        //public void EnableDevice()
        //{
        //    if (IsDeviceConnected == false)
        //        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
        //    if (DeviceHandle > 0)
        //    {
        //        var result = BSSDK.BS_Enable(DeviceHandle);
        //        RaiseErrorIfRequired(result);
        //    }

        //}

        //public void DisableDevice(int timeout)
        //{
        //    var result = BSSDK.BS_Disable(DeviceHandle, timeout);
        //    if (result == BSSDK.BS_SUCCESS)
        //    {
        //        //_isDeviceEnable = false;
        //    }
        //}


        public void RebootDevice()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Reboot))
            {
                LoggingSystem.LogInfo("Suprema 1 RebootDevice is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = BSSDK.BS_Reset(DeviceHandle);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Reboot))
            {
                LoggingSystem.LogInfo("Suprema 1 RebootDevice result", new
                {
                    DeviceInfo,
                    Result = result,
                });
            }
            RaiseErrorIfRequired(result);
        }

        public string GetSerialNumber()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SerialNumber))
            {
                LoggingSystem.LogInfo("Suprema 1 GetSerialNumber", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            return DeviceId.ToString();
        }

        #endregion

        #region Commiunication

        public bool TestConnection()
        {
            Connect();
            return IsDeviceConnected;
        }

        private float GetFirmwareVersion()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Firmware))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFirmwareVersion is calling", DeviceInfo);
            }
            var config = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSysInfoConfig)));
            var result = BSSDK.BS_ReadSysInfoConfig(DeviceHandle, config);
            if (result != BSSDK.BS_SUCCESS)
            {
                Marshal.FreeHGlobal(config);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
            }
            var configHdr = (BSSysInfoConfig)Marshal.PtrToStructure(config, typeof(BSSysInfoConfig));
            Marshal.FreeHGlobal(config);
            var strData = Encoding.Default.GetString(configHdr.firmwareVer);
            var strVersion = "";
            var bSuccess = false;
            foreach (var c in strData)
            {
                if (c == '_')
                {
                    bSuccess = true;
                    break;
                }

                if (char.IsDigit(c) || c == '.')
                {
                    strVersion += c;
                }
            }

            var fVersion = 1.1f;
            if (bSuccess)
            {
                fVersion = Convert.ToSingle(strVersion);
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Firmware))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFirmwareVersion result", new { DeviceInfo, Result = fVersion });
            }
            return fVersion;

        }

        //public string GetSerialNumber()
        //{
        //	var config = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.BSSysInfoConfig)));
        //	var result = BSSDK.BS_ReadSysInfoConfig(_handel, config);
        //	if (result != BSSDK.BS_SUCCESS)
        //	{
        //		Marshal.FreeHGlobal(config);
        //		throw new OperationCannotBeDoneException(NewDeviceHelperMethods.MapToOperationResult(result, DeviceInfo));
        //	}
        //	var configHdr = (BSSDK.BSSysInfoConfig)Marshal.PtrToStructure(config, typeof(BSSDK.BSSysInfoConfig));
        //	Marshal.FreeHGlobal(config);
        //}


        public void Connect()
        {

            if (DeviceInfo.ConnectionType != ConnectionTypeEnumeration.Ethernet) return;
            if (IsDeviceConnected)
            {
                return;
            }
            if (string.IsNullOrEmpty(DeviceInfo.DeviceIp))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorIpIsNotValid);
            if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorTcpPortIsNotValid);
            //var result = -1;
            //Action action = () =>
            //{
            //	result = BSSDK.BS_OpenSocket(DeviceInfo.Ip, DeviceInfo.TcpPort.Value, ref _deviceHandle);
            //};
            //var resultAsync = action.BeginInvoke(null, null);
            //resultAsync.AsyncWaitHandle.WaitOne(DeviceInfo.ConnectTimeout * 1000);
            var result = BSSDK.BS_OpenSocket(DeviceInfo.DeviceIp, DeviceInfo.TcpPort.Value, ref _deviceHandle);
            RaiseErrorIfRequired(result);
            uint deviceId = 0;
            uint productCode = 0;
            result = BSSDK.BS_GetDeviceID(DeviceHandle, ref deviceId, ref productCode);
            RaiseErrorIfRequired(result);
            DeviceId = deviceId;
            ProductCode = productCode;
            result = BSSDK.BS_SetDeviceID(DeviceHandle, deviceId, (int)productCode);
            RaiseErrorIfRequired(result);
            IsDeviceConnected = true;
        }

        //public void ReconnectOnPush()
        //{
        //    //var result = BSSDK.con(SdkContext, DeviceId);
        //    //RaiseErrorIfRequired(result);
        //    //IsDeviceConnected = true;
        //}

        #endregion

        #region Attendance

        public bool ClearData()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 ClearData", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var deletedCount = 0;
            var result = BSSDK.BS_DeleteLog(DeviceHandle, 0, ref deletedCount);
            RaiseErrorIfRequired(result);
            return true;
        }

        public int GetRecordCountWithDefaultDates()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 GetRecordCountWithDefaultDates is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = GetDataWithDefaultDates().Count;
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 GetRecordCountWithDefaultDates result", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            return result;
        }

        public int GetRecordCount(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 GetRecordCount is calling", DeviceInfo);
            }

            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = ReadLogAttendance(startDate, endDate).Count;
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 GetRecordCount result", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            return result;
        }

        public List<DtoAttendance> GetDataWithDefaultDates()
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    return ReadLogAttendance(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(3));
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_FSTATION:
                    return ReadLogExAttendance(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(3));
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public List<DtoAttendance> GetData(DateTime startDate, DateTime endDate)
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    return ReadLogAttendance(startDate, endDate);
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_FSTATION:
                    return ReadLogExAttendance(startDate, endDate);
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public List<DtoAttendance> Readout(DateTime startDate, DateTime endDate)
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            if (startDate.Date > endDate.Date)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGeneralEndDateMustBeGreaterThanOrEqualStartDate);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    return ReadLogAttendance(startDate, endDate);
                default:
                    return ReadLogExAttendance(startDate, endDate);
            }
        }

        // ReSharper disable CommentTypo
        /// <summary>
        /// بازخوانی و جمع آوری اطلاعات
        /// Campatibility : BioStation/BioEntry Plus/BioEntry W/BioLite Net/Xpass/Xpass Slim/Xpass S2
        /// </summary>
        /// <param name="startDate"> زمان شروع بازخوانی</param>
        /// <param name="endDate">زمان پایان بازخوانی</param>
        /// <returns></returns>
        // ReSharper restore CommentTypo
        private List<DtoAttendance> ReadLogAttendance(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogAttendance is calling", new
                {
                    DeviceInfo,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            var mNumOfLog = 0;
            var result = BSSDK.BS_GetLogCount(DeviceHandle, ref mNumOfLog);
            RaiseErrorIfRequired(result);
            var attendanceRecords = new List<DtoAttendance>();
            var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSLogRecord)));
            var logTotalCount = 0;
            var logCount = 0;
            var nMaxLogPerTrial = ProductCode == BSSDK.BS_DEVICE_BIOSTATION ? 32768 : 8192;
            try
            {
                var start = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(startDate.ToUniversalTime());
                var end = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(endDate.ToUniversalTime());
                do
                {
                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSLogRecord)));
                    result = logTotalCount == 0 ? BSSDK.BS_ReadLog(DeviceHandle, start, end, ref logCount, buf) : BSSDK.BS_ReadNextLog(DeviceHandle, start, end, ref logCount, buf);
                    if (result != BSSDK.BS_SUCCESS)
                    {
                        Marshal.FreeHGlobal(logRecord);
                        throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                    }
                    logTotalCount += logCount;
                } while (logCount == nMaxLogPerTrial);

                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSLogRecord)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSLogRecord))), typeof(BSLogRecord));
                    if ((record.eventType != BSSDK.BE_EVENT_IDENTIFY_SUCCESS && record.eventType != BSSDK.BE_EVENT_VERIFY_SUCCESS) || record.userID <= 0) continue;
                    var eventTime = DateTimeHelper.ConvertUnixTimestampToUtc((uint)record.eventTime);
                    var attendanceRecord = new DtoAttendance
                    {
                        LogIdOnDevice = 0,
                        UserIdOnDevice = record.userID,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                        AttendanceDateTime = eventTime,
                        VerificationStyle = record.subEvent,
                        DeviceId = DeviceInfo.Id,
                        LocationId = DeviceInfo.LocationId,
                        CameraId = null,
                        StatusCode = record.tnaEvent,
                        RfCardNumber = null,
                        IsSentToGuardian = false,
                    };
                    attendanceRecords.Add(attendanceRecord);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(logRecord);
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogAttendance result", new
                {
                    DeviceInfo,
                    Result = attendanceRecords
                });
            }

            return attendanceRecords;
        }

        // ReSharper disable CommentTypo

        /// <summary>
        /// بازخوانی و جمع آوری اطلاعات 
        /// Compatibility : FaceStation/BioStation T2/D-Station/X-Station 
        /// </summary>
        /// <param name="startDate"> زمان شروع بازخوانی</param>
        /// <param name="endDate">زمان پایان بازخوانی</param>
        /// <returns></returns>
        // ReSharper restore CommentTypo
        private List<DtoAttendance> ReadLogExAttendance(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogExAttendance is calling", new
                {
                    DeviceInfo,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            var mNumOfLog = 0;
            var resultGetLogCount = BSSDK.BS_GetLogCount(DeviceHandle, ref mNumOfLog);
            RaiseErrorIfRequired(resultGetLogCount);
            var attendanceRecords = new List<DtoAttendance>();
            var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSLogRecordEx)));
            try
            {
                var logTotalCount = 0;
                var logCount = 0;
                var nMaxLogPerTrial = ProductCode == BSSDK.BS_DEVICE_BIOSTATION ? 32768 : 8192;
                var start = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(startDate.ToUniversalTime());
                var end = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(endDate.ToUniversalTime());
                do
                {
                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSLogRecordEx)));
                    var result = logTotalCount == 0 ? BSSDK.BS_ReadLogEx(DeviceHandle, start, end, ref logCount, buf) : BSSDK.BS_ReadNextLogEx(DeviceHandle, start, end, ref logCount, buf);
                    if (result != BSSDK.BS_SUCCESS)
                    {
                        throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                    }
                    logTotalCount += logCount;
                } while (logCount == nMaxLogPerTrial);
                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSLogRecordEx)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSLogRecordEx))), typeof(BSLogRecordEx));
                    if ((record.eventType != BSSDK.BE_EVENT_IDENTIFY_SUCCESS && record.eventType != BSSDK.BE_EVENT_VERIFY_SUCCESS) || record.userID <= 0) continue;
                    var eventTime = DateTimeHelper.ConvertUnixTimestampToUtc((uint)record.eventTime);
                    var attendanceRecord = new DtoAttendance
                    {
                        LogIdOnDevice = 0,
                        UserIdOnDevice = record.userID,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                        AttendanceDateTime = eventTime,
                        VerificationStyle = record.subEvent,
                        DeviceId = DeviceInfo.Id,
                        LocationId = DeviceInfo.LocationId,
                        CameraId = null,
                        StatusCode = record.tnaEvent,
                        RfCardNumber = null,
                        IsSentToGuardian = false,
                    };

                    attendanceRecords.Add(attendanceRecord);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(logRecord);
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogExAttendance result", new
                {
                    DeviceInfo,
                    Result = attendanceRecords
                });
            }

            return attendanceRecords;
        }

        #endregion

        #region Log


        public List<DtoDeviceEventLog> GetLogWithDefaultDates()
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    return ReadLog(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(3));
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_FSTATION:
                    return ReadLogEx(DateTime.Now.AddDays(-5), DateTime.Now.AddDays(3));
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public List<DtoDeviceEventLog> GetLog(DateTime startDate, DateTime endDate)
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    return ReadLog(startDate, endDate);
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_FSTATION:
                    return ReadLogEx(startDate, endDate);
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        private List<DtoDeviceEventLog> ReadLog(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLog is calling", new
                {
                    DeviceInfo,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveEvents)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            var mNumOfLog = 0;
            var result = BSSDK.BS_GetLogCount(DeviceHandle, ref mNumOfLog);
            RaiseErrorIfRequired(result);
            var logsRecords = new List<DtoDeviceEventLog>();
            var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSLogRecord)));
            var logTotalCount = 0;
            var logCount = 0;
            var nMaxLogPerTrial = ProductCode == BSSDK.BS_DEVICE_BIOSTATION ? 32768 : 8192;
            try
            {
                var start = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(startDate.ToUniversalTime());
                var end = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(endDate.ToUniversalTime());
                do
                {
                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSLogRecord)));
                    result = logTotalCount == 0 ? BSSDK.BS_ReadLog(DeviceHandle, start, end, ref logCount, buf) : BSSDK.BS_ReadNextLog(DeviceHandle, start, end, ref logCount, buf);
                    if (result != BSSDK.BS_SUCCESS)
                    {
                        Marshal.FreeHGlobal(logRecord);
                        throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                    }
                    logTotalCount += logCount;
                } while (logCount == nMaxLogPerTrial);

                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSLogRecord)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSLogRecord))), typeof(BSLogRecord));
                    if (record.userID <= 0)
                    {
                        continue;
                    }
                    if (record.eventType == BSSDK.BE_EVENT_IDENTIFY_SUCCESS && record.eventType == BSSDK.BE_EVENT_VERIFY_SUCCESS) continue;
                    var eventTime = new DateTime(1970, 1, 1).AddSeconds(record.eventTime);
                    var currentLogRecord = new DtoDeviceEventLog
                    {
                        Id = 0,
                        UserIdOnDevice = record.userID,
                        EventDateTime = eventTime,
                        EventCode = record.eventType,
                        DeviceId = DeviceInfo.Id,
                        IsFromDevice = false,
                        Producer = DeviceInfo.ProducerNumber,
                        SdkVersion = DeviceInfo.SdkVersion,
                    };
                    logsRecords.Add(currentLogRecord);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(logRecord);
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLog result", new
                {
                    DeviceInfo,
                    Result = logsRecords
                });
            }
            return logsRecords;
        }

        // ReSharper disable CommentTypo
        /// <summary>
        /// بازخوانی و جمع آوری اطلاعات 
        /// Compatibility : FaceStation/BioStation T2/D-Station/X-Station 
        /// </summary>
        /// <param name="startDate"> زمان شروع بازخوانی</param>
        /// <param name="endDate">زمان پایان بازخوانی</param>
        /// <returns></returns>
        // ReSharper restore CommentTypo
        private List<DtoDeviceEventLog> ReadLogEx(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogEx is calling", new
                {
                    DeviceInfo,
                    StartDate = startDate,
                    EndDate = endDate,
                });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveEvents)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceEventCollectionIsNotActive);
            }
            var mNumOfLog = 0;
            var resultGetLogCount = BSSDK.BS_GetLogCount(DeviceHandle, ref mNumOfLog);
            RaiseErrorIfRequired(resultGetLogCount);
            var logRecords = new List<DtoDeviceEventLog>();
            var logRecord = Marshal.AllocHGlobal(mNumOfLog * Marshal.SizeOf(typeof(BSLogRecordEx)));
            try
            {
                var logTotalCount = 0;
                var logCount = 0;
                var nMaxLogPerTrial = ProductCode == BSSDK.BS_DEVICE_BIOSTATION ? 32768 : 8192;
                var start = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(startDate.ToUniversalTime());
                var end = (int)DateTimeHelper.ConvertUtcToUnixTimestamp(endDate.ToUniversalTime());
                do
                {
                    var buf = new IntPtr(logRecord.ToInt32() + logTotalCount * Marshal.SizeOf(typeof(BSLogRecordEx)));
                    var result = logTotalCount == 0 ? BSSDK.BS_ReadLogEx(DeviceHandle, start, end, ref logCount, buf) : BSSDK.BS_ReadNextLogEx(DeviceHandle, start, end, ref logCount, buf);
                    if (result != BSSDK.BS_SUCCESS)
                    {
                        throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                    }
                    logTotalCount += logCount;
                } while (logCount == nMaxLogPerTrial);
                for (var i = 0; i < logTotalCount; i++)
                {
                    var record = (BSLogRecordEx)Marshal.PtrToStructure(new IntPtr(logRecord.ToInt32() + i * Marshal.SizeOf(typeof(BSLogRecordEx))), typeof(BSLogRecordEx));
                    if ((record.eventType == BSSDK.BE_EVENT_IDENTIFY_SUCCESS && record.eventType == BSSDK.BE_EVENT_VERIFY_SUCCESS)) continue;

                    var eventTime = new DateTime(1970, 1, 1).AddSeconds(record.eventTime);
                    var currentLogRecord = new DtoDeviceEventLog
                    {
                        Id = 0,
                        UserIdOnDevice = record.userID,
                        EventDateTime = eventTime,
                        EventCode = record.eventType,
                        DeviceId = DeviceInfo.Id,
                        IsFromDevice = false,
                        Producer = DeviceInfo.ProducerNumber,
                        SdkVersion = DeviceInfo.SdkVersion,
                    };

                    logRecords.Add(currentLogRecord);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(logRecord);
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 ReadLogEx result", new
                {
                    DeviceInfo,
                    Result = logRecords
                });
            }
            return logRecords;
        }


        #endregion

        #region Usering And Finger

        public void DeleteUserById(long userId)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Suprema 1 DeleteUserById is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = BSSDK.BS_DeleteUser(DeviceHandle, (uint)userId);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 DeleteUserById result", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            RaiseErrorIfRequired(result);
        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Suprema 1 DeleteAllUsers is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = BSSDK.BS_DeleteAllUser(DeviceHandle);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 1 DeleteAllUsers result", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            RaiseErrorIfRequired(result);
        }

        public void SetUserInfoWithTemplate(DtoUserDeviceRelatedData userInfo)
        {
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            SetUserWithMode(userInfo, true);
        }

        public void SetUserInfo(DtoUserDeviceRelatedData userInfo)
        {
            SetUserWithMode(userInfo, false);
        }

        public DtoUserDeviceRelatedData GetUserInfoByUserId(long userId, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetUserInfoByUserId is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var user = ReadUser(userId);
            if (!enrollType.HasFlag(TemplateTypeEnumeration.Face))
                user.FaceDataList.Clear();
            if (!enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
                user.FingerDataList.Clear();
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetUserInfoByUserId result", new
                {
                    DeviceInfo,
                    Result = user
                });
            }
            return user;
        }

        //public List<DtoUserDeviceRelatedData> GetAllUserInfo(TemplateTypeEnumeration enrollType)
        //{
        //    if (IsDeviceConnected == false)
        //        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
        //    var userIdList = GetUserId();
        //    return userIdList.Select(userId => GetUserInfoByUserId(userId, enrollType)).ToList();
        //}

        public List<DtoUserInfoDefinedOnDevice> GetAllUsersInfo()
        {
            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            int result;
            var mNumOfUser = 0;
            var mNumOfTemplate = 0;
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new BEUserHdr[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(BEUserHdr)));
                        result = BSSDK.BS_GetAllUserInfoBEPlus(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (BEUserHdr)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BEUserHdr))), typeof(BEUserHdr));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].userID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });


                        }

                        Marshal.FreeHGlobal(userInfo);

                        #endregion
                    }
                    break;
                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new BSUserHdrEx[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(BSUserHdrEx)));
                        result = BSSDK.BS_GetAllUserInfoEx(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (BSUserHdrEx)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BSUserHdrEx))), typeof(BSUserHdrEx));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].ID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });
                        }
                        Marshal.FreeHGlobal(userInfo);

                        #endregion
                    }
                    break;
                case BSSDK.BS_DEVICE_DSTATION:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        result = BSSDK.BS_GetUserFaceInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new DSUserHdr[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(DSUserHdr)));
                        result = BSSDK.BS_GetAllUserInfoDStation(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (DSUserHdr)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(DSUserHdr))), typeof(DSUserHdr));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].ID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });
                        }
                        Marshal.FreeHGlobal(userInfo);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_XSTATION:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new XSUserHdr[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(XSUserHdr)));
                        result = BSSDK.BS_GetAllUserInfoXStation(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (XSUserHdr)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(XSUserHdr))), typeof(XSUserHdr));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].ID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });
                        }
                        Marshal.FreeHGlobal(userInfo);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new BS2UserHdr[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(BS2UserHdr)));
                        result = BSSDK.BS_GetAllUserInfoBioStation2(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (BS2UserHdr)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(BS2UserHdr))), typeof(BS2UserHdr));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].ID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });
                        }
                        Marshal.FreeHGlobal(userInfo);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        #region Codes

                        result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        var userHdr = new FSUserHdr[mNumOfUser];
                        var userInfo = Marshal.AllocHGlobal(mNumOfUser * Marshal.SizeOf(typeof(FSUserHdr)));
                        result = BSSDK.BS_GetAllUserInfoFStation(DeviceHandle, userInfo, ref mNumOfUser);
                        if (result != BSSDK.BS_SUCCESS && result != BSSDK.BS_ERR_NOT_FOUND)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        for (var i = 0; i < mNumOfUser; i++)
                        {
                            userHdr[i] = (FSUserHdr)Marshal.PtrToStructure(new IntPtr(userInfo.ToInt32() + i * Marshal.SizeOf(typeof(FSUserHdr))), typeof(FSUserHdr));
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                UserIdOnDevice = userHdr[i].ID,
                                Privilege = userHdr[i].adminLevel,
                                Name = string.Empty,
                            });
                        }
                        Marshal.FreeHGlobal(userInfo);
                        break;

                        #endregion
                    }
            }
            return usersInfo;
        }

        public int GetFaceCount()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFaceCount is calling", DeviceInfo);
            }
            var mNumOfUser = 0;
            var mNumOfTemplate = 0;
            var resultValue = 0;
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        #region Codes
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        #region Codes
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        #endregion
                    }
                case BSSDK.BS_DEVICE_DSTATION:
                    {
                        #region Codes
                        var result = BSSDK.BS_GetUserFaceInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);
                        resultValue = mNumOfTemplate;
                        #endregion
                        break;
                    }
                case BSSDK.BS_DEVICE_XSTATION:
                    {
                        #region Codes
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        #region Codes
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        #endregion
                    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        #region Codes
                        var result = BSSDK.BS_GetUserFaceInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
                        RaiseErrorIfRequired(result);

                        resultValue = mNumOfTemplate;
                        //throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        #endregion
                        break;
                    }
            }
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFaceCount result", new
                {
                    DeviceInfo,
                    Result = resultValue
                });
            }
            return resultValue;
        }

        public int GetUserCount()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Suprema 1 GetUserCount is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var mNumOfUser = 0;
            var mNumOfTemplate = 0;
            var result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
            RaiseErrorIfRequired(result);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetUserCount result", new
                {
                    DeviceInfo,
                    Result = mNumOfUser
                });
            }
            return mNumOfUser;
        }

        public int GetFingerCount()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFingerCount is calling", DeviceInfo);
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var mNumOfUser = 0;
            var mNumOfTemplate = 0;
            var result = BSSDK.BS_GetUserDBInfo(DeviceHandle, ref mNumOfUser, ref mNumOfTemplate);
            RaiseErrorIfRequired(result);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetFingerCount result", new
                {
                    DeviceInfo,
                    Result = mNumOfTemplate
                });
            }
            return mNumOfTemplate;
        }

        public string ScanCard()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanCard is calling", DeviceInfo);
            }
            if (!DeviceInfo.HasRfReader)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            uint cardId = 0;
            var cardCustomId = 0;
            var result = BSSDK.BS_ReadCardIDEx(DeviceHandle, ref cardId, ref cardCustomId);
            RaiseErrorIfRequired(result);
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanCard result", new
                {
                    DeviceInfo,
                    Result = cardId
                });
            }
            return cardId.ToString();
        }

        public DtoUserFinger ScanFinger(long userId, int fingerIndex)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFinger is calling", new { DeviceInfo, UserId = userId, FingerIndex = fingerIndex });
            }
            if (!DeviceInfo.HasFingerPrint)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            if (fingerIndex > BsMaxFingerPerUser)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationMaxFingerExceeded);
            }

            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOSTATION:
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        ushort fingerChecksum = 0;
                        var templateData = new byte[TemplateSize * 2];

                        var fingerScanResult = BSSDK.BS_ScanTemplate(DeviceHandle, templateData);
                        RaiseErrorIfRequired(fingerScanResult);
                        Buffer.BlockCopy(templateData, 0, templateData, 0, TemplateSize);

                        fingerScanResult = BSSDK.BS_ScanTemplate(DeviceHandle, templateData);
                        RaiseErrorIfRequired(fingerScanResult);
                        Buffer.BlockCopy(templateData, 0, templateData, TemplateSize, TemplateSize);
                        for (var j = 0; j < TemplateSize; j++)
                        {
                            fingerChecksum += templateData[j];
                        }

                        var result = new DtoUserFinger
                        {
                            CheckSum = fingerChecksum,
                            UserIdOnDevice = userId,
                            FingerIndex = fingerIndex,
                            TemplateData = templateData
                        };
                        if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
                        {
                            LoggingSystem.LogInfo("Suprema 1 ScanFinger result", new
                            {
                                DeviceInfo,
                                Result = result
                            });
                        }
                        return result;
                    }
            }


            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
        }

        public DtoUserFace ScanFace(long userId)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFace is calling", new { DeviceInfo, UserId = userId });
            }
            if (!DeviceInfo.HasFace)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            var faceTemplate = new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate];
            var userTemplateData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserTemplateHdr)));

            var faceTemplateFst = new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate];

            var offset = 0;
            var offset2 = 0;
            var userTemplateHdr = (FSUserTemplateHdr)Marshal.PtrToStructure(userTemplateData, typeof(FSUserTemplateHdr));

            //int nCount = (int)userTemplateHdr.numOfFace;
            // face template data (max 25)
            for (var i = 0; i < BsFstMaxFaceTemplate; i++)
            {
                Buffer.BlockCopy(faceTemplate, offset, faceTemplateFst, offset2, userTemplateHdr.faceLen[i]);
                offset += userTemplateHdr.faceLen[i];
                offset2 += userTemplateHdr.faceLen[i];
            }

            var result = new DtoUserFace
            {
                Length = userTemplateHdr.faceLen[0],
                UserIdOnDevice = userId,
                FaceIndex = 1,
                TemplateData = userTemplateHdr.faceTemp,
            };
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFace result", new
                {
                    DeviceInfo,
                    Result = result
                });
            }
            return result;
        }

        private void SetUserWithMode(DtoUserDeviceRelatedData userInfo1, bool setTemplate)
        {
            var userInfoForDevice = userInfo1.WithDeviceLocalDates(DeviceInfo);

            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFace is calling",
                    new { DeviceInfo, User = userInfoForDevice, SetTemplate = setTemplate });
            }

            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 SetUserWithMode is calling",
                    new { DeviceInfo, User = userInfoForDevice, SetTemplate = setTemplate });
            }

            var faceTemplate = new byte[FaceTemplateSize * BsFstMaxFaceTemplate];
            var faceTemplateFst = new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate];

            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        #region Codes [ Finger ]

                        var userHdr = new BEUserHdr
                        {
                            fingerChecksum = new ushort[2],
                            isDuress = new byte[2],
                            numOfFinger = Math.Min((ushort)userInfoForDevice.FingerDataList.Count, (ushort)2),
                            disabled = userInfoForDevice.IsEnable ? 0 : 1,
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfoForDevice.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfoForDevice.FingerDataList[1].CheckSum;
                        if (userInfoForDevice.Password.IsNotNullOrEmpty())
                        {

                            var tmpPw = Encoding.ASCII.GetBytes(userInfoForDevice.Password);
                            userHdr.password = new byte[16];
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.userID = (uint)userInfoForDevice.UserIdOnDevice;
                        userHdr.adminLevel = (ushort)userInfoForDevice.Privilege;
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 1 ? 5 : 3);
                        userHdr.cardFlag = 0;
                        // ReSharper disable PossibleInvalidOperationException
                        userHdr.startTime =
                            (int)DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                        userHdr.expiryTime =
                            (int)DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                        // ReSharper restore PossibleInvalidOperationException
                        userHdr.isDuress[0] = 0;
                        userHdr.isDuress[1] = 0;
                        if (userInfoForDevice.VerificationStyle == (short)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        {
                            userHdr.opMode = 0;
                        }
                        else
                        {
                            userHdr.opMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }


                        //if (userInfoForDevice.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        //{
                        //    userHdr.opMode = 0;
                        //}
                        //else
                        //{
                        //    userHdr.opMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        //}

                        if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.cardCustomID = 0;
                        userHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;
                        userHdr.dualMode = 0;
                        userHdr.accessGroupMask = uint.Parse("FFFFFFFE", NumberStyles.HexNumber);
                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BEUserHdr)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];

                        if (userInfoForDevice.FingerDataList.Count == 1)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfoForDevice.FingerDataList.Count >= 2)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            var a2 = userInfoForDevice.FingerDataList[1].TemplateData;
                            finalTemplateData = a1.Concat(a2).ToArray();
                        }

                        var result = BSSDK.BS_EnrollUserBEPlus(DeviceHandle, userInfoStruct, finalTemplateData);
                        Marshal.FreeHGlobal(userInfoStruct);
                        RaiseErrorIfRequired(result);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        #region Codes [ Finger ]

                        var userHdr = new BSUserHdrEx
                        {
                            checksum = new ushort[5],
                            name = new byte[33],
                            department = new byte[33],
                            password = new byte[17],
                            authLimitCount = 0,
                            timedAntiPassback = 0,
                            disabled = 0,
                            numOfFinger = (Math.Min((ushort)userInfoForDevice.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.checksum[0] = (ushort)userInfoForDevice.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.checksum[1] = (ushort)userInfoForDevice.FingerDataList[1].CheckSum;
                        // name 
                        var username = userInfoForDevice.UserName;
                        var nameBytes = Encoding.ASCII.GetBytes(username); // UTF8
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        // pwd
                        if (userInfoForDevice.Password.IsNotNullOrEmpty())
                        {
                            userHdr.password = new byte[17];
                            var tmpPw = Encoding.ASCII.GetBytes(userInfoForDevice.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;
                        userHdr.adminLevel =
                            (ushort)(userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? BSSDK.BS_USER_ADMIN
                                : BSSDK.BS_USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1

                        // ReSharper disable PossibleInvalidOperationException
                        userHdr.startDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                        userHdr.expireDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                        // ReSharper restore PossibleInvalidOperationException

                        if (userInfoForDevice.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.version = BSSDK.BE_CARD_VERSION_1;
                        userHdr.accessGroupMask = 0xffffffff;
                        // ---finger 
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfoForDevice.FingerDataList.Count == 1)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfoForDevice.FingerDataList.Count == 2)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            var a2 = userInfoForDevice.FingerDataList[1].TemplateData;
                            finalTemplateData = a1.Concat(a2).ToArray();
                        }

                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSUserHdrEx)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var result = BSSDK.BS_EnrollUserEx(DeviceHandle, userInfoStruct, finalTemplateData);
                        Marshal.FreeHGlobal(userInfoStruct);
                        RaiseErrorIfRequired(result);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_DSTATION:
                    {
                        #region Codes  [ Face Finger]

                        var userHdr = new DSUserHdr
                        {
                            name = new ushort[48],
                            department = new ushort[48],
                            password = new ushort[16],
                            duress = new byte[10],
                            reserved = new byte[2],
                            fingerType = new byte[10],
                            reserved1 = new byte[2],
                            fingerChecksum = new uint[10],
                            faceChecksum = new uint[5],
                            reserved2 = new uint[10],
                            disabled = 0,
                            numOfFinger = (Math.Min((ushort)userInfoForDevice.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfoForDevice.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfoForDevice.FingerDataList[1].CheckSum;
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfoForDevice.FingerDataList.Count == 1)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfoForDevice.FingerDataList.Count == 2)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            var a2 = userInfoForDevice.FingerDataList[1].TemplateData;
                            finalTemplateData = a1.Concat(a2).ToArray();
                        }

                        if (userInfoForDevice.FaceDataList.Count > 0)
                        {
                            // face template's checksum
                            var offset = 0;
                            userHdr.numOfFace = 1;
                            userHdr.faceChecksum[0] = 0;
                            var templateData = new byte[FaceTemplateSize];
                            faceTemplate = userInfoForDevice.FaceDataList[0].TemplateData;
                            Buffer.BlockCopy(faceTemplate, offset, templateData, 0, FaceTemplateSize);
                            for (var j = 0; j < FaceTemplateSize; j++)
                            {
                                userHdr.faceChecksum[0] += templateData[j];
                            }

                        }
                        else
                            userHdr.numOfFace = 0;

                        // name 
                        var username = userInfoForDevice.UserName;
                        var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        if (userInfoForDevice.Password.IsNotNullOrEmpty())
                        {
                            var tmpPw = Encoding.Unicode.GetBytes(userInfoForDevice.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;
                        userHdr.adminLevel =
                            (ushort)(userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? DSUserHdr.ENUM.USER_ADMIN
                                : DSUserHdr.ENUM.USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        // ReSharper disable PossibleInvalidOperationException
                        userHdr.startDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                        userHdr.expireDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                        // ReSharper restore PossibleInvalidOperationException
                        userHdr.duress[0] = 0;
                        userHdr.duress[1] = 0;

                        if (userInfoForDevice.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.accessGroupMask = 0xffffffff;
                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DSUserHdr)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var result = BSSDK.BS_EnrollUserDStation(DeviceHandle, userInfoStruct, finalTemplateData,
                            faceTemplate);
                        Marshal.FreeHGlobal(userInfoStruct);
                        RaiseErrorIfRequired(result);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_XSTATION:
                    {
                        #region Codes

                        var userHdr = new XSUserHdr
                        {
                            name = new ushort[48],
                            department = new ushort[48],
                            password = new ushort[16],
                            duress = new byte[10],
                            reserved = new byte[2],
                            fingerType = new byte[10],
                            reserved1 = new byte[2],
                            fingerChecksum = new uint[10],
                            faceChecksum = new uint[5],
                            reserved2 = new uint[10],
                            disabled = 0
                        };
                        var username = userInfoForDevice.UserName;
                        var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        if (userInfoForDevice.Password.IsNotNullOrEmpty())
                        {
                            var tmpPw = Encoding.Unicode.GetBytes(userInfoForDevice.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;

                        // ReSharper disable PossibleInvalidOperationException
                        userHdr.startDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                        userHdr.expireDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                        // ReSharper restore PossibleInvalidOperationException

                        userHdr.adminLevel =
                            (ushort)(userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? XSUserHdr.ENUM.USER_ADMIN
                                : XSUserHdr.ENUM.USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        if (userInfoForDevice.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.accessGroupMask = 0xffffffff;
                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XSUserHdr)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var result = BSSDK.BS_EnrollUserXStation(DeviceHandle, userInfoStruct);
                        Marshal.FreeHGlobal(userInfoStruct);
                        RaiseErrorIfRequired(result);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        #region Codes [ Finger ]

                        var userHdr = new BS2UserHdr
                        {
                            name = new ushort[48],
                            department = new ushort[48],
                            password = new ushort[16],
                            duress = new byte[10],
                            reserved = new byte[2],
                            fingerType = new byte[10],
                            reserved1 = new byte[2],
                            fingerChecksum = new uint[10],
                            faceChecksum = new uint[5],
                            reserved2 = new uint[10],
                            disabled = 0,
                            numOfFinger = (Math.Min((ushort)userInfoForDevice.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfoForDevice.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfoForDevice.FingerDataList[1].CheckSum;
                        userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;
                        userHdr.adminLevel =
                            (userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                ? (ushort)BS2UserHdr.ENUM.USER_ADMIN
                                : (ushort)BS2UserHdr.ENUM.USER_NORMAL;
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        // ReSharper disable PossibleInvalidOperationException
                        userHdr.startDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                        userHdr.expireDateTime =
                            DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                        // ReSharper restore PossibleInvalidOperationException
                        userHdr.duress[0] = 0;
                        userHdr.duress[1] = 0;
                        if (userInfoForDevice.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfoForDevice.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfoForDevice.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.accessGroupMask = 0xffffffff;
                        // ---finger 
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfoForDevice.FingerDataList.Count == 1)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfoForDevice.FingerDataList.Count == 2)
                        {
                            var a1 = userInfoForDevice.FingerDataList[0].TemplateData;
                            var a2 = userInfoForDevice.FingerDataList[1].TemplateData;
                            finalTemplateData = a1.Concat(a2).ToArray();
                        }

                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserHdr)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var result = BSSDK.BS_EnrollUserBioStation2(DeviceHandle, userInfoStruct, finalTemplateData);
                        Marshal.FreeHGlobal(userInfoStruct);
                        RaiseErrorIfRequired(result);
                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        var fVersion = GetFirmwareVersion();
                        if (fVersion >= 1.2f)
                        {
                            #region new FrimWare

                            var userInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdrEx)));
                            var stillCutData = new byte[BsMaxImageSize * BsFstMaxFaceType];
                            var faceTemplateData =
                                new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate * BsFstMaxFaceType];
                            var tmpFaceTemplate = new byte[250000];
                            var nSize = Marshal.SizeOf(typeof(FSUserHdrEx));
                            var bytes = new byte[nSize];
                            Marshal.Copy(userInfoPtr, bytes, 0, nSize);
                            Array.Clear(bytes, 0, nSize);
                            Marshal.Copy(bytes, 0, userInfoPtr, nSize);
                            var userHdr = (FSUserHdrEx)Marshal.PtrToStructure(userInfoPtr, typeof(FSUserHdrEx));
                            userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;
                            if (userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_ADMIN;
                            else userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_NORMAL;
                            userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                                ? BSSDK.BS_USER_SECURITY_DEFAULT
                                : BSSDK.BS_USER_SECURITY_HIGHER);
                            userHdr.disabled = 0;
                            userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                            // ReSharper disable PossibleInvalidOperationException
                            userHdr.startDateTime =
                                DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                            userHdr.expireDateTime =
                                DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                            // ReSharper restore PossibleInvalidOperationException
                            userHdr.authMode = (ushort)userInfoForDevice.VerificationStyle;
                            if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                            {
                                userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                            }

                            userHdr.customID = 0;
                            // name 
                            var username = userInfoForDevice.UserName;
                            var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                            Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                            // pwd
                            if (userInfoForDevice.Password.IsNotNullOrEmpty())
                            {
                                var pwdBytes = Encoding.ASCII.GetBytes(userInfoForDevice.Password);
                                var pwdOut = new byte[32];
                                BSSDK.BS_EncryptSHA256(pwdBytes, pwdBytes.Length, pwdOut);
                                Buffer.BlockCopy(pwdOut, 0, userHdr.password, 0, pwdOut.Length);

                                var tmpPw = Encoding.Unicode.GetBytes(userInfoForDevice.Password);
                                Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                            }

                            userHdr.accessGroupMask = 0xffffffff;

                            #region Fetch and split data from main buffer from db

                            //Buffer from db  [(Face Template len=250,000) ,(USERhdr len=1112) , (Image data  len=512000) ]
                            //---------- Read userHDR from buffer -------
                            var tempUserInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdrEx)));
                            var bufferUserInfo = new byte[1112];
                            if (setTemplate)
                            {
                                Buffer.BlockCopy(userInfoForDevice.FaceDataList[0].TemplateData, 250000, bufferUserInfo, 0,
                                    1112);
                            }

                            Marshal.Copy(bufferUserInfo, 0, tempUserInfo, Marshal.SizeOf(typeof(FSUserHdrEx)));
                            var tempUserHdr = (FSUserHdrEx)Marshal.PtrToStructure(tempUserInfo, typeof(FSUserHdrEx));
                            userHdr.faceChecksum = tempUserHdr.faceChecksum;
                            userHdr.faceStillcutLen = tempUserHdr.faceStillcutLen;
                            userHdr.faceUpdatedIndex = tempUserHdr.faceUpdatedIndex;
                            userHdr.numOfFace = tempUserHdr.numOfFace;
                            userHdr.numOfUpdatedFace = tempUserHdr.numOfUpdatedFace;
                            userHdr.numOfFaceType = tempUserHdr.numOfFaceType;
                            userHdr.faceLen = tempUserHdr.faceLen;
                            //-------- Read Image cute from buffer ---------
                            var bufferImageUser = new byte[512000]; //buffer image with 512000
                            if (setTemplate)
                            {
                                if (userInfoForDevice.HardwareProfileImage != null)
                                    Buffer.BlockCopy(userInfoForDevice.HardwareProfileImage, 0, bufferImageUser, 0,
                                        userInfoForDevice.HardwareProfileImage.Length);
                                //----------- Read Face Template -------------
                                //fetch buffer face data 
                                Buffer.BlockCopy(userInfoForDevice.FaceDataList[0].TemplateData, 0, tmpFaceTemplate, 0, 250000);
                            }

                            #endregion

                            //fill the Stillcut image data
                            var nStillCutBufPos = 0;
                            var nTemplateBufPos = 0;
                            for (var i = 0; i < tempUserHdr.numOfFaceType; i++)
                            {
                                if (userHdr.faceStillcutLen[i] > 0)
                                    Buffer.BlockCopy(bufferImageUser, 0, stillCutData, nStillCutBufPos,
                                        userHdr.faceStillcutLen[i]);
                                nStillCutBufPos += userHdr.faceStillcutLen[i];
                            }

                            //fill the facetemplate data
                            var nOffset = 0;
                            for (var k = 0; k < BsFstMaxFaceTemplate; k++)
                            {
                                Buffer.BlockCopy(tmpFaceTemplate, nOffset, faceTemplateData, nTemplateBufPos,
                                    userHdr.faceLen[k]);
                                nTemplateBufPos += userHdr.faceLen[k];
                                nOffset += userHdr.faceLen[k];
                            }

                            Marshal.StructureToPtr(userHdr, userInfoPtr, true);
                            var result = BSSDK.BS_EnrollUserFStationEx(DeviceHandle, userInfoPtr, stillCutData,
                                faceTemplateData);
                            RaiseErrorIfRequired(result);
                            Marshal.FreeHGlobal(userInfoPtr);
                            Marshal.FreeHGlobal(tempUserInfo);

                            #endregion
                        }
                        else
                        {
                            #region Codes [ Face ] Note : Dige Support Nemishavad.................. 1394-10-18

                            var userTemplateHdr = new FSUserTemplateHdr();
                            if (userInfoForDevice.FaceDataList.Count > 0)
                            {
                                var handle = GCHandle.Alloc(userInfoForDevice.FaceDataList[0].TemplateData, GCHandleType.Pinned);
                                faceTemplateFst = userInfoForDevice.FaceDataList[0].TemplateData;
                                userTemplateHdr =
                                    (FSUserTemplateHdr)
                                    Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(FSUserTemplateHdr));
                                handle.Free();
                            }

                            //---------------------
                            //IntPtr m_userTemplateData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSSDK.FSUserTemplateHdr)));
                            //BSSDK.FSUserTemplateHdr userTemplateHdr = (BSSDK.FSUserTemplateHdr)Marshal.PtrToStructure(m_userTemplateData, typeof(BSSDK.FSUserTemplateHdr));
                            var userHdr = new FSUserHdr
                            {
                                name = new ushort[48],
                                department = new ushort[48],
                                password = new ushort[16],
                                faceLen = new ushort[25],
                                faceTemp = new byte[256],
                                faceChecksum = new uint[25],
                                disabled = 0,
                                numOfFace = userTemplateHdr.numOfFace,
                                numOfUpdatedFace = userTemplateHdr.numOfUpdatedFace
                            };
                            // face template's length
                            for (var i = 0; i < BsFstMaxFaceTemplate; i++)
                            {
                                userHdr.faceLen[i] = userTemplateHdr.faceLen[i];
                            }

                            // face template's checksum
                            var offset = 0;
                            for (var i = 0; i < BsFstMaxFaceTemplate; i++)
                            {
                                int nLen = userTemplateHdr.faceLen[i];
                                if (nLen > 0)
                                {
                                    var templateData = new byte[nLen];
                                    Buffer.BlockCopy(faceTemplateFst, offset, templateData, 0, nLen);
                                    for (var j = 0; j < userTemplateHdr.faceLen[i]; j++)
                                    {
                                        userHdr.faceChecksum[i] += templateData[j];
                                    }

                                    offset += nLen;
                                }
                            }

                            //
                            // face temp data
                            Buffer.BlockCopy(userTemplateHdr.faceTemp, 0, userHdr.faceTemp, 0, 256);
                            // name 
                            var username = userInfoForDevice.UserName;
                            var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                            Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                            if (userInfoForDevice.Password.IsNotNullOrEmpty())
                            {
                                var pwdBytes = Encoding.Unicode.GetBytes(userInfoForDevice.Password);
                                Buffer.BlockCopy(pwdBytes, 0, userHdr.password, 0, pwdBytes.Length);
                            }

                            userHdr.ID = (uint)userInfoForDevice.UserIdOnDevice;
                            if (userInfoForDevice.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_ADMIN;
                            else userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_NORMAL;
                            userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                                ? BSSDK.BS_USER_SECURITY_DEFAULT
                                : BSSDK.BS_USER_SECURITY_HIGHER);
                            userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                            // ReSharper disable PossibleInvalidOperationException
                            userHdr.startDateTime =
                                DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.StartDateTime);
                            userHdr.expireDateTime =
                                DateTimeHelper.ConvertUtcToUnixTimestamp(userInfoForDevice.EndDateTime.Value);
                            // ReSharper restore PossibleInvalidOperationException
                            userHdr.authMode = (ushort)userInfoForDevice.VerificationStyle;
                            if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                            {
                                userHdr.cardID = uint.Parse(userInfoForDevice.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                            }

                            userHdr.customID = 0;
                            userHdr.accessGroupMask = 0xffffffff;
                            var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdr)));
                            Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                            var result = BSSDK.BS_EnrollUserFStation(DeviceHandle, userInfoStruct, faceTemplateFst);
                            Marshal.FreeHGlobal(userInfoStruct);
                            RaiseErrorIfRequired(result);

                            #endregion
                        }

                        break;
                    }
            }

        }

        private DtoUserDeviceRelatedData ReadUser(long id)
        {

            var templateData = new byte[TemplateSize * 2 * 2];
            var faceTemplate = new byte[FaceTemplateSize * BsFstMaxFaceTemplate];

            var resultOfMethod = new DtoUserDeviceRelatedData();
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_BIOLITE:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        #region Codes

                        var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BEUserHdr)));
                        var result = BSSDK.BS_GetUserBEPlus(DeviceHandle, (uint)id, userInfo, templateData);
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        var userHdr = (BEUserHdr)Marshal.PtrToStructure(userInfo, typeof(BEUserHdr));
                        Marshal.FreeHGlobal(userInfo);
                        resultOfMethod.UserIdOnDevice = userHdr.userID;
                        resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                        resultOfMethod.Privilege = userHdr.adminLevel;
                        //resultOfMethod.IsEnable = userHdr.disabled != 1;
                        resultOfMethod.IsEnable = true;
                        resultOfMethod.Password =
                            userHdr.password.Any(row => row > 0)
                                ? Encoding.ASCII.GetString(userHdr.password).TrimEnd('\0')
                                : string.Empty;
                        if (userHdr.opMode >= BSSDK.BS_AUTH_FINGER_ONLY && userHdr.opMode <= BSSDK.BS_AUTH_CARD_ONLY)
                            resultOfMethod.VerificationStyle = userHdr.opMode - BSSDK.BS_AUTH_FINGER_ONLY + 1;
                        else
                            resultOfMethod.VerificationStyle = 0;
                        if (userHdr.numOfFinger > 0)
                        {
                            resultOfMethod.FingerDataList = new List<DtoUserFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.userID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.userID,
                                FingerIndex = 1,
                                TemplateData = FingerDivider(1, templateData),
                                CheckSum = userHdr.fingerChecksum[1]
                            };
                            resultOfMethod.FingerDataList.Add(finger2);
                        }

                        break;

                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        #region Codes

                        var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSUserHdrEx)));
                        var result = BSSDK.BS_GetUserEx(DeviceHandle, (uint)id, userInfo, templateData);
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        var userHdr = (BSUserHdrEx)Marshal.PtrToStructure(userInfo, typeof(BSUserHdrEx));
                        Marshal.FreeHGlobal(userInfo);
                        resultOfMethod.UserIdOnDevice = userHdr.ID;
                        resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();

                        resultOfMethod.Privilege =
                            (userHdr.adminLevel == BSSDK.BS_USER_ADMIN)
                                ? (int)SupremaDevicePrivilegeEnumeration.Administrator
                                : (int)SupremaDevicePrivilegeEnumeration.User;
                        //resultOfMethod.IsEnable = userHdr.disabled != 1;
                        resultOfMethod.IsEnable = true;
                        resultOfMethod.UserName = Encoding.ASCII.GetString(userHdr.name);
                        resultOfMethod.Password = Encoding.Unicode.GetString(userHdr.password);

                        if (userHdr.authMode >= BSSDK.BS_AUTH_FINGER_ONLY &&
                            userHdr.authMode <= BSSDK.BS_AUTH_CARD_ONLY)
                        {
                            resultOfMethod.VerificationStyle = userHdr.authMode - BSSDK.BS_AUTH_FINGER_ONLY + 1;
                        }
                        else
                        {
                            resultOfMethod.VerificationStyle = 0;
                        }


                        //userInfoDto.IsFingerPrintAuth = true;

                        break;
                        #endregion
                    }
                case BSSDK.BS_DEVICE_DSTATION:
                    {
                        #region Codes

                        var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DSUserHdr)));
                        var result = BSSDK.BS_GetUserDStation(DeviceHandle, (uint)id, userInfo, templateData, faceTemplate);
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        var userHdr = (DSUserHdr)Marshal.PtrToStructure(userInfo, typeof(DSUserHdr));
                        Marshal.FreeHGlobal(userInfo);
                        resultOfMethod.UserIdOnDevice = userHdr.ID;

                        resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                        resultOfMethod.Privilege = (userHdr.adminLevel == (ushort)DSUserHdr.ENUM.USER_ADMIN)
                            ? (int)SupremaDevicePrivilegeEnumeration.Administrator
                            : (int)SupremaDevicePrivilegeEnumeration.User;
                        //resultOfMethod.IsEnable = userHdr.disabled != 1;
                        resultOfMethod.IsEnable = true;
                        //userInfoDto.IsFingerPrintAuth = true;
                        ///////////// ------------- finger splitter------------------
                        if (userHdr.numOfFinger > 0)
                        {
                            resultOfMethod.FingerDataList = new List<DtoUserFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.ID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.ID,
                                FingerIndex = 1,
                                TemplateData = FingerDivider(1, templateData),
                                CheckSum = userHdr.fingerChecksum[1]
                            };
                            resultOfMethod.FingerDataList.Add(finger2);
                        }
                        ///////////// ------------- Face------------------
                        resultOfMethod.FaceDataList.Add(new DtoUserFace()
                        {
                            UserIdOnDevice = userHdr.ID,
                            FaceIndex = 0,
                            TemplateData = faceTemplate,
                            CheckSum = userHdr.faceChecksum[0]
                        });
                        var asBytes = new byte[userHdr.name.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.name, 0, asBytes, 0, asBytes.Length);
                        resultOfMethod.UserName = Encoding.Unicode.GetString(asBytes);
                        var pwdBytes = new byte[userHdr.password.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.password, 0, pwdBytes, 0, pwdBytes.Length);
                        resultOfMethod.Password = Encoding.Unicode.GetString(pwdBytes);
                        if (userHdr.authMode >= BSSDK.BS_AUTH_FINGER_ONLY &&
                            userHdr.authMode <= BSSDK.BS_AUTH_CARD_ONLY)
                        {
                            resultOfMethod.VerificationStyle = userHdr.authMode - BSSDK.BS_AUTH_FINGER_ONLY + 1;
                        }
                        else
                        {
                            resultOfMethod.VerificationStyle = 0;
                        }


                        //if (userHdr.authMode >= BSSDK.BS_AUTH_FINGER_ONLY && userHdr.authMode <= BSSDK.BS_AUTH_CARD_ONLY)
                        //	userInfoDto.IsFingerPrintAuth = true;
                        //else userInfoDto.IsCardAuth = true;
                        break;
                        #endregion
                    }
                case BSSDK.BS_DEVICE_XSTATION:
                    {
                        #region Codes

                        var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(XSUserHdr)));
                        var result = BSSDK.BS_GetUserXStation(DeviceHandle, (uint)id, userInfo);
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        var userHdr = (XSUserHdr)Marshal.PtrToStructure(userInfo, typeof(XSUserHdr));
                        Marshal.FreeHGlobal(userInfo);
                        resultOfMethod.UserIdOnDevice = userHdr.ID;
                        resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                        resultOfMethod.Privilege = (userHdr.adminLevel == (ushort)XSUserHdr.ENUM.USER_ADMIN)
                            ? (int)SupremaDevicePrivilegeEnumeration.Administrator
                            : (int)SupremaDevicePrivilegeEnumeration.User;

                        resultOfMethod.IsEnable = true;
                        //resultOfMethod.IsEnable = userHdr.disabled != 1;
                        var asBytes = new byte[userHdr.name.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.name, 0, asBytes, 0, asBytes.Length);
                        resultOfMethod.UserName = Encoding.Unicode.GetString(asBytes);
                        var pwdBytes = new byte[userHdr.password.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.password, 0, pwdBytes, 0, pwdBytes.Length);
                        resultOfMethod.Password = Encoding.Unicode.GetString(pwdBytes);
                        if (userHdr.authMode >= BSSDK.BS_AUTH_FINGER_ONLY &&
                            userHdr.authMode <= BSSDK.BS_AUTH_CARD_ONLY)
                        {
                            resultOfMethod.VerificationStyle = userHdr.authMode - BSSDK.BS_AUTH_FINGER_ONLY + 1;
                        }
                        else
                        {
                            resultOfMethod.VerificationStyle = 0;
                        }
                        //userInfoDto.IsCardAuth = false;
                        break;
                        #endregion
                    }
                case BSSDK.BS_DEVICE_BIOSTATION2:
                    {
                        #region Codes

                        var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserHdr)));
                        var result = BSSDK.BS_GetUserBioStation2(DeviceHandle, (uint)id, userInfo, templateData);
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            Marshal.FreeHGlobal(userInfo);
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                        }
                        var userHdr = (BS2UserHdr)Marshal.PtrToStructure(userInfo, typeof(BS2UserHdr));
                        Marshal.FreeHGlobal(userInfo);
                        resultOfMethod.UserIdOnDevice = userHdr.ID;
                        resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                        resultOfMethod.Privilege = (userHdr.adminLevel == (ushort)BS2UserHdr.ENUM.USER_ADMIN)
                            ? (int)SupremaDevicePrivilegeEnumeration.Administrator
                            : (int)SupremaDevicePrivilegeEnumeration.User;
                        var nameBytes = new byte[userHdr.name.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.name, 0, nameBytes, 0, nameBytes.Length);
                        resultOfMethod.UserName = Encoding.Unicode.GetString(nameBytes);
                        var pwdBytes = new byte[userHdr.password.Length * sizeof(ushort)];
                        Buffer.BlockCopy(userHdr.password, 0, pwdBytes, 0, pwdBytes.Length);
                        resultOfMethod.Password = Encoding.Unicode.GetString(pwdBytes);
                        //if (userHdr.authMode >= BSSDK.BS_AUTH_FINGER_ONLY && userHdr.authMode <= BSSDK.BS_AUTH_CARD_ONLY)
                        //	userInfoDto.IsFingerPrintAuth = true;
                        //else userInfoDto.IsCardAuth = true;
                        if (userHdr.numOfFinger > 0)
                        {
                            resultOfMethod.FingerDataList = new List<DtoUserFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.ID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoUserFinger
                            {
                                UserIdOnDevice = userHdr.ID,
                                FingerIndex = 1,
                                TemplateData = FingerDivider(1, templateData),
                                CheckSum = userHdr.fingerChecksum[1]
                            };
                            resultOfMethod.FingerDataList.Add(finger2);
                        }

                        break;
                        #endregion
                    }
                case BSSDK.BS_DEVICE_FSTATION:
                    {
                        var fVersion = GetFirmwareVersion();
                        if (fVersion >= 1.2f)
                        {
                            #region new FrimWare

                            var imageData = new byte[BsMaxImageSize * BsFstMaxFaceType];
                            faceTemplate = new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate * BsFstMaxFaceType];
                            var userInfoPTr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdrEx)));
                            // IntPtr m_userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdrEx)));
                            var result = BSSDK.BS_GetUserFStationEx(DeviceHandle, (uint)id, userInfoPTr, imageData, faceTemplate);
                            if (result != BSSDK.BS_SUCCESS)
                            {
                                Marshal.FreeHGlobal(userInfoPTr);
                                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                            }
                            //copy userInfo -> m_userInfo
                            var nSize = Marshal.SizeOf(typeof(FSUserHdrEx));
                            var bufferFsUserHdrEx = new byte[nSize];
                            Marshal.Copy(userInfoPTr, bufferFsUserHdrEx, 0, nSize);
                            // Marshal.Copy(bytes, 0, m_userInfo, nSize);
                            var userHdr = (FSUserHdrEx)Marshal.PtrToStructure(userInfoPTr, typeof(FSUserHdrEx));
                            resultOfMethod.UserIdOnDevice = userHdr.ID;
                            resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                            resultOfMethod.Privilege = (userHdr.adminLevel == (ushort)FSUserHdr.ENUM.USER_ADMIN)
                                ? (int)SupremaDevicePrivilegeEnumeration.Administrator
                                : (int)SupremaDevicePrivilegeEnumeration.User;
                            var asBytes = new byte[userHdr.name.Length * sizeof(ushort)];
                            Buffer.BlockCopy(userHdr.name, 0, asBytes, 0, asBytes.Length);
                            resultOfMethod.UserName = Encoding.Unicode.GetString(asBytes);
                            var checkFaceZero = faceTemplate.All(x => x == 0);
                            if (!checkFaceZero)
                            {
                                resultOfMethod.FaceDataList.Add(new DtoUserFace
                                {
                                    UserIdOnDevice = userHdr.ID,
                                    FaceIndex = 0,
                                    TemplateData = faceTemplate,
                                });
                                resultOfMethod.FaceDataList[0].TemplateData = resultOfMethod.FaceDataList[0].TemplateData.Concat(bufferFsUserHdrEx).ToArray();
                            }
                            //===== import data userHDR

                            //===== import data Image User
                            var checkImageZero = imageData.All(x => x == 0);
                            if (!checkImageZero)
                                resultOfMethod.FaceDataList[0].TemplateData = resultOfMethod.FaceDataList[0].TemplateData.Concat(imageData).ToArray();
                            Marshal.FreeHGlobal(userInfoPTr);
                            resultOfMethod.VerificationStyle = userHdr.authMode;

                            #endregion
                        }
                        else
                        {
                            #region Codes

                            var faceTemplateFst = new byte[BsFstFaceTemplateSize * BsFstMaxFaceTemplate];
                            var userInfo = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FSUserHdr)));
                            var result = BSSDK.BS_GetUserFStation(DeviceHandle, (uint)id, userInfo, faceTemplateFst);
                            if (result != BSSDK.BS_SUCCESS)
                            {
                                Marshal.FreeHGlobal(userInfo);
                                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(result, DeviceInfo));
                            }
                            var userHdr = (FSUserHdr)Marshal.PtrToStructure(userInfo, typeof(FSUserHdr));
                            Marshal.FreeHGlobal(userInfo);
                            resultOfMethod.UserIdOnDevice = userHdr.ID;
                            resultOfMethod.RfCardNumbers = userHdr.cardID > 0 ? new List<string> { userHdr.cardID.ToString() } : new List<string>();
                            resultOfMethod.Privilege = userHdr.adminLevel == 1
                                ? (ushort)SupremaDevicePrivilegeEnumeration.Administrator
                                : (ushort)SupremaDevicePrivilegeEnumeration.User;
                            var nameBytes = new byte[userHdr.name.Length * sizeof(ushort)];
                            Buffer.BlockCopy(userHdr.name, 0, nameBytes, 0, nameBytes.Length);
                            resultOfMethod.UserName = Encoding.Unicode.GetString(nameBytes).Trim();
                            var pwdBytes = new byte[userHdr.password.Length * sizeof(ushort)];
                            Buffer.BlockCopy(userHdr.password, 0, pwdBytes, 0, pwdBytes.Length);
                            resultOfMethod.Password = Encoding.Unicode.GetString(pwdBytes);
                            //switch (userHdr.authMode)
                            //{
                            //	case 1:
                            //		userInfoDto.IsFingerPrintAuth = true;
                            //		break;
                            //	case 2: // Finger and Password"
                            //		userInfoDto.IsFingerPrintAuth = true;
                            //		userInfoDto.IsPasswordAuth = true;
                            //		break;
                            //	case 3: // Finger or Password"
                            //		userInfoDto.IsFingerPrintAuth = true;
                            //		userInfoDto.IsPasswordAuth = true;
                            //		break;
                            //	case 4:
                            //		userInfoDto.IsPasswordAuth = true;
                            //		break;
                            //	case 5:
                            //		userInfoDto.IsCardAuth = true;
                            //		break;
                            //}
                            resultOfMethod.FaceDataList.Add(new DtoUserFace
                            {
                                UserIdOnDevice = userHdr.ID,
                                FaceIndex = 0,
                                TemplateData = faceTemplateFst
                            });

                            #endregion
                        }
                        break;
                    }
            }
            return resultOfMethod;
        }

        #endregion

        #region Access Control

        public void OpenDoorPermanent(DtoDeviceDoorFullInfo doorInfo)
        {
            if (doorInfo.DeviceSpecificDoorSetting?.Suprema1DoorSetting == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.DeviceDoorSettingIsNotValid);
            }
            var result = BSSDK.BS_RelayControlEx(_deviceHandle
                , doorInfo.DeviceSpecificDoorSetting.Suprema1DoorSetting.RelayDeviceId
                , doorInfo.DeviceSpecificDoorSetting.Suprema1DoorSetting.DoorSensor
                , true);
            RaiseErrorIfRequired(result);
        }

        public void CloseDoorPermanent(DtoDeviceDoorFullInfo doorInfo)
        {
            var result = BSSDK.BS_RelayControlEx(_deviceHandle
                , doorInfo.DeviceSpecificDoorSetting.Suprema1DoorSetting.RelayDeviceId
                , doorInfo.DeviceSpecificDoorSetting.Suprema1DoorSetting.DoorSensor
                , false);
            RaiseErrorIfRequired(result);
        }


        public void OpenDoor(DtoDeviceDoorFullInfo doorInfo)
        {
            OpenDoorPermanent(doorInfo);
            Thread.Sleep(doorInfo.OpenDoorDelay * 1000);
            CloseDoorPermanent(doorInfo);
        }

        public void OpenDoorWithDelay(DtoDeviceDoorFullInfo doorInfo, int delayInSecond)
        {
            OpenDoorPermanent(doorInfo);
            Thread.Sleep(delayInSecond * 1000);
            CloseDoorPermanent(doorInfo);
        }

        #endregion


        #endregion


        #region Utilities

        private void RaiseErrorIfRequired(int errorCode)
        {
            if (errorCode != BSSDK.BS_SUCCESS)
            {
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
        }

        #endregion


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;

            if (disposing)
            {
            }

            if (!IsInPushMode)
            {
                BSSDK.BS_CloseSocket(DeviceHandle);
            }
            IsDeviceConnected = false;
        }


        ~SupremaSdk1OnDemandAdapter()
        {
            Dispose(false);
        }

        #endregion


    }
}