using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Pw.PwConcepts;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using MD.PersianDateTime;
using xLink;

namespace GuardianCommunication.Hardware.Pw
{
    //link_enc_disable != 96 Authentication Enable else Disable
    public class PwOnDemandAdapter : IDisposable
    {
        public DtoCommunicationDeviceData DeviceInfo { get; set; }


        #region Private Fields

        private byte[] _pwVersion;
        private int _pwVersionString;
        private const int AuthenticationDefaultKey = 96;
        private const int RecordCountReadData = 10000;
        private const int RecordCountReadout = 100000;
        private const int LengthFingerPrintArray = 4000;
        private const int Pw1650 = 1650;
        private const byte KeyDefault = 97;
        private readonly NetTypes.LINK_PARAMS_TYPE _cbf;
        private xLinkClass _xDll;
        private readonly string _filePathBonse;
        private readonly string _filePathFingerPrint;
        private readonly int _sleepTimeForReadout;
        private readonly string _filePathUser;
        private readonly string _communicationKeyForAuth;
        private int _sysKeyType;
        private int _sysCode;
        private ushort _pwPort = 10001;
        private bool _isDeviceConnected { get; set; }
        private const string AdminsUsersFileName = "users.txt";
        private const string PersonnelFileName = "inspers.txt";
        //private const string PersonnelFileName = "opers.txt";

        #endregion


        public PwOnDemandAdapter(DtoCommunicationDeviceData deviceInfo)
        {
            DeviceInfo = deviceInfo;
            _xDll = new xLinkClass();
            _filePathBonse = GetPwDriverPath(PwFileType.Bones);
            _filePathFingerPrint = GetPwDriverPath(PwFileType.FingerPrint);
            _filePathUser = GetPwDriverPath(PwFileType.User);
            _communicationKeyForAuth = DeviceInfo.CommunicationPassword;
            _sleepTimeForReadout = 1000;
            _sysCode = deviceInfo.DeviceTypeCode == 8 || deviceInfo.DeviceTypeCode == 9 ? 1650 : 0;
            _cbf = new NetTypes.LINK_PARAMS_TYPE
            {
                textFormat = NetConsts.TEXT_NEW_FORMAT,
                viaF = 0,
                ioAuto = 1,
                sysId = NetConsts.SYS_PW1XXX,
                hwType = (byte)DeviceInfo.DeviceTypeCode,
                maxRetry = (byte)deviceInfo.PwMaxRetry,
                timeout = (ushort)deviceInfo.PwConnectionTimeout,
                blkLen = 4000,
                dateType = NetConsts.DATE_CHRIST,
                appendText = true,
                saveInStructF = true,
                sysNo = (ushort)DeviceInfo.DeviceNumber,
                clearPW = false,
                comPort = deviceInfo.ComPort.HasValue ? (byte)deviceInfo.ComPort.Value : (byte)1,
                baudrate = deviceInfo.BuadRate.HasValue ? (int)deviceInfo.BuadRate.Value : 1,
            };
            switch (deviceInfo.ConnectionTypeEnum)
            {

                case ConnectionTypeEnumeration.Ethernet:
                    if (!deviceInfo.TcpPort.HasValue)
                    {
                        deviceInfo.TcpPort = _pwPort;
                    }
                    _cbf.chType = NetConsts.CH_ETNET;
                    _cbf.pwIP = deviceInfo.Ip;
                    _cbf.pcPort = (ushort)deviceInfo.PwPcPort;
                    _cbf.pwPort = (ushort)deviceInfo.TcpPort.Value;
                    break;

            }
        }


        #region Private Methods

        private static void ClearDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var filePaths = Directory.GetFiles(directoryPath);
            foreach (var filePath in filePaths)
                File.Delete(filePath);
        }

