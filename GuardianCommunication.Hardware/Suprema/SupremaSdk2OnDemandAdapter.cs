using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;

namespace GuardianCommunication.Hardware.Suprema
{

    public class SupremaSdk2OnDemandAdapter : IDisposable
    {
        private uint _deviceId;
        private bool IsInPushMode { get; }
        private const int UserPageSizeForListUserId = 1024;

        public BS2SimpleDeviceInfo DeviceInfoFromDevice;
        public BS2SimpleDeviceInfoEx DeviceInfoExFromDevice;
        public DtoCommunicationDeviceData DeviceInfo { get; set; }
        public bool IsDeviceConnected { get; private set; }
        public IntPtr SdkContext { get; private set; }
        public uint DeviceId => _deviceId;

        public SupremaSdk2OnDemandAdapter(DtoCommunicationDeviceData deviceInfo)
        {
            DeviceInfo = deviceInfo;
            _deviceId = 0;
            SdkContext = ApiV2.BS2_AllocateContext();
            if (SdkContext == IntPtr.Zero)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk2CantAllocateSdkContext);
            }
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_Initialize(SdkContext);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            catch
            {
                // Release the just-allocated context so a failed initialize doesn't leak it.
                ApiV2.BS2_ReleaseContext(SdkContext);
                SdkContext = IntPtr.Zero;
                throw;
            }
        }

        internal SupremaSdk2OnDemandAdapter(DtoCommunicationDeviceData deviceInfo, IntPtr sdkContext, uint deviceId)
        {
            IsDeviceConnected = true;
            IsInPushMode = true;
            _deviceId = deviceId;
            SdkContext = sdkContext;
            DeviceInfo = deviceInfo;

            var resultOfNativeMethodCall = ApiV2.BS2_GetDeviceInfoEx(SdkContext, _deviceId, out DeviceInfoFromDevice, out DeviceInfoExFromDevice);
            RaiseErrorIfRequired(resultOfNativeMethodCall);
        }


        #region Private Methods

        private static T[] AllocateStructureArray<T>(int count)
        {
            var result = new T[count];
            var structSize = Marshal.SizeOf(typeof(T));
            var buffer = Marshal.AllocHGlobal(structSize * count);
            var curBuffer = buffer;
            for (var idx = 0; idx < count; idx++)
            {
                result[idx] = (T)Marshal.PtrToStructure(curBuffer, typeof(T));
                curBuffer = (IntPtr)((long)curBuffer + structSize);
            }
            Marshal.FreeHGlobal(buffer);
            return result;
        }

        public static T AllocateStructure<T>()
        {
            var structSize = Marshal.SizeOf(typeof(T));
            var empty = new byte[structSize];
            Array.Clear(empty, 0, empty.Length);
            var buffer = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(empty, 0, buffer, structSize);
            var instance = (T)Marshal.PtrToStructure(buffer, typeof(T));
            Marshal.FreeHGlobal(buffer);

            return instance;

        }

        #endregion


        #region public Methods

        #region Other

        public bool SetDateTime()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 2 SetDateTime is calling", DeviceInfo);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var timestamp = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestamp(DateTime.Now);
            var resultOfNativeMethodCall = ApiV2.BS2_SetDeviceTime(SdkContext, _deviceId, timestamp);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 2 SetDateTime is calling", new
                {
                    DeviceInfo,
                    Result = resultOfNativeMethodCall
                });
            }
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            //Thread.Sleep(60000);
            return true;
        }

        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 2 GetDateTime is calling", DeviceInfo);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            var resultOfNativeMethodCall = ApiV2.BS2_GetDeviceTime(SdkContext, _deviceId, out var timestamp);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Suprema 2 GetDateTimeResult", new
                {
                    DeviceInfo,
                    Result = resultOfNativeMethodCall
                });
            }
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            return DeviceSharedHelperMethods.ConvertUnixTimestampToServerLocalTime(timestamp, DeviceInfo.TimeSetting);
        }

        public string GetSerialNumber()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.SerialNumber))
            {
                LoggingSystem.LogInfo("Suprema 2 GetSerialNumber", DeviceInfo);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            return _deviceId.ToString();
        }

        public void RebootDevice()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Reboot))
            {
                LoggingSystem.LogInfo("Suprema 2 RebootDevice is calling", DeviceInfo);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = ApiV2.BS2_RebootDevice(SdkContext, _deviceId);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Reboot))
            {
                LoggingSystem.LogInfo("Suprema 2 RebootDevice result", new
                {
                    DeviceInfo,
                    Result = result,
                });
            }
            RaiseErrorIfRequired(result);
        }

        public string GetFirmwareVersion()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Firmware))
            {
                LoggingSystem.LogInfo("Suprema 2 GetFirmwareVersion is calling", DeviceInfo);
            }
            var resultOfNativeMethodCall = ApiV2.BS2_GetFactoryConfig(SdkContext, _deviceId, out var factoryConfig);
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            var result =
                $"{factoryConfig.firmwareVer.major}.{factoryConfig.firmwareVer.minor}.{factoryConfig.firmwareVer.ext}";
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Firmware))
            {
                LoggingSystem.LogInfo("Suprema 2 GetFirmwareVersion result", new { DeviceInfo, Result = result });
            }
            return result;
        }

        public void UpgradeFirmware(string fileName, byte[] firmwareFile)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Firmware))
            {
                LoggingSystem.LogInfo("Suprema 2 UpgradeFirmware is calling", new
                {
                    DeviceInfo,
                    FileName = fileName,
                });
            }
            var finalFileName = FileHelper.CreateFileOnCurrentExecutingPath(Path.Combine("Firmware", "Suprema", "V2", fileName), firmwareFile);
            if (SupremaV2Utility.LoadBinary(finalFileName, out var firmwareData, out var firmwareDataLen))
            {
                var resultOfNativeMethodCall = ApiV2.BS2_UpgradeFirmware(SdkContext, _deviceId, firmwareData, firmwareDataLen, 0, null);
                Marshal.FreeHGlobal(firmwareData);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
        }

        public BS2Configs GetDeviceConfig(BS2ConfigMaskEnum config)
        {
            var configs = SupremaV2Utility.AllocateStructure<BS2Configs>();
            configs.configMask = (uint)config;
            Console.WriteLine("Trying to get AllConfig");

            //var structureType = typeof(BS2Configs);
            //var structSize = Marshal.SizeOf(structureType);

            var resultOfNativeMethodCall = ApiV2.BS2_GetConfig(SdkContext, _deviceId, ref configs);
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            return configs;
        }

        public void SendDeviceFunctionTitles(List<string> titles)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.FunctionTitle))
            {
                LoggingSystem.LogInfo("Suprema 2 SendDeviceFunctionTitles is calling", new { DeviceInfo, Titles = titles });
            }
            if (IsDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusConnectTheDeviceFirst);
            }

            if (!Convert.ToBoolean(DeviceInfoFromDevice.tnaSupported))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            if (titles == null)
            {
                titles = new List<string>();
            }
            var tnaConfig = SupremaV2Utility.AllocateStructure<BS2TNAConfig>();
            tnaConfig.tnaInfo.tnaMode = 1; // by user
            tnaConfig.tnaInfo.tnaKey = 0; // not specified because of tnaMode = by user
            tnaConfig.tnaInfo.tnaRequired = 1;
            Array.Clear(tnaConfig.tnaExtInfo.tnaLabel, 0, BS2Environment.BS2_MAX_TNA_KEY * BS2Environment.BS2_MAX_TNA_LABEL_LEN);

            for (var i = 0; i < titles.Count; i++)
            {
                var item = titles[i];
                var labelName = item;
                if (labelName.Length > BS2Environment.BS2_MAX_TNA_LABEL_LEN)
                {
                    labelName = labelName.Substring(0, BS2Environment.BS2_MAX_TNA_LABEL_LEN);
                }

                var labelNameArray = Encoding.UTF8.GetBytes(labelName);
                Array.Copy(labelNameArray, 0, tnaConfig.tnaExtInfo.tnaLabel, i * BS2Environment.BS2_MAX_TNA_LABEL_LEN, labelNameArray.Length);
            }

            var result = ApiV2.BS2_SetTNAConfig(SdkContext, _deviceId, ref tnaConfig);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.FunctionTitle))
            {
                LoggingSystem.LogInfo("Suprema 2 SendDeviceFunctionTitles result", new { DeviceInfo, Result = result });
            }
            RaiseErrorIfRequired(result);
        }

        public void DisableDeviceFunctionTitles()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.FunctionTitle))
            {
                LoggingSystem.LogInfo("Suprema 2 DisableDeviceFunctionTitles is calling", new { DeviceInfo });
            }
            if (IsDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusConnectTheDeviceFirst);
            }
            if (!Convert.ToBoolean(DeviceInfoFromDevice.tnaSupported))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            var tnaConfig = SupremaV2Utility.AllocateStructure<BS2TNAConfig>();
            tnaConfig.tnaInfo.tnaMode = 0; // by user
            tnaConfig.tnaInfo.tnaKey = 0; // not specified because of tnaMode = by user
            tnaConfig.tnaInfo.tnaRequired = 0;
            Array.Clear(tnaConfig.tnaExtInfo.tnaLabel, 0, BS2Environment.BS2_MAX_TNA_KEY * BS2Environment.BS2_MAX_TNA_LABEL_LEN);

            var result = ApiV2.BS2_SetTNAConfig(SdkContext, _deviceId, ref tnaConfig);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.FunctionTitle))
            {
                LoggingSystem.LogInfo("Suprema 2 DisableDeviceFunctionTitles result", new { DeviceInfo, Result = result });
            }
            RaiseErrorIfRequired(result);
        }


        #endregion

        #region Communication

        public bool TestConnection()
        {
            Connect();
            return true;
        }

        public void Connect()
        {
            var ipAddressPointer = IntPtr.Zero;
            try
            {
                if (DeviceInfo.ConnectionTypeEnum != ConnectionTypeEnumeration.Ethernet) return;
                if (string.IsNullOrEmpty(DeviceInfo.Ip))
                    throw new OperationCannotBeDoneException(OperationResultEnumeration
                        .CommunicationStatusSupremaSdk1ErrorIpIsNotValid);
                if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                    throw new OperationCannotBeDoneException(OperationResultEnumeration
                        .CommunicationStatusSupremaSdk1ErrorTcpPortIsNotValid);
                var resultOfNativeMethodCall = -1;

                ipAddressPointer = Marshal.StringToHGlobalAnsi(DeviceInfo.Ip);
                resultOfNativeMethodCall = ApiV2.BS2_ConnectDeviceViaIP(SdkContext, ipAddressPointer,
                    (ushort)DeviceInfo.TcpPort.Value, out _deviceId);
                //Action action = () =>
                //{
                //    resultOfNativeMethodCall = ApiV2.BS2_ConnectDeviceViaIP(SdkContext, ipAddressPointer, (ushort)DeviceInfo.TcpPort.Value, out _deviceId);
                //};
                //var resultAsync = action.BeginInvoke(null, null);
                //resultAsync.AsyncWaitHandle.WaitOne(DeviceInfo.ConnectTimeout * 1000);
                if (resultOfNativeMethodCall != (int)BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    IsDeviceConnected = false;
                    RaiseErrorIfRequired(resultOfNativeMethodCall);
                }

                resultOfNativeMethodCall = ApiV2.BS2_GetDeviceInfoEx(SdkContext, _deviceId, out DeviceInfoFromDevice,
                    out DeviceInfoExFromDevice);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                if (ipAddressPointer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ipAddressPointer);
                }

            }
            //BS2ErrorCode result = (BS2ErrorCode)ApiV2.BS2_GetDeviceInfoEx(_sdkContext, _deviceId, out var deviceInfo, out var deviceInfoEx);
            //if (result != BS2ErrorCode.BS_SDK_SUCCESS)
            //{
            //	Console.WriteLine("Can't get device information(errorCode : {0}).", result);
            //	return;
            //}
            IsDeviceConnected = true;
        }

        public void Disconnect()
        {
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_DisconnectDevice(SdkContext, _deviceId);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                IsDeviceConnected = false;
            }
        }

        #endregion

        #region Attendance


        public bool ClearData()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 2 ClearData is calling", new { DeviceInfo });
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var resultOfNativeMethodCall = ApiV2.BS2_ClearLog(SdkContext, _deviceId);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.ClearData))
            {
                LoggingSystem.LogInfo("Suprema 2 ClearData result", new { DeviceInfo, Result = resultOfNativeMethodCall });
            }
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            return true;
        }

        public List<DtoAttendance> GetDataWithDefault()
        {
            return GetData(0).OrderByDescending(row => row.AttendanceDateTime)
                .Take(100).ToList();
        }

        public List<DtoAttendance> GetData(uint lastLogId)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 GetData is calling", new { DeviceInfo });
            }
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            var result = new List<DtoAttendance>();
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            var structureType = typeof(BS2Event);
            var structSize = Marshal.SizeOf(structureType);
            const uint amount = 0;
            var eventLogObjects = IntPtr.Zero;
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_GetLog(SdkContext, _deviceId, lastLogId, amount, out eventLogObjects, out var countOfEventLogs);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                var currentEventLogObjects = eventLogObjects;
                for (var idx = 0; idx < countOfEventLogs; idx++)
                {
                    var eventLog = (BS2Event)Marshal.PtrToStructure(currentEventLogObjects, typeof(BS2Event));
                    if (SupremaV2Utility.GetEventType(eventLog.code) == SupremaSdk2EventTypeEnumeration.VerifySuccess)
                    {
                        result.Add(SupremaV2Utility.ConvertBs2EventToDtoAttendance(
                            eventLog
                            , DeviceInfo.DeviceNumber
                            , DeviceInfo.TimeSetting
                            , DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand));
                    }
                    currentEventLogObjects = (IntPtr)((long)currentEventLogObjects + structSize);
                }
            }
            finally
            {
                if (eventLogObjects != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(eventLogObjects);
                }
            }
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 GetData result", new { DeviceInfo, Result = result });
            }
            return result;
        }

        public List<DtoAttendance> Readout(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 Readout is calling", new { DeviceInfo });
            }
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            if (startDate.Date > endDate.Date)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGeneralEndDateMustBeGreaterThanOrEqualStartDate);
            var result = new List<DtoAttendance>();
            var start = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestampAndConsiderDateAsUtc(startDate);
            var end = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestampAndConsiderDateAsUtc(endDate);

            var eventLogObjects = IntPtr.Zero;
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_GetFilteredLog(SdkContext, _deviceId, (IntPtr)0,
                    0, start, end, 0, out eventLogObjects, out var outNumEventLogs);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                var currentEventLogObject = eventLogObjects;
                var structSize = Marshal.SizeOf(typeof(BS2Event));
                for (var idx = 0; idx < outNumEventLogs; idx++)
                {
                    var eventLog = (BS2Event)Marshal.PtrToStructure(currentEventLogObject, typeof(BS2Event));
                    if (SupremaV2Utility.GetEventType(eventLog.code) == SupremaSdk2EventTypeEnumeration.VerifySuccess)
                    {

                        result.Add(SupremaV2Utility.ConvertBs2EventToDtoAttendance(
                            eventLog
                            , DeviceInfo.DeviceNumber
                            , DeviceInfo.TimeSetting
                            , DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand));
                    }

                    currentEventLogObject = (IntPtr)((long)currentEventLogObject + structSize);
                }
            }
            finally
            {
                if (eventLogObjects != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(eventLogObjects);
                }
            }

            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 Readout result", new { DeviceInfo, Result = result });
            }
            return result;
        }

        public int GetRecordCount()
        {
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var eventLogObjects = IntPtr.Zero;
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_GetLog(SdkContext, _deviceId, 0, 0, out eventLogObjects, out var logRecordCount);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                return (int)logRecordCount;
            }
            finally
            {
                if (eventLogObjects != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(eventLogObjects);
                }
            }
        }

        #endregion

        #region Log

        public List<DtoDeviceEventLog> GetLogWithDefault()
        {
            return GetLog(0).OrderByDescending(row => row.EventDateTime)
                .Take(100).ToList();
        }

        public List<DtoDeviceEventLog> GetLog(uint lastLogId)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 GetLog is calling", new { DeviceInfo });
            }
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceEventCollectionIsNotActive);
            }
            var result = new List<DtoDeviceEventLog>();
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            var structureType = typeof(BS2Event);
            var structSize = Marshal.SizeOf(structureType);
            const uint amount = 0;
            var eventLogObjects = IntPtr.Zero;
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_GetLog(SdkContext, _deviceId, lastLogId, amount, out eventLogObjects, out var countOfEventLogs);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                var currentEventLogObjects = eventLogObjects;
                for (var idx = 0; idx < countOfEventLogs; idx++)
                {
                    var eventLog = (BS2Event)Marshal.PtrToStructure(currentEventLogObjects, typeof(BS2Event));
                    result.Add(SupremaV2Utility.ConvertBs2EventToDtoDeviceEventLog(
                        eventLog
                        , DeviceInfo.DeviceNumber
                        , DeviceInfo.TimeSetting));
                    currentEventLogObjects = (IntPtr)((long)currentEventLogObjects + structSize);
                }
            }
            finally
            {
                if (eventLogObjects != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(eventLogObjects);
                }
            }
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetData))
            {
                LoggingSystem.LogInfo("Suprema 2 GetLog result", new { DeviceInfo, Result = result });
            }
            return result;
        }

        #endregion

        #region Usering And Finger

        public void DeleteUserById(long userId)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Suprema 2 DeleteUserById is calling", new { DeviceInfo });
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            var userIdArray = new byte[BS2Environment.BS2_USER_ID_SIZE];
            var rawUerId = Encoding.UTF8.GetBytes(userId.ToString());
            var userIds = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);
            try
            {
                Array.Clear(userIdArray, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(rawUerId, 0, userIdArray, 0, rawUerId.Length);
                Marshal.Copy(userIdArray, 0, userIds, BS2Environment.BS2_USER_ID_SIZE);
                var resultOfNativeMethodCall = ApiV2.BS2_RemoveUser(SdkContext, _deviceId, userIds, 1);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                Marshal.FreeHGlobal(userIds);
            }

        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Suprema 2 DeleteAllUsers is calling", new { DeviceInfo });
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var resultOfNativeMethodCall = ApiV2.BS2_RemoveAllUser(SdkContext, _deviceId);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Suprema 2 DeleteAllUsers result", new { DeviceInfo, Result = resultOfNativeMethodCall });
            }
            RaiseErrorIfRequired(resultOfNativeMethodCall);
        }

        public List<DtoUserInfoDefinedOnDevice> GetAllUsersInfo()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.GetUser))
            {
                LoggingSystem.LogInfo("Suprema 2 GetAllUserId is calling", new { DeviceInfo });
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            var userIdsObject = IntPtr.Zero;
            var currentUserIdObject = IntPtr.Zero;
            try
            {
                ApiV2.IsAcceptableUserID cbIsAcceptableUserId = null; // we don't need to user id filtering
                var resultOfNativeMethodCall = ApiV2.BS2_GetUserList(SdkContext, _deviceId, out userIdsObject, out var numberOfUserIds, cbIsAcceptableUserId);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                if (numberOfUserIds > 0)
                {
                    currentUserIdObject = userIdsObject;
                    var userBlobs = new BS2UserBlob[UserPageSizeForListUserId];
                    for (uint idx = 0; idx < numberOfUserIds;)
                    {
                        var available = numberOfUserIds - idx;
                        if (available > UserPageSizeForListUserId)
                        {
                            available = UserPageSizeForListUserId;
                        }

                        resultOfNativeMethodCall = ApiV2.BS2_GetUserDatas(SdkContext, _deviceId, currentUserIdObject, available, userBlobs, (uint)BS2UserMaskEnum.ALL);
                        RaiseErrorIfRequired(resultOfNativeMethodCall);
                        for (uint i = 0; i < available; ++i)
                        {
                            var userName = string.Empty;
                            if (userBlobs[i].name != null)
                            {
                                userName = Encoding.UTF8.GetString(userBlobs[i].name).TrimEnd('\0');
                            }
                            usersInfo.Add(new DtoUserInfoDefinedOnDevice
                            {
                                EmployeeNumber = SupremaV2Utility.GetUserId(userBlobs[i].user.userID),
                                Name = userName,
                                Privilege = null
                            });
                            if (userBlobs[i].cardObjs != IntPtr.Zero)
                                ApiV2.BS2_ReleaseObject(userBlobs[i].cardObjs);
                            if (userBlobs[i].fingerObjs != IntPtr.Zero)
                                ApiV2.BS2_ReleaseObject(userBlobs[i].fingerObjs);
                            if (userBlobs[i].faceObjs != IntPtr.Zero)
                                ApiV2.BS2_ReleaseObject(userBlobs[i].faceObjs);
                        }
                        idx += available;
                        currentUserIdObject += (int)available * BS2Environment.BS2_USER_ID_SIZE;
                    }

                }
            }
            finally
            {
                if (userIdsObject != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(userIdsObject);
                }
            }
            var operatorLevelObj = IntPtr.Zero;
            try
            {
                var resultOfNativeMethodCall = ApiV2.BS2_GetAllAuthOperatorLevelEx(SdkContext, _deviceId, out operatorLevelObj, out var numOperatorLevel);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                if (numOperatorLevel > 0)
                {
                    var curOperatorLevelObj = operatorLevelObj;
                    var structSize = Marshal.SizeOf(typeof(BS2AuthOperatorLevel));

                    for (var idx = 0; idx < numOperatorLevel; ++idx)
                    {
                        var item = (BS2AuthOperatorLevel)Marshal.PtrToStructure(curOperatorLevelObj, typeof(BS2AuthOperatorLevel));
                        try
                        {
                            var userIdString = Encoding.UTF8.GetString(item.userID).TrimEnd('\0');
                            if (long.TryParse(userIdString, out var userId))
                            {
                                var userInResult = usersInfo.FirstOrDefault(u => u.EmployeeNumber == userId);
                                if (userInResult != null)
                                {
                                    userInResult.Privilege = item.level;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "Error on GetAllUsersInfo");
                        }
                        curOperatorLevelObj = (IntPtr)((long)curOperatorLevelObj + structSize);
                    }
                }
            }
            finally
            {
                if (operatorLevelObj != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(operatorLevelObj);
                }
            }

            return usersInfo;
        }

        public string ScanCard()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 2 ScanCard is calling", new { DeviceInfo });
            }
            var supportedFeatures = GetSupportedFeatures(DeviceInfoFromDevice, DeviceInfoExFromDevice);
            if (!supportedFeatures.CardSupported)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            var card = AllocateStructure<BS2Card>();
            var result = ApiV2.BS2_ScanCard(SdkContext, _deviceId, out card, null);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 2 ScanCard result", new { DeviceInfo, Result = result });
            }
            RaiseErrorIfRequired((int)result);
            return Sdk2UserFaceExDecoder.CardToCardNumber(card);
        }

        public DtoEmployeeFinger ScanFinger(long userId, int fingerIndex)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 2 ScanFinger is calling", new { DeviceInfo, UserId = userId, FingerIndex = fingerIndex });
            }
            var supportedFeatures = GetSupportedFeatures(DeviceInfoFromDevice, DeviceInfoExFromDevice);
            if (!supportedFeatures.FingerScanSupported)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            var fingerPrint = SupremaV2Utility.AllocateStructure<BS2Fingerprint>();
            for (uint templateIndex = 0; templateIndex < BS2Environment.BS2_TEMPLATE_PER_FINGER;)
            {
                var resultOfScanFingerprint = (BS2ErrorCode)ApiV2.BS2_ScanFingerprintEx
                    (SdkContext, _deviceId
                    , ref fingerPrint, templateIndex
                    , (uint)BS2FingerprintQualityEnum.QUALITY_STANDARD
                    , (byte)BS2FingerprintTemplateFormatEnum.FORMAT_SUPREMA, out _, null);
                if (resultOfScanFingerprint != BS2ErrorCode.BS_SDK_SUCCESS)
                {
                    if (resultOfScanFingerprint == BS2ErrorCode.BS_SDK_ERROR_EXTRACTION_LOW_QUALITY ||
                        resultOfScanFingerprint == BS2ErrorCode.BS_SDK_ERROR_CAPTURE_LOW_QUALITY)
                    {
                        continue;
                    }
                    RaiseErrorIfRequired((int)resultOfScanFingerprint);
                }
                else
                {
                    templateIndex++;
                }
            }

            var resultOfVerifyFingerprint = (BS2ErrorCode)ApiV2.BS2_VerifyFingerprint(SdkContext, _deviceId, ref fingerPrint);
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 2 ScanFinger result", new { DeviceInfo, Result = resultOfVerifyFingerprint });
            }
            RaiseErrorIfRequired((int)resultOfVerifyFingerprint);
            return new DtoEmployeeFinger
            {
                EmployeeNumber = userId,
                FingerIndex = fingerIndex,
                TemplateData = fingerPrint.data,
            };
        }

        public DtoEmployeeFace ScanFace(long userId)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
            {
                LoggingSystem.LogInfo("Suprema 2 ScanFace is calling", new { DeviceInfo, UserId = userId });
            }
            var supportedFeatures = GetSupportedFeatures(DeviceInfoFromDevice, DeviceInfoExFromDevice);
            if (!supportedFeatures.FaceScanSupported && !supportedFeatures.FaceExScanSupported)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            if (supportedFeatures.FaceExScanSupported)
            {
                var faceEx = SupremaV2Utility.AllocateStructureArray<BS2FaceExWarped>(1);
                var resultOfNativeMethod = ApiV2.BS2_ScanFaceEx(SdkContext, _deviceId, faceEx, (byte)BS2FaceEnrollThreshold.THRESHOLD_DEFAULT, null);
                RaiseErrorIfRequired(resultOfNativeMethod);
                var result = new DtoEmployeeFace
                {
                    EmployeeNumber = userId,
                    Length = faceEx[0].imageData.Length,
                    FaceIndex = 1,
                    TemplateData = faceEx[0].imageData,
                    SupremaSdk2FaceFlag = faceEx[0].flag,
                    SupremaSdk2FaceImageData = faceEx[0].imageData,
                    SupremaSdk2FaceImageLen = faceEx[0].imageData.Length,
                    SupremaSdk2FaceNumOfTemplate = faceEx[0].numOfTemplate,
                };
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
                {
                    LoggingSystem.LogInfo("Suprema 2 ScanFace result", new { DeviceInfo, Result = result });
                }
                return result;
            }
            if (supportedFeatures.FaceScanSupported)
            {
                var resultOfNativeMethod = ApiV2.BS2_GetFaceConfig(SdkContext, _deviceId, out var faceConfig);
                RaiseErrorIfRequired(resultOfNativeMethod);
                var enrollThreshold = faceConfig.enrollThreshold;

                var faces = SupremaV2Utility.AllocateStructureArray<BS2Face>(1);
                resultOfNativeMethod = ApiV2.BS2_ScanFace(SdkContext, _deviceId, faces, enrollThreshold, null);
                RaiseErrorIfRequired(resultOfNativeMethod);

                var result = new DtoEmployeeFace
                {
                    EmployeeNumber = userId,
                    Length = faces[0].templateData.Length,
                    FaceIndex = 1,
                    TemplateData = faces[0].templateData,
                    SupremaSdk2FaceFlag = faces[0].flag,
                    SupremaSdk2FaceImageData = faces[0].imageData,
                    SupremaSdk2FaceImageLen = faces[0].imageLen,
                    SupremaSdk2FaceNumOfTemplate = faces[0].numOfTemplate,
                };
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Scan))
                {
                    LoggingSystem.LogInfo("Suprema 2 ScanFace result", new { DeviceInfo, Result = result });
                }
                return result;
            }

            return null;
        }

        public int GetFaceCount()
        {
            return GetDeviceStatistics().CountOfFaces;
        }

        public int GetFingerCount()
        {
            return GetDeviceStatistics().CountOfFingers;
        }

        public int GetUserCount()
        {
            return GetDeviceStatistics().CountOfUsers;
        }

        public DtoDeviceStatistics GetDeviceStatistics()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Statistics))
            {
                LoggingSystem.LogInfo("Suprema 2 GetDeviceStatistics is calling", DeviceInfo);
            }
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var resultOfNativeMethodCall = ApiV2.BS2_GetUserDatabaseInfo(SdkContext, _deviceId, out var numUsers, out _, out var numFingers, out var numFaces, null);
            RaiseErrorIfRequired(resultOfNativeMethodCall);
            var result = new DtoDeviceStatistics
            {
                IsConnected = true,
                CountOfFaces = (int)numFaces,
                CountOfFingers = (int)numFingers,
                CountOfUsers = (int)numUsers,
                CountOfUnreadAttendance = GetRecordCount()
            };
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.Statistics))
            {
                LoggingSystem.LogInfo("Suprema 2 GetDeviceStatistics result", new { DeviceInfo, Result = result });
            }
            return result;
        }



        public void SetUserInfo(DtoEmployeeDeviceRelatedData userInfo)
        {
            userInfo.ClearTemplateData();
            SetUserInfoWithTemplate(userInfo);
        }

        public void SetUserInfoWithTemplate(DtoEmployeeDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.SetUser))
            {
                LoggingSystem.LogInfo("Suprema 2 SetUserInfoWithTemplate is calling", userInfo);
            }

            var userBlob = AllocateStructure<BS2UserFaceExBlob>();

            try
            {
                var deviceSupportedFeatures = GetSupportedFeatures(DeviceInfoFromDevice, DeviceInfoExFromDevice);

                userBlob.user.version = 0;
                userBlob.user.formatVersion = 0;
                userBlob.user.faceChecksum = 0;
                userBlob.user.authGroupID = 0;
                userBlob.user.numCards = 0;
                userBlob.user.numFingers = 0;
                userBlob.user.numFaces = 0;
                userBlob.user.flag = 0;
                userBlob.cardObjs = IntPtr.Zero;
                userBlob.fingerObjs = IntPtr.Zero;
                userBlob.faceObjs = IntPtr.Zero;
                userBlob.faceExObjs = IntPtr.Zero;
                userBlob.user_photo_obj = IntPtr.Zero;

                //Id
                var userIdArray = Encoding.UTF8.GetBytes(userInfo.EmployeeNumber.ToString());
                Array.Clear(userBlob.user.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                Array.Copy(userIdArray, userBlob.user.userID, userIdArray.Length);

                //StartTime And EndTime
                var startTime = userInfo.StartTime;
                var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime, ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion2);

                userBlob.setting.startTime = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestampAndConsiderDateAsUtc(startTime);
                userBlob.setting.endTime = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestampAndConsiderDateAsUtc(endTime);

                //Name
                Array.Clear(userBlob.name, 0, BS2Environment.BS2_USER_NAME_LEN);
                var userNameArray = Encoding.UTF8.GetBytes(userInfo.UserName);
                Array.Copy(userNameArray, userBlob.name, userNameArray.Length);


                // Access Group
                if (userInfo.SupremaSdk2AccessGroups.IsCollectionNotNullOrEmpty())
                {
                    Array.Clear(userBlob.accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
                    for (var i = 0; i < userInfo.SupremaSdk2AccessGroups.Count; i++)
                    {
                        userBlob.accessGroupId[i] = (uint)userInfo.SupremaSdk2AccessGroups[i];
                    }
                }
                else if (AppConfigs.SupremaSdkAccessGroupCode > 0)
                {
                    Array.Clear(userBlob.accessGroupId, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_PER_USER);
                    userBlob.accessGroupId[0] = (uint)AppConfigs.SupremaSdkAccessGroupCode;
                }



                // VERIFICATION STYLE
                userBlob.setting.securityLevel = (byte)BS2UserSecurityLevelEnum.NORMAL;

                if (userInfo.IsEnable)
                {
                    userBlob.setting.cardAuthMode = (byte)BS2CardAuthModeEnum.NONE;
                    userBlob.setting.fingerAuthMode = (byte)BS2FingerAuthModeEnum.NONE;
                    userBlob.setting.idAuthMode = (byte)BS2IDAuthModeEnum.NONE;
                    userBlob.settingEx.faceAuthMode = (byte)BS2ExtFaceAuthModeEnum.NONE;
                    userBlob.settingEx.fingerprintAuthMode = (byte)BS2ExtFingerprintAuthModeEnum.NONE;
                    userBlob.settingEx.cardAuthMode = (byte)BS2ExtCardAuthModeEnum.NONE;
                    userBlob.settingEx.idAuthMode = (byte)BS2ExtIDAuthModeEnum.NONE;
                    if (deviceSupportedFeatures.FaceExScanSupported)
                    {
                        var verificationStyle =
                            MapSoftwareAuthModeToHardwareAuthModeEx(
                                (SupremaSdk2VerificationStyleEnumeration)userInfo.VerificationStyle);
                        if (deviceSupportedFeatures.FaceSupported || deviceSupportedFeatures.FaceExScanSupported)
                        {
                            userBlob.settingEx.faceAuthMode = (byte)verificationStyle.FaceAuthMode;
                        }

                        if (deviceSupportedFeatures.FingerSupported)
                        {
                            userBlob.settingEx.fingerprintAuthMode = (byte)verificationStyle.FingerAuthMode;
                        }

                        if (deviceSupportedFeatures.CardSupported)
                        {
                            userBlob.settingEx.cardAuthMode = (byte)verificationStyle.CardAuthMode;
                        }

                        userBlob.settingEx.idAuthMode = (byte)verificationStyle.IdAuthMode;
                    }
                    else
                    {
                        var verificationStyle =
                            MapSoftwareAuthModeToHardwareAuthMode(
                                (SupremaSdk2VerificationStyleEnumeration)userInfo.VerificationStyle);
                        if (deviceSupportedFeatures.FingerSupported || deviceSupportedFeatures.FaceSupported)
                        {
                            userBlob.setting.fingerAuthMode = (byte)verificationStyle.BioMetricAuthMode;
                        }

                        if (deviceSupportedFeatures.CardSupported)
                        {
                            userBlob.setting.cardAuthMode = (byte)verificationStyle.CardAuthMode;
                        }

                        userBlob.setting.idAuthMode = (byte)verificationStyle.PasswordAuthMode;
                    }
                }
                else
                {
                    if (deviceSupportedFeatures.FaceExScanSupported)
                    {
                        if (deviceSupportedFeatures.FaceSupported || deviceSupportedFeatures.FaceExScanSupported)
                        {
                            userBlob.settingEx.faceAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                        }

                        if (deviceSupportedFeatures.FingerSupported)
                        {
                            userBlob.settingEx.fingerprintAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                        }

                        if (deviceSupportedFeatures.CardSupported)
                        {
                            userBlob.settingEx.cardAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                        }

                        userBlob.settingEx.idAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                    }
                    else
                    {
                        if (deviceSupportedFeatures.FingerSupported || deviceSupportedFeatures.FaceSupported)
                        {
                            userBlob.setting.fingerAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                        }

                        if (deviceSupportedFeatures.CardSupported)
                        {
                            userBlob.setting.cardAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                        }
                        userBlob.setting.idAuthMode = (byte)BS2CardAuthModeEnum.PROHIBITED;
                    }
                }



                //Image
                if (deviceSupportedFeatures.PhotoSupported
                    && userInfo.HardwareProfileImage.IsCollectionNotNullOrEmpty()
                    && userInfo.HardwareProfileImage.Length <= BS2Environment.BS2_USER_PHOTO_SIZE)
                {

                    var binaryData = Marshal.AllocHGlobal(userInfo.HardwareProfileImage.Length);
                    try
                    {
                        Marshal.Copy(userInfo.HardwareProfileImage, 0, binaryData, userInfo.HardwareProfileImage.Length);

                        userBlob.user_photo_obj = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2UserPhoto)));
                        var curPhotoObj = userBlob.user_photo_obj;

                        Marshal.WriteInt32(curPhotoObj, userInfo.HardwareProfileImage.Length);
                        curPhotoObj += 4;
                        var curDest = curPhotoObj;
                        var curSrc = binaryData;
                        for (var idx = 0; idx < Math.Min((int)userInfo.HardwareProfileImage.Length, BS2Environment.BS2_USER_PHOTO_SIZE); ++idx)
                        {
                            Marshal.WriteByte(curDest, Marshal.ReadByte(curSrc));
                            curDest += 1;
                            curSrc += 1;
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(binaryData);
                    }
                }


                //Password
                Array.Clear(userBlob.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                if (deviceSupportedFeatures.PinSupported && userInfo.Password.IsNotNullOrEmpty())
                {
                    var ptrChar = Marshal.StringToHGlobalAnsi(userInfo.Password);
                    var pinCode = Marshal.AllocHGlobal(BS2Environment.BS2_PIN_HASH_SIZE);
                    try
                    {
                        var resultOfNativeMethodCall = ApiV2.BS2_MakePinCode(SdkContext, ptrChar, pinCode);
                        RaiseErrorIfRequired(resultOfNativeMethodCall);
                        Marshal.Copy(pinCode, userBlob.pin, 0, BS2Environment.BS2_PIN_HASH_SIZE);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptrChar);
                        Marshal.FreeHGlobal(pinCode);
                    }
                }


                // RfCardNumber
                if (deviceSupportedFeatures.CardSupported && userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                {
                    userBlob.user.numCards = 1;
                    userBlob.cardObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2CSNCard)) * userInfo.RfCardNumbers.Count);
                    var currentCardObjects = userBlob.cardObjs;
                    foreach (var rfCardNumber in userInfo.RfCardNumbers)
                    {
                        var cardObject = Sdk2UserFaceExDecoder.CardNumberToCard(rfCardNumber, 1, BS2Environment.BS2_CARD_DATA_SIZE);
                        Marshal.WriteByte(currentCardObjects, cardObject.type);
                        currentCardObjects += 1;
                        Marshal.WriteByte(currentCardObjects, cardObject.size);
                        currentCardObjects += 1;
                        Marshal.Copy(cardObject.data, 0, currentCardObjects, BS2Environment.BS2_CARD_DATA_SIZE);
                        currentCardObjects += BS2Environment.BS2_CARD_DATA_SIZE;
                    }
                }
                else
                {
                    userBlob.user.numCards = 0;
                    userBlob.cardObjs = IntPtr.Zero;
                }


                // Finger
                if (deviceSupportedFeatures.FingerSupported && userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
                {
                    var fingerPrintsList = new List<BS2Fingerprint>();
                    userBlob.user.numFingers = (byte)userInfo.FingerDataList.Count;
                    foreach (var finger in userInfo.FingerDataList)
                    {
                        var fingerStructure = AllocateStructure<BS2Fingerprint>();
                        fingerStructure.data = finger.TemplateData;
                        fingerStructure.flag = (byte)BS2FingerprintFlagEnum.NORMAL;
                        fingerStructure.index = (byte)finger.FingerIndex;
                        fingerPrintsList.Add(fingerStructure);
                    }

                    userBlob.fingerObjs = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Fingerprint)) * (byte)userInfo.FingerDataList.Count);
                    var currentFingerObjects = userBlob.fingerObjs;

                    //userBlob.fingerObjs =
                    //	Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BS2Fingerprint)) * fingerPrintsList.Count);
                    //var curFingerPtr = userBlob.fingerObjs;
                    foreach (var fingerprint in fingerPrintsList)
                    {
                        Marshal.WriteByte(currentFingerObjects, fingerprint.index);
                        currentFingerObjects += 1;
                        Marshal.WriteByte(currentFingerObjects, (byte)BS2FingerprintFlagEnum.NORMAL);
                        currentFingerObjects += 3;
                        Marshal.Copy(fingerprint.data, 0, currentFingerObjects, BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE);
                        currentFingerObjects += BS2Environment.BS2_TEMPLATE_PER_FINGER * BS2Environment.BS2_FINGER_TEMPLATE_SIZE;
                    }
                }
                else
                {
                    userBlob.user.numFingers = 0;
                    userBlob.fingerObjs = IntPtr.Zero;
                }

                //Face
                if (deviceSupportedFeatures.FaceExScanSupported && userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                {
                    userBlob.user.numFaces = 1;

                    var imageData = Marshal.AllocHGlobal(userInfo.VisibleLightImage.Length);
                    try
                    {
                        Marshal.Copy(userInfo.VisibleLightImage, 0, imageData, userInfo.VisibleLightImage.Length);

                        var structHeaderSize = Marshal.SizeOf(typeof(BS2FaceExUnwarped));
                        var totalSize = structHeaderSize + (int)userInfo.VisibleLightImage.Length;
                        userBlob.faceExObjs = Marshal.AllocHGlobal(totalSize);
                        var curFaceExObjects = userBlob.faceExObjs;

                        var unwarped = SupremaV2Utility.AllocateStructure<BS2FaceExUnwarped>();
                        unwarped.flag = 0;
                        unwarped.imageLen = (uint)userInfo.VisibleLightImage.Length;

                        Marshal.StructureToPtr(unwarped, curFaceExObjects, false);
                        curFaceExObjects += structHeaderSize;
                        SupremaV2Utility.CopyMemory(curFaceExObjects, imageData, (uint)userInfo.VisibleLightImage.Length);

                    }
                    finally
                    {
                        Marshal.FreeHGlobal(imageData);
                    }


                }
                else if (deviceSupportedFeatures.FaceSupported && userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                {

                    var faceStructSize = Marshal.SizeOf(typeof(BS2Face));
                    //var faceList = new List<BS2Face>();

                    userBlob.user.numFaces = (byte)userInfo.FaceDataList.Count;
                    userBlob.faceObjs = Marshal.AllocHGlobal(faceStructSize * userInfo.FaceDataList.Count);
                    var curFacePtr = userBlob.faceObjs;

                    for (var i = 0; i < userInfo.FaceDataList.Count; i++)
                    {


                        var face = userInfo.FaceDataList[i];
                        if (face.SupremaSdk2FaceImageData.IsCollectionNotNullOrEmpty() && face.SupremaSdk2FaceImageData.Length > BS2Environment.BS2_FACE_IMAGE_SIZE)
                        {
                            LoggingSystem.LogInfo("Suprema SDK 2 face data is not valid. ImageData len is to long", new { Face = face, DeviceNumber = DeviceInfo.DeviceNumber });
                            continue;
                        }
                        if (face.TemplateData.IsCollectionNotNullOrEmpty() && face.TemplateData.Length > BS2Environment.BS2_TEMPLATE_PER_FACE * BS2Environment.BS2_FACE_TEMPLATE_LENGTH)
                        {
                            LoggingSystem.LogInfo("Suprema SDK 2 face data is not valid. Template data len is to long", new { Face = face, DeviceNumber = DeviceInfo.DeviceNumber });
                            continue;
                        }
                        var faceStructure = AllocateStructure<BS2Face>();
                        faceStructure.templateData = face.TemplateData;
                        faceStructure.faceIndex = (byte)i;
                        faceStructure.flag = (byte)face.SupremaSdk2FaceFlag;
                        faceStructure.imageData = face.SupremaSdk2FaceImageData;
                        faceStructure.imageLen = (ushort)face.SupremaSdk2FaceImageLen;
                        faceStructure.numOfTemplate = (byte)face.SupremaSdk2FaceNumOfTemplate;
                        Marshal.StructureToPtr(faceStructure, curFacePtr, false);
                        curFacePtr += faceStructSize;
                    }
                }
                else
                {
                    userBlob.user.numFaces = 0;
                    userBlob.faceObjs = IntPtr.Zero;
                }


                var userBlobArray = AllocateStructureArray<BS2UserFaceExBlob>(1);
                userBlobArray[0] = userBlob;
                var res = ApiV2.BS2_EnrollUserFaceEx(SdkContext, _deviceId, userBlobArray, 1, 1);
                RaiseErrorIfRequired(res);

            }
            finally
            {
                if (userBlob.cardObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.cardObjs);
                }
                if (userBlob.fingerObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.fingerObjs);
                }
                if (userBlob.faceObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.faceObjs);
                }
                if (userBlob.user_photo_obj != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.user_photo_obj);
                }
                if (userBlob.faceExObjs != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(userBlob.faceExObjs);
                }
            }

            // USER PRIVILEGE
            var userIds = new List<string> { userInfo.EmployeeNumber.ToString() };
            var item = SupremaV2Utility.AllocateStructure<BS2AuthOperatorLevel>();
            var operatorLevelStructSize = Marshal.SizeOf(typeof(BS2AuthOperatorLevel));
            var operatorLevelObject = Marshal.AllocHGlobal(operatorLevelStructSize * userIds.Count);
            var currentOperatorLevelObject = operatorLevelObject;

            try
            {
                foreach (var strUserId in userIds)
                {
                    var userIdArrayForPrivilege = Encoding.UTF8.GetBytes(strUserId);
                    Array.Clear(item.userID, 0, BS2Environment.BS2_USER_ID_SIZE);
                    Array.Copy(userIdArrayForPrivilege, item.userID, userIdArrayForPrivilege.Length);
                    //item.level = (byte)BS2UserOperatorEnum.ADMIN;
                    item.level = (byte)userInfo.Privilege;// (byte)BS2UserOperatorEnum.ADMIN;
                    Marshal.StructureToPtr(item, currentOperatorLevelObject, false);
                    currentOperatorLevelObject = (IntPtr)((long)currentOperatorLevelObject + operatorLevelStructSize);
                }

                var result = ApiV2.BS2_SetAuthOperatorLevelEx(SdkContext, _deviceId, operatorLevelObject, (uint)userIds.Count);
                RaiseErrorIfRequired(result, new List<BS2ErrorCode> { BS2ErrorCode.BS_SDK_ERROR_NOT_SUPPORTED });
            }
            finally
            {
                Marshal.FreeHGlobal(operatorLevelObject);
            }


        }

        public DtoEmployeeDeviceRelatedData GetUserById(long userId, TemplateTypeEnumeration enrollType)
        {
            var uid = IntPtr.Zero;
            var operatorLevelObject = IntPtr.Zero;
            var result = new DtoEmployeeDeviceRelatedData();
            try
            {
                if (IsDeviceConnected == false)
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
                uid = Marshal.AllocHGlobal(BS2Environment.BS2_USER_ID_SIZE);
                var userIdArray = SupremaV2Utility.StringToByte(BS2Environment.BS2_USER_ID_SIZE, userId.ToString());
                Marshal.Copy(userIdArray, 0, uid, userIdArray.Length);

                var supportedFeatures = GetSupportedFeatures(DeviceInfoFromDevice, DeviceInfoExFromDevice);
                var allUserBlobs = new BS2UserFaceExBlob[1];
                var resultOfNativeMethodCall = ApiV2.BS2_GetUserDatasFaceEx(SdkContext, _deviceId, uid, 1, allUserBlobs, (uint)BS2UserMaskEnum.ALL);
                RaiseErrorIfRequired(resultOfNativeMethodCall);
                var userBlobForProcess = allUserBlobs[0];
                if (!SupremaV2Utility.IsUserDefinedInDevice(userBlobForProcess))
                {
                    return null;
                }

                using (var decodedUser = new Sdk2UserFaceExDecoder(userBlobForProcess, supportedFeatures))
                {
                    result.UserName = decodedUser.UserName;
                    result.EmployeeNumber = decodedUser.UserId;
                    result.Privilege = (int)BS2UserOperatorEnum.NONE;
                    result.StartTime = DeviceSharedHelperMethods.ConvertUnixTimestampToServerLocalTime(userBlobForProcess.setting.startTime, DeviceInfo.TimeSetting);
                    result.EndTime = DeviceSharedHelperMethods.ConvertUnixTimestampToServerLocalTime(userBlobForProcess.setting.endTime, DeviceInfo.TimeSetting);
                    result.HardwareProfileImage = decodedUser.UserProfileImage;
                    result.RfCardNumbers = decodedUser.CardNumbers.IsCollectionNotNullOrEmpty() ? decodedUser.CardNumbers : null;
                    result.VisibleLightImage = decodedUser.VisibleLightImage;


                    // VERIFICATION STYLE
                    if (supportedFeatures.FaceExScanSupported)
                    {
                        result.VerificationStyle = (int)MapHardwareAuthModeExToSoftwareAuthMode(decodedUser.UserSettingsEx);
                    }
                    else
                    {
                        result.VerificationStyle = (int)MapHardwareAuthModeToSoftwareAuthMode(decodedUser.UserSettings);
                    }

                    if (enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
                    {
                        foreach (var fp in decodedUser.FingerprintList)
                        {
                            var dtoFingerData = new DtoEmployeeFinger
                            {
                                EmployeeNumber = decodedUser.UserId,
                                FingerIndex = fp.index,
                                TemplateData = fp.data
                            };
                            result.FingerDataList.Add(dtoFingerData);
                        }
                    }

                    if (enrollType.HasFlag(TemplateTypeEnumeration.Face))
                    {
                        foreach (var face in decodedUser.FaceList)
                        {
                            var dtoFace = new DtoEmployeeFace
                            {
                                EmployeeNumber = decodedUser.UserId,
                                FaceIndex = face.faceIndex,
                                TemplateData = face.templateData,
                                Length = face.templateData.Length,
                                SupremaSdk2FaceFlag = face.flag,
                                SupremaSdk2FaceImageData = face.imageData,
                                SupremaSdk2FaceImageLen = face.imageLen,
                                SupremaSdk2FaceNumOfTemplate = face.numOfTemplate,
                            };
                            result.FaceDataList.Add(dtoFace);
                        }
                    }
                }

                resultOfNativeMethodCall = ApiV2.BS2_GetAuthOperatorLevelEx(SdkContext, _deviceId, uid, 1, out operatorLevelObject, out var numOperatorLevel);
                RaiseErrorIfRequired(resultOfNativeMethodCall, new List<BS2ErrorCode> { BS2ErrorCode.BS_SDK_ERROR_NOT_SUPPORTED });
                if (numOperatorLevel > 0)
                {
                    var currentUserPrivilegesObj = operatorLevelObject;
                    var currentOperationLevel = (BS2AuthOperatorLevel)Marshal.PtrToStructure(currentUserPrivilegesObj, typeof(BS2AuthOperatorLevel));
                    result.Privilege = currentOperationLevel.level;
                }

            }
            finally
            {
                if (uid != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(uid);
                }
                if (operatorLevelObject != IntPtr.Zero)
                {
                    ApiV2.BS2_ReleaseObject(operatorLevelObject);
                }
            }

            return result;

        }


        #endregion

        #region Access Control


        public void OpenDoorPermanent(DtoSupremaSdk2DeviceDoor doorInfo)
        {
            var doorIdObj = Marshal.AllocHGlobal(4 * /*(Count)*/1);
            var currentDoorIdObj = doorIdObj;
            try
            {
                var doorIds = new List<int> { doorInfo.DeviceDoorId };
                foreach (uint item in doorIds)
                {
                    Marshal.WriteInt32(currentDoorIdObj, (int)item);
                    currentDoorIdObj = (IntPtr)((long)currentDoorIdObj + 4);
                }
                var resultOfNativeMethodCall = ApiV2.BS2_UnlockDoor(SdkContext, _deviceId, doorInfo.UnlockFlag, doorIdObj, 1);
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.OpenDoor))
                {
                    LoggingSystem.LogInfo("Suprema 2 OpenDoorPermanent", new
                    {
                        DeviceInfo,
                        Result = resultOfNativeMethodCall
                    });
                }
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                Marshal.FreeHGlobal(doorIdObj);
            }
        }

        public void CloseDoorPermanent(DtoSupremaSdk2DeviceDoor doorInfo)
        {
            var doorIdObj = Marshal.AllocHGlobal(4 * /*(Count)*/1);
            var currentDoorIdObj = doorIdObj;
            try
            {
                var doorIds = new List<int> { doorInfo.DeviceDoorId };
                foreach (uint item in doorIds)
                {
                    Marshal.WriteInt32(currentDoorIdObj, (int)item);
                    currentDoorIdObj = (IntPtr)((long)currentDoorIdObj + 4);
                }
                var resultOfNativeMethodCall = ApiV2.BS2_LockDoor(SdkContext, _deviceId, doorInfo.LockFlags, doorIdObj, 1);
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.OpenDoor))
                {
                    LoggingSystem.LogInfo("Suprema 2 OpenDoorPermanent", new
                    {
                        DeviceInfo,
                        Result = resultOfNativeMethodCall
                    });
                }
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                Marshal.FreeHGlobal(doorIdObj);
            }
        }

        public void OpenDoor(DtoSupremaSdk2DeviceDoor doorInfo)
        {
            OpenDoorPermanent(doorInfo);
            Thread.Sleep(doorInfo.AutoLockTimeout * 1000);
            ReleaseDoor(doorInfo);
        }

        public void OpenDoorWithDelay(DtoSupremaSdk2DeviceDoor doorInfo, int delayInSecond)
        {
            OpenDoorPermanent(doorInfo);
            Thread.Sleep(delayInSecond * 1000);
            ReleaseDoor(doorInfo);
        }

        public void SendHolidays(List<DtoSupremaSdk2DeviceHolidayGroup> holidayGroups)
        {
            var holidayGroupList = new List<BS2HolidayGroup>();
            for (var i = 0; i < holidayGroups.Count; ++i)
            {
                var currentHolidayGroup = holidayGroups[i];
                if (currentHolidayGroup.Holidays.IsCollectionNullOrEmpty())
                {
                    continue;
                }
                var holidayGroup = AllocateStructure<BS2HolidayGroup>();
                holidayGroup.id = (uint)currentHolidayGroup.GroupNumber;
                holidayGroup.numHolidays = (byte)currentHolidayGroup.Holidays.Count;
                var holidayGroupArray = Encoding.UTF8.GetBytes(currentHolidayGroup.GroupName);
                Array.Clear(holidayGroup.name, 0, BS2Environment.BS2_MAX_HOLIDAY_GROUP_NAME_LEN);
                Array.Copy(holidayGroupArray, holidayGroup.name, holidayGroupArray.Length);

                for (byte j = 0; j < holidayGroup.numHolidays; ++j)
                {
                    var currentHoliday = currentHolidayGroup.Holidays[j];
                    holidayGroup.holidays[j].date = (uint)DeviceSharedHelperMethods.ConvertToUnixTimestampAndConsiderDateAsUtc
                        (currentHoliday.HolidayDate.Date);
                    holidayGroup.holidays[j].recurrence = (byte)(currentHoliday.IsRepeatYearly ? 1 : 0);
                }
                holidayGroupList.Add(holidayGroup);
            }
            var structSize = Marshal.SizeOf(typeof(BS2HolidayGroup));
            var holidayGroupListObj = Marshal.AllocHGlobal(structSize * holidayGroupList.Count);

            try
            {
                var curHolidayGroupListObj = holidayGroupListObj;
                foreach (var item in holidayGroupList)
                {
                    Marshal.StructureToPtr(item, curHolidayGroupListObj, false);
                    curHolidayGroupListObj = (IntPtr)((long)curHolidayGroupListObj + structSize);
                }

                var result = ApiV2.BS2_SetHolidayGroup(SdkContext, _deviceId, holidayGroupListObj, (UInt32)holidayGroupList.Count);
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(holidayGroupListObj);
            }

        }

        public void SetAccessSchedules(List<DtoSupremaSdk2AccessSchedule> accessSchedules)
        {
            if (accessSchedules.IsCollectionNullOrEmpty())
            {
                return;
            }
            var accessScheduleList = new List<CSP_BS2Schedule>();
            for (var i = 0; i < accessSchedules.Count; ++i)
            {
                var currentAccessSchedule = accessSchedules[i];
                if (currentAccessSchedule.Elements.IsCollectionNullOrEmpty())
                {
                    continue;
                }
                var accessSchedule = AllocateStructure<CSP_BS2Schedule>();
                accessSchedule.id = (uint)currentAccessSchedule.AccessScheduleNumber;
                var accessScheduleArray = Encoding.UTF8.GetBytes(currentAccessSchedule.Title);
                Array.Clear(accessSchedule.name, 0, BS2Environment.BS2_MAX_SCHEDULE_NAME_LEN);
                Array.Copy(accessScheduleArray, accessSchedule.name, accessScheduleArray.Length);

                byte countOfHolidays = 0;
                if (currentAccessSchedule.HolidayGroupNumber1.HasValue)
                {
                    accessSchedule.holidaySchedules[countOfHolidays].id = (uint)currentAccessSchedule.HolidayGroupNumber1.Value;
                    accessSchedule.holidaySchedules[countOfHolidays].schedule =
                        GetDaySchedule(AcSupremaSdk2TimezoneElementCodeEnumeration.Holiday1, currentAccessSchedule.Elements);
                    countOfHolidays++;

                }
                if (currentAccessSchedule.HolidayGroupNumber2.HasValue)
                {
                    accessSchedule.holidaySchedules[countOfHolidays].id = (uint)currentAccessSchedule.HolidayGroupNumber2.Value;
                    accessSchedule.holidaySchedules[countOfHolidays].schedule =
                        GetDaySchedule(AcSupremaSdk2TimezoneElementCodeEnumeration.Holiday2, currentAccessSchedule.Elements);
                    countOfHolidays++;

                }
                if (currentAccessSchedule.HolidayGroupNumber3.HasValue)
                {
                    countOfHolidays++;
                    accessSchedule.holidaySchedules[countOfHolidays].id = (uint)currentAccessSchedule.HolidayGroupNumber3.Value;
                    accessSchedule.holidaySchedules[countOfHolidays].schedule =
                        GetDaySchedule(AcSupremaSdk2TimezoneElementCodeEnumeration.Holiday3, currentAccessSchedule.Elements);
                    countOfHolidays++;

                }
                if (currentAccessSchedule.HolidayGroupNumber4.HasValue)
                {
                    accessSchedule.holidaySchedules[countOfHolidays].id = (uint)currentAccessSchedule.HolidayGroupNumber4.Value;
                    accessSchedule.holidaySchedules[countOfHolidays].schedule =
                        GetDaySchedule(AcSupremaSdk2TimezoneElementCodeEnumeration.Holiday4, currentAccessSchedule.Elements);
                    countOfHolidays++;
                }
                accessSchedule.numHolidaySchedules = countOfHolidays;
                accessSchedule.isDaily = Convert.ToByte(false);
                for (byte loop = 0; loop < BS2Environment.BS2_NUM_WEEKDAYS; ++loop)
                {
                    accessSchedule.scheduleUnion.weekly.schedule[loop] =
                        GetDaySchedule((AcSupremaSdk2TimezoneElementCodeEnumeration)loop, currentAccessSchedule.Elements);
                }
                accessScheduleList.Add(accessSchedule);
            }
            var result = (int)ApiV2.CSP_BS2_SetAccessSchedule(SdkContext, _deviceId,
                accessScheduleList.ToArray(), (uint)accessScheduleList.Count);
            RaiseErrorIfRequired(result);
        }

        public void SetAccessLevels(List<DtoSupremaSdk2AccessLevel> accessLevels)
        {
            if (accessLevels.IsCollectionNullOrEmpty())
            {
                return;
            }
            var accessLevelList = new List<BS2AccessLevel>();

            for (var i = 0; i < accessLevels.Count; ++i)
            {
                var currentAccessGroup = accessLevels[i];
                if (currentAccessGroup.DoorAccessSchedules.IsCollectionNullOrEmpty())
                {
                    continue;
                }
                var accessLevel = AllocateStructure<BS2AccessLevel>();
                accessLevel.id = (uint)currentAccessGroup.AccessLevelNumber;
                var accessGroupArray = Encoding.UTF8.GetBytes(currentAccessGroup.Title);
                Array.Clear(accessLevel.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
                Array.Copy(accessGroupArray, accessLevel.name, accessGroupArray.Length);

                accessLevel.numDoorSchedules = (byte)currentAccessGroup.DoorAccessSchedules.Count;

                for (byte j = 0; j < accessLevel.numDoorSchedules; ++j)
                {
                    accessLevel.doorSchedules[j].doorID = (uint)currentAccessGroup.DoorAccessSchedules[i].DoorId;
                    accessLevel.doorSchedules[j].scheduleID = (uint)currentAccessGroup.DoorAccessSchedules[i].AccessScheduleNumber;
                }

                accessLevelList.Add(accessLevel);
            }

            var structSize = Marshal.SizeOf(typeof(BS2AccessLevel));
            var accessLevelListObj = Marshal.AllocHGlobal(structSize * accessLevelList.Count);
            try
            {
                var curAccessLevelListObj = accessLevelListObj;
                foreach (var item in accessLevelList)
                {
                    Marshal.StructureToPtr(item, curAccessLevelListObj, false);
                    curAccessLevelListObj = (IntPtr)((long)curAccessLevelListObj + structSize);
                }
                var result = (int)(BS2ErrorCode)ApiV2.BS2_SetAccessLevel(SdkContext, _deviceId, accessLevelListObj, (uint)accessLevelList.Count);
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(accessLevelListObj);
            }

        }

        public void SetAccessGroups(List<DtoSupremaSdk2AccessGroup> accessGroups)
        {
            if (accessGroups.IsCollectionNullOrEmpty())
            {
                return;
            }
            var accessGroupList = new List<BS2AccessGroup>();
            foreach (var currentAccessGroup in accessGroups)
            {
                if (currentAccessGroup.AccessLevelNumbers.IsCollectionNullOrEmpty())
                {
                    continue;
                }
                var accessGroup = AllocateStructure<BS2AccessGroup>();
                accessGroup.id = (uint)currentAccessGroup.AccessGroupNumber;
                var accessGroupArray = Encoding.UTF8.GetBytes(currentAccessGroup.Title);
                Array.Clear(accessGroup.name, 0, BS2Environment.BS2_MAX_ACCESS_GROUP_NAME_LEN);
                Array.Copy(accessGroupArray, accessGroup.name, accessGroupArray.Length);

                accessGroup.numOflevelUnion.numAccessLevels = (byte)currentAccessGroup.AccessLevelNumbers.Count;
                for (var j = 0; j < accessGroup.numOflevelUnion.numAccessLevels; j++)
                {
                    accessGroup.levelUnion.accessLevels[j] = (uint)currentAccessGroup.AccessLevelNumbers[j];
                }
                accessGroupList.Add(accessGroup);
            }

            var structSize = Marshal.SizeOf(typeof(BS2AccessGroup));
            var accessGroupListObj = Marshal.AllocHGlobal(structSize * accessGroupList.Count);
            try
            {
                var curAccessGroupListObj = accessGroupListObj;
                foreach (var item in accessGroupList)
                {
                    Marshal.StructureToPtr(item, curAccessGroupListObj, false);
                    curAccessGroupListObj = (IntPtr)((long)curAccessGroupListObj + structSize);
                }
                var result = (int)(BS2ErrorCode)ApiV2.BS2_SetAccessGroup(SdkContext, _deviceId, accessGroupListObj, (UInt32)accessGroupList.Count); ;
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(accessGroupListObj);
            }

        }

        public void SetDoorInfo(List<DtoSupremaSdk2DeviceDoor> doorInfos)
        {
            var doorList = new List<BS2Door>();

            foreach (var currentDoor in doorInfos)
            {
                var door = AllocateStructure<BS2Door>();
                door.doorID = (uint)currentDoor.DeviceDoorId;
                var doorArray = Encoding.UTF8.GetBytes(currentDoor.Title);
                Array.Clear(door.name, 0, BS2Environment.BS2_MAX_DOOR_NAME_LEN);
                Array.Copy(doorArray, door.name, doorArray.Length);
                door.entryDeviceID = _deviceId;
                door.exitDeviceID = 0;
                door.autoLockTimeout = (uint)currentDoor.AutoLockTimeout;
                door.instantLock = (byte)(currentDoor.InstantLock ? 1 : 0);
                door.unconditionalLock = (byte)(currentDoor.UnconditionalLock ? 1 : 0);
                door.lockFlags = currentDoor.LockFlags;
                door.unlockFlags = currentDoor.UnlockFlag;
                door.heldOpenTimeout = (uint)currentDoor.HeldOpenTimeout;
                for (var loop = 0; loop < BS2Environment.BS2_MAX_FORCED_OPEN_ALARM_ACTION; ++loop)
                {
                    door.forcedOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }
                for (var loop = 0; loop < BS2Environment.BS2_MAX_HELD_OPEN_ALARM_ACTION; ++loop)
                {
                    door.heldOpenAlarm[loop].type = (byte)BS2ActionTypeEnum.NONE;
                }
                door.heldOpenAlarm[0].type = currentDoor.HeldOpenAlarmType;
                door.forcedOpenAlarm[0].type = currentDoor.ForceOpenAlarmType;
                if (currentDoor.HasRelay)
                {
                    door.relay.deviceID = _deviceId;
                    door.relay.port = (byte)currentDoor.Relay;
                }
                if (currentDoor.HasDoorSensor)
                {
                    door.sensor.deviceID = _deviceId;
                    door.sensor.port = currentDoor.DoorSensor;
                    door.sensor.switchType = currentDoor.SensorType;
                    door.sensor.apbUseDoorSensor = (byte)(currentDoor.ApbUseDoorSensor ? 1 : 0);
                }
                if (currentDoor.HasExitButton)
                {
                    door.button.deviceID = _deviceId;
                    door.button.port = currentDoor.ExitButton;
                    door.button.switchType = currentDoor.ExitButtonType;
                }
                door.dualAuthDevice = (byte)BS2DualAuthDeviceEnum.NO_DEVICE;
                door.dualAuthScheduleID = (uint)BS2ScheduleIDEnum.NEVER;
                door.dualAuthTimeout = 0;
                door.dualAuthApprovalType = (byte)BS2DualAuthApprovalEnum.NONE;
                door.numDualAuthApprovalGroups = 0;
                doorList.Add(door);
            }

            var structSize = Marshal.SizeOf(typeof(BS2Door));
            var doorListObj = Marshal.AllocHGlobal(structSize * doorList.Count);
            try
            {
                var curDoorListObj = doorListObj;
                foreach (var item in doorList)
                {
                    Marshal.StructureToPtr(item, curDoorListObj, false);
                    curDoorListObj = (IntPtr)((long)curDoorListObj + structSize);
                }

                var result = ApiV2.BS2_SetDoor(SdkContext, _deviceId, doorListObj, (UInt32)doorList.Count);
                RaiseErrorIfRequired(result);
            }
            finally
            {
                Marshal.FreeHGlobal(doorListObj);
            }
        }


        private void ReleaseDoor(DtoSupremaSdk2DeviceDoor doorInfo)
        {
            var doorIdObj = Marshal.AllocHGlobal(4 * /*(Count)*/1);
            var currentDoorIdObj = doorIdObj;
            try
            {
                var doorIds = new List<int> { doorInfo.DeviceDoorId };
                foreach (uint item in doorIds)
                {
                    Marshal.WriteInt32(currentDoorIdObj, (int)item);
                    currentDoorIdObj = (IntPtr)((long)currentDoorIdObj + 4);
                }
                var resultOfNativeMethodCall = ApiV2.BS2_ReleaseDoor(SdkContext, _deviceId, doorInfo.LockFlags, doorIdObj, 1);
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.OpenDoor))
                {
                    LoggingSystem.LogInfo("Suprema 2 OpenDoorPermanent", new
                    {
                        DeviceInfo,
                        Result = resultOfNativeMethodCall
                    });
                }
                RaiseErrorIfRequired(resultOfNativeMethodCall);
            }
            finally
            {
                Marshal.FreeHGlobal(doorIdObj);
            }
        }


        #endregion

        #endregion


        #region Utilities

        private void RaiseErrorIfRequired(int errorCode, ICollection<BS2ErrorCode> excludeList = null)
        {
            if (errorCode != (int)BS2ErrorCode.BS_SDK_SUCCESS)
            {
                if (excludeList.IsCollectionNotNullOrEmpty() && excludeList.Contains((BS2ErrorCode)errorCode))
                {
                    return;
                }
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
        }

        private static BS2DaySchedule GetDaySchedule(AcSupremaSdk2TimezoneElementCodeEnumeration dayType, IEnumerable<DtoSupremaSdk2AccessScheduleElement> elements)
        {
            var currentDayElements = elements.Where(row => row.ElementCode == (short)dayType).OrderBy(row => row.StartTime)
                .Take(BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY).ToList();
            var result = new BS2DaySchedule
            {
                periods = new BS2TimePeriod[BS2Environment.BS2_MAX_TIME_PERIODS_PER_DAY],
                numPeriods = (byte)currentDayElements.Count
            };
            for (var i = 0; i < currentDayElements.Count; i++)
            {
                var element = currentDayElements[i];
                result.periods[i].startTime = (ushort)DateTimeHelper.ConvertTimeDurationToMinutes(element.StartTime);
                result.periods[i].endTime = (ushort)DateTimeHelper.ConvertTimeDurationToMinutes(element.EndTime);
            }
            return result;
        }

        private static SupremV2DeviceSupportedFeatures GetSupportedFeatures(BS2SimpleDeviceInfo deviceInfo, BS2SimpleDeviceInfoEx deviceInfoEx)
        {
            var result = new SupremV2DeviceSupportedFeatures
            {
                PhotoSupported = Convert.ToBoolean(deviceInfo.userPhotoSupported),
                NameSupported = Convert.ToBoolean(deviceInfo.userNameSupported),
                CardSupported = Convert.ToBoolean(deviceInfo.cardSupported),
                FingerSupported = Convert.ToBoolean(deviceInfo.fingerSupported),
                PinSupported = Convert.ToBoolean(deviceInfo.pinSupported),
                FaceSupported = Convert.ToBoolean(deviceInfo.faceSupported),
                QrSupported = Convert.ToBoolean(deviceInfoEx.supported | (uint)BS2SupportedInfoMask.BS2_SUPPORT_QR),
                FingerScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (uint)BS2SupportedInfoMask.BS2_SUPPORT_FINGER_SCAN),
                FaceScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (uint)BS2SupportedInfoMask.BS2_SUPPORT_FACE_SCAN),
                FaceExScanSupported = Convert.ToBoolean(deviceInfoEx.supported & (uint)BS2SupportedInfoMask.BS2_SUPPORT_FACE_EX_SCAN)
            };
            return result;
        }

        private static uint GetUserMaskBasedOnSupportedFeatures(SupremV2DeviceSupportedFeatures supportedFeatures)
        {
            var userMask = (uint)BS2UserMaskEnum.DATA | (uint)BS2UserMaskEnum.SETTING | (UInt32)BS2UserMaskEnum.ACCESS_GROUP;
            if (supportedFeatures.NameSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.NAME;
            }
            if (supportedFeatures.PinSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.PIN;
            }
            if (supportedFeatures.PhotoSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.PHOTO;
            }
            if (supportedFeatures.PinSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.CARD;
            }
            if (supportedFeatures.FingerSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.FINGER;
            }
            if (supportedFeatures.FaceSupported)
            {
                userMask |= (UInt32)BS2UserMaskEnum.FACE;
            }
            if (supportedFeatures.FaceExScanSupported)
            {
                userMask |= ((UInt32)BS2UserMaskEnum.SETTING_EX | (UInt32)BS2UserMaskEnum.FACE_EX);
            }

            return userMask;
        }


        private static PrivateAuthMode MapSoftwareAuthModeToHardwareAuthMode(SupremaSdk2VerificationStyleEnumeration verificationStyle)
        {
            var result = new PrivateAuthMode();
            switch (verificationStyle)
            {
                case SupremaSdk2VerificationStyleEnumeration.BiometricOnly:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricAndPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.BIOMETRIC_PIN;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardOnly:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_ONLY;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometric:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_BIOMETRIC;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_PIN;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometricOrPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometricAndPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometric:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_BIOMETRIC;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometricOrPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometricAndPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_BIOMETRIC_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                    result.CardAuthMode = BS2CardAuthModeEnum.PROHIBITED;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrCard:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_ONLY;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrCardOrPassword:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.BIOMETRIC_ONLY;
                    result.CardAuthMode = BS2CardAuthModeEnum.CARD_ONLY;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.Prohibited:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2CardAuthModeEnum.PROHIBITED;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.DeviceSetting:
                default:
                    result.BioMetricAuthMode = BS2FingerAuthModeEnum.NONE;
                    result.CardAuthMode = BS2CardAuthModeEnum.NONE;
                    result.PasswordAuthMode = BS2IDAuthModeEnum.NONE;
                    break;
            }
            return result;
        }

        private static PrivateAuthModeEx MapSoftwareAuthModeToHardwareAuthModeEx(SupremaSdk2VerificationStyleEnumeration verificationStyle)
        {
            var result = new PrivateAuthModeEx();
            switch (verificationStyle)
            {
                case SupremaSdk2VerificationStyleEnumeration.BiometricOnly:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricAndPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_PIN;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_PIN;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardOnly:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_ONLY;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometric:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_PIN;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometricOrPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.CardAndBiometricAndPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_PIN;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;

                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometric:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometricOrPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_OR_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.IdAndBiometricAndPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_PIN;

                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrCard:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_ONLY;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.BiometricOrCardOrPassword:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.EXT_CARD_ONLY;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.EXT_ID_PIN;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.Prohibited:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.PROHIBITED;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.PROHIBITED;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.PROHIBITED;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.PROHIBITED;
                    break;
                case SupremaSdk2VerificationStyleEnumeration.DeviceSetting:
                default:
                    result.FaceAuthMode = BS2ExtFaceAuthModeEnum.NONE;
                    result.FingerAuthMode = BS2ExtFingerprintAuthModeEnum.NONE;
                    result.CardAuthMode = BS2ExtCardAuthModeEnum.NONE;
                    result.IdAuthMode = BS2ExtIDAuthModeEnum.NONE;
                    break;
            }
            return result;
        }

        private static SupremaSdk2VerificationStyleEnumeration MapHardwareAuthModeExToSoftwareAuthMode(BS2UserSettingEx userSettings)
        {
            if (((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY
                || (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY)
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOnly;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.EXT_FACE_FINGERPRINT_PIN
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_FACE_PIN
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricAndPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_ONLY
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.CardOnly;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.CardAndBiometric;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_PIN
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.CardAndPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_OR_PIN
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.CardAndBiometricOrPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_FACE_OR_FINGERPRINT_PIN
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.CardAndBiometricAndPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.IdAndBiometric;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_PIN
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.IdAndPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_OR_PIN
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.IdAndBiometricOrPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_FACE_OR_FINGERPRINT_PIN
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.IdAndBiometricAndPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_PIN
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_ONLY
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrCard;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.EXT_FACE_ONLY
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.EXT_FINGERPRINT_ONLY
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.EXT_CARD_ONLY
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.EXT_ID_PIN
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrCardOrPassword;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.PROHIBITED
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.PROHIBITED
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.PROHIBITED
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.Prohibited;
            }
            if ((BS2ExtFaceAuthModeEnum)userSettings.faceAuthMode == BS2ExtFaceAuthModeEnum.NONE
                && (BS2ExtFingerprintAuthModeEnum)userSettings.fingerprintAuthMode == BS2ExtFingerprintAuthModeEnum.NONE
                && (BS2ExtCardAuthModeEnum)userSettings.cardAuthMode == BS2ExtCardAuthModeEnum.NONE
                && (BS2ExtIDAuthModeEnum)userSettings.idAuthMode == BS2ExtIDAuthModeEnum.NONE
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.DeviceSetting;
            }
            return SupremaSdk2VerificationStyleEnumeration.DeviceSetting;
        }

        private static SupremaSdk2VerificationStyleEnumeration MapHardwareAuthModeToSoftwareAuthMode(BS2UserSetting userSettings)
        {
            switch ((BS2FingerAuthModeEnum)userSettings.fingerAuthMode)
            {
                case BS2FingerAuthModeEnum.BIOMETRIC_ONLY:
                    return SupremaSdk2VerificationStyleEnumeration.BiometricOnly;
                case BS2FingerAuthModeEnum.BIOMETRIC_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.BiometricAndPassword;
                case BS2FingerAuthModeEnum.NUM_OF_BIOMETRIC_AUTH_MODE:
                    return SupremaSdk2VerificationStyleEnumeration.CardOnly;
            }
            switch ((BS2CardAuthModeEnum)userSettings.cardAuthMode)
            {
                case BS2CardAuthModeEnum.CARD_ONLY:
                    return SupremaSdk2VerificationStyleEnumeration.CardOnly;
                case BS2CardAuthModeEnum.CARD_BIOMETRIC:
                    return SupremaSdk2VerificationStyleEnumeration.CardAndBiometric;
                case BS2CardAuthModeEnum.CARD_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.CardAndPassword;
                case BS2CardAuthModeEnum.CARD_BIOMETRIC_OR_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.CardAndBiometricOrPassword;
                case BS2CardAuthModeEnum.CARD_BIOMETRIC_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.CardAndBiometricAndPassword;
                case BS2CardAuthModeEnum.NUM_OF_CARD_AUTH_MODE:
                    return SupremaSdk2VerificationStyleEnumeration.IdAndBiometric;
            }
            switch ((BS2IDAuthModeEnum)userSettings.idAuthMode)
            {
                case BS2IDAuthModeEnum.ID_BIOMETRIC:
                    return SupremaSdk2VerificationStyleEnumeration.IdAndBiometric;
                case BS2IDAuthModeEnum.ID_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.IdAndPassword;
                case BS2IDAuthModeEnum.ID_BIOMETRIC_OR_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.IdAndBiometricOrPassword;
                case BS2IDAuthModeEnum.ID_BIOMETRIC_PIN:
                    return SupremaSdk2VerificationStyleEnumeration.IdAndBiometricAndPassword;
                case BS2IDAuthModeEnum.NUM_OF_ID_AUTH_MODE:
                    break;
            }

            if ((BS2FingerAuthModeEnum)userSettings.fingerAuthMode == BS2FingerAuthModeEnum.BIOMETRIC_ONLY
                && (BS2CardAuthModeEnum)userSettings.cardAuthMode == BS2CardAuthModeEnum.PROHIBITED
                && (BS2IDAuthModeEnum)userSettings.idAuthMode == BS2IDAuthModeEnum.ID_PIN
                )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrPassword;
            }
            else if ((BS2FingerAuthModeEnum)userSettings.fingerAuthMode == BS2FingerAuthModeEnum.BIOMETRIC_ONLY
                && (BS2CardAuthModeEnum)userSettings.cardAuthMode == BS2CardAuthModeEnum.CARD_ONLY
                && (BS2IDAuthModeEnum)userSettings.idAuthMode == BS2IDAuthModeEnum.PROHIBITED
                )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrCard;
            }
            else if ((BS2FingerAuthModeEnum)userSettings.fingerAuthMode == BS2FingerAuthModeEnum.BIOMETRIC_ONLY
                && (BS2CardAuthModeEnum)userSettings.cardAuthMode == BS2CardAuthModeEnum.CARD_ONLY
                && (BS2IDAuthModeEnum)userSettings.idAuthMode == BS2IDAuthModeEnum.ID_PIN
                )
            {
                return SupremaSdk2VerificationStyleEnumeration.BiometricOrCardOrPassword;
            }
            else if ((BS2FingerAuthModeEnum)userSettings.fingerAuthMode == BS2FingerAuthModeEnum.PROHIBITED
               && (BS2CardAuthModeEnum)userSettings.cardAuthMode == BS2CardAuthModeEnum.PROHIBITED
               && (BS2IDAuthModeEnum)userSettings.idAuthMode == BS2IDAuthModeEnum.PROHIBITED
               )
            {
                return SupremaSdk2VerificationStyleEnumeration.Prohibited;
            }
            else
            {
                return SupremaSdk2VerificationStyleEnumeration.DeviceSetting;
            }

        }


        #endregion


        #region Types

        private class PrivateAuthMode
        {
            public BS2CardAuthModeEnum CardAuthMode { get; set; }

            public BS2FingerAuthModeEnum BioMetricAuthMode { get; set; }

            public BS2IDAuthModeEnum PasswordAuthMode { get; set; }
        }



        private class PrivateAuthModeEx
        {
            public BS2ExtFaceAuthModeEnum FaceAuthMode { get; set; }
            public BS2ExtFingerprintAuthModeEnum FingerAuthMode { get; set; }
            public BS2ExtCardAuthModeEnum CardAuthMode { get; set; }
            public BS2ExtIDAuthModeEnum IdAuthMode { get; set; }
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
            if (disposing)
            {
                // Managed/COM op that can throw (RaiseErrorIfRequired) — explicit Dispose only,
                // never the finalizer thread (would terminate the process if it threw).
                if (!IsInPushMode && IsDeviceConnected)
                {
                    Disconnect();
                }
            }
            // Release the owned native context on both paths (finalizer backstop for a per-operation
            // adapter). BS2_ReleaseContext is a P/Invoke that returns an error code rather than raising
            // a managed exception, so it is safe on the finalizer thread. Push-mode contexts are owned
            // by the server, not the adapter, so they are not released here.
            if (!IsInPushMode && SdkContext != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseContext(SdkContext);
                SdkContext = IntPtr.Zero;
            }
            _disposed = true;
        }


        ~SupremaSdk2OnDemandAdapter()
        {
            Dispose(false);
        }

        #endregion





    }
}
