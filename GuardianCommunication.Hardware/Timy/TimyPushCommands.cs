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
        public static DtoDeviceCommand GetRebootCommand(DtoCommunicationDeviceData deviceInfo,
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
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.Reboot,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetUnlockDoorCommand
        (DtoCommunicationDeviceData deviceInfo,
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
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.Unlock,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetUnlockLockerDoorCommand
        (DtoCommunicationDeviceData deviceInfo,
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
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.Unlock,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }


        public static List<DtoDeviceCommand> GetEnrollUserCommands(
            DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
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
            var userFullName = userInfo.UserName; //  userInfo.UserName;
            var startDate = userInfo.StartTime.ToString("yyyy-MM-dd HH:mm");
            string endDate;
            if (!userInfo.EndTime.HasValue)
            {
                endDate = userInfo.StartTime.AddYears(30).ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                // یعنی ساعت پایان وجود ندارد و تا انتهای روز باید در نظر گرفته شود
                endDate = userInfo.EndTime == userInfo.EndTime.Value.Date
                    ? userInfo.EndTime.Value.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm")
                    : userInfo.EndTime.Value.ToString("yyyy-MM-dd HH:mm");
            }
            result.Add(new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setuserinfo",
                    enrollid = userInfo.EmployeeNumber,
                    name = userFullName,
                    //verifymode = 0,
                    //card = rfCardNumber.IsNotNullOrEmpty() && deviceInfo.HasRfCard ? rfCardNumber : null,
                    //pwd = password.IsNotNullOrEmpty() ? password : null,
                    //shiftid = 1,
                    //zoneid = 1,
                    //groupid = 1,
                    starttime = startDate,
                    endtime = endDate
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber,
                RetryCount = 0,
                EmployeeNumber = userInfo.EmployeeNumber,
                CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            });



            if (rfCardNumber.IsNotNullOrEmpty() && deviceInfo.HasRfCard)
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "setuserinfo",
                        enrollid = userInfo.EmployeeNumber,
                        name = userFullName,
                        backupnum = 11,
                        admin = userInfo.Privilege,
                        record = rfCardNumber
                    }), // "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfo.EmployeeNumber + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 11 + ",\"admin\":" + userInfo.Privilege + ",\"record\":" + rfCardNumber + "}",
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseTime = null,
                    ResponseValue = null,
                    SendTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
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
                        enrollid = userInfo.EmployeeNumber,
                        name = userFullName,
                        backupnum = 10,
                        admin = userInfo.Privilege,
                        record = password.ToInt32()
                    }),  // "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfo.EmployeeNumber + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 10 + ",\"admin\":" + userInfo.Privilege + ",\"record\":" + password + "}"
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber,
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseTime = null,
                    ResponseValue = null,
                    SendTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
            }


            if (userInfo.FingerDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasFinger)
            {
                foreach (var fingerData in userInfo.FingerDataList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new
                        {
                            cmd = "setuserinfo",
                            enrollid = userInfo.EmployeeNumber,
                            name = userFullName,
                            backupnum = fingerData.FingerIndex,
                            admin = userInfo.Privilege,
                            record = Encoding.UTF8.GetString(fingerData.TemplateData)
                        }),// "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfo.EmployeeNumber + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + fingerData.FingerIndex + ",\"admin\":" + userInfo.Privilege + ",\"record\":" + TimyHelpers.ConvertBytesToString(fingerData.TemplateData) + "}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        EmployeeNumber = userInfo.EmployeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.SetFinger,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            if (deviceInfo.HasVisibleLight)
            {
                if (userInfo.VisibleLightImage.IsCollectionNotNullOrEmpty())
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfo.EmployeeNumber, name = userFullName, backupnum = 50, admin = userInfo.Privilege, record = Convert.ToBase64String(userInfo.VisibleLightImage) }), //"{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfo.EmployeeNumber + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + 50 + ",\"admin\":" + userInfo.Privilege + ",\"record\":" + Convert.ToBase64String(userInfo.VisibleLightImage) + "}",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        EmployeeNumber = userInfo.EmployeeNumber,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        CommandType = DeviceCommandTypeEnumeration.SetFace,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }

            }
            else
            {
                if (userInfo.FaceDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasFace)
                {
                    foreach (var faceData in userInfo.FaceDataList)
                    {
                        result.Add(new DtoDeviceCommand
                        {
                            CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfo.EmployeeNumber, name = userFullName, backupnum = (20 + faceData.FaceIndex), admin = userInfo.Privilege, record = Convert.ToBase64String(faceData.TemplateData) }), //  "{\"cmd\":\"setuserinfo\",\"enrollid\":" + userInfo.EmployeeNumber + ",\"name\":\"" + userFullName + "\",\"backupnum\":" + (20 + faceData.FaceIndex) + ",\"admin\":" + userInfo.Privilege + ",\"record\":" + TimyHelpers.ConvertBytesToString(faceData.TemplateData) + "}",
                            CommitTime = DateTime.Now,
                            DeviceSerialNumber = deviceInfo.SerialNumber,
                            RetryCount = 0,
                            EmployeeNumber = userInfo.EmployeeNumber,
                            CommandType = DeviceCommandTypeEnumeration.SetFace,
                            Priority = priority ?? CommandPriorityEnumeration.Medium,
                            MaxRetry = maxRetry,
                            ResponseValue = null,
                            SendTime = null,
                            ResponseTime = null,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerEnum,
                            SdkVersion = deviceInfo.SdkVersionEnum,
                            VisiblilityTime = visibilityTime,
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }

            }

            if (userInfo.PalmDataList.IsCollectionNotNullOrEmpty() && deviceInfo.HasPalm)
            {
                foreach (var palmData in userInfo.PalmDataList)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfo.EmployeeNumber, name = userFullName, backupnum = palmData.Index, admin = userInfo.Privilege, record = Convert.ToBase64String(palmData.TemplateData) }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        EmployeeNumber = userInfo.EmployeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            if (deviceInfo.ApplicationId.HasFlag(ApplicationTypeEnumeration.Elevator)
                && ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Elevator)
                && userInfo.ElevatorInfoInJsonFormat.IsNotNullOrEmpty())
            {
                var elevatorFloorNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfo.ElevatorInfoInJsonFormat);
                if (elevatorFloorNumbers.IsCollectionNotNullOrEmpty())
                {
                    //{ "userid":123456,"username":"ABC","verifymode":"face","inout":0,"datatime":"2022/08/16 16:53","floor":2,"event":0,"userprofile":"1,3,5,7,9"}\ncrc16: 0x0f2c
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new
                        {
                            cmd = "setuserprofile",
                            enrollid = userInfo.EmployeeNumber,
                            profile = elevatorFloorNumbers.JoinWithComma(),
                        }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber,
                        RetryCount = 0,
                        EmployeeNumber = userInfo.EmployeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }
            if (deviceInfo.ApplicationId.HasFlag(ApplicationTypeEnumeration.Cabinet)
                && ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Cabinet)
                && userInfo.CabinetInfoInJsonFormat.IsNotNullOrEmpty()
                && userInfo.EmployeeNumber <= int.MaxValue)
            {
                var cabinetNumbers = ObjectHelper.DeserializeAsJson<int[]>(userInfo.CabinetInfoInJsonFormat);
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
                                enrollid = userInfo.EmployeeNumber,
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
                        EmployeeNumber = userInfo.EmployeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.SetPalm,
                        Priority = priority ?? CommandPriorityEnumeration.Medium,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    });
                }
            }

            result.Add(new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "enableuser",
                    enrollid = userInfo.EmployeeNumber,
                    enflag = userInfo.IsEnable ? 1 : 0
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber,
                RetryCount = 0,
                EmployeeNumber = userInfo.EmployeeNumber,
                CommandType = DeviceCommandTypeEnumeration.SetPalm,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            });
            if (userInfo.TimyWeekTimezoneDeviceIndex.HasValue)
            {
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "setuserlock",
                        count = 1,
                        record = new List<object>
                        {
                            new
                            {
                                enrollid = userInfo.EmployeeNumber,
                                weekzone = userInfo.TimyWeekTimezoneDeviceIndex.Value,
                                weekzone2 = userInfo.TimyWeekTimezoneDeviceIndex.Value,
                                weekzone3 = userInfo.TimyWeekTimezoneDeviceIndex.Value,
                                weekzone4 = userInfo.TimyWeekTimezoneDeviceIndex.Value,
                                group = 0,
                                starttime = startDate,
                                endtime = endDate
                            }
                        }
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.TimySetWeekTimezone,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
            }
            //if (userInfo.HardwareProfileImage.IsCollectionNotNullOrEmpty() && deviceInfo.SendProfileImage)
            //{
            //    result.Add(new DtoDeviceCommand
            //    {
            //        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "setuserinfo", enrollid = userInfo.EmployeeNumber, name = userFullName, backupnum = 10, admin = userInfo.Privilege, record = TimyHelpers.ConvertBytesToString(userInfo.HardwareProfileImage) }),
            //        CommitTime = DateTime.Now,
            //        DeviceSerialNumber = deviceInfo.SerialNumber,
            //        RetryCount = 0,
            //        EmployeeNumber = userInfo.EmployeeNumber,
            //        CommandType = DeviceCommandTypeEnumeration.SetPalm,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerEnum,
            //        SdkVersion = deviceInfo.SdkVersionEnum,
            //        VisiblilityTime = visibilityTime,
            //        CommandIdentifier = commandIdentifier
            //    });
            //}
            return result;
        }

        public static List<DtoDeviceCommand> GetDeleteUserCommands(
            DtoCommunicationDeviceData deviceInfo,
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
                    EmployeeNumber = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                    Priority = priority ?? CommandPriorityEnumeration.Medium,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
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
            //        EmployeeNumber = employeeNumber,
            //        CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerEnum,
            //        SdkVersion = deviceInfo.SdkVersionEnum,
            //        VisiblilityTime = visibilityTime,
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
            //        EmployeeNumber = employeeNumber,
            //        CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //        Priority = priority ?? CommandPriorityEnumeration.Medium,
            //        MaxRetry = maxRetry,
            //        ResponseValue = null,
            //        SendTime = null,
            //        ResponseTime = null,
            //        DeviceNumber = deviceInfo.DeviceNumber,
            //        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
            //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //        ProducerNumber = deviceInfo.ProducerEnum,
            //        SdkVersion = deviceInfo.SdkVersionEnum,
            //        VisiblilityTime = visibilityTime,
            //        CommandIdentifier = commandIdentifier
            //    });
            //}

            //result.Add(new DtoDeviceCommand
            //{
            //    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "deleteuser", enrollid = employeeNumber, backupnum = 50 }), // "{\"cmd\":\"deleteuser\",\"enrollid\":" + employeeNumber + ",\"backupnum\":" + PackupNumbers[i] + "}",
            //    CommitTime = DateTime.Now,
            //    DeviceSerialNumber = deviceInfo.SerialNumber,
            //    RetryCount = 0,
            //    EmployeeNumber = employeeNumber,
            //    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
            //    Priority = priority ?? CommandPriorityEnumeration.Medium,
            //    MaxRetry = maxRetry,
            //    ResponseValue = null,
            //    SendTime = null,
            //    ResponseTime = null,
            //    DeviceNumber = deviceInfo.DeviceNumber,
            //    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
            //    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
            //    ProducerNumber = deviceInfo.ProducerEnum,
            //    SdkVersion = deviceInfo.SdkVersionEnum,
            //    VisiblilityTime = visibilityTime,
            //    CommandIdentifier = commandIdentifier
            //});

            //return result;
        }

        public static DtoDeviceCommand GetReadoutFromDeviceCommand
        (DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getalllog", stn = true, from = startDate.ToString("yyyy-MM-dd"), to = endDate.ToString("yyyy-MM-dd") }),   // "{\"cmd\":\"getalllog\",\"stn\":true,\"from\":\"" + startDate.ToString("yyyy-MM-dd") + "\",\"to\":\"" + endDate.ToString("yyyy - MM - dd") + "\"}",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.ReadoutAttendance,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static List<DtoDeviceCommand> GetUserInfoCommand
            (DtoCommunicationDeviceData deviceInfo,
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
                    EmployeeNumber = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                    Priority = CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    CommandIdentifier = commandIdentifier
                },
                new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 11 }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ReadUser,
                    Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                }
            };

            if (templateType.HasFlag(TemplateTypeEnumeration.Face) && deviceInfo.HasFace)
            {
                if (deviceInfo.HasVisibleLight)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 50 }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        EmployeeNumber = employeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.ReadUser,
                        Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
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
                            EmployeeNumber = employeeNumber,
                            CommandType = DeviceCommandTypeEnumeration.ReadUser,
                            Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                            MaxRetry = maxRetry,
                            ResponseValue = null,
                            SendTime = null,
                            ResponseTime = null,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                            DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                            ProducerNumber = deviceInfo.ProducerEnum,
                            SdkVersion = deviceInfo.SdkVersionEnum,
                            VisiblilityTime = visibilityTime,
                            CommandIdentifier = commandIdentifier
                        });
                    }
                }
            }
            if (templateType.HasFlag(TemplateTypeEnumeration.FingerPrint) && deviceInfo.HasFinger)
            {
                for (var i = 0; i < 10; i++)
                {
                    result.Add(new DtoDeviceCommand
                    {
                        CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = i }),
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        EmployeeNumber = employeeNumber,
                        CommandType = DeviceCommandTypeEnumeration.ReadUser,
                        Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
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
                    EmployeeNumber = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ReadUser,
                    Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
                result.Add(new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new { cmd = "getuserinfo", enrollid = employeeNumber, backupnum = 41 }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = employeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ReadUser,
                    Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                });
            }

            return result;
        }

        public static DtoDeviceCommand GetClearDataCommand
            (DtoCommunicationDeviceData deviceInfo,
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
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
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
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = deviceInfo.ProducerEnum,
                        SdkVersion = deviceInfo.SdkVersionEnum,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }


        public static DtoDeviceCommand GetScanFaceCommand
            (DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasFace || deviceInfo.HasVisibleLight)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.EmployeeNumber,
                        backupnum = deviceInfo.HasVisibleLight ? 50 : 20,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;
        }

        public static DtoDeviceCommand GetScanCardCommand
            (DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasRfCard)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.EmployeeNumber,
                        backupnum = 11,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;

        }

        public static DtoDeviceCommand GetScanFingerCommand
            (DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int fingerIndex,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (deviceInfo.HasFace || deviceInfo.HasVisibleLight)
            {
                var userFullName = userInfo.UserName;
                return new DtoDeviceCommand
                {
                    CommandContent = ObjectHelper.SerializeAsJson(new
                    {
                        cmd = "adduser",
                        enrollid = userInfo.EmployeeNumber,
                        backupnum = fingerIndex,
                        admin = userInfo.Privilege,
                        name = userFullName,
                        flag = 10
                    }),
                    CommitTime = DateTime.Now,
                    DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                    RetryCount = 0,
                    EmployeeNumber = userInfo.EmployeeNumber,
                    CommandType = DeviceCommandTypeEnumeration.ScanFace,
                    Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                    MaxRetry = maxRetry,
                    ResponseValue = null,
                    SendTime = null,
                    ResponseTime = null,
                    DeviceNumber = deviceInfo.DeviceNumber,
                    Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                    DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                    ProducerNumber = deviceInfo.ProducerEnum,
                    SdkVersion = deviceInfo.SdkVersionEnum,
                    VisiblilityTime = visibilityTime,
                    CommandIdentifier = commandIdentifier
                };
            }

            return null;
        }


        public static DtoDeviceCommand GetDayTimezoneControlCommand
            (DtoCommunicationDeviceData deviceInfo,
            List<DtoTimyDayTimezoneGroup> dayTimezoneGroups,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            if (dayTimezoneGroups.IsCollectionNullOrEmpty())
            {
                return null;
            }
            var sections = new List<object>();
            for (var i = 1; i <= 8; i++)
            {
                var currentDayTimezoneGroups = dayTimezoneGroups.FirstOrDefault(tz => tz.DeviceIndex == i);
                if (currentDayTimezoneGroups == null || currentDayTimezoneGroups.DayTimezoneIntervals.IsCollectionNullOrEmpty())
                {
                    sections.Add(new
                    {
                        day = new[]
                        {
                            new { section = "00:00~00:00" }
                        }
                    });
                }
                else
                {
                    var timezones = currentDayTimezoneGroups.DayTimezoneIntervals
                        .OrderBy(tz => tz.StartHourMinute).ToList();

                    sections.Add(new
                    {
                        day = timezones.Select(tz => new
                        {
                            section = $"{tz.StartHourMinute.FormatIntAsTimeString(":")}~{tz.EndHourMinute.FormatIntAsTimeString(":")}"
                        }).ToArray()
                    });
                }
            }

            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setdevlock",
                    sn = deviceInfo.SerialNumber,
                    dayzone = sections
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.TimySetDayTimezone,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };


        }


        public static DtoDeviceCommand GetWeekTimezoneControlCommand
            (DtoCommunicationDeviceData deviceInfo,
            List<DtoTimyWeekTimezoneGroup> dayTimezoneGroups,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var dayOrder = new[]
            {
                DayOfWeek.Sunday,
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday
            };

            var result = dayTimezoneGroups.OrderBy(g => g.DeviceIndex).Select(g => new
            {
                week = dayOrder.Select(d =>
                {
                    var tz = g.Timezones?.FirstOrDefault(t => t.WeekDay == d);
                    return new { day = tz?.DayTimezoneIndex ?? 0 };
                }).ToList()
            }).ToList();

            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setdevlock",
                    sn = deviceInfo.SerialNumber,
                    weekzone = result
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.TimySetWeekTimezone,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };


        }

        public static DtoDeviceCommand GetUserTimezoneCommand
            (DtoCommunicationDeviceData deviceInfo,
            long userId,
            int dayTimezoneIndex,
            DateTime startDateTime,
            DateTime? endDateTime,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var startDate = startDateTime.ToString("yyyy-MM-dd HH:mm");
            string endDate;
            if (!endDateTime.HasValue)
            {
                endDate = startDateTime.AddYears(30).ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                // یعنی ساعت پایان وجود ندارد و تا انتهای روز باید در نظر گرفته شود
                endDate = endDateTime == endDateTime.Value.Date
                    ? endDateTime.Value.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm")
                    : endDateTime.Value.ToString("yyyy-MM-dd HH:mm");
            }
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setuserlock",
                    count = 1,
                    record = new List<object> {
                        new {
                            enrollid = userId,
                            weekzone = dayTimezoneIndex,
                            weekzone2 = dayTimezoneIndex,
                            weekzone3 = dayTimezoneIndex,
                            weekzone4 = dayTimezoneIndex,
                            group = 0,
                            starttime = startDate,
                            endtime = endDate
                        }
                    }
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.TimySetWeekTimezone,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };


        }

        public static DtoDeviceCommand GetHolidayCommand
            (DtoCommunicationDeviceData deviceInfo,
            List<DtoTimyHoliday> holidays,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new
                {
                    cmd = "setholiday",
                    holidays = holidays.Take(30).Select(hd => new
                    {
                        name = hd.Title,
                        startday = hd.StartDate.ToString("MM-dd"),
                        endday = hd.EndDate.ToString("MM-dd"),
                        shift = 0,
                        dayzone = hd.DayTimezoneIndex,
                    }) 
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.TimySetHoliday,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };


        }

        //private static string Globalization2312Utf8(string text)
        //{
        //    Encoding utf8, gb2312;
        //    //gb2312   
        //    gb2312 = Encoding.GetEncoding("gb2312");
        //    //utf8   
        //    utf8 = Encoding.GetEncoding("utf-8");
        //    byte[] gb;
        //    gb = gb2312.GetBytes(text);
        //    gb = Encoding.Convert(gb2312, utf8, gb);
        //    return utf8.GetString(gb);
        //}


    }
}
