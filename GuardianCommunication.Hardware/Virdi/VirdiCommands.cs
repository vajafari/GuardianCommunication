using System;
using System.Collections.Generic;
using GuardianCommunication.Hardware.Shared.Commands;
using GuardianCommunication.Hardware.Virdi.VirdiConcepts;

namespace GuardianCommunication.Hardware.Virdi
{
    public static class VirdiCommands
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
            };
        }

        public static DtoDeviceCommand GetUserInfoCommand
            (DtoCommunicationDeviceData deviceInfo, long employeeNumber,
            TemplateTypeEnumeration
            templateType,
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
            };
        }

        public static DtoDeviceCommand GetClearAllDataCommand
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
                        ProducerNumber = ProducerEnumeration.Virdi,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime,
                        CommandIdentifier = commandIdentifier,
                    };
                case DeviceLogTypeEnumeration.Attendance:
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }


        }

        //public static DtoDeviceCommand ScanFinger
        //  (DtoCommunicationDeviceData deviceInfo,
        //  long employeeNumber,
        //  int fingerIndex,
        //  int maxRetry,
        //  int? deadline,
        //  DateTime? visibilityTime,
        //  DeviceCommandPriorityEnumeration? priority
        //  )
        //{
        //    return new DtoDeviceCommand
        //    {
        //        CommandContent = ObjectHelper.SerializeAsJson(new CommandScanFinger { UserId = employeeNumber, FingerIndex = fingerIndex }),
        //        CommitTime = DateTime.Now,
        //        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
        //        RetryCount = 0,
        //        EmployeeNumber = employeeNumber,
        //        CommandType = DeviceCommandTypeEnumeration.ScanFinger,
        //        Priority = priority.HasValue ? priority.Value : DeviceCommandPriorityEnumeration.VeryHigh,
        //        MaxRetry = maxRetry,
        //        ResponseValue = null,
        //        SendTime = null,
        //        ResponseTime = null,
        //        DeviceNumber = deviceInfo.DeviceNumber,
        //        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
        //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
        //        ProducerNumber = ProducerEnumeration.Virdi,
        //        SdkVersion = SdkVersionEnumeration.SdkVersion1,
        //        VisiblilityTime = visibilityTime,
        //        CommandIdentifier = commandIdentifier,
        //    };
        //}

        public static DtoDeviceCommand GetDataCommand
            (DtoCommunicationDeviceData deviceInfo,
            VirdiDeviceLogTypeEnum logType,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new VirdiGetDataCommand() { LogType = logType }),
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
            };
        }

        public static DtoDeviceCommand GetAttendanceLogCount
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
                CommandContent = "ATTENDANCE LOG COUNT",
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
            };
        }

        public static DtoDeviceCommand GetUserCount
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
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
                GetAttendanceLogCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
                GetUserCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
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
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }

        public static DtoDeviceCommand ScanIris
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
                CommandType = DeviceCommandTypeEnumeration.ScanIris,
                Priority = priority ?? CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }

        //public static DtoDeviceCommand ScanCard
        //  (DtoCommunicationDeviceData deviceInfo,
        //  long employeeNumber,
        //  int maxRetry,
        //  int? deadline,
        //  DateTime? visibilityTime,
        //  DeviceCommandPriorityEnumeration? priority
        //  )
        //{
        //    return new DtoDeviceCommand
        //    {
        //        CommandContent = ObjectHelper.SerializeAsJson(new CommandUserId { UserId = employeeNumber }),
        //        CommitTime = DateTime.Now,
        //        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
        //        RetryCount = 0,
        //        EmployeeNumber = employeeNumber,
        //        CommandType = DeviceCommandTypeEnumeration.ScanFinger,
        //        Priority = priority.HasValue ? priority.Value : DeviceCommandPriorityEnumeration.VeryHigh,
        //        MaxRetry = maxRetry,
        //        ResponseValue = null,
        //        SendTime = null,
        //        ResponseTime = null,
        //        DeviceNumber = deviceInfo.DeviceNumber,
        //        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
        //        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
        //        ProducerNumber = ProducerEnumeration.Virdi,
        //        SdkVersion = SdkVersionEnumeration.SdkVersion1,
        //        VisiblilityTime = visibilityTime,
        //    };
        //}


        public static DtoDeviceCommand GetSendAccessControlDataCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoVirdiAccessControlData accessControlData,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(accessControlData),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.VirdiAccessControlData,
                Priority = priority ?? CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.Virdi,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier,
            };
        }


    }
}
