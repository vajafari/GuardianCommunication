using AccessControl.TimeHandling;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GuardianCommunication.Hardware.Timy
{
    public static class TimyPushCommands
    {
        public static List<DeviceCommandTypeEnumeration> GetDefineAndDeleteUserCommandTypes()
        {
            return new List<DeviceCommandTypeEnumeration>
            {
                DeviceCommandTypeEnumeration.SetUserInfo,
                DeviceCommandTypeEnumeration.SetFinger,
                DeviceCommandTypeEnumeration.SetFace,
                DeviceCommandTypeEnumeration.DeleteUser,
                DeviceCommandTypeEnumeration.SetPalm,
            };
        }


        // Control
        public static DtoDeviceCommand GetRebootCommand(DtoDevice deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "reboot" }),// -- "{\"cmd\":\"reboot\"}",
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetUnlockDoorCommand
        (DtoDevice deviceInfo,
            int doorNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "opendoor", doornum = doorNumber }),  // "{\"cmd\":\"opendoor\",\"doornum\":1}",
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetUnlockLockerDoorCommand
        (DtoDevice deviceInfo,
            int doorNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "opendoor", racknum = doorNumber }),  // "{\"cmd\":\"opendoor\",\"doornum\":1}",
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }


        public static List<DtoDeviceCommand> GetEnrollUserCommands(
            DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo1,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var userInfoProcessed = userInfo1.WithDeviceLocalDates(deviceInfo);

            var result = new List<DtoDeviceCommand>();
            var password = userInfoProcessed.Password.IsNotNullOrEmpty() ? userInfoProcessed.Password : string.Empty;
            var rfCardNumber = userInfoProcessed.RfCardNumbers.IsCollectionNotNullOrEmpty() ? userInfoProcessed.RfCardNumbers[0] : string.Empty;
            var userFullName = userInfoProcessed.UserName; //  userInfoProcessed.UserName;

            // ReSharper disable PossibleInvalidOperationException
            var startDate = userInfoProcessed.StartDateTime.Value.ToString("yyyy-MM-dd HH:mm");
            var endDate = userInfoProcessed.EndDateTime.Value.ToString("yyyy-MM-dd HH:mm");
            // ReSharper restore PossibleInvalidOperationException

            result.Add(new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setuserinfo",
                    enrollid = userInfoProcessed.UserIdOnDevice,
                    name = userFullName,
                    starttime = startDate,
                    endtime = endDate
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber,
                RetryCount = 0,
                UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            });



            if (rfCardNumber.IsNotNullOrEmpty() && deviceInfo.HasRfReader)
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "setuserinfo",
                        enrollid = userInfoProcessed.UserIdOnDevice,
                        name = userFullName,
                        backupnum = 11,
                        admin = userInfoProcessed.Privilege,
                        record = rfCardNumber
                    }), // "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfoProcessed.UserIdOnDevice + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 11 + ",\"admin\":" + userInfoProcessed.Privilege + ",\"record\":" + rfCardNumber + "}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseTime = null,
                    ResponseValue = null,
                    SendTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                });
            }

            if (password.IsNotNullOrEmpty())
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "setuserinfo",
                        enrollid = userInfoProcessed.UserIdOnDevice,
                        name = userFullName,
                        backupnum = 10,
                        admin = userInfoProcessed.Privilege,
                        record = password.ToInt32()
                    }),  // "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfoProcessed.UserIdOnDevice + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 10 + ",\"admin\":" + userInfoProcessed.Privilege + ",\"record\":" + password + "}"
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseTime = null,
                    ResponseValue = null,
                    SendTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                });
            }


            if (userInfoProcessed.FingerDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasFingerPrint)
            {
                foreach (var fingerData in userInfoProcessed.FingerDataList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new
                        {
                            cmd = "setuserinfo",
                            enrollid = userInfoProcessed.UserIdOnDevice,
                            name = userFullName,
                            backupnum = fingerData.FingerIndex,
                            admin = userInfoProcessed.Privilege,
                            record = Encoding.UTF8.GetString(fingerData.TemplateData)
                        }),// "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfoProcessed.UserIdOnDevice + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + fingerData.FingerIndex + ",\"admin\":" + userInfoProcessed.Privilege + ",\"record\":" + TimyHelpers.ConvertBytesToString(fingerData.TemplateData) + "}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetFinger,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            if (deviceInfo.HasVisiblelight)
            {
                if (userInfoProcessed.VisibleLightImage.IsCollectionNotNullOrEmpty())
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfoProcessed.UserIdOnDevice, name = userFullName, backupnum = 50, admin = userInfoProcessed.Privilege, record = Convert.ToBase64String(userInfoProcessed.VisibleLightImage) }), //"{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfoProcessed.UserIdOnDevice + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 50 + ",\"admin\":" + userInfoProcessed.Privilege + ",\"record\":" + Convert.ToBase64String(userInfoProcessed.VisibleLightImage) + "}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        CommandType = DeviceCommandTypeEnumeration.SetFace,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }

            }
            else
            {
                if (userInfoProcessed.FaceDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasFace)
                {
                    foreach (var faceData in userInfoProcessed.FaceDataList)
                    {
                        result.Add(new DtoDeviceCommand
                        {
                            CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfoProcessed.UserIdOnDevice, name = userFullName, backupnum = (20 + faceData.FaceIndex), admin = userInfoProcessed.Privilege, record = Convert.ToBase64String(faceData.TemplateData) }), //  "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfoProcessed.UserIdOnDevice + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + (20 + faceData.FaceIndex) + ",\"admin\":" + userInfoProcessed.Privilege + ",\"record\":" + TimyHelpers.ConvertBytesToString(faceData.TemplateData) + "}",
                            CommitTime = DateTime.Now,
                            DeviceSerialNumber = deviceInfo.SerialNumber,
                            RetryCount = 0,
                            UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                            CommandType = DeviceCommandTypeEnumeration.SetFace,
                            Priority = priority ?? CommandPriorityEnumeration.Medium,
                            MaxRetry = maxRetry,
                            ResponseValue = null,
                            SendTime = null,
                            ResponseTime = null,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerNumber,
                            SdkVersion = deviceInfo.SdkVersion,
                            VisiblilityTime = visibilityTime?.ToUniversalTime(),
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }

            }

            if (userInfoProcessed.PalmDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasPalm)
            {
                foreach (var palmData in userInfoProcessed.PalmDataList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfoProcessed.UserIdOnDevice, name = userFullName, backupnum = palmData.Index, admin = userInfoProcessed.Privilege, record = Convert.ToBase64String(palmData.TemplateData) }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            if (deviceInfo.ModuleId.HasFlag(ModuleEnumeration.Elevator)
              && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Elevator)
              && userInfoProcessed.ElevatorInfoInJsonFormat.IsNotNullOrEmpty())
            {

                var elevatorFloorNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoProcessed.ElevatorInfoInJsonFormat);
                if (elevatorFloorNumbers.IsCollectionNotNullOrEmpty())
                {
                    //{ "userid":123456,"username":"ABC","verifymode":"face","inout":0,"datatime":"2022/08/16 16:53","floor":2,"event":0,"userprofile":"1,3,5,7,9"}\ncrc16: 0x0f2c
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new
                        {
                            cmd = "setuserprofile",
                            enrollid = userInfoProcessed.UserIdOnDevice,
                            profile = elevatorFloorNumbers.JoinWithComma(),
                        }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            if (deviceInfo.ModuleId.HasFlag(ModuleEnumeration.Cabinet)
                && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Cabinet)
                && userInfoProcessed.CabinetInfoInJsonFormat.IsNotNullOrEmpty()
                && userInfoProcessed.UserIdOnDevice <= int.MaxValue)
            {
                var cabinetNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfoProcessed.CabinetInfoInJsonFormat);
                if (cabinetNumbers.IsCollectionNotNullOrEmpty())
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new
                        {
                            cmd = "setuserlock",
                            count = cabinetNumbers.Length,
                            record = cabinetNumbers.Select(cb => new
                            {
                                enrollid = userInfoProcessed.UserIdOnDevice,
                                weekzone = 1,
                                weekzone2 = 1,
                                weekzone3 = 1,
                                weekzone4 = 1,
                                group = cb,
                                starttime = startDate,
                                endtime = endDate
                            }).ToList()
                        }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            result.Add(new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "enableuser",
                    enrollid = userInfoProcessed.UserIdOnDevice,
                    enflag = userInfoProcessed.IsEnable ? 1 : 0
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber,
                RetryCount = 0,
                UserIdOnDevice = userInfoProcessed.UserIdOnDevice,
                CommandType = DeviceCommandTypeEnumeration.SetPalm,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            });
            //if (userInfo.HardwareProfileImage.IsCollectionNotNullOrEmpty() && deviceInfo.SendProfileImage)
            //{
            //    result.Add(new DtoDeviceCommand
            //    {
            //        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfo.UserIdOnDevice, name = userFullName, backupnum = 10, admin = userInfo.Privilege, record = TimyHelpers.ConvertBytesToString(userInfo.HardwareProfileImage) }),
            //        CommitTime = DateTime.Now,
            //        DeviceSerialNumber = deviceInfo.SerialNumber,
            //        RetryCount = 0,
            //        UserIdOnDevice = userInfo.UserIdOnDevice,
            //        CommandType = DeviceCommandTypeEnumeration.SetPalm,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerNumber,
            //        SdkVersion = deviceInfo.SdkVersion,
            //        VisiblilityTime = visibilityTime?.ToUniversalTime(),
            //        CommandIdentifier = commandIdentifier
            //    });
            //}
            return result;
        }

        public static List<DtoDeviceCommand> GetDeleteUserCommands(
            DtoDevice deviceInfo,
            long employeeNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            // Delete all fingers and card and passwords

            var result = new List<DtoDeviceCommand>
            {
                new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "deleteuser", enrollid = employeeNumber, backupnum = 13 }), // "{\"cmd\":\"deleteuser\",\"enrollid\":" + employeeNumber + ",\"backupnum\":" + PackupNumbers[i] + "}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    UserIdOnDevice = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                }
            };
            return result;
            //var result = new List<DtoDeviceCommand>
            //{
            //    new DtoDeviceCommand
            //    {
            //        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "deleteuser", enrollid = employeeNumber, backupnum = 12 }), // "{\"cmd\":\"deleteuser\",\"enrollid\":" + employeeNumber + ",\"backupnum\":" + PackupNumbers[i] + "}",
            //        CommitTime = DateTime.Now,
            //        DeviceSerialNumber = deviceInfo.SerialNumber,
            //        RetryCount = 0,
            //        UserIdOnDevice = employeeNumber,
            //        CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerNumber,
            //        SdkVersion = deviceInfo.SdkVersion,
            //        VisiblilityTime = visibilityTime?.ToUniversalTime(),
            //        CommandIdentifier = commandIdentifier
            //    }
            //};
            //for (var i = 20; i < 28; i++)
            //{
            //    result.Add(new DtoDeviceCommand
            //    {
            //        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "deleteuser", enrollid = employeeNumber, backupnum = i }), // "{\"cmd\":\"deleteuser\",\"enrollid\":" + employeeNumber + ",\"backupnum\":" + PackupNumbers[i] + "}",
            //        CommitTime = DateTime.Now,
            //        DeviceSerialNumber = deviceInfo.SerialNumber,
            //        RetryCount = 0,
            //        UserIdOnDevice = employeeNumber,
            //        CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerNumber,
            //        SdkVersion = deviceInfo.SdkVersion,
            //        VisiblilityTime = visibilityTime?.ToUniversalTime(),
            //        CommandIdentifier = commandIdentifier
            //    });
            //}

            //result.Add(new DtoDeviceCommand
            //{
            //    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "deleteuser", enrollid = employeeNumber, backupnum = 50 }), // "{\"cmd\":\"deleteuser\",\"enrollid\":" + employeeNumber + ",\"backupnum\":" + PackupNumbers[i] + "}",
            //    CommitTime = DateTime.Now,
            //    DeviceSerialNumber = deviceInfo.SerialNumber,
            //    RetryCount = 0,
            //    UserIdOnDevice = employeeNumber,
            //    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //    Priority = priority ?? CommandPriorityEnumeration.Medium,
            //    MaxRetry = maxRetry,
            //    ResponseValue = null,
            //    SendTime = null,
            //    ResponseTime = null,
            //    DeviceNumber = deviceInfo.DeviceNumber,
            //    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
            //    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //    ProducerNumber = deviceInfo.ProducerNumber,
            //    SdkVersion = deviceInfo.SdkVersion,
            //    VisiblilityTime = visibilityTime?.ToUniversalTime(),
            //    CommandIdentifier = commandIdentifier
            //});

            //return result;
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
            var timeService = new DeviceTimeService();
            var startDateProcessed = timeService.UtcToDeviceTime
                (startDate.ToUniversalTime(), deviceInfo.IanaTimeZoneId);
            var endDateProcessed = timeService.UtcToDeviceTime
                (endDate.ToUniversalTime(), deviceInfo.IanaTimeZoneId);

            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getalllog", stn = true, from = startDateProcessed.ToString("yyyy-MM-dd"), to = endDateProcessed.ToString("yyyy-MM-dd") }),   // "{\"cmd\":\"getalllog\",\"stn\":true,\"from\":\"" + startDate.ToString("yyyy-MM-dd") + "\",\"to\":\"" + endDate.ToString("yyyy - MM - dd") + "\"}",
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerNumber,
                SdkVersion = deviceInfo.SdkVersion,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
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
            var result = new List<DtoDeviceCommand>
            {
                new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new {cmd = "getuserinfo", enrollid=employeeNumber, backupnum = 10 }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    UserIdOnDevice = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                    Priority = CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    CommandIdentifier = commandIdentifier
                },
                new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 11 }),
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
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                }
            };

            if (templateType.HasFlag(TemplateTypeEnumeration.Face) && deviceInfo.HasFace)
            {
                if (deviceInfo.HasVisiblelight)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 50 }),
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
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
                else
                {
                    for (var i = 20; i < 28; i++)
                    {
                        result.Add(new DtoDeviceCommand
                        {
                            CommandContent = ObjectHelper.SerializeAsJson(new
                            { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = i }),
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
                            Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerNumber,
                            SdkVersion = deviceInfo.SdkVersion,
                            VisiblilityTime = visibilityTime?.ToUniversalTime(),
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }
            }
            if (templateType.HasFlag(TemplateTypeEnumeration.FingerPrint) && deviceInfo.HasFingerPrint)
            {
                for (var i = 0; i < 10; i++)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = i }),
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
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            if (templateType.HasFlag(TemplateTypeEnumeration.Palm) && deviceInfo.HasPalm)
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 40 }),
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
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                });
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 41 }),
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
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                });
            }

            return result;
        }

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
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "cleanlog" }),
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
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier,
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "cleanuser" }),
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
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }


        public static DtoDeviceCommand GetScanFaceCommand
            (DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasFace || deviceInfo.HasVisiblelight)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.UserIdOnDevice,
                        backupnum = deviceInfo.HasVisiblelight ? 50 : 20,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = userInfo.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;
        }

        public static DtoDeviceCommand GetScanCardCommand
            (DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasRfReader)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.UserIdOnDevice,
                        backupnum = 11,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = userInfo.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;

        }

        public static DtoDeviceCommand GetScanFingerCommand
            (DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int fingerIndex,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasFace || deviceInfo.HasFingerPrint)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.UserIdOnDevice,
                        backupnum = fingerIndex,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    UserIdOnDevice = userInfo.UserIdOnDevice,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerNumber,
                    SdkVersion = deviceInfo.SdkVersion,
                    VisiblilityTime = visibilityTime?.ToUniversalTime(),
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;
        }


    }
}
