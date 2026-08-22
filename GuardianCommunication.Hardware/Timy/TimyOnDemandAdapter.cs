using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FP_CLOCKLib;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Timy.TimyConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.Timy
{
    //link_enc_disable != 96 Authentication Enable else Disable
    public class TimyOnDemandAdapter : IDisposable
    {
        private const int EMachineNumber = 0;
        private const int VisibleLightImageLength = 400800;

        public DtoDevice DeviceInfo { get; set; }


        #region Private Fields


        private readonly FP_CLOCKClass _communicationOcx = new FP_CLOCKClass();
        public bool IsDeviceConnected { get; set; }

        #endregion


        public TimyOnDemandAdapter(DtoDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }


        #region Private Methods

        public bool EnableDevice(bool throwException = false)
        {
            var result = _communicationOcx.EnableDevice(DeviceInfo.DeviceNumber, 1);
            if (!result && throwException)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusTimyCannotDisableDevice);
            }
            return result;
        }

        public bool DisableDevice(bool throwException = false)
        {
            var result = _communicationOcx.EnableDevice(DeviceInfo.DeviceNumber, 0);
            if (!result && throwException)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusTimyCannotDisableDevice);
            }

            return result;
        }

        #endregion


        #region public Methods

        #region Other

        public bool SetDateTime()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("Timy OnDemand SetDateTime", new { DeviceInfo });
            }
            var result = _communicationOcx.SetDeviceTime(DeviceInfo.DeviceNumber);
            if (!result)
            {
                ThrowLastError();
            }
            return true;
        }

        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDateTime is calling", new { DeviceInfo });
            }
            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var dayOfWeek = 0;
            if (!_communicationOcx.GetDeviceTime
                    (DeviceInfo.DeviceNumber, ref year, ref month, ref day, ref hour, ref minute, ref dayOfWeek))
            {

                ThrowLastError();
            }

            var result = new DateTime(year, month, day, hour, minute, 0);
            var timeService = new DeviceTimeService();
            var processedResult = timeService.DeviceTimeToUtc(result, DeviceInfo.IanaTimeZoneId);
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDateTime", new { DeviceInfo, Time = result, UtcTime = processedResult });
            }
            return processedResult;
        }

        public string GetSerialNumber()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetSerialNumber))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetSerialNumber is calling", new { DeviceInfo });
            }
            var serialNumber = string.Empty;
            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                result = _communicationOcx.GetSerialNumber(DeviceInfo.DeviceNumber, ref serialNumber);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetSerialNumber))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetSerialNumber result", new { DeviceInfo, SerialNumber = serialNumber });
            }
            return serialNumber;
        }

        public void RebootDevice()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.RebootDevice))
            {
                LoggingSystem.LogInfo("Timy OnDemand RebootDevice", new { DeviceInfo });
            }
            if (!_communicationOcx.SetDoorStatus(DeviceInfo.DeviceNumber, (int)TimyDoorStatus.REBOOT_FPA_MACHINE))
            {
                ThrowLastError();
            }
        }

        #endregion

        #region Commiunication

        public bool Connect()
        {
            // Bounded, cross-platform connect (replaces Delegate.BeginInvoke): run the blocking
            // SDK connect on a task that RETURNS the result. The result is committed to
            // IsDeviceConnected only if the task finished in time, so a late-completing connect
            // can never flip the property after we've already reported failure.
            var connectTask = Task.Run(() =>
            {
                var ipAddress = DeviceInfo.DeviceIp;
                // ReSharper disable PossibleInvalidOperationException
                var resultConnectionTry = _communicationOcx.SetIPAddress(ref ipAddress,
                    DeviceInfo.TcpPort.Value, DeviceInfo.DevicePassword.ToInt32());
                // ReSharper restore PossibleInvalidOperationException
                if (!resultConnectionTry)
                {
                    return false;
                }
                return _communicationOcx.OpenCommPort(DeviceInfo.DeviceNumber);
            });

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
                connected = false;
            }

            IsDeviceConnected = connected;
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }
            return IsDeviceConnected;
        }

        public void Disconnect()
        {
            _communicationOcx.CloseCommPort();
            IsDeviceConnected = false;

        }

        public bool TestConnection()
        {
            Connect();
            return IsDeviceConnected;
        }

        #endregion

        #region Attendance

        public List<DtoAttendance> GetData()
        {
            return DeviceInfo.DeviceSettings?.TimyDeviceSettings != null 
                   && DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit
                ? GetDataOldVersion()
                : GetDataLongVersion();
        }

        private List<DtoAttendance> GetDataLongVersion()
        {
            var attRecords = new List<DtoAttendance>();
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetData))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDataLongVersion is calling", new { DeviceInfo });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(
                    OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {

                //_communicationOcx.ReadMark = true;
                DisableDevice();

                var count = 0;
                var result = _communicationOcx.GetDeviceStatus(
                    DeviceInfo.DeviceNumber, 6, ref count);
                if (!result)
                {
                    ThrowLastError();
                }
                if (count <= 0)
                {
                    return attRecords;
                }

                result = _communicationOcx.ReadGLogDataLongID(DeviceInfo.DeviceNumber, 1);
                if (!result)
                {
                    ThrowLastError();
                }

                var gLogInfo = new GeneralLogInfo();

                var iGlCount = 0;
                do
                {
                    var userIdString = string.Empty;
                    object userIdObject = new VariantWrapper(userIdString);
                    result = _communicationOcx.GetGLogDataLongID(DeviceInfo.DeviceNumber,
                        ref userIdObject,
                        ref gLogInfo.dwVerifyMode,
                        ref gLogInfo.dwInout,
                        ref gLogInfo.dwEvent,
                        ref gLogInfo.dwYear,
                        ref gLogInfo.dwMonth,
                        ref gLogInfo.dwDay,
                        ref gLogInfo.dwHour,
                        ref gLogInfo.dwMinute,
                        ref gLogInfo.dwSecond
                    );
                    if (result)
                    {
                        var timeService = new DeviceTimeService();

                        var returnedUserIdString = userIdObject.ToString();
                        if (returnedUserIdString.IsNotNullOrEmpty() && returnedUserIdString.CanConvertToInt64())
                        {
                            var attendanceDateTime = new DateTime(gLogInfo.dwYear, gLogInfo.dwMonth, gLogInfo.dwDay,
                                gLogInfo.dwHour, gLogInfo.dwMinute, gLogInfo.dwSecond);
                            var processedAttendanceDateTime = timeService.DeviceTimeToUtc(attendanceDateTime, DeviceInfo.IanaTimeZoneId);
                            iGlCount++;
                            var att = new DtoAttendance
                            {
                                LogIdOnDevice = iGlCount,
                                UserIdOnDevice = returnedUserIdString.ToInt64(),
                                VerificationStyle = (int)TimyUtils.GetVerificationStyle(gLogInfo.dwVerifyMode),
                                StatusCode = gLogInfo.dwInout,
                                DeviceId = DeviceInfo.Id,
                                LocationId = DeviceInfo.LocationId,
                                CameraId = null,
                                AttendanceDateTime = processedAttendanceDateTime,
                                AttendanceSource = AttendanceSourceEnumeration.Device,
                                DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                                RfCardNumber = null,
                            };
                            attRecords.Add(att);
                        }
                    }

                } while (result);
            }
            finally
            {
                //_communicationOcx.ReadMark = false;
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetData))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDataLongVersion result", new { DeviceInfo, Attendances = attRecords });
            }

            return attRecords;
        }

        private List<DtoAttendance> GetDataOldVersion()
        {
            var attRecords = new List<DtoAttendance>();
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetData))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDataOldVersion is calling", new { DeviceInfo });
            }
            if (DeviceInfo.DeviceSettings != null && DeviceInfo.DeviceSettings.DontSaveAttendance)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            try
            {

                _communicationOcx.ReadMark = true;

                DisableDevice();


                var count = 0;
                var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 6, ref count);

                if (!result)
                {
                    ThrowLastError();
                }
                if (count <= 0)
                {
                    return attRecords;
                }

                result = _communicationOcx.ReadGeneralLogData(DeviceInfo.DeviceNumber);
                if (!result)
                {
                    ThrowLastError();
                }

                var gLogInfo = new GeneralLogInfo();

                var iGlCount = 0;
                do
                {
                    result = _communicationOcx.GetGeneralLogDataWithSecond(DeviceInfo.DeviceNumber,
                        ref gLogInfo.dwTMachineNumber,
                        ref gLogInfo.dwEnrollNumber,
                        ref gLogInfo.dwEMachineNumber,
                        ref gLogInfo.dwVerifyMode,
                        ref gLogInfo.dwInout,
                        ref gLogInfo.dwEvent,
                        ref gLogInfo.dwYear,
                        ref gLogInfo.dwMonth,
                        ref gLogInfo.dwDay,
                        ref gLogInfo.dwHour,
                        ref gLogInfo.dwMinute,
                        ref gLogInfo.dwSecond
                    );
                    var timeService = new DeviceTimeService();

                    if (result)
                    {
                        if (gLogInfo.dwEnrollNumber > 0)
                        {
                            var attendanceDateTime = new DateTime(gLogInfo.dwYear, gLogInfo.dwMonth, gLogInfo.dwDay,
                                gLogInfo.dwHour, gLogInfo.dwMinute, gLogInfo.dwSecond);
                            var processedAttendanceDateTime = timeService.DeviceTimeToUtc(attendanceDateTime, DeviceInfo.IanaTimeZoneId);

                            iGlCount++;
                            var att = new DtoAttendance
                            {
                                LogIdOnDevice = iGlCount,
                                UserIdOnDevice = gLogInfo.dwEnrollNumber,
                                VerificationStyle = (int)TimyUtils.GetVerificationStyle(gLogInfo.dwVerifyMode),
                                StatusCode = gLogInfo.dwInout,
                                DeviceId = DeviceInfo.Id,
                                LocationId = DeviceInfo.LocationId,
                                CameraId = null,
                                AttendanceDateTime = processedAttendanceDateTime,
                                AttendanceSource = AttendanceSourceEnumeration.Device,
                                DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                                RfCardNumber = null,
                            };
                            attRecords.Add(att);
                        }
                    }

                } while (result);
            }
            finally
            {
                //_communicationOcx.ReadMark = false;
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetData))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetDataOldVersion result", new { DeviceInfo, Attendances = attRecords });
            }

            return attRecords;
        }

        public void ClearData()
        {
            // به دلیل اینکه فعلا نباید داده ها را حذف کنیم 
            // این متد را return کرده ایم
            // چون اگر داده ها را پاک کنیم دیگر با هیچ متدی قابل بازیابی نیستند
            //try
            //{

            //    DisableDevice();
            //    var result = _communicationOcx.EmptyGeneralLogData(DeviceInfo.DeviceNumber);
            //    if (!result)
            //    {
            //        ThrowLastError();
            //    }
            //}
            //finally
            //{
            //    EnableDevice();
            //}
        }

        #endregion

        #region Usering And Finger


        public void DeleteUserById(long userIdOnDevice)
        {
            if (DeviceInfo.DeviceSettings?.TimyDeviceSettings != null && DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit)
            {
                DeleteUserByIdOldVersion(userIdOnDevice);
            }
            else
            {
                DeleteUserByIdLongVersion(userIdOnDevice);
            }
        }

        private void DeleteUserByIdLongVersion(long userIdOnDevice)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand DeleteUserByIdLongVersion", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {

                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                object userIdOnDeviceObj = new VariantWrapper(userIdOnDevice.ToString());
                result = _communicationOcx.DeleteUserInfoLongID(DeviceInfo.DeviceNumber, ref userIdOnDeviceObj);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }

        }

        private void DeleteUserByIdOldVersion(long userIdOnDevice)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand DeleteUserByIdOldVersion", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {

                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }
                result = _communicationOcx.DeleteEnrollData
                    (DeviceInfo.DeviceNumber, (int)userIdOnDevice, EMachineNumber, 12);
                if (!result)
                {
                    ThrowLastError();
                }
                result = _communicationOcx.DeleteEnrollData(DeviceInfo.DeviceNumber, (int)userIdOnDevice, EMachineNumber, 50);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }

        }


        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand DeleteAllUsers", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }
                result = _communicationOcx.EmptyEnrollData(DeviceInfo.DeviceNumber);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }

        }


        public DtoUserDeviceRelatedData GetUserInfoByUserId(long userIdOnDevice, TemplateTypeEnumeration enrollType)
        {
            return DeviceInfo.DeviceSettings?.TimyDeviceSettings != null 
                   && DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit
            
                ? GetUserInfoByUserIdOldVersion(userIdOnDevice, enrollType)
                : GetUserInfoByUserIdLongVersion(userIdOnDevice, enrollType);
        }

        private DtoUserDeviceRelatedData GetUserInfoByUserIdLongVersion(long userIdOnDevice, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserInfoByUserIdLongVersion is calling", new { DeviceInfo, UserId = userIdOnDevice, EnrollType = enrollType });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var finalResult = new DtoUserDeviceRelatedData()
            {
                UserIdOnDevice = userIdOnDevice,
            };

            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }


                object userIdOnDeviceObject = new VariantWrapper(userIdOnDevice.ToString());
                var username = string.Empty;
                object usernameObject = new VariantWrapper(username);
                var card = "";
                object cardObject = new VariantWrapper(card);

                var password = 0;
                var faceFlag = 0;
                var palmFlag = 0;
                var fingerPrintFlag = 0;
                var postId = 0;
                var privilege = 0;
                var enabled = 0;
                var shiftId = 0;
                var zoneId = 0;
                var groupId = 0;
                var userControl = 0;
                var startTime = 0;
                var endTime = 0;
                var birthDay = 0;
                result = _communicationOcx.GetUserInfoLongID(DeviceInfo.DeviceNumber,
                    ref userIdOnDeviceObject,
                    ref usernameObject,
                    ref password,
                    ref cardObject,
                    ref faceFlag,
                    ref fingerPrintFlag,
                    ref palmFlag,
                    ref postId,
                    ref privilege,
                    ref enabled,
                    ref shiftId,
                    ref zoneId,
                    ref groupId,
                    ref userControl,
                    ref startTime,
                    ref endTime,
                    ref birthDay
                );


                if (!result)
                {
                    ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                }
                finalResult.UserName = username;
                finalResult.Password = password.ToString();
                if (cardObject.ToString().IsCollectionNotNullOrEmpty() && cardObject.ToString() != "0")
                {
                    finalResult.RfCardNumbers = new List<string> { cardObject.ToString() };
                }

                finalResult.Privilege = privilege;
                if (enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
                {
                    finalResult.FingerDataList = new List<DtoUserFinger>();
                    for (var index = 0; index < 10; index++)
                    {
                        object userIdObjectFingerPrint = new VariantWrapper(userIdOnDevice.ToString());
                        var fingerArray = new int[1888 / 4];
                        object fingerObj = new VariantWrapper(fingerArray);
                        result = _communicationOcx.GetFPDataLongID(
                            DeviceInfo.DeviceNumber,
                            ref userIdObjectFingerPrint,
                            index,
                            ref fingerObj
                        );
                        if (!result)
                        {
                            ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                            continue;
                        }

                        var fingerData = (int[])fingerObj;
                        var indexData = new byte[1420];
                        var ptrIndex = IntPtr.Zero;
                        try
                        {
                            ptrIndex = Marshal.AllocHGlobal(indexData.Length);
                            Marshal.Copy(fingerData, 0, ptrIndex, 1420 / 4);
                            Marshal.Copy(ptrIndex, indexData, 0, 1420);
                            finalResult.FingerDataList.Add(new DtoUserFinger
                            {
                                FingerIndex = index,
                                UserIdOnDevice = userIdOnDevice,
                                TemplateData = indexData
                            });

                        }
                        finally
                        {
                            if (ptrIndex != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(ptrIndex);
                            }
                        }
                    }
                }

                if (enrollType.HasFlag(TemplateTypeEnumeration.Face))
                {
                    if (DeviceInfo.HasVisiblelight)
                    {

                        // ReSharper disable CollectionNeverQueried.Local
                        var indexDataFacePhoto = new int[VisibleLightImageLength];
                        // ReSharper restore CollectionNeverQueried.Local
                        var ptrIndexFacePhoto = IntPtr.Zero;
                        try
                        {
                            ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                            var vPhotoSize = 0;
                            object userIdFacePhoto = new VariantWrapper(userIdOnDevice.ToString());
                            result = _communicationOcx.GetEnrollPhotoCSLongID
                                (DeviceInfo.DeviceNumber, userIdFacePhoto, ref vPhotoSize, ptrIndexFacePhoto);
                            if (!result)
                            {
                                ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                            }
                            var visibleLightImage = new byte[vPhotoSize];
                            Marshal.Copy(ptrIndexFacePhoto, visibleLightImage, 0, vPhotoSize);
                            finalResult.VisibleLightImage = visibleLightImage;
                        }
                        finally
                        {
                            if (ptrIndexFacePhoto != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(ptrIndexFacePhoto);
                            }
                        }
                    }
                    else
                    {
                        for (var index = 20; index < 28; index++)
                        {
                            object userIdObjectFingerPrint = new VariantWrapper(userIdOnDevice.ToString());
                            var faceArray = new int[1888 / 4];
                            object faceObj = new VariantWrapper(faceArray);
                            result = _communicationOcx.GetFPDataLongID(
                                DeviceInfo.DeviceNumber,
                                ref userIdObjectFingerPrint,
                                index,
                                ref faceObj
                            );
                            if (!result)
                            {
                                ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                                continue;
                            }

                            var faceDataInt = (int[])faceObj;
                            var indexDataFace = new byte[1888];
                            var ptrIndexFace = IntPtr.Zero;
                            try
                            {
                                ptrIndexFace = Marshal.AllocHGlobal(indexDataFace.Length);
                                Marshal.Copy(faceDataInt, 0, ptrIndexFace, 1888 / 4); //be careful
                                Marshal.Copy(ptrIndexFace, indexDataFace, 0, 1888);

                                finalResult.FaceDataList.Add(new DtoUserFace()
                                {
                                    FaceIndex = index - 20,
                                    UserIdOnDevice = userIdOnDevice,
                                    TemplateData = indexDataFace,
                                    Length = indexDataFace.Length,
                                });
                            }
                            finally
                            {
                                if (ptrIndexFace != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ptrIndexFace);
                                }
                            }
                        }
                    }
                }

            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserInfoByUserIdLongVersion Result", new { DeviceInfo, User = finalResult });
            }
            return finalResult;
        }

        private DtoUserDeviceRelatedData GetUserInfoByUserIdOldVersion(long userId, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserInfoByUserIdOldVersion is calling", new { DeviceInfo, UserId = userId, EnrollType = enrollType });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var finalResult = new DtoUserDeviceRelatedData()
            {
                UserIdOnDevice = userId,
            };

            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                // User name
                var userName = string.Empty;
                object objectUsername = new VariantWrapper(userName);
                result = _communicationOcx.GetUserNameUTF8(0,
                    DeviceInfo.DeviceNumber,
                    (int)userId,
                    EMachineNumber,
                    ref objectUsername
                );
                if (!result)
                {
                    ThrowLastError();
                }
                finalResult.UserName = userName;

                // Password and privilege
                var privilege = 0;
                var password = 0;
                var dataPassword = new int[1888 / 4];
                object objectPassword = new VariantWrapper(dataPassword);
                result = _communicationOcx.GetEnrollData(
                    DeviceInfo.DeviceNumber,
                    (int)userId,
                    EMachineNumber,
                    10,
                    ref privilege,
                    ref objectPassword,
                    ref password
                );
                if (!result)
                {
                    ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                }
                finalResult.Password = password.ToString();
                finalResult.Privilege = privilege;


                // Card number
                var cardNumber = 0;
                var dataCardNumber = new int[1888 / 4];
                object objectCardNumber = new VariantWrapper(dataCardNumber);

                result = _communicationOcx.GetEnrollData(
                    DeviceInfo.DeviceNumber,
                    (int)userId,
                    EMachineNumber,
                    11,
                    ref privilege,
                    ref objectCardNumber,
                    ref cardNumber
                );
                if (!result)
                {
                    ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                }
                finalResult.RfCardNumbers = new List<string> { cardNumber.ToString() };



                if (enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
                {
                    finalResult.FingerDataList = new List<DtoUserFinger>();
                    for (var index = 0; index < 10; index++)
                    {
                        var fingerArray = new int[1888 / 4];
                        object fingerObj = new VariantWrapper(fingerArray);
                        result = _communicationOcx.GetEnrollData(
                            DeviceInfo.DeviceNumber,
                            (int)userId,
                            EMachineNumber,
                            index,
                            ref privilege,
                            ref fingerObj,
                            ref password
                        );
                        if (!result)
                        {
                            ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                            continue;
                        }

                        var fingerData = (int[])fingerObj;
                        var indexData = new byte[1420];
                        var ptrIndex = IntPtr.Zero;
                        try
                        {
                            ptrIndex = Marshal.AllocHGlobal(indexData.Length);
                            Marshal.Copy(fingerData, 0, ptrIndex, 1420 / 4);
                            Marshal.Copy(ptrIndex, indexData, 0, 1420);
                            finalResult.FingerDataList.Add(new DtoUserFinger
                            {
                                FingerIndex = index,
                                UserIdOnDevice = userId,
                                TemplateData = indexData
                            });
                        }
                        finally
                        {
                            if (ptrIndex != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(ptrIndex);
                            }
                        }

                    }
                }

                if (enrollType.HasFlag(TemplateTypeEnumeration.Face))
                {
                    if (DeviceInfo.HasVisiblelight)
                    {
                        // ReSharper disable CollectionNeverQueried.Local
                        var indexDataFacePhoto = new int[VisibleLightImageLength];
                        // ReSharper restore CollectionNeverQueried.Local
                        var ptrIndexFacePhoto = IntPtr.Zero;
                        try
                        {
                            ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                            var vPhotoSize = 0;
                            result = _communicationOcx.GetEnrollPhotoCS
                                (DeviceInfo.DeviceNumber, (int)userId, ref vPhotoSize, ptrIndexFacePhoto);
                            if (!result)
                            {
                                ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                            }

                            var visibleLightImage = new byte[vPhotoSize];
                            Marshal.Copy(ptrIndexFacePhoto, visibleLightImage, 0, vPhotoSize);
                            finalResult.VisibleLightImage = visibleLightImage;
                        }
                        finally
                        {
                            if (ptrIndexFacePhoto != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(ptrIndexFacePhoto);
                            }
                        }
                    }
                    else
                    {
                        for (var index = 20; index < 28; index++)
                        {
                            var faceArray = new int[1888 / 4];
                            object faceObj = new VariantWrapper(faceArray);
                            result = _communicationOcx.GetEnrollData(
                                DeviceInfo.DeviceNumber,
                                (int)userId,
                                EMachineNumber,
                                index,
                                ref privilege,
                                ref faceObj,
                                ref password
                            );
                            if (!result)
                            {
                                ThrowLastError(new List<TimyErrorEnum> { TimyErrorEnum.ErrorCarryOut });
                                continue;
                            }

                            var faceDataInt = (int[])faceObj;
                            var indexDataFace = new byte[1888];
                            var ptrIndexFace = IntPtr.Zero;
                            try
                            {
                                ptrIndexFace = Marshal.AllocHGlobal(indexDataFace.Length);
                                Marshal.Copy(faceDataInt, 0, ptrIndexFace, 1888 / 4); //be careful
                                Marshal.Copy(ptrIndexFace, indexDataFace, 0, 1888);
                                finalResult.FaceDataList.Add(new DtoUserFace()
                                {
                                    FaceIndex = index - 20,
                                    UserIdOnDevice = userId,
                                    TemplateData = indexDataFace,
                                    Length = indexDataFace.Length,
                                });
                            }
                            finally
                            {
                                if (ptrIndexFace != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ptrIndexFace);
                                }
                            }
                        }
                    }
                }

            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserInfoByUserIdOldVersion Result", new { DeviceInfo, User = finalResult });
            }
            return finalResult;
        }




        public void SetUserInfoWithTemplate(DtoUserDeviceRelatedData userInfo)
        {
            if (userInfo.IsEnable)
            {
                if (DeviceInfo.DeviceSettings?.TimyDeviceSettings != null && DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit)
                {
                    SetUserInfoWithTemplateOldVersion(userInfo);
                }
                else
                {
                    SetUserInfoWithTemplateLongVersion(userInfo);
                }
            }
            else
            {
                DeleteUserById(userInfo.UserIdOnDevice);
            }
        }

        private void SetUserInfoWithTemplateLongVersion(DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand SetUserInfoWithTemplateLongVersion is calling", new { DeviceInfo, User = userInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var fingerErrorCode = 0;
            var faceErrorCode = 0;

            try
            {

                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                SetUserInfo(userInfo, false);
                if (DeviceInfo.HasFingerPrint && userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
                {
                    foreach (var fingerData in userInfo.FingerDataList)
                    {
                        object userIdObj = new VariantWrapper(userInfo.UserIdOnDevice.ToString());
                        object fingerObject = new VariantWrapper(fingerData.TemplateData);
                        result = _communicationOcx.SetFPDataLongID(DeviceInfo.DeviceNumber,
                            ref userIdObj,
                            fingerData.FingerIndex,
                            ref fingerObject);
                        if (!result)
                        {
                            _communicationOcx.GetLastError(ref fingerErrorCode);
                        }
                    }
                }

                if (DeviceInfo.HasVisiblelight)
                {
                    if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                    {
                        if (userInfo.VisibleLightImage.Length <= VisibleLightImageLength)
                        {

                            object userIdObj = new VariantWrapper(userInfo.UserIdOnDevice.ToString());
                            // ReSharper disable CollectionNeverQueried.Local
                            var indexDataFacePhoto = new int[VisibleLightImageLength];
                            // ReSharper restore CollectionNeverQueried.Local
                            var ptrIndexFacePhoto = IntPtr.Zero;
                            try
                            {
                                ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                                Marshal.Copy(userInfo.VisibleLightImage, 0, ptrIndexFacePhoto, userInfo.VisibleLightImage.Length);
                                result = _communicationOcx.SetEnrollPhotoCSLongID
                                    (DeviceInfo.DeviceNumber, userIdObj, userInfo.VisibleLightImage.Length, ptrIndexFacePhoto);
                                if (!result)
                                {
                                    _communicationOcx.GetLastError(ref faceErrorCode);
                                }
                            }
                            finally
                            {
                                if (ptrIndexFacePhoto != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                                }
                            }
                        }
                        else
                        {
                            faceErrorCode = (int)TimyErrorEnum.ErrorCustomVisibleLightImageLengthIsTooLong;
                        }

                    }
                }
                else
                {
                    if (DeviceInfo.HasFace && userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var faceData in userInfo.FaceDataList)
                        {
                            object userIdObj = new VariantWrapper(userInfo.UserIdOnDevice.ToString());
                            object faceObject = new VariantWrapper(faceData.TemplateData);
                            result = _communicationOcx.SetFPDataLongID(DeviceInfo.DeviceNumber,
                                ref userIdObj,
                                20 + faceData.FaceIndex,
                                ref faceObject);
                            if (!result)
                            {
                                _communicationOcx.GetLastError(ref fingerErrorCode);
                            }
                        }
                    }
                }

                if (fingerErrorCode != (int)TimyErrorEnum.Successful)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(fingerErrorCode, DeviceInfo));
                }
                if (faceErrorCode != (int)TimyErrorEnum.Successful)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(faceErrorCode, DeviceInfo));
                }

            }
            finally
            {
                EnableDevice();
            }

        }

        private void SetUserInfoWithTemplateOldVersion(DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand SetUserInfoWithTemplateOldVersion is calling", new { DeviceInfo, User = userInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            var fingerErrorCode = 0;
            var faceErrorCode = 0;

            try
            {

                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                SetUserInfo(userInfo, false);
                if (DeviceInfo.HasFingerPrint && userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
                {
                    foreach (var fingerData in userInfo.FingerDataList)
                    {
                        result = _communicationOcx.SetEnrollData(DeviceInfo.DeviceNumber,
                            (int)userInfo.UserIdOnDevice,
                            EMachineNumber,
                            fingerData.FingerIndex,
                            userInfo.Privilege,
                             new VariantWrapper(fingerData.TemplateData),
                            userInfo.RfCardNumbers.First().ToInt32());
                        if (!result)
                        {
                            _communicationOcx.GetLastError(ref fingerErrorCode);
                        }
                    }
                }

                if (DeviceInfo.HasVisiblelight)
                {
                    if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                    {
                        if (userInfo.VisibleLightImage.Length <= VisibleLightImageLength)
                        {
                            // ReSharper disable CollectionNeverQueried.Local
                            var indexDataFacePhoto = new int[VisibleLightImageLength];
                            // ReSharper restore CollectionNeverQueried.Local
                            var ptrIndexFacePhoto = IntPtr.Zero;
                            try
                            {
                                ptrIndexFacePhoto = Marshal.AllocHGlobal(indexDataFacePhoto.Length);
                                Marshal.Copy(userInfo.VisibleLightImage, 0, ptrIndexFacePhoto,
                                    userInfo.VisibleLightImage.Length);
                                result = _communicationOcx.SetEnrollPhotoCS
                                (DeviceInfo.DeviceNumber, (int)userInfo.UserIdOnDevice,
                                    userInfo.VisibleLightImage.Length, ptrIndexFacePhoto);
                                if (!result)
                                {
                                    _communicationOcx.GetLastError(ref faceErrorCode);
                                }
                            }
                            finally
                            {
                                if (ptrIndexFacePhoto != IntPtr.Zero)
                                {
                                    Marshal.FreeHGlobal(ptrIndexFacePhoto);
                                }
                            }
                        }
                        else
                        {
                            faceErrorCode = (int)TimyErrorEnum.ErrorCustomVisibleLightImageLengthIsTooLong;
                        }
                    }
                }
                else
                {
                    if (DeviceInfo.HasFace && userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                    {
                        foreach (var faceData in userInfo.FaceDataList)
                        {
                            result = _communicationOcx.SetEnrollData(DeviceInfo.DeviceNumber,
                                (int)userInfo.UserIdOnDevice,
                                EMachineNumber,
                                20 + faceData.FaceIndex,
                                userInfo.Privilege,
                                new VariantWrapper(faceData.TemplateData),
                                userInfo.RfCardNumbers.First().ToInt32());
                            if (!result)
                            {
                                _communicationOcx.GetLastError(ref faceErrorCode);
                            }
                        }
                    }
                }
                if (fingerErrorCode != (int)TimyErrorEnum.Successful)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(fingerErrorCode, DeviceInfo));
                }
                if (faceErrorCode != (int)TimyErrorEnum.Successful)
                {
                    throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(faceErrorCode, DeviceInfo));
                }

            }
            finally
            {
                EnableDevice();
            }

        }



        public void SetUserInfo(DtoUserDeviceRelatedData userInfo, bool disableDevice)
        {
            if (userInfo.IsEnable)
            {
                if (DeviceInfo.DeviceSettings?.TimyDeviceSettings != null && DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit)
                {
                    SetUserInfoOldVersion(userInfo, disableDevice);
                }
                else
                {
                    SetUserInfoLongVersion(userInfo, disableDevice);
                }
            }
            else
            {
                DeleteUserById(userInfo.UserIdOnDevice);
            }
        }

        private void SetUserInfoLongVersion(DtoUserDeviceRelatedData userInfo, bool disableDevice)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand SetUserInfoLongVersion is calling", new { DeviceInfo, User = userInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {
                bool result;
                if (disableDevice)
                {
                    result = DisableDevice();
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }
                var userInfoForDevice = userInfo.WithDeviceLocalDates(DeviceInfo);

                object userIdObj = new VariantWrapper(userInfoForDevice.UserIdOnDevice.ToString());
                object userNameObject = new VariantWrapper(userInfoForDevice.UserName.ToNotNullString());
                var cardString = userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty() ? userInfoForDevice.RfCardNumbers.First() : string.Empty;
                object cardObj = new VariantWrapper(cardString);
                var password = userInfoForDevice.Password.IsNotNullOrEmpty() ? userInfoForDevice.Password.ToInt32() : 0;
                const int postId = 0;
                var privilege = userInfoForDevice.Privilege;
                var enabled = userInfoForDevice.IsEnable ? 1 : 0;
                const int shiftId = 0;
                const int zoneId = 0;
                const int groupId = 0;
                const int userCtrl = 0;


                // ReSharper disable PossibleInvalidOperationException
                var startTime = TimyHelpers.GetTimeStamp(userInfoForDevice.StartDateTime);
                var endTime = TimyHelpers.GetTimeStamp(userInfoForDevice.EndDateTime.Value);
                // ReSharper restore PossibleInvalidOperationException
                const int birthDay = 0;


                result = _communicationOcx.SetUserInfoLongID(DeviceInfo.DeviceNumber,
                    ref userIdObj,
                    ref userNameObject,
                    password,
                    cardObj,
                    postId,
                    privilege,
                    enabled,
                    shiftId,
                    zoneId,
                    groupId,
                    userCtrl,
                    startTime,
                    endTime,
                    birthDay
                );

                if (!result)
                {
                    ThrowLastError();
                }

                if (DeviceInfo.ModuleId.HasFlag(ModuleEnumeration.Elevator)
                    && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Elevator)
                    && userInfoForDevice.ElevatorInfoInJsonFormat.IsNotNullOrEmpty())
                {
                    var elevatorFloorNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoForDevice.ElevatorInfoInJsonFormat);
                    if (elevatorFloorNumbers.IsCollectionNotNullOrEmpty())
                    {
                        var elevatorFloorObject = new VariantWrapper(elevatorFloorNumbers.JoinWithComma());
                        result = _communicationOcx.SetUserProfileLongID(DeviceInfo.DeviceNumber, userIdObj, elevatorFloorObject);
                        if (!result)
                        {
                            ThrowLastError();
                        }
                    }
                }
                if (DeviceInfo.ModuleId.HasFlag(ModuleEnumeration.Cabinet)
                                    && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Cabinet)
                                    && userInfoForDevice.CabinetInfoInJsonFormat.IsNotNullOrEmpty()
                                    && userInfoForDevice.UserIdOnDevice <= int.MaxValue)
                {
                    var cabinetNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoForDevice.CabinetInfoInJsonFormat);
                    if (cabinetNumbers.IsCollectionNotNullOrEmpty())
                    {
                        result = _communicationOcx.SetUserCtrlEx(DeviceInfo.DeviceNumber
                            , (int)userInfoForDevice.UserIdOnDevice
                            , 0
                            , 0
                            , 0
                            , 0
                            , cabinetNumbers.First()
                            , userInfoForDevice.StartDateTime.Year
                            , userInfoForDevice.StartDateTime.Month
                            , userInfoForDevice.StartDateTime.Day
                            , userInfoForDevice.EndDateTime.Value.Year
                            , userInfoForDevice.EndDateTime.Value.Month
                            , userInfoForDevice.EndDateTime.Value.Day

                            );
                        if (!result)
                        {
                            ThrowLastError();
                        }
                    }
                }


            }
            finally
            {
                if (disableDevice)
                {
                    EnableDevice();
                }
            }


        }

        private void SetUserInfoOldVersion(DtoUserDeviceRelatedData userInfo, bool disableDevice)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand SetUserInfoOldVersion  is calling", new { DeviceInfo, User = userInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }

            try
            {
                bool result;
                if (disableDevice)
                {
                    result = DisableDevice();
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }

                var userInfoForDevice = userInfo.WithDeviceLocalDates(DeviceInfo);
                
                result = _communicationOcx.SetUserNameUTF8(0,
                    DeviceInfo.DeviceNumber,
                    (int)userInfoForDevice.UserIdOnDevice,
                        EMachineNumber,
                     new VariantWrapper(userInfoForDevice.UserName)
                );
                if (!result)
                {
                    ThrowLastError();
                }

                if (userInfoForDevice.Password.IsCollectionNotNullOrEmpty())
                {
                    var objUserPasswordData = new int[1888 / 4];
                    object objUserPassword = new VariantWrapper(objUserPasswordData);

                    result = _communicationOcx.SetEnrollData(DeviceInfo.DeviceNumber,
                        (int)userInfoForDevice.UserIdOnDevice,
                        EMachineNumber,
                        10,
                        userInfoForDevice.Privilege,
                        ref objUserPassword,
                        userInfoForDevice.Password.IsNotNullOrEmpty() ? userInfoForDevice.Password.ToInt32() : 0);
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }

                if (userInfoForDevice.RfCardNumbers.IsCollectionNotNullOrEmpty())
                {
                    var objUserCardNumberData = new int[1888 / 4];
                    object objUserCardNumber = new VariantWrapper(objUserCardNumberData);

                    result = _communicationOcx.SetEnrollData(DeviceInfo.DeviceNumber,
                        (int)userInfoForDevice.UserIdOnDevice,
                        EMachineNumber,
                        11,
                        userInfoForDevice.Privilege,
                        ref objUserCardNumber,
                        userInfoForDevice.RfCardNumbers.First().ToInt32());
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }

                if (userInfoForDevice.Password.IsCollectionNullOrEmpty() && userInfoForDevice.RfCardNumbers.IsCollectionNullOrEmpty())
                {

                    result = _communicationOcx.ModifyPrivilege(DeviceInfo.DeviceNumber,
                        (int)userInfoForDevice.UserIdOnDevice,
                        EMachineNumber,
                        0,
                        userInfoForDevice.Privilege);
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }

                if (DeviceInfo.ModuleId.HasFlag(ModuleEnumeration.Elevator)
                    && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Elevator)
                    && userInfoForDevice.ElevatorInfoInJsonFormat.IsNotNullOrEmpty())
                {
                    var elevatorFloorNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoForDevice.ElevatorInfoInJsonFormat);
                    if (elevatorFloorNumbers.IsCollectionNotNullOrEmpty())
                    {
                        var elevatorFloorObject = new VariantWrapper(elevatorFloorNumbers.JoinWithComma());
                        result = _communicationOcx.SetUserProfile(0
                            , DeviceInfo.DeviceNumber
                            , (int)userInfoForDevice.UserIdOnDevice
                            , EMachineNumber
                            , elevatorFloorObject);
                        if (!result)
                        {
                            ThrowLastError();
                        }
                    }
                }

                if (DeviceInfo.ModuleId.HasFlag(ModuleEnumeration.Cabinet)
                    && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Cabinet)
                    && userInfoForDevice.CabinetInfoInJsonFormat.IsNotNullOrEmpty())
                {
                    var cabinetNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoForDevice.CabinetInfoInJsonFormat);
                    if (cabinetNumbers.IsCollectionNotNullOrEmpty())
                    {
                        // ReSharper disable PossibleInvalidOperationException
                        result = _communicationOcx.SetUserCtrl(DeviceInfo.DeviceNumber
                            , (int)userInfoForDevice.UserIdOnDevice
                            , 0
                            , cabinetNumbers.First()
                            , userInfoForDevice.StartDateTime.Year
                            , userInfoForDevice.StartDateTime.Month
                            , userInfoForDevice.StartDateTime.Day
                            , userInfoForDevice.EndDateTime.Value.Year
                            , userInfoForDevice.EndDateTime.Value.Month
                            , userInfoForDevice.EndDateTime.Value.Day
                        );
                        // ReSharper restore PossibleInvalidOperationException

                        if (!result)
                        {
                            ThrowLastError();
                        }
                    }
                }

            }
            finally
            {
                if (disableDevice)
                {
                    EnableDevice();
                }
            }

        }



        public List<DtoUserInfoDefinedOnDevice> GetAllUsersInfo()
        {

            if (DeviceInfo.DeviceSettings?.TimyDeviceSettings != null &&
                DeviceInfo.DeviceSettings.TimyDeviceSettings.IsUserId32Bit)
            {
                return GetAllUsersInfoOldVersion();
            }
            return GetAllUsersInfoLongVersion();
        }

        private List<DtoUserInfoDefinedOnDevice> GetAllUsersInfoLongVersion()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetAllUserIdLongVersion is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                result = _communicationOcx.ReadAllUserIDLongID(DeviceInfo.DeviceNumber);
                if (!result)
                {
                    ThrowLastError();
                }


                var dwBackupNum = 0;
                var dwPrivilegeNum = 0;
                var dwEnable = 0;

                do
                {
                    var userIdString = string.Empty;
                    object userIdStringObject = new VariantWrapper(userIdString);
                    result = _communicationOcx.GetAllUserIDLongID(
                        DeviceInfo.DeviceNumber,
                        ref userIdStringObject,
                        ref dwBackupNum,
                        ref dwPrivilegeNum,
                        ref dwEnable
                        );
                    var userId = ((string)userIdStringObject).ToInt64();
                    if (usersInfo.All(ui => ui.UserIdOnDevice != userId))
                    {
                        usersInfo.Add(new DtoUserInfoDefinedOnDevice
                        {
                            UserIdOnDevice = userId,
                            Name = string.Empty,
                            Privilege = dwPrivilegeNum,
                        });
                    }


                } while (result);

            }
            finally
            {
                EnableDevice();
            }

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetAllUserIdOldVersion Result", new { DeviceInfo, Result = usersInfo });
            }
            return usersInfo;
        }

        private List<DtoUserInfoDefinedOnDevice> GetAllUsersInfoOldVersion()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetAllUserIdOldVersion is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            try
            {
                var result = DisableDevice();
                if (!result)
                {
                    ThrowLastError();
                }

                result = _communicationOcx.ReadAllUserID(DeviceInfo.DeviceNumber);
                if (!result)
                {
                    ThrowLastError();
                }


                var dwEnrollNumber = 0;
                var dwEnMachineId = 0;
                var dwBackupNum = 0;
                var dwPrivilegeNum = 0;
                var dwEnable = 0;

                do
                {

                    result = _communicationOcx.GetAllUserID(
                        DeviceInfo.DeviceNumber,
                        ref dwEnrollNumber,
                        ref dwEnMachineId,
                        ref dwBackupNum,
                        ref dwPrivilegeNum,
                        ref dwEnable
                    );
                    if (usersInfo.All(ui => ui.UserIdOnDevice != dwEnrollNumber))
                    {
                        usersInfo.Add(new DtoUserInfoDefinedOnDevice
                        {
                            UserIdOnDevice = dwEnrollNumber,
                            Name = string.Empty,
                            Privilege = dwPrivilegeNum,
                        });
                    }

                } while (result);

            }
            finally
            {
                EnableDevice();
            }

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetAllUserIdOldVersion Result", new { DeviceInfo, Result = usersInfo });
            }
            return usersInfo;
        }




        public int GetUserCount()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserCount is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var count = 0;

            try
            {
                var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 2, ref count);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetUserCount Result", new { DeviceInfo, Count = count });
            }
            return count;
        }

        public int GetFingerCount()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetFingerCount is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var count = 0;

            try
            {
                var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 3, ref count);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetFingerCount Result", new { DeviceInfo, Count = count });
            }
            return count;
        }

        public int GetFaceCount()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetFaceCount is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var count = 0;

            try
            {
                if (DeviceInfo.HasVisiblelight)
                {
                    var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 9, ref count);
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }
                else
                {
                    var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 21, ref count);
                    if (!result)
                    {
                        ThrowLastError();
                    }
                }

            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetFaceCount Result", new { DeviceInfo, Count = count });
            }
            return count;
        }

        public int GetRecordCount()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetRecordCount is calling", new { DeviceInfo });
            }
            if (!IsDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var count = 0;

            try
            {
                var result = _communicationOcx.GetDeviceStatus(DeviceInfo.DeviceNumber, 6, ref count);
                if (!result)
                {
                    ThrowLastError();
                }
            }
            finally
            {
                EnableDevice();
            }
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetStatistics))
            {
                LoggingSystem.LogInfo("Timy OnDemand GetRecordCount Result", new { DeviceInfo, Count = count });
            }
            return count;
        }

        #endregion

        #region Scan



        public void ScanFace(DtoUserDeviceRelatedData userInfo)
        {
            object employeeNumberObject = new VariantWrapper(userInfo.UserIdOnDevice);
            object nameObject = new VariantWrapper(userInfo.UserName);

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.Scan))
            {
                LoggingSystem.LogInfo("Timy OnDemand Scan is calling");
            }

            var result = !_communicationOcx.AddUser(DeviceInfo.DeviceNumber,
                ref employeeNumberObject,
                DeviceInfo.HasVisiblelight ? 50 : 20,
                userInfo.Privilege,
                ref nameObject
            );
            if (!result)
            {
                ThrowLastError();
            }
        }


        public void ScanFinger(DtoUserDeviceRelatedData userInfo, int fingerIndex)
        {
            object employeeNumberObject = new VariantWrapper(userInfo.UserIdOnDevice);
            object nameObject = new VariantWrapper(userInfo.UserName);

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.Scan))
            {
                LoggingSystem.LogInfo("Timy OnDemand Scan is calling");
            }

            var result = !_communicationOcx.AddUser(DeviceInfo.DeviceNumber,
                ref employeeNumberObject,
                fingerIndex,
                userInfo.Privilege,
                ref nameObject
            ); 
            if (!result)
            {
                ThrowLastError();
            }
        }


        public void ScanCard(DtoUserDeviceRelatedData userInfo)
        {
            object employeeNumberObject = new VariantWrapper(userInfo.UserIdOnDevice);
            object nameObject = new VariantWrapper(userInfo.UserName);

            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.Scan))
            {
                LoggingSystem.LogInfo("Timy OnDemand Scan is calling");
            }

            var result = !_communicationOcx.AddUser(DeviceInfo.DeviceNumber,
                ref employeeNumberObject,
                11,
                userInfo.Privilege,
                ref nameObject
            ); 
            if (!result)
            {
                ThrowLastError();
            }
        }


        #endregion

        #region AccessControl


        public void OpenDoor(int timeoutInSecond)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.OpenDoor))
            {
                LoggingSystem.LogInfo("Timy OnDemand OpenDoor is calling", new { DeviceInfo, Timeout = timeoutInSecond });
            }
            var result = !_communicationOcx.SetDoorStatus(DeviceInfo.DeviceNumber, (int)TimyDoorStatus.SOFTWAREOPEN);
            if (!result)
            {
                ThrowLastError();
            }
        }

        public void OpenCabinetDoor(int cabinetNumber)
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.OpenDoor))
            {
                LoggingSystem.LogInfo("Timy OnDemand OpenCabinetDoor is calling", new { DeviceInfo, CabinetNumber = cabinetNumber });
            }
            _communicationOcx.OpendoorEx(DeviceInfo.DeviceNumber,cabinetNumber);
        }


        #endregion

        

        #endregion


        private void ThrowLastError(ICollection<TimyErrorEnum> errorsToExclude = null)
        {
            var errorCode = 0;
            _communicationOcx.GetLastError(ref errorCode);
            if (errorCode != (int)TimyErrorEnum.Successful)
            {
                if (errorsToExclude != null && errorsToExclude.Contains((TimyErrorEnum)errorCode))
                {
                    return;
                }
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }
        }



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
                // COM teardown only on explicit Dispose — never on the finalizer thread.
                if (IsDeviceConnected)
                {
                    Disconnect();
                }
            }
            _disposed = true;
        }

        #endregion


    }
}