        private bool Authentication(string communicationKey)
        {
            byte keyType = 0;
            byte tries = 0;
            var keyTemp = communicationKey.PadRight(16, '0');
            var key = Encoding.ASCII.GetBytes(keyTemp);
            if (string.IsNullOrEmpty(communicationKey) || _cbf.hwType == (int)PwDeviceTypeEnum.Pw1600)
            {
                key = new byte[16];
                if (_sysKeyType == KeyDefault)
                {
                    for (var i = 0; i < 16; i++)
                        key[i] = (byte)(0x40 + i);
                }
                else
                {
                    //below key is only for test, application must use adequate key
                    for (var i = 0; i < 16; i++)
                        key[i] = (byte)(0x60 + i);
                }
            }
            var result = _xDll.authenticate(_cbf, key, ref keyType, ref tries, _pwVersion);
            ThrowErrorIfRequired(DeviceInfo, result);
            return true;
        }

        private void SendFingerPrintToDevice(DtoEmployeeDeviceRelatedData user)
        {

            if (user.FingerDataList.IsCollectionNullOrEmpty()) return;
            var fingerPrintForSend = new byte[LengthFingerPrintArray];
            var counter = 0;
            var storedSingerPrints = user.FingerDataList.OrderBy(row => row.FingerIndex).ToList();
            foreach (var finger in storedSingerPrints)
            {
                var currentFingerData = finger.TemplateData.ToArray();
                foreach (var current in currentFingerData)
                {
                    fingerPrintForSend[counter++] = current;
                }
                var index = currentFingerData.Length;
                while (index < 386)
                {
                    fingerPrintForSend[counter++] = 0;
                    index++;
                }
            }
            var result = _xDll.send_this_id_templates(_cbf, (uint)user.EmployeeNumber, (byte)storedSingerPrints.Count, fingerPrintForSend, false);
            ThrowErrorIfRequired(DeviceInfo, result);
        }

        #endregion


        #region public Methods

        #region Other

