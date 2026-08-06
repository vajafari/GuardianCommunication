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
        public DtoCommunicationDeviceData DeviceInfo { get; }
        public bool IsInPushMode { get; }
        public uint DeviceId { get; private set; }
        public uint ProductCode { get; private set; }

        public SupremaSdk1OnDemandAdapter(DtoCommunicationDeviceData deviceInfo)
        {
            DeviceInfo = deviceInfo;
            ProductCode = (uint)DeviceInfo.DeviceTypeCode;
            //var resultInit = BSSDK.BS_InitSDK();
            //RaiseErrorIfRequired(resultInit);
            var resultOpen = BSSDK.BS_OpenInternalUDP(ref _deviceHandle);
            RaiseErrorIfRequired(resultOpen);

        }

        internal SupremaSdk1OnDemandAdapter(DtoCommunicationDeviceData deviceInfo, int handle, uint deviceId, uint productCode)
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var localTime = (int)((DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
            DisableClock(10);
            var result = BSSDK.BS_SetTime(DeviceHandle, localTime);
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
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var time = 0;
            var result = BSSDK.BS_GetTime(DeviceHandle, ref time);
            RaiseErrorIfRequired(result);
            var resultFinal = new DateTime(1970, 1, 1).AddSeconds(time);
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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

            if (DeviceInfo.ConnectionTypeEnum != ConnectionTypeEnumeration.Ethernet) return;
            if (IsDeviceConnected)
            {
                return;
            }
            if (string.IsNullOrEmpty(DeviceInfo.Ip))
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
            var result = BSSDK.BS_OpenSocket(DeviceInfo.Ip, DeviceInfo.TcpPort.Value, ref _deviceHandle);
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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

        public int GetRecordCount(DateTime? startDate, DateTime? endDate)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 1 GetRecordCount is calling", DeviceInfo);
            }

            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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

        public List<DtoAttendance> GetData(DateTime? startDate, DateTime? endDate)
        {
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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
        private List<DtoAttendance> ReadLogAttendance(DateTime? startDate, DateTime? endDate)
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
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
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
                var start = startDate == null ? 0 : Convert.ToInt32(startDate.Value.ConvertToTimestamp());
                var end = endDate == null ? 0 : Convert.ToInt32(endDate.Value.ConvertToTimestamp());
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
                    var eventTime = new DateTime(1970, 1, 1).AddSeconds(record.eventTime);
                    var attendanceRecord = new DtoAttendance
                    {
                        Id = 0,
                        EmployeeNumber = record.userID,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                        AttendanceDateTime = eventTime,
                        VerificationStyle = record.subEvent,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        CameraId = null,
                        StatusCode = record.tnaEvent,
                        IsInvalid = false,
                        RfCardNumber = null,
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
        private List<DtoAttendance> ReadLogExAttendance(DateTime? startDate, DateTime? endDate)
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
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
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
                var start = startDate == null ? 0 : Convert.ToInt32(startDate.Value.ConvertToTimestamp());
                var end = endDate == null ? 0 : Convert.ToInt32(endDate.Value.ConvertToTimestamp());
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
                    var eventTime = new DateTime(1970, 1, 1).AddSeconds(record.eventTime);
                    var attendanceRecord = new DtoAttendance
                    {
                        Id = 0,
                        EmployeeNumber = record.userID,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                        AttendanceDateTime = eventTime,
                        VerificationStyle = record.subEvent,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        CameraId = null,
                        StatusCode = record.tnaEvent,
                        IsInvalid = false,
                        RfCardNumber = null,
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
            if (IsDeviceConnected == false)
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

        public List<DtoDeviceEventLog> GetLog(DateTime? startDate, DateTime? endDate)
        {
            if (IsDeviceConnected == false)
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

        private List<DtoDeviceEventLog> ReadLog(DateTime? startDate, DateTime? endDate)
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
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
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
                var start = startDate == null ? 0 : Convert.ToInt32(startDate.Value.ConvertToTimestamp());
                var end = endDate == null ? 0 : Convert.ToInt32(endDate.Value.ConvertToTimestamp());
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
                        EmployeeNumber = record.userID,
                        EventDateTime = eventTime,
                        EventCode = record.eventType,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        IsFromDevice = false,
                        Producer = DeviceInfo.ProducerEnum,
                        SdkVersion = DeviceInfo.SdkVersionEnum,
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
        private List<DtoDeviceEventLog> ReadLogEx(DateTime? startDate, DateTime? endDate)
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
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
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
                var start = startDate == null ? 0 : Convert.ToInt32(startDate.Value.ConvertToTimestamp());
                var end = endDate == null ? 0 : Convert.ToInt32(endDate.Value.ConvertToTimestamp());
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
                        EmployeeNumber = record.userID,
                        EventDateTime = eventTime,
                        EventCode = record.eventType,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        IsFromDevice = false,
                        Producer = DeviceInfo.ProducerEnum,
                        SdkVersion = DeviceInfo.SdkVersionEnum,
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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

        public void SetUserInfoWithTemplate(DtoEmployeeDeviceRelatedData userInfo)
        {
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            SetUserWithMode(userInfo, true);
        }

        public void SetUserInfo(DtoEmployeeDeviceRelatedData userInfo)
        {
            SetUserWithMode(userInfo, false);
        }

        public DtoEmployeeDeviceRelatedData GetUserInfoByUserId(long userId, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 GetUserInfoByUserId is calling", DeviceInfo);
            }
            if (IsDeviceConnected == false)
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

        //public List<DtoEmployeeDeviceRelatedData> GetAllUserInfo(TemplateTypeEnumeration enrollType)
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
                                EmployeeNumber = userHdr[i].userID,
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
                                EmployeeNumber = userHdr[i].ID,
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
                                EmployeeNumber = userHdr[i].ID,
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
                                EmployeeNumber = userHdr[i].ID,
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
                                EmployeeNumber = userHdr[i].ID,
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
                                EmployeeNumber = userHdr[i].ID,
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
            if (IsDeviceConnected == false)
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
            if (IsDeviceConnected == false)
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
            if (!DeviceInfo.HasRfCard)
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

        public DtoEmployeeFinger ScanFinger(long userId, int fingerIndex)
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFinger is calling", new { DeviceInfo, UserId = userId, FingerIndex = fingerIndex });
            }
            if (!DeviceInfo.HasFinger)
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

                        var result = new DtoEmployeeFinger
                        {
                            CheckSum = fingerChecksum,
                            EmployeeNumber = userId,
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

        public DtoEmployeeFace ScanFace(long userId)
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

            var result = new DtoEmployeeFace
            {
                Length = userTemplateHdr.faceLen[0],
                EmployeeNumber = userId,
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

        private void SetUserWithMode(DtoEmployeeDeviceRelatedData userInfo, bool setTemplate)
        {


            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 ScanFace is calling",
                    new { DeviceInfo, User = userInfo, SetTemplate = setTemplate });
            }

            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.SetUser))
            {
                LoggingSystem.LogInfo("Suprema 1 SetUserWithMode is calling",
                    new { DeviceInfo, User = userInfo, SetTemplate = setTemplate });
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
                            numOfFinger = Math.Min((ushort)userInfo.FingerDataList.Count, (ushort)2),
                            disabled = userInfo.IsEnable ? 0 : 1,
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfo.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfo.FingerDataList[1].CheckSum;
                        if (userInfo.Password.IsNotNullOrEmpty())
                        {

                            var tmpPw = Encoding.ASCII.GetBytes(userInfo.Password);
                            userHdr.password = new byte[16];
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.userID = (uint)userInfo.EmployeeNumber;
                        userHdr.adminLevel = (ushort)userInfo.Privilege;
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 1 ? 5 : 3);
                        userHdr.cardFlag = 0;
                        var startTime = userInfo.StartTime;
                        var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                            ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);
                        userHdr.startTime = (int)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.expiryTime = (int)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.isDuress[0] = 0;
                        userHdr.isDuress[1] = 0;
                        if (userInfo.VerificationStyle == (short)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        {
                            userHdr.opMode = 0;
                        }
                        else
                        {
                            userHdr.opMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }


                        //if (userInfo.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        //{
                        //    userHdr.opMode = 0;
                        //}
                        //else
                        //{
                        //    userHdr.opMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        //}

                        if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.cardCustomID = 0;
                        userHdr.cardVersion = BSSDK.BE_CARD_VERSION_1;
                        userHdr.dualMode = 0;
                        userHdr.accessGroupMask = uint.Parse("FFFFFFFE", NumberStyles.HexNumber);
                        var userInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BEUserHdr)));
                        Marshal.StructureToPtr(userHdr, userInfoStruct, true);
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];

                        if (userInfo.FingerDataList.Count == 1)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfo.FingerDataList.Count >= 2)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            var a2 = userInfo.FingerDataList[1].TemplateData;
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
                            numOfFinger = (Math.Min((ushort)userInfo.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.checksum[0] = (ushort)userInfo.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.checksum[1] = (ushort)userInfo.FingerDataList[1].CheckSum;
                        // name 
                        var username = userInfo.UserName;
                        var nameBytes = Encoding.ASCII.GetBytes(username); // UTF8
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        // pwd
                        if (userInfo.Password.IsNotNullOrEmpty())
                        {
                            userHdr.password = new byte[17];
                            var tmpPw = Encoding.ASCII.GetBytes(userInfo.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfo.EmployeeNumber;
                        userHdr.adminLevel =
                            (ushort)(userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? BSSDK.BS_USER_ADMIN
                                : BSSDK.BS_USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        var startTime = userInfo.StartTime;
                        var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                            ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);

                        userHdr.startDateTime = (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.expireDateTime = (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.duressMask = 0;
                        if (userInfo.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.version = BSSDK.BE_CARD_VERSION_1;
                        userHdr.accessGroupMask = 0xffffffff;
                        // ---finger 
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfo.FingerDataList.Count == 1)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfo.FingerDataList.Count == 2)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            var a2 = userInfo.FingerDataList[1].TemplateData;
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
                            numOfFinger = (Math.Min((ushort)userInfo.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfo.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfo.FingerDataList[1].CheckSum;
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfo.FingerDataList.Count == 1)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfo.FingerDataList.Count == 2)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            var a2 = userInfo.FingerDataList[1].TemplateData;
                            finalTemplateData = a1.Concat(a2).ToArray();
                        }

                        if (userInfo.FaceDataList.Count > 0)
                        {
                            // face template's checksum
                            var offset = 0;
                            userHdr.numOfFace = 1;
                            userHdr.faceChecksum[0] = 0;
                            var templateData = new byte[FaceTemplateSize];
                            faceTemplate = userInfo.FaceDataList[0].TemplateData;
                            Buffer.BlockCopy(faceTemplate, offset, templateData, 0, FaceTemplateSize);
                            for (var j = 0; j < FaceTemplateSize; j++)
                            {
                                userHdr.faceChecksum[0] += templateData[j];
                            }

                        }
                        else
                            userHdr.numOfFace = 0;

                        // name 
                        var username = userInfo.UserName;
                        var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        if (userInfo.Password.IsNotNullOrEmpty())
                        {
                            var tmpPw = Encoding.Unicode.GetBytes(userInfo.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfo.EmployeeNumber;
                        userHdr.adminLevel =
                            (ushort)(userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? DSUserHdr.ENUM.USER_ADMIN
                                : DSUserHdr.ENUM.USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        var startTime = userInfo.StartTime;
                        var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                            ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);

                        userHdr.startDateTime = (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.expireDateTime = (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.duress[0] = 0;
                        userHdr.duress[1] = 0;

                        if (userInfo.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
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
                        var username = userInfo.UserName;
                        var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                        Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                        if (userInfo.Password.IsNotNullOrEmpty())
                        {
                            var tmpPw = Encoding.Unicode.GetBytes(userInfo.Password);
                            Buffer.BlockCopy(tmpPw, 0, userHdr.password, 0, tmpPw.Length);
                        }

                        userHdr.ID = (uint)userInfo.EmployeeNumber;
                        var startTime = userInfo.StartTime;
                        var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                            ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);

                        userHdr.startDateTime = (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.expireDateTime = (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);

                        userHdr.adminLevel =
                            (ushort)(userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator
                                ? XSUserHdr.ENUM.USER_ADMIN
                                : XSUserHdr.ENUM.USER_NORMAL);
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        if (userInfo.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
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
                            numOfFinger = (Math.Min((ushort)userInfo.FingerDataList.Count, (ushort)2))
                        };
                        if (userHdr.numOfFinger > 0)
                            userHdr.fingerChecksum[0] = (ushort)userInfo.FingerDataList[0].CheckSum;
                        if (userHdr.numOfFinger > 1)
                            userHdr.fingerChecksum[1] = (ushort)userInfo.FingerDataList[1].CheckSum;
                        userHdr.ID = (uint)userInfo.EmployeeNumber;
                        userHdr.adminLevel =
                            (userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                ? (ushort)BS2UserHdr.ENUM.USER_ADMIN
                                : (ushort)BS2UserHdr.ENUM.USER_NORMAL;
                        userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                            ? BSSDK.BS_USER_SECURITY_DEFAULT
                            : BSSDK.BS_USER_SECURITY_HIGHER);
                        userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                        var startTime = userInfo.StartTime;
                        var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                            ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);

                        userHdr.startDateTime = (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.expireDateTime = (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                        userHdr.duress[0] = 0;
                        userHdr.duress[1] = 0;
                        if (userInfo.VerificationStyle == (int)SupremaVerificationStyleEnumeration.Disabled || !userInfo.IsEnable)
                        {
                            userHdr.authMode = 0;
                        }
                        else
                        {
                            userHdr.authMode = (ushort)(userInfo.VerificationStyle + BSSDK.BS_AUTH_FINGER_ONLY - 1);
                        }

                        if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                        {
                            userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                        }

                        userHdr.customID = 0;
                        userHdr.accessGroupMask = 0xffffffff;
                        // ---finger 
                        var finalTemplateData = new byte[TemplateSize * 2 * 2];
                        if (userInfo.FingerDataList.Count == 1)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            Buffer.BlockCopy(a1, 0, finalTemplateData, 0, a1.Length);
                        }
                        else if (userInfo.FingerDataList.Count == 2)
                        {
                            var a1 = userInfo.FingerDataList[0].TemplateData;
                            var a2 = userInfo.FingerDataList[1].TemplateData;
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
                            userHdr.ID = (uint)userInfo.EmployeeNumber;
                            if (userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_ADMIN;
                            else userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_NORMAL;
                            userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                                ? BSSDK.BS_USER_SECURITY_DEFAULT
                                : BSSDK.BS_USER_SECURITY_HIGHER);
                            userHdr.disabled = 0;
                            userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                            var startTime = userInfo.StartTime;
                            var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                                ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);

                            userHdr.startDateTime =
                                (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                            userHdr.expireDateTime =
                                (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                            userHdr.authMode = (ushort)userInfo.VerificationStyle;
                            if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                            {
                                userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
                            }

                            userHdr.customID = 0;
                            // name 
                            var username = userInfo.UserName;
                            var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                            Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                            // pwd
                            if (userInfo.Password.IsNotNullOrEmpty())
                            {
                                var pwdBytes = Encoding.ASCII.GetBytes(userInfo.Password);
                                var pwdOut = new byte[32];
                                BSSDK.BS_EncryptSHA256(pwdBytes, pwdBytes.Length, pwdOut);
                                Buffer.BlockCopy(pwdOut, 0, userHdr.password, 0, pwdOut.Length);

                                var tmpPw = Encoding.Unicode.GetBytes(userInfo.Password);
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
                                Buffer.BlockCopy(userInfo.FaceDataList[0].TemplateData, 250000, bufferUserInfo, 0,
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
                                if (userInfo.HardwareProfileImage != null)
                                    Buffer.BlockCopy(userInfo.HardwareProfileImage, 0, bufferImageUser, 0,
                                        userInfo.HardwareProfileImage.Length);
                                //----------- Read Face Template -------------
                                //fetch buffer face data 
                                Buffer.BlockCopy(userInfo.FaceDataList[0].TemplateData, 0, tmpFaceTemplate, 0, 250000);
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
                            if (userInfo.FaceDataList.Count > 0)
                            {
                                var handle = GCHandle.Alloc(userInfo.FaceDataList[0].TemplateData, GCHandleType.Pinned);
                                faceTemplateFst = userInfo.FaceDataList[0].TemplateData;
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
                            var username = userInfo.UserName;
                            var nameBytes = Encoding.Unicode.GetBytes(username); // UTF16
                            Buffer.BlockCopy(nameBytes, 0, userHdr.name, 0, nameBytes.Length);
                            if (userInfo.Password.IsNotNullOrEmpty())
                            {
                                var pwdBytes = Encoding.Unicode.GetBytes(userInfo.Password);
                                Buffer.BlockCopy(pwdBytes, 0, userHdr.password, 0, pwdBytes.Length);
                            }

                            userHdr.ID = (uint)userInfo.EmployeeNumber;
                            if (userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                                userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_ADMIN;
                            else userHdr.adminLevel = (ushort)FSUserHdr.ENUM.USER_NORMAL;
                            userHdr.securityLevel = (ushort)(userHdr.adminLevel == 0
                                ? BSSDK.BS_USER_SECURITY_DEFAULT
                                : BSSDK.BS_USER_SECURITY_HIGHER);
                            userHdr.bypassCard = 0; //normal=0 -- Bypass = 1
                            var startTime = userInfo.StartTime;
                            var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime,
                                ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1);
                            userHdr.startDateTime =
                                (uint)((startTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                            userHdr.expireDateTime =
                                (uint)((endTime.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
                            userHdr.authMode = (ushort)userInfo.VerificationStyle;
                            if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                            {
                                userHdr.cardID = uint.Parse(userInfo.RfCardNumbers[0] /*, NumberStyles.HexNumber*/);
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

        private DtoEmployeeDeviceRelatedData ReadUser(long id)
        {

            var templateData = new byte[TemplateSize * 2 * 2];
            var faceTemplate = new byte[FaceTemplateSize * BsFstMaxFaceTemplate];

            var resultOfMethod = new DtoEmployeeDeviceRelatedData();
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
                        resultOfMethod.EmployeeNumber = userHdr.userID;
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
                            resultOfMethod.FingerDataList = new List<DtoEmployeeFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.userID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.userID,
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
                        resultOfMethod.EmployeeNumber = userHdr.ID;
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
                        resultOfMethod.EmployeeNumber = userHdr.ID;

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
                            resultOfMethod.FingerDataList = new List<DtoEmployeeFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.ID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.ID,
                                FingerIndex = 1,
                                TemplateData = FingerDivider(1, templateData),
                                CheckSum = userHdr.fingerChecksum[1]
                            };
                            resultOfMethod.FingerDataList.Add(finger2);
                        }
                        ///////////// ------------- Face------------------
                        resultOfMethod.FaceDataList.Add(new DtoEmployeeFace()
                        {
                            EmployeeNumber = userHdr.ID,
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
                        resultOfMethod.EmployeeNumber = userHdr.ID;
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
                        resultOfMethod.EmployeeNumber = userHdr.ID;
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
                            resultOfMethod.FingerDataList = new List<DtoEmployeeFinger>();
                        }
                        if (userHdr.numOfFinger > 0)
                        {
                            var finger1 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.ID,
                                FingerIndex = 0,
                                TemplateData = FingerDivider(0, templateData),
                                CheckSum = userHdr.fingerChecksum[0]
                            };
                            resultOfMethod.FingerDataList.Add(finger1);
                        }
                        if (userHdr.numOfFinger > 1)
                        {
                            var finger2 = new DtoEmployeeFinger
                            {
                                EmployeeNumber = userHdr.ID,
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
                            resultOfMethod.EmployeeNumber = userHdr.ID;
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
                                resultOfMethod.FaceDataList.Add(new DtoEmployeeFace
                                {
                                    EmployeeNumber = userHdr.ID,
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
                            resultOfMethod.EmployeeNumber = userHdr.ID;
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
                            resultOfMethod.FaceDataList.Add(new DtoEmployeeFace
                            {
                                EmployeeNumber = userHdr.ID,
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

        public void SendHolidays(List<DtoSupremaSdk1DeviceHolidayGroup> holidayGroups)
        {
            if (holidayGroups.IsCollectionNullOrEmpty())
            {
                return;
            }

            var data = Marshal.AllocHGlobal(BSSDK.BS_MAX_HOLIDAY_EX * Marshal.SizeOf(typeof(BSHolidayEx)));
            try
            {
                var holidayEx = new BSHolidayEx[BSSDK.BS_MAX_HOLIDAY_EX];
                for (var i = 0; i < BSSDK.BS_MAX_HOLIDAY_EX; i++)
                {
                    holidayEx[i].name = new byte[32];
                    holidayEx[i].holiday = new BSHolidayElemEx[32];
                    for (var k = 0; k < 32; k++)
                    {
                        holidayEx[i].holiday[k].reserved = new byte[3];
                    }
                    holidayEx[i].reserved = new int[2];
                }

                var numOfHolidaySchedule = holidayGroups.Count;
                for (var i = 0; i < holidayGroups.Count && i < BSSDK.BS_MAX_HOLIDAY_EX; i++)
                {
                    if (holidayGroups[i].Holidays.IsCollectionNotNullOrEmpty())
                    {
                        holidayEx[i].holidayID = holidayGroups[i].GroupNumber;
                        var name = Encoding.Unicode.GetBytes(holidayGroups[i].GroupName);
                        Buffer.BlockCopy(name, 0, holidayEx[0].name, 0, name.Length);
                        holidayEx[i].numOfHoliday = holidayGroups.Count;
                        for (var j = 0; j < holidayEx[i].numOfHoliday; j++)
                        {
                            var holiday = holidayGroups[i].Holidays[j];

                            holidayEx[i].holiday[j].year = (byte)(holiday.HolidayDate.Year - 2000);
                            holidayEx[i].holiday[j].month = (byte)(holiday.HolidayDate.Month);
                            holidayEx[i].holiday[j].startDay = (byte)(holiday.HolidayDate.Day);
                            holidayEx[i].holiday[j].duration = (byte)(holiday.HollidayDuration);
                            holidayEx[i].holiday[j].flag = (byte)(holiday.IsRepeatYearly ? 1 : 0);
                        }
                    }
                }

                var longPtr = data.ToInt64();
                foreach (var t in holidayEx)
                {
                    var tempPtr = new IntPtr(longPtr);
                    Marshal.StructureToPtr(t, tempPtr, false);
                    longPtr += Marshal.SizeOf(typeof(BSHolidayEx));
                }
                var result = BSSDK.BS_SetAllHolidayEx(_deviceHandle, numOfHolidaySchedule, data);
                RaiseErrorIfRequired(result);

            }
            finally
            {
                Marshal.FreeHGlobal(data);
            }

        }

        public void SetTimezones(List<DtoSupremaSdk1Timezone> timezones)
        {
            if (timezones.IsCollectionNullOrEmpty())
            {
                return;
            }
            var data = Marshal.AllocHGlobal(BSSDK.DF_MAX_TIMESCHEDULE * Marshal.SizeOf(typeof(BSTimeScheduleEx)));
            try
            {
                var timezoneEx = new BSTimeScheduleEx[BSSDK.DF_MAX_TIMESCHEDULE];
                for (var i = 0; i < BSSDK.DF_MAX_TIMESCHEDULE; i++)
                {
                    timezoneEx[i].name = new byte[32];
                    timezoneEx[i].holiday = new int[2];
                    timezoneEx[i].timeCode = new BSTimeCodeEx[9];
                    for (var k = 0; k < 9; k++)
                    {
                        timezoneEx[i].timeCode[k].codeElement = new BSTimeCodeElemEx[5];
                    }
                    timezoneEx[i].reserved = new int[2];
                }
                for (var i = 0; i < timezones.Count; i++)
                {
                    timezoneEx[i].scheduleID = timezones[i].TimezoneNumber;
                    var name = Encoding.Unicode.GetBytes(timezones[i].TimezoneTitle);   // name
                    Buffer.BlockCopy(name, 0, timezoneEx[0].name, 0, name.Length);

                    if (timezones[i].HolidayGroupNumber1.HasValue)
                    {
                        timezoneEx[i].holiday[0] = timezones[i].HolidayGroupNumber1.Value;
                    }
                    else
                    {
                        timezoneEx[i].holiday[0] = 0;
                    }
                    if (timezones[i].HolidayGroupNumber2.HasValue)
                    {
                        timezoneEx[i].holiday[1] = timezones[i].HolidayGroupNumber2.Value;
                    }
                    else
                    {
                        timezoneEx[i].holiday[1] = 0;
                    }
                    for (var j = 0; j < 9; j++)
                    {
                        timezoneEx[i].timeCode[j].codeElement[0].startTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[0].endTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[1].startTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[1].endTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[2].startTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[2].endTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[3].startTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[3].endTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[4].startTime = 0;
                        timezoneEx[i].timeCode[j].codeElement[4].endTime = 0;
                    }
                    var elementCodesOfCurrentTimezone = timezones[i].Elements.Select(row => row.ElementCode).Distinct().ToList();
                    foreach (var elementCode in elementCodesOfCurrentTimezone)
                    {
                        var timesOfCurrentElement = timezones[i].Elements.Where(row => row.ElementCode == elementCode).OrderBy(row => row.StartTime).ToList();
                        for (var j = 0; j < timesOfCurrentElement.Count; j++)
                        {
                            timezoneEx[i].timeCode[elementCode].codeElement[j].startTime = (ushort)timesOfCurrentElement[j].StartTime;
                            timezoneEx[i].timeCode[elementCode].codeElement[j].endTime = (ushort)timesOfCurrentElement[j].EndTime;
                        }

                    }
                }
                var longPtr = data.ToInt64();
                foreach (var t in timezoneEx)
                {
                    var tempPtr = new IntPtr(longPtr);
                    Marshal.StructureToPtr(t, tempPtr, false);
                    longPtr += Marshal.SizeOf(typeof(BSTimeScheduleEx));
                }

                var result = BSSDK.BS_SetAllTimeScheduleEx(_deviceHandle, timezones.Count, data);
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(data);
            }
        }

        public void SetAccessGroups(List<DtoSupremaSdk1AccessGroup> accessGroups)
        {
            if (accessGroups.IsCollectionNullOrEmpty())
            {
                return;
            }
            var data = Marshal.AllocHGlobal(BSSDK.DF_MAX_ACCESSGROUP * Marshal.SizeOf(typeof(BSAccessGroupEx)));
            try
            {
                var accessGroupEx = new BSAccessGroupEx[BSSDK.DF_MAX_ACCESSGROUP];
                for (var i = 0; i < BSSDK.DF_MAX_ACCESSGROUP; i++)
                {
                    accessGroupEx[i].name = new byte[32];
                    accessGroupEx[i].readerID = new uint[32];
                    accessGroupEx[i].scheduleID = new int[32];
                    accessGroupEx[i].reserved = new int[2];
                }

                for (var i = 0; i < accessGroups.Count; i++)
                {
                    accessGroupEx[i].groupID = accessGroups[i].AccessGroupNumber;
                    var name = Encoding.Unicode.GetBytes(accessGroups[i].Title);
                    Buffer.BlockCopy(name, 0, accessGroupEx[i].name, 0, name.Length);
                    accessGroupEx[i].numOfReader = accessGroups[i].DoorTimezones.Count;
                    for (var j = 0; j < accessGroups[i].DoorTimezones.Count && j < 32; j++)
                    {
                        accessGroupEx[i].readerID[j] = (uint)accessGroups[i].DoorTimezones[j].DeviceDoorId;
                        accessGroupEx[i].scheduleID[j] = accessGroups[i].DoorTimezones[j].TimezoneNumber;
                    }
                }

                var longPtr = data.ToInt64();
                foreach (var t in accessGroupEx)
                {
                    var tempPtr = new IntPtr(longPtr);
                    Marshal.StructureToPtr(t, tempPtr, false);
                    longPtr += Marshal.SizeOf(typeof(BSAccessGroupEx));
                }

                var result = BSSDK.BS_SetAllAccessGroupEx(_deviceHandle, accessGroups.Count, data);
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(data);
            }
        }

        public void SetDoorInfo(DtoSupremaSdk1DeviceDoor doorInfo)
        {
            switch (ProductCode)
            {
                case BSSDK.BS_DEVICE_DSTATION:
                case BSSDK.BS_DEVICE_FSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION2:
                case BSSDK.BS_DEVICE_XSTATION:
                case BSSDK.BS_DEVICE_BIOSTATION:
                    {
                        var data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BSDoorConfig)));
                        try
                        {
                            var result = BSSDK.BS_ReadDoorConfig(_deviceHandle, data);
                            RaiseErrorIfRequired(result);
                            var doorConfig = (BSDoorConfig)Marshal.PtrToStructure(data, typeof(BSDoorConfig));

                            doorConfig.door[0].relay = doorInfo.Relay;
                            doorConfig.door[0].useRTE = 0;
                            doorConfig.door[0].useDoorSensor = 0;
                            doorConfig.door[0].openEvent = doorInfo.OpenEvent;
                            doorConfig.door[0].openTime = doorInfo.OpenTime;
                            doorConfig.door[0].heldOpenTime = doorInfo.HeldOpenTime;
                            doorConfig.door[0].forcedOpenSchedule = doorInfo.ForcedOpenSchedule;
                            doorConfig.door[0].forcedCloseSchedule = doorInfo.ForcedCloseSchedule;
                            doorConfig.door[0].RTEType = doorInfo.RteType;
                            doorConfig.door[0].sensorType = doorInfo.SensorType;
                            doorConfig.door[0].reader[0] = (short)doorInfo.Reader1;
                            doorConfig.door[0].reader[1] = (short)doorInfo.Reader1;
                            doorConfig.door[0].useRTEEx = doorInfo.UseRteEx ? (byte)1 : (byte)0;
                            doorConfig.door[0].useSoundForcedOpen = doorInfo.UseSoundForcedOpen ? (byte)1 : (byte)0;
                            doorConfig.door[0].useSoundHeldOpen = doorInfo.UseSoundHeldOpen ? (byte)1 : (byte)0;
                            doorConfig.door[0].openOnce = doorInfo.OpenOnce ? (byte)1 : (byte)0;
                            doorConfig.door[0].RTE = doorInfo.Rte;
                            doorConfig.door[0].useDoorSensorEx = doorInfo.UseDoorSensorEx ? (byte)1 : (byte)0;
                            doorConfig.door[0].alarmStatus = doorInfo.AlarmStatus ? (byte)1 : (byte)0;
                            doorConfig.door[0].reserved2[0] = 0;
                            doorConfig.door[0].reserved2[1] = 0;
                            doorConfig.door[0].doorSensor = doorInfo.DoorSensor;
                            doorConfig.door[0].relayDeviceId = doorInfo.RelayDeviceId;
                            doorConfig.apbType = 0;
                            doorConfig.apbResetTime = 0;
                            switch (DeviceInfo.DoorTypeEnum)
                            {
                                case DoorTypeEnumeration.NotSupport:
                                    doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                                case DoorTypeEnumeration.Standalone:
                                    doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.ONE_DOOR;
                                    break;
                                case DoorTypeEnumeration.TwoDoor:
                                case DoorTypeEnumeration.ThreeDoor:
                                    doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.TWO_DOOR;
                                    break;
                                default:
                                    doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                            }
                            Marshal.StructureToPtr(doorConfig, data, true);
                            result = BSSDK.BS_WriteDoorConfig(_deviceHandle, data);
                            RaiseErrorIfRequired(result);

                        }
                        finally
                        {
                            Marshal.FreeHGlobal(data);
                        }


                    }
                    break;

                case BSSDK.BS_DEVICE_BIOENTRY_PLUS:
                case BSSDK.BS_DEVICE_BIOENTRY_W:
                case BSSDK.BS_DEVICE_XPASS:
                case BSSDK.BS_DEVICE_XPASS_SLIM:
                case BSSDK.BS_DEVICE_XPASS_SLIM2:
                    {
                        var data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BEConfigData)));
                        try
                        {
                            var configSize = 0;
                            var result = BSSDK.BS_ReadConfig(_deviceHandle, BSSDK.BEPLUS_CONFIG, ref configSize, data);
                            RaiseErrorIfRequired(result);
                            var configBePlus = (BEConfigData)Marshal.PtrToStructure(data, typeof(BEConfigData));

                            configBePlus.doorConfig.door[0].relay = doorInfo.Relay;
                            configBePlus.doorConfig.door[0].useRTE = 0;
                            configBePlus.doorConfig.door[0].useDoorSensor = 0;
                            configBePlus.doorConfig.door[0].openEvent = doorInfo.OpenEvent;
                            configBePlus.doorConfig.door[0].openTime = doorInfo.OpenTime;
                            configBePlus.doorConfig.door[0].heldOpenTime = doorInfo.HeldOpenTime;
                            configBePlus.doorConfig.door[0].forcedOpenSchedule = doorInfo.ForcedOpenSchedule;
                            configBePlus.doorConfig.door[0].forcedCloseSchedule = doorInfo.ForcedCloseSchedule;
                            configBePlus.doorConfig.door[0].RTEType = doorInfo.RteType;
                            configBePlus.doorConfig.door[0].sensorType = doorInfo.SensorType;
                            configBePlus.doorConfig.door[0].reader[0] = (short)doorInfo.Reader1;
                            configBePlus.doorConfig.door[0].reader[1] = (short)doorInfo.Reader2;
                            configBePlus.doorConfig.door[0].useRTEEx = doorInfo.UseRteEx ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].useSoundForcedOpen = doorInfo.UseSoundForcedOpen ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].useSoundHeldOpen = doorInfo.UseSoundHeldOpen ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].openOnce = doorInfo.OpenOnce ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].RTE = doorInfo.Rte;
                            configBePlus.doorConfig.door[0].useDoorSensorEx = doorInfo.UseDoorSensorEx ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].alarmStatus = doorInfo.AlarmStatus ? (byte)1 : (byte)0;
                            configBePlus.doorConfig.door[0].reserved2[0] = 0;
                            configBePlus.doorConfig.door[0].reserved2[1] = 0;
                            configBePlus.doorConfig.door[0].doorSensor = doorInfo.DoorSensor;
                            configBePlus.doorConfig.door[0].relayDeviceId = doorInfo.RelayDeviceId;
                            configBePlus.doorConfig.apbType = 0;
                            configBePlus.doorConfig.apbResetTime = 0;
                            switch (DeviceInfo.DoorTypeEnum)
                            {
                                case DoorTypeEnumeration.NotSupport:
                                    configBePlus.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                                case DoorTypeEnumeration.Standalone:
                                    configBePlus.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.ONE_DOOR;
                                    break;
                                case DoorTypeEnumeration.TwoDoor:
                                case DoorTypeEnumeration.ThreeDoor:
                                    configBePlus.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.TWO_DOOR;
                                    break;
                                default:
                                    configBePlus.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                            }
                            Marshal.StructureToPtr(configBePlus, data, true);
                            configSize = Marshal.SizeOf(typeof(BEConfigData));
                            result = BSSDK.BS_WriteConfig(_deviceHandle, BSSDK.BEPLUS_CONFIG, configSize, data);
                            RaiseErrorIfRequired(result);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(data);
                        }
                    }
                    break;

                case BSSDK.BS_DEVICE_BIOLITE:
                    {
                        var data = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BEConfigDataBLN)));
                        try
                        {
                            var configSize = 0;
                            var result = BSSDK.BS_ReadConfig(_deviceHandle, BSSDK.BIOLITE_CONFIG, ref configSize, data);
                            RaiseErrorIfRequired(result);
                            var configBln = (BEConfigDataBLN)Marshal.PtrToStructure(data, typeof(BEConfigDataBLN));

                            configBln.doorConfig.door[0].relay = doorInfo.Relay;
                            configBln.doorConfig.door[0].useRTE = 0;
                            configBln.doorConfig.door[0].useDoorSensor = 0;
                            configBln.doorConfig.door[0].openEvent = doorInfo.OpenEvent;
                            configBln.doorConfig.door[0].openTime = doorInfo.OpenTime;
                            configBln.doorConfig.door[0].heldOpenTime = doorInfo.HeldOpenTime;
                            configBln.doorConfig.door[0].useSoundHeldOpen = doorInfo.UseSoundHeldOpen ? (byte)1 : (byte)0;
                            configBln.doorConfig.door[0].useSoundForcedOpen = doorInfo.UseSoundForcedOpen ? (byte)1 : (byte)0;
                            configBln.doorConfig.door[0].forcedOpenSchedule = doorInfo.ForcedOpenSchedule;
                            configBln.doorConfig.door[0].forcedCloseSchedule = doorInfo.ForcedCloseSchedule;
                            configBln.doorConfig.door[0].RTEType = doorInfo.RteType;
                            configBln.doorConfig.door[0].sensorType = doorInfo.SensorType;
                            configBln.doorConfig.door[0].reader[0] = (short)doorInfo.Reader1;
                            configBln.doorConfig.door[0].reader[1] = (short)doorInfo.Reader2;
                            configBln.doorConfig.door[0].useRTEEx = doorInfo.UseRteEx ? (byte)1 : (byte)0;
                            configBln.doorConfig.door[0].openOnce = doorInfo.OpenOnce ? (byte)1 : (byte)0;
                            configBln.doorConfig.door[0].RTE = doorInfo.Rte;
                            configBln.doorConfig.door[0].useDoorSensorEx = 1;
                            configBln.doorConfig.door[0].alarmStatus = doorInfo.AlarmStatus ? (byte)1 : (byte)0;
                            configBln.doorConfig.door[0].reserved2[0] = 0;
                            configBln.doorConfig.door[0].reserved2[1] = 0;
                            configBln.doorConfig.door[0].doorSensor = doorInfo.DoorSensor;
                            configBln.doorConfig.door[0].relayDeviceId = doorInfo.RelayDeviceId;
                            configBln.doorConfig.apbType = 0;
                            configBln.doorConfig.apbResetTime = 0;
                            switch (DeviceInfo.DoorTypeEnum)
                            {
                                case DoorTypeEnumeration.NotSupport:
                                    configBln.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                                case DoorTypeEnumeration.Standalone:
                                    configBln.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.ONE_DOOR;
                                    break;
                                case DoorTypeEnumeration.TwoDoor:
                                case DoorTypeEnumeration.ThreeDoor:
                                    configBln.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.TWO_DOOR;
                                    break;
                                default:
                                    configBln.doorConfig.doorMode = (int)BSDoorConfig.BSDOORCONFIG.NO_DOOR;
                                    break;
                            }
                            Marshal.StructureToPtr(configBln, data, true);
                            configSize = Marshal.SizeOf(typeof(BEConfigDataBLN));
                            result = BSSDK.BS_WriteConfig(_deviceHandle, BSSDK.BIOLITE_CONFIG, configSize, data);
                            RaiseErrorIfRequired(result);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(data);
                        }
                    }
                    break;
            }
        }

        public void OpenDoorPermanent(DtoSupremaSdk1DeviceDoor doorInfo)
        {
            var result = BSSDK.BS_RelayControlEx(_deviceHandle, doorInfo.RelayDeviceId, doorInfo.DoorSensor, true);
            RaiseErrorIfRequired(result);
        }

        public void CloseDoorPermanent(DtoSupremaSdk1DeviceDoor doorInfo)
        {
            var result = BSSDK.BS_RelayControlEx(_deviceHandle, doorInfo.RelayDeviceId, doorInfo.DoorSensor, false);
            RaiseErrorIfRequired(result);
        }


        public void OpenDoor(DtoSupremaSdk1DeviceDoor doorInfo)
        {
            OpenDoorPermanent(doorInfo);
            Thread.Sleep(doorInfo.OpenDoorDelay * 1000);
            CloseDoorPermanent(doorInfo);
        }

        public void OpenDoorWithDelay(DtoSupremaSdk1DeviceDoor doorInfo, int delayInSecond)
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