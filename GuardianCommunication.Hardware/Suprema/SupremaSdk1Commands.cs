using System;
using System.Collections.Generic;
using GuardianCommunication.Hardware.Shared.Commands;

namespace GuardianCommunication.Hardware.Suprema
{
    public static class SupremaSdk1Commands
    {
        public static List<DeviceCommandTypeEnumeration> GetDefineAndDeleteUserCommandTypes()
        {
            return new List<DeviceCommandTypeEnumeration>
            {
                DeviceCommandTypeEnumeration.EnrollUserWithTemplate,
                DeviceCommandTypeEnumeration.SetUserInfo,
                DeviceCommandTypeEnumeration.DeleteUser,
            };
        }


        public static DtoDeviceCommand GetEnrollUserCommand(
            DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfo),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = userInfo.EmployeeNumber,
                CommandType = DeviceCommandTypeEnumeration.EnrollUserWithTemplate,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetDeleteUserCommand(
            DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetRebootCommand
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "REBOOT",
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
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetDataCommand
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandStartAndEndDate { StartDate = startDate, EndDate = endDate }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.ReadAttendance,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandStartAndEndDate { StartDate = startDate, EndDate = endDate }),
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
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetUserInfoCommand
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
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new CommandReadUser { TemplateType = templateType, UserId = employeeNumber }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ReadUser,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = null,
                CommandIdentifier = commandIdentifier
            };
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
                        CommandContent = "CLEAR ALL DATA",
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
                        ProducerNumber = ProducerEnumeration.Suprema,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = "CLEAR ALL USERS",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        CommandType = DeviceCommandTypeEnumeration.ClearUser,
                        Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = ProducerEnumeration.Suprema,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

        }

        public static DtoDeviceCommand GetFaceCount
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "FACE COUNT",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.FaceCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetFingerCount(
            DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "FINGER COUNT",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.FingerCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetUserCount(
            DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "USER COUNT",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.UserCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetAttendanceLogCount(
            DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandStartAndEndDate { StartDate = startDate, EndDate = endDate }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.AttendanceLogCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };

        }

        public static List<DtoDeviceCommand> GetDeviceStatistics
           (DtoCommunicationDeviceData deviceInfo,
           int maxRetry,
           int? deadline,
           DateTime? visibilityTime,
           CommandPriorityEnumeration? priority,
           Guid? commandIdentifier = null
           )
        {
            return new List<DtoDeviceCommand>
            {
                //GetAttendanceLogCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
                GetFaceCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
                GetFingerCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
                GetUserCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
            };

        }

        public static DtoDeviceCommand SetDateAndTimeCommand
           (DtoCommunicationDeviceData deviceInfo,
           int maxRetry,
           int? deadline,
           DateTime? visibilityTime,
           CommandPriorityEnumeration? priority,
           Guid? commandIdentifier = null
           )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "SET DATE AND TIME",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SetDateAndTime,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }


        public static DtoDeviceCommand ScanFace
          (DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanFace,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand ScanFinger
          (DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandScanFinger { UserId = employeeNumber, FingerIndex = fingerIndex }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanFinger,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand ScanCard
          (DtoCommunicationDeviceData deviceInfo,
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanCard,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand SendHolidays(DtoCommunicationDeviceData deviceInfo,
                List<DtoSupremaSdk1DeviceHolidayGroup> holidayGroups,
                int maxRetry,
                int? deadline,
                DateTime? visibilityTime,
                CommandPriorityEnumeration? priority,
                Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(holidayGroups),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SendHolidays,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand SendTimezones(DtoCommunicationDeviceData deviceInfo,
            List<DtoSupremaSdk1Timezone> timezones,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(timezones),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SetTimeZone,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand SendAccessGroups(DtoCommunicationDeviceData deviceInfo,
            List<DtoSupremaSdk1AccessGroup> accessGroups,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(accessGroups),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SendAccessGroup,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand SetDoorInfo(DtoCommunicationDeviceData deviceInfo,
            DtoSupremaSdk1DeviceDoor doorInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(doorInfo),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SetDoorInfo,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

    }
}
