using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Hardware.Zk
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class ZkPushCommands
    {

        public static List<DeviceCommandTypeEnumeration> GetDefineAndDeleteUserCommandTypes()
        {
            return new List<DeviceCommandTypeEnumeration>
            {
                DeviceCommandTypeEnumeration.SetUserInfo,
                DeviceCommandTypeEnumeration.SetFinger,
                DeviceCommandTypeEnumeration.SetFace,
                DeviceCommandTypeEnumeration.SetPalm,
                DeviceCommandTypeEnumeration.SetPhoto,
                DeviceCommandTypeEnumeration.DeleteUser,
            };
        }

        //Control
        private const string Command_ControlReboot = "REBOOT";
        public static DtoDeviceCommand GetRebootCommand
            (DtoDevice deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = Command_ControlReboot,
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.Reboot,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        private const string Command_ControlUnLock = "AC_UNLOCK";
        public static DtoDeviceCommand GetUnlockDoorCommand
            (DtoDevice deviceInfo,
             int maxRetry,
             int? deadline,
             DateTime? visibilityTime,
             CommandPriorityEnumeration? priority,
             Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = Command_ControlUnLock,
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.Unlock,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }


        public static List<DtoDeviceCommand> GetEnrollUserCommands(
            DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var result = new List<DtoDeviceCommand>();
            var password = userInfo.Password.IsNotNullOrEmpty() ? userInfo.Password : string.Empty;
            var rfCardNumber = userInfo.RfCardNumbers.IsCollectionNotNullOrEmpty() ? userInfo.RfCardNumbers[0] : string.Empty;
            var verificationStyle = string.Empty;
            if (userInfo.IsEnable)
            {
                var mappedVerification = ZkCommunicationHelpers.MapSdkVerificationStyleToPush((ZkVerificationStyleEnumeration)userInfo.VerificationStyle);
                if (mappedVerification != ZkPushVerificationStyleEnumeration.GroupVerify)
                {
                    verificationStyle = $"\tVerify={(int)mappedVerification}";
                }
            }
            else
            {
                var mappedVerification = ZkCommunicationHelpers.MapSdkVerificationStyleToPush(ZkVerificationStyleEnumeration.Pin);
                if (mappedVerification != ZkPushVerificationStyleEnumeration.GroupVerify)
                {
                    verificationStyle = $"\tVerify={(int)mappedVerification}";
                }
            }


            var endTime = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime, ProducerEnumeration.Zk, SdkVersionEnumeration.SdkVersion1);

            result.Add(new DtoDeviceCommand
            {
                CommandContent = $"DATA UPDATE USERINFO PIN={userInfo.UserIdOnDevice}\tName={userInfo.UserName}\tPri={userInfo.Privilege}\tPasswd={(userInfo.IsEnable ? password : ZkUtils.ZkForbiddenPassword)}\tCard={rfCardNumber}\tGrp={0}\tTZ={0}{verificationStyle}\tStartDatetime={userInfo.StartTime:yyyy-MM-dd}\tEndDatetime={endTime:yyyy-MM-dd}\tViceCard=\tUserValidTimeFun=1",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = userInfo.UserIdOnDevice,
                CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            });



            if (deviceInfo.HasFingerPrint && userInfo.FingerDataList.IsCollectionNotNullOrEmpty())
            {
                foreach (var fingerData in userInfo.FingerDataList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent =
                            $"DATA UPDATE FINGERTMP PIN={userInfo.UserIdOnDevice}\tFID={fingerData.FingerIndex}\tSize={fingerData.TemplateData.Length}\tValid={1}\tTMP={Encoding.UTF8.GetString(fingerData.TemplateData)}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        UserIdOnDevice = userInfo.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetFinger,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            if (deviceInfo.HasVisiblelight)
            {
                if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent =
                            $"DATA UPDATE BIOPHOTO PIN={userInfo.UserIdOnDevice}\tType={9}\tSize={userInfo.VisibleLightImage.Length}\tContent={Convert.ToBase64String(userInfo.VisibleLightImage)}\tFormat={0}\tUrl={string.Empty}\tPostBackTmpFlag={0}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        UserIdOnDevice = userInfo.UserIdOnDevice,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        CommandType = DeviceCommandTypeEnumeration.SetFace,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            else
            {
                if (deviceInfo.HasFace && userInfo.FaceDataList.IsCollectionNotNullOrEmpty())
                {
                    foreach (var faceData in userInfo.FaceDataList)
                    {
                        result.Add(new DtoDeviceCommand
                        {
                            CommandContent =
                                $"DATA UPDATE FACE PIN={userInfo.UserIdOnDevice}\tFID={50}\tValid={1}\tSize={faceData.Length}\tTMP={Encoding.UTF8.GetString(faceData.TemplateData)}",
                            CommitTime = DateTime.Now,
                            DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                            RetryCount = 0,
                            UserIdOnDevice = userInfo.UserIdOnDevice,
                            CommandType = DeviceCommandTypeEnumeration.SetFace,
                            Priority = priority ?? CommandPriorityEnumeration.Medium,
                            MaxRetry = maxRetry,
                            ResponseValue = null,
                            SendTime = null,
                            ResponseTime = null,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerNumber,
                            SdkVersion = deviceInfo.SdkVersion,
                            VisiblilityTime = visibilityTime,
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }
            }

            if (deviceInfo.HasPalm && userInfo.PalmDataList.IsCollectionNotNullOrEmpty())
            {
                var orderedList = userInfo.PalmDataList.OrderBy(row => row.Index).ToList();
                foreach (var palm in orderedList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = $"DATA UPDATE BIODATA Pin={userInfo.UserIdOnDevice}\tNo={0}\tIndex={palm.Index}\tValid={1}\tDuress={0}\tType={8}\tMajorVer={12}\tMinorVer ={0}\tFormat={0}\tTmp={Convert.ToBase64String(palm.TemplateData)}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        UserIdOnDevice = userInfo.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            if (deviceInfo.DeviceSettings != null
                && deviceInfo.DeviceSettings.IsSendProfileImageActive
                && userInfo.HardwareProfileImage.IsCollectionNotNullOrEmpty())
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent =
                        $"DATA UPDATE USERPIC PIN={userInfo.UserIdOnDevice}\tSize={userInfo.HardwareProfileImage.Length}\tContent={Convert.ToBase64String(userInfo.HardwareProfileImage)}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = userInfo.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.SetPhoto,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
            }

            return result;
        }


        //Delete
        public static DtoDeviceCommand GetDeleteUserCommands(
            DtoDevice deviceInfo,
            long employeeNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = $"DATA DELETE USERINFO PIN={employeeNumber}",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }


        public static DtoDeviceCommand GetReadoutFromDeviceCommand
            (DtoDevice deviceInfo,
            DateTime startDate,
            DateTime endDate,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = $"DATA QUERY ATTLOG StartTime={startDate:yyyy-MM-dd HH:mm:ss}\tEndTime={endDate:yyyy-MM-dd HH:mm:ss}",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.ReadoutAttendance,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }


        public static List<DtoDeviceCommand> GetUserInfoCommand
            (DtoDevice deviceInfo,
            long employeeNumber,
            TemplateTypeEnumeration templateType,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var result = new List<DtoDeviceCommand>();
            if (templateType.HasFlag(TemplateTypeEnumeration.Face))
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = $"DATA QUERY USERINFO PIN={employeeNumber}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ReadUser,
                    Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
            }
            else
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = $"DATA QUERY USERINFO PIN={employeeNumber}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ReadUser,
                    Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
                if (templateType.HasFlag(TemplateTypeEnumeration.FingerPrint))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        result.Add(new DtoDeviceCommand
                        {
                            CommandContent = $"DATA QUERY FINGERTMP PIN={employeeNumber}\tFID={i}",
                            CommitTime = DateTime.Now,
                            DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                            RetryCount = 0,
                            UserIdOnDevice = employeeNumber,
                            CommandType = DeviceCommandTypeEnumeration.ReadFingerPrint,
                            Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                            MaxRetry = maxRetry,
                            ResponseValue = null,
                            SendTime = null,
                            ResponseTime = null,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerNumber,
                            SdkVersion = deviceInfo.SdkVersion,
                            VisiblilityTime = visibilityTime,
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }
            }

            return result;
        }

        //Cancel Operation
        private const string Command_CancelOperation = "CANCEL OPERATION";

        public static DtoDeviceCommand GetClearDataCommand
            (DtoDevice deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = Command_CancelOperation,
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                CommandType = DeviceCommandTypeEnumeration.CancelOperation,
                Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }


        //Clear
        private const string Command_ClearLog = "CLEAR LOG";
        private const string Command_ClearData = "CLEAR DATA";

        public static DtoDeviceCommand GetClearDataCommand
            (DtoDevice deviceInfo,
            DeviceLogTypeEnumeration logType,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            switch (logType)
            {
                case DeviceLogTypeEnumeration.Attendance:
                    return new DtoDeviceCommand
                    {
                        CommandContent = Command_ClearLog,
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        CommandType = DeviceCommandTypeEnumeration.ClearData,
                        Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = Command_ClearData,
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        CommandType = DeviceCommandTypeEnumeration.ClearUser,
                        Priority = CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

        }

        //Check
        private const string Command_Check = "CHECK";
        public static DtoDeviceCommand GetCheckCommand
            (DtoDevice deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = Command_Check,
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                CommandType = DeviceCommandTypeEnumeration.Check,
                Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }


        public static DtoDeviceCommand GetScanFaceCommand
            (DtoDevice deviceInfo,
            long employeeNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = $"ENROLL_FP PIN={employeeNumber}\tFID={111}\tRETRY={2}\tOVERWRITE={3}",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanFace,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetScanFingerCommand
            (DtoDevice deviceInfo,
            long employeeNumber,
            int fingerIndex,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = $"ENROLL_FP PIN={employeeNumber}\tFID={fingerIndex}\tRETRY={3}\tOVERWRITE={1}",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanFinger,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetDeviceStatisticsCommand
        (DtoDevice deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "INFO",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.ScanFinger,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

    }
}
