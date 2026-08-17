using System;
using System.Collections.Generic;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;

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
            DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            var userInfoForDevice = userInfo.WithDeviceLocalDates(deviceInfo);

            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfoForDevice),
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = userInfo.UserIdOnDevice,
                CommandType = DeviceCommandTypeEnumeration.EnrollUserWithTemplate,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetDeleteUserCommand(
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.DeleteUser,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

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
                CommandContent = "REBOOT",
                CommitTime = DateTime.Now.ToUniversalTime(),
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
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetDataCommand
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandStartAndEndDate { StartDate = startDate, EndDate = endDate }),
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.ReadAttendance,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandStartAndEndDate { StartDate = startDate, EndDate = endDate }),
                CommitTime = DateTime.Now.ToUniversalTime(),
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
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetUserInfoCommand
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
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new CommandReadUser { TemplateType = templateType, UserId = employeeNumber }),
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ReadUser,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = null,
                CommandIdentifier = commandIdentifier
            };
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
                        CommandContent = "CLEAR ALL DATA",
                        CommitTime = DateTime.Now.ToUniversalTime(),
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
                        ProducerNumber = ProducerEnumeration.Suprema,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = "CLEAR ALL USERS",
                        CommitTime = DateTime.Now.ToUniversalTime(),
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        CommandType = DeviceCommandTypeEnumeration.ClearUser,
                        Priority = priority ?? CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = ProducerEnumeration.Suprema,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

        }

        public static DtoDeviceCommand GetFaceCount
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
                CommandContent = "FACE COUNT",
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.FaceCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetFingerCount(
            DtoDevice deviceInfo,
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
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.FingerCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetUserCount(
            DtoDevice deviceInfo,
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
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.UserCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static DtoDeviceCommand GetAttendanceLogCount(
            DtoDevice deviceInfo,
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
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.AttendanceLogCount,
                Priority = priority ?? CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };

        }

        public static List<DtoDeviceCommand> GetDeviceStatistics
           (DtoDevice deviceInfo,
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
                CommandContent = "SET DATE AND TIME",
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.SetDateAndTime,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }


        public static DtoDeviceCommand ScanFace
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now.ToUniversalTime(),
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand ScanFinger
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandScanFinger { UserId = employeeNumber, FingerIndex = fingerIndex }),
                CommitTime = DateTime.Now.ToUniversalTime(),
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
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand ScanCard
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
                CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
                CommitTime = DateTime.Now.ToUniversalTime(),
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = employeeNumber,
                CommandType = DeviceCommandTypeEnumeration.ScanCard,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.ToUniversalTime().AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Suprema,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime?.ToUniversalTime(),
                CommandIdentifier = commandIdentifier
            };
        }

    }
}
