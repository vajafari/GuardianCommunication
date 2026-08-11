using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.Zk
{
    public class ZkOnDemandAdapter : IDisposable
    {
        private bool _isDeviceConnected;
        private bool _isDeviceEnable;
        private readonly zkemkeeper.CZKEMClass _communicationOcx;
        public DtoDevice DeviceInfo { get; set; }

        public ZkOnDemandAdapter(DtoDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;
            _communicationOcx = new zkemkeeper.CZKEMClass();
            if (!string.IsNullOrEmpty(DeviceInfo.DevicePassword) && DeviceInfo.DevicePassword.CanConvertToInt32() && DeviceInfo.DevicePassword.ToInt32() > 0)
            {
                _communicationOcx.SetCommPassword(DeviceInfo.DevicePassword.ToInt32());
            }

            if (DeviceInfo.DeviceNumber <= 0)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorMachineNumberIsNotValid);
            }

        }

        #region Public Methods

        #region Other

        public bool CancelOperation()
        {
           
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var result = _communicationOcx.CancelOperation();
            if (result)
            {
                return true;
            }
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public bool SetDateTime(DateTime now)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandSetDateTime))
            {
                LoggingSystem.LogInfo("ZK OnDemand SetDateTime", DeviceInfo);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var result = _communicationOcx.SetDeviceTime2(DeviceInfo.DeviceNumber, now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            if (result)
            {
                return true;
            }
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetDateTime))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetDateTime", DeviceInfo);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            if (_communicationOcx.GetDeviceTime
                    (DeviceInfo.DeviceNumber, ref year, ref month, ref day, ref hour, ref minute, ref second))
            {
                return new DateTime(year, month, day, hour, minute, second);
            }
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public void EnableDevice()
        {

            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            _isDeviceEnable = _communicationOcx.EnableDevice(DeviceInfo.DeviceNumber, true);
        }

        public void DisableDevice(int timeout)
        {

            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            if (_communicationOcx.EnableDevice(DeviceInfo.DeviceNumber, false))
            {
                _isDeviceEnable = false;
            }
        }

        public void RebootDevice()
        {
            var errorCode = 0;
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandRebootDevice))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetDateTime", DeviceInfo);
            }
            if (!_communicationOcx.RestartDevice(DeviceInfo.DeviceNumber))
            {
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
        }

        public void SendDeviceFunctionTitles(List<string> titles)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFunctionTitle))
            {
                LoggingSystem.LogInfo("ZK OnDemand Send Device Function Titles is calling", new
                {
                    DeviceInfo,
                    Titles = titles
                });
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            if (titles == null)
            {
                titles = new List<string>();
            }
            var culture = CultureInfo.CurrentCulture.Name;
            var uiCulture = CultureInfo.CurrentUICulture.Name;

            try
            {

                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-IR");
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");

                for (var i = 0; i < 8; i++)
                {
                    var res = _communicationOcx.SSR_SetShortkey(i + 1, 0, i + 1, string.Empty, 0,
                        "0000;0000;0000;0000;0000;0000;0000;");
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFunctionTitle))
                    {
                        LoggingSystem.LogInfo("ZK OnDemand Send Device Function Titles result", new
                        {
                            DeviceInfo,
                            Result = res
                        });
                    }
                }
                for (var i = 0; i < 8; i++)
                {
                    if (i < titles.Count)
                    {
                        //result = _communicationOcx.SSR_SetShortkey(i + 1, 1, i + 1, titles[i], 0,
                        //    "00:00;00:00;00:00;00:00;00:00;00:00;00:00;");
                        var res = _communicationOcx.SSR_SetShortkey(i + 1, 1, i + 1, titles[i], 0,
                            "0000;0000;0000;0000;0000;0000;0000;");
                        if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFunctionTitle))
                        {
                            LoggingSystem.LogInfo("ZK OnDemand Send Device Function Titles result", new
                            {
                                DeviceInfo,
                                Result = res
                            });
                        }
                    }
                }
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(uiCulture);
            }
        }

        public void DisableDeviceFunctionTitles()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFunctionTitle))
            {
                LoggingSystem.LogInfo("ZK OnDemand Disable Device Function Titles", DeviceInfo);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var culture = CultureInfo.CurrentCulture.Name;
            var uiCulture = CultureInfo.CurrentUICulture.Name;
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-IR");
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");

                for (var i = 0; i < 8; i++)
                {
                    var res = _communicationOcx.SSR_SetShortkey(i + 1, 0, i + 1, string.Empty, 0,
                        "0000;0000;0000;0000;0000;0000;0000;");
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFunctionTitle))
                    {
                        LoggingSystem.LogInfo("ZK OnDemand Disable Device Function Titles result", new
                        {
                            DeviceInfo,
                            Result = res
                        });
                    }
                }
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(uiCulture);
            }
        }
        
        public string GetFirmwareVersion()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFirmware))
            {
                LoggingSystem.LogInfo("ZK OnDemand Disable Device Function Titles", DeviceInfo);
            }
            var firmwareVersion = string.Empty;
            var result = _communicationOcx.GetFirmwareVersion(DeviceInfo.DeviceNumber, ref firmwareVersion);
            if (!result)
            {
                var errorCode = 0;
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
            return firmwareVersion;
        }

        public void UpgradeFirmware(string fileName, byte[] firmwareFile)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandFirmware))
            {
                LoggingSystem.LogInfo("ZK OnDemand Disable Device Function Titles", new { DeviceInfo, FileName = fileName });
            }
            var finalFileName = FileHelper.CreateFileOnCurrentExecutingPath(Path.Combine("Firmware", "Zk", fileName),
                firmwareFile);
            var result = _communicationOcx.UpdateFirmware(finalFileName);
            if (!result)
            {
                var errorCode = 0;
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
            RebootDevice();
        }

        #endregion

        #region Communication

        public bool SetCommunicationPassword(int passwordKey)
        {

            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var errorCode = 0;
            var result = _communicationOcx.SetDeviceCommPwd(DeviceInfo.DeviceNumber, passwordKey);
            if (result) return true;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public bool TestConnection()
        {
            Connect();
            return _isDeviceConnected;
        }

        public string GetSerialNumber()
        {

            _communicationOcx.GetSerialNumber(DeviceInfo.DeviceNumber, out var dwSerialNumber);
            return dwSerialNumber;
        }

        public void Connect()
        {
            if (_isDeviceConnected) return;
            if (string.IsNullOrEmpty(DeviceInfo.DeviceIp))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorIpIsNotValid);
            if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorTcpPortIsNotValid);
            // Bounded, cross-platform connect (replaces Delegate.BeginInvoke): run the blocking
            // Connect_Net on a task and wait up to the timeout. The result is committed to
            // _isDeviceConnected only if the task finished in time, so a late-completing connect
            // can never flip the field after we've already reported failure.
            var connectTask = Task.Run(() => _communicationOcx.Connect_Net(DeviceInfo.DeviceIp, DeviceInfo.TcpPort.Value));
            var connected = false;
            try
            {
                if (connectTask.Wait(DeviceInfo.ConnectTimeout * 1000))
                {
                    connected = connectTask.Result;
                }
                else
                {
                    // Timed out: observe any later fault so it isn't an unobserved task exception.
                    connectTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            catch (AggregateException)
            {
                // Connect_Net threw; treated as a failed connection (handled by GetLastError below).
            }
            _isDeviceConnected = connected;
            if (!_isDeviceConnected)
            {
                var errorCode = 0;
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
        }

        public void Disconnect()
        {

            _isDeviceConnected = false;
            _communicationOcx.Disconnect();
        }

        #endregion

        #region Attendance

        public bool ClearData()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandClearData))
            {
                LoggingSystem.LogInfo("ZK OnDemand Clear Data", DeviceInfo);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var result = _communicationOcx.ClearGLog(DeviceInfo.DeviceNumber);
            if (result)
            {
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber); //the data in the device should be refreshed
            }
            return result;
        }

        public List<DtoAttendance> Readout(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetData))
            {
                LoggingSystem.LogInfo("ZK OnDemand Readout is calling", DeviceInfo);
            }
            if (DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var errorCode = 0;
            var workCode = 0;
            var iGlCount = 0;
            var attRecords = new List<DtoAttendance>();
            if (_communicationOcx.ReadTimeGLogData(DeviceInfo.DeviceNumber,
                startDate.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")), endDate.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))))
            {

                while (_communicationOcx.SSR_GetGeneralLogData(DeviceInfo.DeviceNumber,
                    out var enrollNumber,
                    out var idwVerifyMode,
                    out var idwInOutMode,
                    out var idwYear,
                    out var idwMonth,
                    out var idwDay,
                    out var idwHour,
                    out var idwMinute,
                    out var idwSecond,
                    ref workCode))
                {
                    try
                    {
                        iGlCount++;
                        var status = idwInOutMode & 0x7F;
                        
                        attRecords.Add(new DtoAttendance
                        {
                            LogIdOnDevice = iGlCount,
                            UserIdOnDevice = enrollNumber.ToInt64(),
                            VerificationStyle = (int)ZkUtils.GetVerificationStyle(idwVerifyMode),
                            StatusCode = status,
                            DeviceId = DeviceInfo.Id,
                            CameraId = null,
                            AttendanceDateTime = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                            RfCardNumber = null,
                        });
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on converting Zk SSR_GetGeneralLogData to On Readout");
                    }
                }

            }
            else
            {
                _communicationOcx.GetLastError(ref errorCode);
                if (errorCode == 0) return attRecords;
                if (errorCode != (int)ZkErrorEnum.ErrorCodeNegative1
                    && errorCode != (int)ZkErrorEnum.ErrorCodeNegative2
                    && errorCode != (int)ZkErrorEnum.ErrorCodeNegative4991)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                }
            }
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetData))
            {
                LoggingSystem.LogInfo("ZK OnDemand Readout result", attRecords);
            }
            return attRecords;

        }

        public List<DtoAttendance> GetData(DateTime startDate, DateTime endDate)
        {
            return Readout(startDate, endDate);
        }

        public List<DtoAttendance> GetDataOldVersion()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetData))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetDataOldVersion is calling", DeviceInfo);
            }
            if (DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            var errorCode = 0;
            var idwWorkCode = 0;
            var iGlCount = 0;
            var attRecords = new List<DtoAttendance>();
            if (_communicationOcx.ReadGeneralLogData(DeviceInfo.DeviceNumber))
            {
                while (_communicationOcx.SSR_GetGeneralLogData(DeviceInfo.DeviceNumber, out var enrollNumber, out var idwVerifyMode, out var idwInOutMode,
                    out var idwYear, out var idwMonth, out var idwDay, out var idwHour, out var idwMinute, out var idwSecond, ref idwWorkCode))
                {
                    iGlCount++;
                    var att = new DtoAttendance
                    {
                        LogIdOnDevice = iGlCount,
                        UserIdOnDevice = enrollNumber.ToInt64(),
                        VerificationStyle = (int)ZkUtils.GetVerificationStyle(idwVerifyMode),
                        StatusCode = idwInOutMode,
                        DeviceId = DeviceInfo.Id,
                        CameraId = null,
                        AttendanceDateTime = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond),
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                        RfCardNumber = null,
                    };
                    attRecords.Add(att);
                }
            }
            else
            {
                _communicationOcx.GetLastError(ref errorCode);
                if (errorCode == 0) return attRecords;
                if (errorCode != (int)ZkErrorEnum.ErrorCodeNegative1
                    && errorCode != (int)ZkErrorEnum.ErrorCodeNegative2
                    && errorCode != (int)ZkErrorEnum.ErrorCodeNegative4991)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                }
            }
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetData))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetDataOldVersion result", attRecords);
            }
            return attRecords;
        }

        public List<DtoAttendance> GetDataWithDefaultDates()
        {
            return GetData(DateTime.Now.Date.AddDays(-5), DateTime.Now.Date.AddDays(3));
        }

        public int GetRecordCount()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetRecordCount", DeviceInfo);
            }
            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var recordCount = 0;
            var errorCode = 0;
            if (_communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 6, ref recordCount))
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
                {
                    LoggingSystem.LogInfo("ZK OnDemand GetRecordCount result", new
                    {
                        DeviceInfo,
                        Count = recordCount
                    });
                }
                return recordCount;
            }
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        #endregion

        #region Usering And Finger

        public void DeleteUserById(long userId)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandDeleteUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand DeleteUserById", new { DeviceInfo, UserId = userId });
            }
            if (!_isDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var result = _communicationOcx.SSR_DeleteEnrollDataExt(DeviceInfo.DeviceNumber, userId.ToString(), 12);
            if (!result)
            {
                var errorCode = 0;
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));


            }

            _communicationOcx.DelUserFace(DeviceInfo.DeviceNumber, userId.ToString(), 50);
        }

        public void DeleteAdministrators()
        {
            if (!_isDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            if (_communicationOcx.ClearAdministrators(DeviceInfo.DeviceNumber))
            {
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
                return;
            }
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandDeleteUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand DeleteAllUsers", new { DeviceInfo });
            }
            if (!_isDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            const int iDataFlag = 5;
            if (_communicationOcx.ClearData(DeviceInfo.DeviceNumber, iDataFlag))
            {
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
                return;
            }
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));

        }

        public DtoUserDeviceRelatedData GetUserInfoByUserId(long userId, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetUserInfoByUserId is calling", new { DeviceInfo, UserId = userId, EnrollType = enrollType });
            }
            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var enrollNumber = userId.ToString();
            var result = new DtoUserDeviceRelatedData();
            if (!_communicationOcx.SSR_GetUserInfo(
                DeviceInfo.DeviceNumber
                , enrollNumber
                , out var name
                , out var password
                , out var privilege
                , out var enabled))
            {
                return result;
            }
            result.UserIdOnDevice = enrollNumber.ToInt64();
            result.UserName = name;
            result.Password = password;
            //result.IsEnable = enabled;
            result.IsEnable = true;
            result.Privilege = privilege;
            if (privilege > 0)
            {
                result.Privilege = (int)ZkDevicePrivilegeEnumeration.SuperAdministrator;
            }




            if (_communicationOcx.GetUserInfoEx(DeviceInfo.DeviceNumber,
                enrollNumber.ToInt32(),
                out var verifyStyle, out _))
            {
                result.VerificationStyle = verifyStyle;
            }
            _communicationOcx.GetStrCardNumber(out var cardNumber);
            if (cardNumber.IsNotNullOrEmpty())
            {
                result.RfCardNumbers = new List<string> { cardNumber };
            }

            if (enrollType.HasFlag(TemplateTypeEnumeration.Face))
            {
                if (DeviceInfo.HasVisiblelight)
                {
                    var photoData = new byte[1024 * 1024];
                    if (_communicationOcx.GetUserFacePhotoByName
                        (DeviceInfo.DeviceNumber
                        , $"{userId}.jpg".ToString()
                        , out photoData[0]
                        , out var photoSize))
                    {
                        result.VisibleLightImage = photoData.Take(photoSize).ToArray();
                    }

                }
                else
                {
                    const int faceIndex = 50;
                    var tmpData = "";
                    var length = 0;
                    //get the face templates from the memory
                    if (_communicationOcx.GetUserFaceStr(DeviceInfo.DeviceNumber, userId.ToString(), faceIndex, ref tmpData,
                        ref length))
                    {
                        result.FaceDataList = new List<DtoUserFace>
                        {
                            new DtoUserFace
                            {
                                FaceIndex = faceIndex,
                                TemplateData = DeviceSharedHelperMethods.ConvertStringToArray(tmpData),
                                UserIdOnDevice = userId,
                                Length = length
                            }
                        };
                    }
                }
            }

            if (enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
            {
                _communicationOcx.ReadAllTemplate(DeviceInfo.DeviceNumber);
                result.FingerDataList = new List<DtoUserFinger>();

                int fingerIndex;
                for (fingerIndex = 0; fingerIndex < 10; fingerIndex++)
                {
                    if (!_communicationOcx.GetUserTmpExStr(DeviceInfo.DeviceNumber, enrollNumber, fingerIndex, out _, out var tempDataFinger, out _)) continue;
                    if (!tempDataFinger.IsNotNullOrEmpty()) continue;
                    result.FingerDataList.Add(new DtoUserFinger
                    {
                        UserIdOnDevice = enrollNumber.ToInt64(),
                        FingerIndex = fingerIndex,
                        TemplateData = DeviceSharedHelperMethods.ConvertStringToArray(tempDataFinger)
                    });
                }
            }
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetUserInfoByUserId result", new { DeviceInfo, Result = result });
            }
            return result;
        }

        public void SetUserInfoWithTemplate(DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandSetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand SetUserInfoWithTemplate", new { DeviceInfo, User = userInfo });
            }

            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var fingerErrorCode = 0;
            var faceErrorCode = 0;
            SetUserInfo(userInfo);
            if (DeviceInfo.HasFingerPrint && userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
            {
                foreach (var fingerData in userInfo.FingerDataList)
                {
                    var resultFinger = _communicationOcx.SetUserTmpExStr
                    (DeviceInfo.DeviceNumber
                        , userInfo.UserIdOnDevice.ToString()
                        , fingerData.FingerIndex
                        , 1
                        , DeviceSharedHelperMethods.ConvertArrayToString(fingerData.TemplateData));
                    if (!resultFinger)
                    {
                        _communicationOcx.GetLastError(ref fingerErrorCode);
                    }
                }
            }

            if (DeviceInfo.HasVisiblelight)
            {
                if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                {
                    var filePath = Path.Combine(ServiceConstants.DeviceVisibleLightImageFolder,
                        $"verify_biophoto_9_{userInfo.UserIdOnDevice}.jpg");
                    var filePathFull = Path.GetFullPath(filePath);
                    if (!Directory.Exists(ServiceConstants.DeviceVisibleLightImageFolder))
                    {
                        Directory.CreateDirectory(ServiceConstants.DeviceVisibleLightImageFolder);
                    }

                    File.WriteAllBytes(filePath, userInfo.VisibleLightImage);
                    bool resultFace = _communicationOcx.SendFile(DeviceInfo.DeviceNumber, filePathFull);
                    if (resultFace)
                    {
                        _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
                    }
                    else
                    {
                        _communicationOcx.GetLastError(ref faceErrorCode);
                    }
                }
            }
            else
            {
                if (DeviceInfo.HasFace && userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                {
                    foreach (var faceData in userInfo.FaceDataList)
                    {
                        bool resultFace = _communicationOcx.SetUserFaceStr
                        (DeviceInfo.DeviceNumber
                            , userInfo.UserIdOnDevice.ToString()
                            , faceData.FaceIndex
                            , DeviceSharedHelperMethods.ConvertArrayToString(faceData.TemplateData)
                            , faceData.Length);

                        if (!resultFace)
                        {
                            _communicationOcx.GetLastError(ref faceErrorCode);

                        }
                    }
                }
            }
            var fingerStatus = DeviceSharedHelperMethods.MapToOperationResult(fingerErrorCode, DeviceInfo);
            if (fingerStatus != OperationResultEnumeration.CommunicationStatusSuccessful)
            {
                throw new OperationCannotBeDoneException(fingerStatus);
            }
            var faceStatus = DeviceSharedHelperMethods.MapToOperationResult(faceErrorCode, DeviceInfo);

            if (faceStatus != OperationResultEnumeration.CommunicationStatusSuccessful)
            {
                throw new OperationCannotBeDoneException(faceStatus);
            }

        }

        public void SetUserInfo(DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandSetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand SetUserInfo", new { DeviceInfo, User = userInfo });
            }
            var startDate = userInfo.StartTime;
            var endDate = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime, ProducerEnumeration.Zk, SdkVersionEnumeration.SdkVersion1);
            var startDateString = startDate.ToString("yyyy-M-d HH:mm:ss");
            var endDateString = endDate.ToString("yyyy-M-d HH:mm:ss");


            var culture = CultureInfo.CurrentCulture.Name;
            var uiCulture = CultureInfo.CurrentUICulture.Name;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-IR");
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR");
            try
            {
                if (userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty())
                {
                    if (!_communicationOcx.SetStrCardNumber(userInfo.RfCardNumbers[0]))
                    {
                        var errorCode = 0;
                        _communicationOcx.GetLastError(ref errorCode);
                        throw new OperationCannotBeDoneException(
                            DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                    }
                }

                var devicePassword = userInfo.Password.ToNotNullString();
                var verificationStyle = (ZkVerificationStyleEnumeration)userInfo.VerificationStyle;
                if (!userInfo.IsEnable)
                {
                    devicePassword = ZkUtils.ZkForbiddenPassword;
                    verificationStyle = ZkVerificationStyleEnumeration.Pin;
                }

                var bytes = Encoding.Default.GetBytes(userInfo.UserName);
                var userNameUtf8 = Encoding.UTF8.GetString(bytes);
                if (!_communicationOcx.SSR_SetUserInfo(DeviceInfo.DeviceNumber,
                    userInfo.UserIdOnDevice.ToString()
                    , DeviceInfo.DeviceSettings?.ZkDeviceSettings != null
                      && DeviceInfo.DeviceSettings.ZkDeviceSettings.IsZkOldName ? userNameUtf8 : userInfo.UserName
                    , devicePassword
                    , userInfo.Privilege > 0 ? 3 : 0
                    , userInfo.IsEnable))
                {
                    var errorCode = 0;
                    _communicationOcx.GetLastError(ref errorCode);
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));

                }

                if (DeviceInfo.DeviceSettings?.ZkDeviceSettings != null && !DeviceInfo.DeviceSettings.ZkDeviceSettings.IsOldVersion)
                {
                    if (!_communicationOcx.SetUserValidDate(DeviceInfo.DeviceNumber,
                            userInfo.UserIdOnDevice.ToString()
                            , 1
                            , 0
                            , startDateString
                            , endDateString))
                    {
                        var errorCode = 0;
                        _communicationOcx.GetLastError(ref errorCode);
                        throw new OperationCannotBeDoneException(
                            DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                    }
                }


                byte reserved = 0;
                if (!_communicationOcx.SetUserInfoEx(DeviceInfo.DeviceNumber
                    , Convert.ToInt32(userInfo.UserIdOnDevice)
                    , (int)verificationStyle
                    , ref reserved))
                {
                    var errorCode = 0;
                    _communicationOcx.GetLastError(ref errorCode);
                    throw new OperationCannotBeDoneException(
                        DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                }

                SetUserPhoto(new DtoUserImage
                {
                    UserIdOnDevice = userInfo.UserIdOnDevice,
                    PhotoData = userInfo.HardwareProfileImage
                });
                _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);

            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(uiCulture);
            }


        }

        public void SetUserPhoto(DtoUserImage userPhoto)
        {
            if (userPhoto.PhotoData.IsCollectionNotNullOrEmpty() && DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.IsSendProfileImageActive)
            {
                var filePath = Path.Combine(ServiceConstants.DevicePersonalImageFolder, userPhoto.UserIdOnDevice + ".jpg");
                var filePathFull = Path.GetFullPath(filePath);
                if (!Directory.Exists(ServiceConstants.DevicePersonalImageFolder))
                {
                    Directory.CreateDirectory(ServiceConstants.DevicePersonalImageFolder);
                }

                File.WriteAllBytes(filePath, userPhoto.PhotoData);
                if (_communicationOcx.SendFile(DeviceInfo.DeviceNumber, filePathFull))
                {
                    _communicationOcx.RefreshData(DeviceInfo.DeviceNumber);
                }
                else
                {
                    var errorCode = 0;
                    _communicationOcx.GetLastError(ref errorCode);
                    throw new OperationCannotBeDoneException(
                        DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                }
            }

        }

        public List<DtoUserInfoDefinedOnDevice> GetAllUsers()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetAllUserId is calling", DeviceInfo);
            }

            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            _communicationOcx.ReadAllUserID(DeviceInfo.DeviceNumber);
            while (_communicationOcx.SSR_GetAllUserInfo(DeviceInfo.DeviceNumber, out var sdwEnrollNumber, out var name, out _, out var privilege, out _))
            {
                usersInfo.Add(new DtoUserInfoDefinedOnDevice
                {
                    UserIdOnDevice = sdwEnrollNumber.ToInt64(),
                    Name = name,
                    Privilege = privilege > 0 ? (int)ZkDevicePrivilegeEnumeration.SuperAdministrator : privilege,
                });
            }
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandGetUser))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetAllUserId result", new
                {
                    DeviceInfo,
                    UserIds = usersInfo
                });
            }
            return usersInfo;
        }

        public int GetUserCount()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetUserCount is calling", DeviceInfo);
            }
            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var userCount = 0;
            var errorCode = 0;
            if (_communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 2, ref userCount))
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
                {
                    LoggingSystem.LogInfo("ZK OnDemand GetUserCount result", new
                    {
                        DeviceInfo,
                        Count = userCount
                    });
                }
                return userCount;
            }
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public int GetFaceCount()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetFaceCount is calling", DeviceInfo);
            }
            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var faceCount = 0;
            var errorCode = 0;
            //21=  Number of faces
            //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            if (_communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 21, ref faceCount))
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
                {
                    LoggingSystem.LogInfo("ZK OnDemand GetFaceCount result", new
                    {
                        DeviceInfo,
                        Count = faceCount
                    });
                }
                return faceCount;
            }
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public int GetFingerCount()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandStatistics))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetFingerCount is calling", DeviceInfo);
            }
            if (_isDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var fingerCount = 0;
            var errorCode = 0;
            //3 =Number of fingerprint templates inthe device
            //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            if (_communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 3, ref fingerCount))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetFingerCount result", new
                {
                    DeviceInfo,
                    Count = fingerCount
                });
                return fingerCount;
            }
            _communicationOcx.GetLastError(ref errorCode);
            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
        }

        public DtoUserFinger ScanFinger(long userId, int fingerIndex)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandScan))
            {
                LoggingSystem.LogInfo("ZK OnDemand GetFingerCount is calling", new
                {
                    DeviceInfo,
                    UserId = userId,
                    FingerIndex = fingerIndex
                });
            }
            var errorCode = 0;
            const int flag = 1; // 0:Invalid; 1: Valid; 3: Threatened fingerprint template.
                                //if (fingerIndex <= 0)
                                //{
                                //	fingerIndex = 1;
                                //}
            _communicationOcx.CancelOperation();

            if (_communicationOcx.StartEnrollEx(userId.ToString(), fingerIndex, flag))
            {
                _communicationOcx.StartIdentify();//After enrolling templates,you should let the device into the 1:N verification condition
                return null;
                // برای دستگاه های زد-کا امکان دریافت بلافاصله تمپلیت ها نیست
                //if (_czkemClass.GetUserTmpExStr(DeviceInfo.DeviceNumber, userId.ToString(), fingerIndex, out _,
                //	out var tempDataFinger, out _))
                //{
                //	return new DtoUserFinger
                //	{
                //		UserIdOnDevice = userId,
                //		FingerIndex = fingerIndex,
                //		TemplateData = DeviceHelperMethods.ConvertStringToArray(tempDataFinger)
                //	};
                //}
                //_czkemClass.GetLastError(ref errorCode);
                //throw new OperationCannotBeDoneException(DeviceHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
            else
            {
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }

        }

        public DtoUserFace ScanFace(long userId)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandScan))
            {
                LoggingSystem.LogInfo("ZK OnDemand ScanFace is calling", new
                {
                    DeviceInfo,
                    UserId = userId,
                });
            }
            var errorCode = 0;
            const int flag = 1; // 0:Invalid; 1: Valid; 3: Threatened fingerprint template.
            const int index = 111; // face
            _communicationOcx.CancelOperation();

            if (_communicationOcx.StartEnrollEx(userId.ToString(), index, flag))
            {
                _communicationOcx.StartIdentify();//After enrolling templates,you should let the device into the 1:N verification condition
                return null;
                // برای دستگاه های زد-کا امکان دریافت بلافاصله تمپلیت ها نیست

                //const int faceIndex = 50;
                //var tmpData = "";
                //var length = 0;
                ////get the face templates from the memory
                //if (_czkemClass.GetUserFaceStr(DeviceInfo.DeviceNumber, userId.ToString(), faceIndex, ref tmpData, ref length))
                //{
                //	return new DtoUserFace
                //	{
                //		FaceIndex = faceIndex,
                //		TemplateData = DeviceHelperMethods.ConvertStringToArray(tmpData),
                //		UserIdOnDevice = userId,
                //		Length = length
                //	};
                //}
                //_czkemClass.GetLastError(ref errorCode);
                //throw new OperationCannotBeDoneException(DeviceHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
            else
            {
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));

            }

        }

        #endregion

        #region AccessControl

        public void OpenDoor(int timeoutInSecond)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.OnDemandDoorControl))
            {
                LoggingSystem.LogInfo("ZK OnDemand OpenDoor is calling", new
                {
                    DeviceInfo,
                    TimeoutInSecond = timeoutInSecond,
                });
            }

            var errorCode = 0;
            if (!_communicationOcx.ACUnlock(DeviceInfo.DeviceNumber, timeoutInSecond))
            {
                _communicationOcx.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }

        }

        #endregion

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
                // COM teardown only on explicit Dispose — never on the finalizer thread
                // (EnableDevice/Disconnect can throw, which would terminate the process).
                if (_isDeviceConnected && _isDeviceEnable == false)
                {
                    EnableDevice();
                }
                if (_isDeviceConnected)
                {
                    Disconnect();
                }
            }
            _disposed = true;
        }

        #endregion

    }

}