        public bool SetDateTime(DateTime now)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("PW OnDemand SetDateTime", DeviceInfo);
            }
            var result = _xDll.sendTime(_cbf);
            ThrowErrorIfRequired(DeviceInfo, result);
            return true;
        }

        public bool ChangeDevicePassword(string key)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.ChangeDevicePassword))
            {
                LoggingSystem.LogInfo("PW OnDemand ChangeDevicePassword", new { DeviceInfo, Key = key });
            }

            switch (_cbf.hwType)
            {
                case (int)PwDeviceTypeEnum.Pw1000:
                case (int)PwDeviceTypeEnum.Pw1200:
                case (int)PwDeviceTypeEnum.Pw1400:
                case (int)PwDeviceTypeEnum.Pw1410:
                case (int)PwDeviceTypeEnum.Pw1500:
                case (int)PwDeviceTypeEnum.Pw1510:
                    {
                        if (key.Length < 8 || key.Length > 8)
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwKeyLengthIsNotValid);
                        }
                        var result = _xDll.sendPassword(_cbf, key);
                        ThrowErrorIfRequired(DeviceInfo, result);
                        break;
                    }
                default:
                    {
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwKeyLengthIsNotValid);
                    }
            }
            return true;
        }

        #endregion

        #region Commiunication

        public PwDeviceConnectResult Connect(bool setTime = false)
        {
            var finalResult = new PwDeviceConnectResult
            {
                IsConnected = false,
            };
            var resultOfTestLinkMethod = PwErrorEnum.LINK_OK;
            var stat = new NetTypes.PW_STATUS_TYPE
            {
                dateTime = new byte[7],
                ver = new byte[3],
                biosVer = new byte[3],
                grpCodes = new ushort[5]
            };
            // Bounded, cross-platform connect (replaces Delegate.BeginInvoke): run only the blocking
            // testLink on a task. The device-mutating steps (auth / key-change) are performed on the
            // calling thread on success, so a timed-out connect can never fire them late.
            var connectTask = Task.Run(() => (PwErrorEnum)_xDll.testLink(_cbf, false, ref stat));
            var completed = false;
            try
            {
                if (connectTask.Wait(DeviceInfo.ConnectTimeout * 1000))
                {
                    resultOfTestLinkMethod = connectTask.Result;
                    completed = true;
                }
                else
                {
                    // Timed out: observe any later fault so it isn't an unobserved task exception.
                    connectTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
            catch (AggregateException)
            {
                // testLink threw; treated as a failed connection.
            }

            if (!completed || resultOfTestLinkMethod != PwErrorEnum.LINK_OK && resultOfTestLinkMethod != PwErrorEnum.ERR_POOR_Security)
                return finalResult;
            _pwVersion = stat.ver;
            _pwVersionString = string.Join("", _pwVersion.Select(row => $"{row:D2}")).ToInt32();
            _sysKeyType = stat.link_code1;
            _sysCode = stat.sysCode;
            if (_sysCode == Pw1650)
            {
                _cbf.sysId = NetConsts.SYS_JT1XXX;
            }
            // Auth / key-change relocated from the background delegate (same gate as before) so they
            // only run on a timely, successful connect and never fire late after a timeout.
            if ((_cbf.hwType == (int)PwDeviceTypeEnum.Pw1600 || _cbf.hwType == (int)PwDeviceTypeEnum.Pw1650)
                && stat.link_enc_disable != AuthenticationDefaultKey)
            {
                var auth = Authentication(_communicationKeyForAuth);
                if (auth && resultOfTestLinkMethod == PwErrorEnum.ERR_POOR_Security)
                {
                    ChangeCommunicationKey(string.Empty, _communicationKeyForAuth);
                }
            }
            _isDeviceConnected = true;
            finalResult.IsConnected = true;
            finalResult.BiosVersion = _pwVersionString;
            finalResult.RecordCount = stat.records;
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.ConnectProcess))
            {
                LoggingSystem.LogInfo("Pw connect process", "Status of connect", stat);
            }

            if (setTime && stat.dateTime.IsCollectionNotNullOrEmpty() && stat.dateTime.Length == 7)
            {
                if (stat.dateTime[6] == 0)
                {
                    // Miladi
                    finalResult.DeviceDateTime = new DateTime(
                        stat.dateTime[0], stat.dateTime[1], stat.dateTime[2], stat.dateTime[4], stat.dateTime[5],
                        0);
                }
                else
                {
                    // Shamsi
                    finalResult.DeviceDateTime = new PersianDateTime(
                        stat.dateTime[0], stat.dateTime[1], stat.dateTime[2], stat.dateTime[4], stat.dateTime[5],
                        0);
                }
            }

            return finalResult;
        }

        public void Disconnect()
        {
            _isDeviceConnected = false;
        }

        public void ChangeCommunicationKey(string oldPassword, string newPassword)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.ChangeCommunicationKey))
            {
                LoggingSystem.LogInfo("PW OnDemand Set ChangeCommunicationKey", new { DeviceInfo, OldPassword = oldPassword, NewPassword = newPassword });
            }

            if (_cbf.hwType != (int)PwDeviceTypeEnum.Pw1600 && _cbf.hwType != (int)PwDeviceTypeEnum.Pw1650)
            {
                ChangeDevicePassword(newPassword);
                return;
            }

            if (oldPassword.Length > 16 || newPassword.Length > 16)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwKeyLengthIsNotValid);
            }
            //------------- Auth with defual--------
            //keyold = ASCIIEncoding.ASCII.GetBytes(oldPassword);
            //string d = "1234";
            //d = d.PadRight(16, '0');
            //key = Encoding.ASCII.GetBytes(d);
            //if (EPwError.ERR_POOR_Security == (EPwError)rc)
            //{
            //}
            // rc = _xDll.authenticate(_cbf, keyold, ref keyType, ref tries);
            //if (rc > 0)
            //{
            //	throw rc.MapErrorThrowException(this);
            //}

            Authentication(oldPassword);

            //var keyTemp = newPassword.PadRight(16, '0');
            //if (oldPassword.IsNullOrEmpty())
            //{
            var keyNew = new byte[16];
            for (var i = 0; i < 16; i++)
                keyNew[i] = (byte)(0x60 + i);
            //}
            var result = _xDll.change_key(_cbf, keyNew, 0);
            ThrowErrorIfRequired(DeviceInfo, result);
            _sysKeyType = 96;
        }

        #endregion

        #region Attendance

        public List<DtoAttendance> GetData()
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetData))
            {
                LoggingSystem.LogInfo("PW OnDemand GetData is calling", DeviceInfo);
            }
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            ushort recordsCount = 0;
            var bonesFn = "";
            var records = new NetTypes.IN_OUT_RECORD_TYPE[RecordCountReadData];
            const uint gateNumber = 1234;
            if (_filePathBonse.Length > 0 && !Directory.Exists(_filePathBonse))
                Directory.CreateDirectory(_filePathBonse);
            _cbf.dateType = NetConsts.DATE_CHRIST;   //print dates in solar.(text file)
            var result = _xDll.getIOs(_cbf, _filePathBonse, ref recordsCount, ref bonesFn, ref records, gateNumber, DateTime.Now.ToString("YYYY-MM-dd HH-mm-ss"));
            if (recordsCount == 0 && result > 0)
            {
                if ((PwErrorEnum)result == PwErrorEnum.ERR_NO_RECORDS)
                {
                    return new List<DtoAttendance>();
                }
                ThrowErrorIfRequired(DeviceInfo, result);
            }
            var recordsList = new List<DtoAttendance>();
            for (var i = 0; i < recordsCount; i++)
            {
                if (records[i].cardNo < 1) continue;
                var row = new DtoAttendance
                {
                    Id = i + 1,
                    EmployeeNumber = records[i].cardNo,
                    DeviceNumber = DeviceInfo.DeviceNumber,
                    CameraId = null,
                    AttendanceDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m, records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                    VerificationStyle = records[i].flags,
                    IsSent = false,
                    IsInvalid = false,
                    StatusCode = 0,
                    AttendanceSource = AttendanceSourceEnumeration.Device,
                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                    RfCardNumber = null,
                };

                recordsList.Add(row);
            }
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetData))
            {
                LoggingSystem.LogInfo("PW OnDemand GetData result ", new { DeviceInfo, Attendances = recordsList });
            }
            return recordsList;
        }

        public List<DtoAttendance> Readout(DateTime startDate, DateTime endDate)
        {
            return ReadoutForPw1600AndAbove(startDate, endDate);
        }

        private List<DtoAttendance> ReadoutForPw1600AndAbove(DateTime startDate, DateTime endDate)
        {

            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetData))
            {
                LoggingSystem.LogInfo("PW OnDemand ReadoutForPw1600AndAbove is calling ", DeviceInfo);
            }
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            /*validDays متغیری است که مشخص میکند فراخوانی برای تاریخ امروز است یا روزهای قبل.اگر تاریخ امروز باشد مقدارش 0 اگر روزهای قبل مقدارش 1 خواهد بود*/
            ushort recordsCount = 0;
            var bonesFn = "";
            var records = new NetTypes.IN_OUT_RECORD_TYPE[RecordCountReadout];
            uint gateNumber = 1234;
            if (!Directory.Exists(_filePathBonse))
                Directory.CreateDirectory(_filePathBonse);
            _cbf.dateType = NetConsts.DATE_CHRIST;   //print dates in solar.(text file)
            var recordsList = new List<DtoAttendance>();
            var days = (endDate.Date - startDate.Date).TotalDays;
            for (var j = 0; j <= days; j++)
            {
                var currentDay = startDate.AddDays(j);
                var xNextDate = new NetTypes.DATE_TYPE
                {
                    y = (short)currentDay.Date.Year,
                    m = (byte)currentDay.Date.Month,
                    d = (byte)currentDay.Date.Day
                };
                //var validDays = currentDay.Date == DateTime.Now.Date ? (byte)0 : (byte)1;
                Thread.Sleep(_sleepTimeForReadout);
                _xDll.NEW_recoverIOs(_cbf, _filePathBonse, xNextDate, xNextDate, ref recordsCount, ref bonesFn, ref records, gateNumber, 0, false);
                for (var i = 0; i < recordsCount; i++)
                {
                    var pwRow = records[i];
                    var row = new DtoAttendance
                    {
                        Id = i + 1,
                        EmployeeNumber = pwRow.cardNo,
                        VerificationStyle = (int)GetVerificationStyle(pwRow.flags),
                        //IoType = pwRow.ioType,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        CameraId = null,
                        AttendanceDateTime = new DateTime(records[i].xDate.y, records[i].xDate.m, records[i].xDate.d, records[i].hh, records[i].mn, records[i].ss),
                        IsSent = false,
                        IsInvalid = false,
                        StatusCode = 0,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                    };
                    recordsList.Add(row);
                }
            }
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetData))
            {
                LoggingSystem.LogInfo("PW OnDemand ReadoutForPw1600AndAbove result", new { DeviceInfo, Attendances = recordsList });
            }
            return recordsList;
        }

        public void ClearData()
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("PW OnDemand ClearData", DeviceInfo);
            }
            try
            {
                _cbf.clearPW = true;
                GetData();
            }
            finally
            {
                _cbf.clearPW = false;
            }
        }

        private static AttendanceVerificationStyleEnumeration GetVerificationStyle(int verifyMode)
        {
            switch (verifyMode)
            {
                case 1:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case 2:
                    return AttendanceVerificationStyleEnumeration.Card;
                default:
                    return AttendanceVerificationStyleEnumeration.Unknown;
            }
        }

        #endregion

        #region Usering And Finger

        public void DeleteUserById(long employeeNumber)
        {
            var id = Convert.ToUInt32(employeeNumber);
            DeleteUserRecord(new List<long> { employeeNumber });
            var templateCount = GetTemplateCount(id);
            for (var fingerprintIndex = 1; fingerprintIndex <= templateCount; fingerprintIndex++)
            {
                var result = _xDll.erase_pw1410_template(_cbf, (uint)employeeNumber, (byte)fingerprintIndex, true);
                if (result != (int)PwErrorEnum.errInvalidId && result != 0)
                {
                    ThrowErrorIfRequired(DeviceInfo, result);
                }
            }
        }

        private void DeleteUserRecord(List<long> employeeNumbers)
        {
            var sortedEmployeeNumbers = employeeNumbers.OrderBy(row => row).ToList();
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("PW OnDemand DeleteUserRecord", new { DeviceInfo, EmployeeNumbers = employeeNumbers });
            }

            switch ((PwDeviceTypeEnum)DeviceInfo.DeviceTypeCode)
            {
                case PwDeviceTypeEnum.Pw1680:
                case PwDeviceTypeEnum.Pw1600:
                case PwDeviceTypeEnum.Pw1650:
                case PwDeviceTypeEnum.Pw1700:
                case PwDeviceTypeEnum.Pw1610:
                case PwDeviceTypeEnum.Pw1660:
                    {
                        var batchCounter = 0;
                        while (true)
                        {
                            var records = sortedEmployeeNumbers.Skip(batchCounter * 100).Take(100).Select(en => (uint)en).ToArray();
                            batchCounter++;
                            if (records.Length == 0)
                            {
                                break;
                            }
                            ushort done = 0;
                            var result = _xDll.delete_pers_records(_cbf, (byte)records.Length, records, ref done);
                            if (result != (int)PwErrorEnum.errInvalidId && result != 0)
                            {
                                ThrowErrorIfRequired(DeviceInfo, result);
                            }
                        }
                    }
                    break;
            }
        }

        private byte GetTemplateCount(uint employeeNumber)
        {
            byte templateCount = 0;
            var result = _xDll.get_this_id_templatesNo(_cbf, employeeNumber, ref templateCount);
            ThrowErrorIfRequired(DeviceInfo, result);
            return templateCount;
        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.DeleteUser))
            {
                LoggingSystem.LogInfo("PW OnDemand DeleteAllUsers", DeviceInfo);
            }
            switch ((PwDeviceTypeEnum)DeviceInfo.DeviceTypeCode)
            {
                case PwDeviceTypeEnum.Pw1680:
                case PwDeviceTypeEnum.Pw1600:
                case PwDeviceTypeEnum.Pw1650:
                case PwDeviceTypeEnum.Pw1700:
                case PwDeviceTypeEnum.Pw1610:
                case PwDeviceTypeEnum.Pw1660:
                    var userIds = GetAllUsersInfo();
                    if (userIds.IsCollectionNullOrEmpty())
                    {
                        return;
                    }
                    DeleteUserRecord(userIds.Select(e => e.EmployeeNumber).ToList());
                    break;
            }

            var deleteTemplateResult = _xDll.erase_all_pw1410_template(_cbf);
            ThrowErrorIfRequired(DeviceInfo, deleteTemplateResult);
        }

        public DtoEmployeeDeviceRelatedData GetUserInfoByUserId(long userId, TemplateTypeEnumeration enrollType)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand GetUserInfoByUserId is calling", new { DeviceInfo, UserId = userId, EnrollType = enrollType });
            }
            ClearDirectory(_filePathFingerPrint);
            var user = new DtoEmployeeDeviceRelatedData
            {
                EmployeeNumber = userId,

            };
            if (enrollType.HasFlag(TemplateTypeEnumeration.FingerPrint))
            {
                var templates = new byte[LengthFingerPrintArray];
                var templateCount = GetTemplateCount((uint)userId);
                if (templateCount < 1) return user;
                var result = _xDll.get_this_id_templates(_cbf, (uint)userId, templateCount, ref templates, false);
                if (result == 0)
                {
                    try
                    {
                        var startIndexOfCurrentFinger = 0;
                        for (var i = 0; i < templateCount; i++)
                        {
                            var lenOfCurrentFinger = BitConverter.ToInt16(templates, startIndexOfCurrentFinger);
                            if (lenOfCurrentFinger <= 0) continue;
                            var currentTemplate = new byte[lenOfCurrentFinger];
                            Array.ConstrainedCopy(templates, startIndexOfCurrentFinger, currentTemplate, 0,
                                lenOfCurrentFinger);
                            user.FingerDataList.Add(new DtoEmployeeFinger
                            {
                                FingerIndex = i + 1,
                                CheckSum = 0,
                                TemplateData = currentTemplate,
                                EmployeeNumber = userId
                            });
                            startIndexOfCurrentFinger += lenOfCurrentFinger + 2;
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on get finger prints");
                    }
                }
                else
                {
                    ThrowErrorIfRequired(DeviceInfo, result);
                }
            }
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand GetUserInfoByUserId result", new { DeviceInfo, User = user });
            }
            return user;
        }

        public void SetUserPhoto(DtoEmployeeImage userPhoto)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand SetUserPhoto is calling", new { DeviceInfo, UserPhoto = userPhoto });
            }
            //var result = new List<DeviceUserOperationResult>();
            var userIds = GetAllUsersInfo();
            if (userIds.Any(e => e.EmployeeNumber == userPhoto.EmployeeNumber))
            {
                var pcPath = GetPwDriverPath(PwFileType.Bones);
                var filePath = $"{pcPath}ph_{userPhoto.EmployeeNumber:D10}.jpg";
                using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    file.Write(userPhoto.PhotoData, 0, userPhoto.PhotoData.Length);
                    file.Close();
                }
                byte grp = 1;
                var result = _xDll.send_photo(_cbf, pcPath, grp, userPhoto.EmployeeNumber.ToString());
                ThrowErrorIfRequired(DeviceInfo, result);
                File.Delete(filePath);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwUserNotFound);
            }

        }

        public List<DtoUserInfoDefinedOnDevice> GetAllUsersInfo()
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand GetAllUserId is calling", DeviceInfo);
            }
            var usersInfo = new List<DtoUserInfoDefinedOnDevice>();
            var records = new uint[10000];
            ushort numberOfRecords = 0;
            var result = _xDll.get_pw1410_templates_list(_cbf, ref numberOfRecords, ref records);
            ThrowErrorIfRequired(DeviceInfo, result);
            for (var i = 0; i < numberOfRecords; i++)
            {
                usersInfo.Add(new DtoUserInfoDefinedOnDevice
                {
                    EmployeeNumber = records[i],
                    Name = string.Empty,
                    Privilege = null,
                });
            }
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.GetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand GetAllUserId  result", new { DeviceInfo, Result = usersInfo });
            }
            return usersInfo;
        }

        public int GetUserCount()
        {
            if (_isDeviceConnected == false)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            }
            return GetAllUsersInfo().Count;
        }

        public void SetUserInfoWithTemplate(List<DtoEmployeeDeviceRelatedData> userInfos)
        {

            if (userInfos.IsCollectionNullOrEmpty())
            {
                return;
            }
            var sortedUserInfo = userInfos.OrderBy(row => row.EmployeeNumber).ToList();

            SetUserInfo(sortedUserInfo);
            foreach (var userInfo in sortedUserInfo)
            {
                SendFingerPrintToDevice(userInfo);
            }
        }

        public void SetUserInfo(List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.SetUser))
            {
                LoggingSystem.LogInfo("PW OnDemand SetUserInfo is calling", new { DeviceInfo, User = userInfos });
            }
            switch ((PwDeviceTypeEnum)DeviceInfo.DeviceTypeCode)
            {
                case PwDeviceTypeEnum.Pw1680:
                case PwDeviceTypeEnum.Pw1600:
                case PwDeviceTypeEnum.Pw1650:
                case PwDeviceTypeEnum.Pw1700:
                case PwDeviceTypeEnum.Pw1610:
                case PwDeviceTypeEnum.Pw1660:
                    SetEmployees(userInfos);
                    SetAdmins(userInfos);
                    break;
            }

        }

        public void SendWithoutFinger(List<DtoEmployeeDeviceRelatedData> userInfos)
        {

            switch ((PwDeviceTypeEnum)DeviceInfo.DeviceTypeCode)
            {
                case PwDeviceTypeEnum.Pw1680:
                case PwDeviceTypeEnum.Pw1600:
                case PwDeviceTypeEnum.Pw1650:
                case PwDeviceTypeEnum.Pw1700:
                case PwDeviceTypeEnum.Pw1610:
                case PwDeviceTypeEnum.Pw1660:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            var fileContentBuilder = new StringBuilder();
            var sortedUserInfo = userInfos.OrderBy(row => row.EmployeeNumber).ToList();
            foreach (var user in sortedUserInfo)
            {
                var userInfo = $@"{user.EmployeeNumber:D10} {user.Password.ToNotNullString().PadLeft(4, '0')}";
                fileContentBuilder.AppendLine(userInfo);
            }
            var withoutFingerPath = Path.Combine(_filePathUser, "WO_FNG_O.TXT");
            if (!Directory.Exists(_filePathUser))
            {
                Directory.CreateDirectory(_filePathUser);
            }
            File.WriteAllText(withoutFingerPath, fileContentBuilder.ToString());
            if (!File.Exists(withoutFingerPath))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwFileNotExist);
            Thread.Sleep(_sleepTimeForReadout);
            var errorResult = _xDll.sendWithoutFingers(_cbf, withoutFingerPath);
            if (errorResult > 0)
            {
                ThrowErrorIfRequired(DeviceInfo, errorResult);
            }
        }

        public void SetValidInvalidList(List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            switch ((PwDeviceTypeEnum)DeviceInfo.DeviceTypeCode)
            {
                case PwDeviceTypeEnum.Pw1680:
                case PwDeviceTypeEnum.Pw1600:
                case PwDeviceTypeEnum.Pw1650:
                case PwDeviceTypeEnum.Pw1700:
                case PwDeviceTypeEnum.Pw1610:
                case PwDeviceTypeEnum.Pw1660:
                    DeleteUserRecord(userInfos.Select(row => row.EmployeeNumber).ToList());
                    break;
                default:
                    {
                        var fileContentBuilder = new StringBuilder();
                        var sortedUserInfo = userInfos.OrderBy(row => row.EmployeeNumber).ToList();
                        foreach (var user in sortedUserInfo)
                        {
                            fileContentBuilder.AppendLine(user.EmployeeNumber.ToString("D10"));
                        }
                        var validInvalidFilePath = Path.Combine(_filePathUser, "FCARDS_O.TXT");
                        if (!Directory.Exists(_filePathUser))
                        {
                            Directory.CreateDirectory(_filePathUser);
                        }
                        File.WriteAllText(validInvalidFilePath, fileContentBuilder.ToString());
                        if (!File.Exists(validInvalidFilePath))
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwFileNotExist);
                        Thread.Sleep(_sleepTimeForReadout);
                        var result = _xDll.sendAllowedCards(_cbf, validInvalidFilePath, DeviceInfo.PwAcceptValidList);
                        ThrowErrorIfRequired(DeviceInfo, result);
                    }
                    break;
            }


        }

        public int GetFingerCount()
        {
            var records = new uint[10000];
            ushort numberOfRecords = 0;
            var result = _xDll.get_pw1410_templates_list(_cbf, ref numberOfRecords, ref records);
            ThrowErrorIfRequired(DeviceInfo, result);
            return numberOfRecords;
        }


        private void SetEmployees(List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            var sortedUserInfo = userInfos.OrderBy(row => row.EmployeeNumber).ToList();

            var sb = new StringBuilder();
            foreach (var userInfo in sortedUserInfo)
            {
                var sbTempOneRecord = new StringBuilder();
                sbTempOneRecord.AppendLine($"*{userInfo.UserName}*");
                var s = $"{userInfo.EmployeeNumber:D10} {userInfo.VerificationStyle:D3} {0:D6} {(userInfo.Password.IsNotNullOrEmpty() ? userInfo.Password.ToInt32() : 0):D5} 255 255 000 001 000 000 001";
                sbTempOneRecord.AppendLine(s);
                sb.AppendLine(sbTempOneRecord.ToString());
            }
            if (sb.Length == 0)
            {
                return;
            }

            var fileName = Path.Combine(_filePathUser, PersonnelFileName);
            if (!Directory.Exists(_filePathUser))
            {
                Directory.CreateDirectory(_filePathUser);
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var userStrings = sb.ToString();
            File.WriteAllText(fileName, userStrings);
            if (!File.Exists(fileName))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwFileNotExist);
            }
            _cbf.persRecOper = 2;
            const byte id = 1;
            ushort updated = 0;
            ushort inserted = 0;
            uint records = 0;
            string errorString = string.Empty;
            var result = _xDll.send_pers(_cbf, _filePathUser + "\\", ref records, id, ref inserted, ref updated, ref errorString);
            ThrowErrorIfRequired(DeviceInfo, result);
        }

        private void SetAdmins(List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            var sortedUserInfo = userInfos.OrderBy(row => row.EmployeeNumber).ToList();
            if (userInfos.IsCollectionNullOrEmpty()) return;
            var sb = new StringBuilder();
            foreach (var userInfo in sortedUserInfo)
            {
                if (userInfo.Privilege == (int)SupremaDevicePrivilegeEnumeration.Administrator)
                {
                    sb.AppendLine($"{userInfo.EmployeeNumber:D10} {userInfo.VerificationStyle:D3} {000:D3} {(userInfo.Password.IsNotNullOrEmpty() ? userInfo.Password.ToInt32() : 0):D5} 254 255 255 255 255 255 007 000 000 000 000 247");
                }
            }
            if (sb.Length == 0)
            {
                return;
            }
            var fileName = Path.Combine(_filePathUser, AdminsUsersFileName);
            if (!Directory.Exists(_filePathUser))
            {
                Directory.CreateDirectory(_filePathUser);
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, sb.ToString());
            if (!File.Exists(fileName))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPwFileNotExist);
            }
            uint records = 0;
            var result = _xDll.send_users(_cbf, _filePathUser + "\\", ref records, 1);
            ThrowErrorIfRequired(DeviceInfo, result);

        }

        #endregion

        #endregion


        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static string GetPwDriverPath(PwFileType fileType)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            var exe = processModule.FileName;
            var pwPath = Path.Combine(Path.GetDirectoryName(exe) ?? string.Empty, "PW", "xlink");
            if (!Directory.Exists(pwPath))
            {
                Directory.CreateDirectory(pwPath);
            }

            var targetPath = pwPath;
            switch (fileType)
            {
                case PwFileType.Bones:
                    targetPath = Path.Combine(pwPath, fileType.ToString());
                    break;
                case PwFileType.FingerPrint:
                    targetPath = Path.Combine(pwPath, fileType.ToString());
                    break;
                case PwFileType.User:
                    targetPath = Path.Combine(pwPath, "pers");
                    break;

            }
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            return targetPath;
        }

        private static void ThrowErrorIfRequired(DtoCommunicationDeviceData deviceInfo, int errorCode)
        {
            var resultOfLogCount = (PwErrorEnum)errorCode;
            if (resultOfLogCount != PwErrorEnum.LINK_OK)
            {
                throw new OperationCannotBeDoneException(DeviceSharedHelperMethods.MapToOperationResult(errorCode, deviceInfo));
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
                // Managed/COM teardown only on explicit Dispose — never on the finalizer thread.
                if (_isDeviceConnected)
                {
                    Disconnect();
                }
                _xDll = null;
            }
            _disposed = true;
        }

        #endregion


    }
}