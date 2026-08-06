using System;
using System.Collections.Generic;
using GuardianCommunication.Hardware.Shared.Commands;

namespace GuardianCommunication.Hardware.Pw
{
    public static class PwCommands
    {
        public static DtoDeviceCommand GetEnrollUserWithTemplatesCommand(
            DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority)
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfo),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = userInfo.EmployeeNumber,
                CommandType = DeviceCommandTypeEnumeration.EnrollUserWithTemplate,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
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
            };
        }

        public static DtoDeviceCommand GetSendUserCommand(
            DtoCommunicationDeviceData deviceInfo,
            DtoEmployeeDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority)
        {
            userInfo.ClearTemplateData();
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfo),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = userInfo.EmployeeNumber,
                CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
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
            };
        }


        public static DtoDeviceCommand GetDeleteUserCommand(
            DtoCommunicationDeviceData deviceInfo,
            long employeeNumber,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }

        public static DtoDeviceCommand GetDataCommand
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "GET DATA",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.ReadAttendance,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }

        public static DtoDeviceCommand GetReadoutFromDeviceCommand
            (DtoCommunicationDeviceData deviceInfo,
            DateTime startDate,
            DateTime endDate,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }

        public static DtoDeviceCommand GetUserInfoCommand
            (DtoCommunicationDeviceData deviceInfo,
            long employeeNumber,
            TemplateTypeEnumeration templateType,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = null,
            };
        }

        public static DtoDeviceCommand GetClearDataCommand
            (DtoCommunicationDeviceData deviceInfo,
            DeviceLogTypeEnumeration logType,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                        Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = ProducerEnumeration.ProcessingWorld,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime,
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = "CLEAR ALL USERS",
                        CommitTime = DateTime.Now,
                        DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                        RetryCount = 0,
                        CommandType = DeviceCommandTypeEnumeration.ClearUser,
                        Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.VeryLow,
                        MaxRetry = maxRetry,
                        ResponseValue = null,
                        SendTime = null,
                        ResponseTime = null,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                        DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                        ProducerNumber = ProducerEnumeration.ProcessingWorld,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        VisiblilityTime = visibilityTime,
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }

        }

        public static DtoDeviceCommand GetAttendanceLogCount
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };

        }

        public static DtoDeviceCommand GetFaceCount
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };

        }

        public static DtoDeviceCommand GetFingerCount
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = "FINGER COUNT",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.FaceCount,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };

        }

        public static DtoDeviceCommand GetUserCount
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Low,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };

        }

        public static List<DtoDeviceCommand> GetDeviceStatistics
            (DtoCommunicationDeviceData deviceInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
            )
        {
            return new List<DtoDeviceCommand>
            {
                GetAttendanceLogCount(deviceInfo, maxRetry, deadline, visibilityTime, priority),
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
           CommandPriorityEnumeration? priority
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
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.VeryHigh,
                MaxRetry = maxRetry,
                ResponseValue = null,
                SendTime = null,
                ResponseTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }



        public static DtoDeviceCommand GetSendWithoutFingerCommand(
            DtoCommunicationDeviceData deviceInfo,
            List<DtoEmployeeDeviceRelatedData> userInfos,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfos),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SendWithoutFinger,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }


        public static DtoDeviceCommand GetSendValidInvalidCommand(
            DtoCommunicationDeviceData deviceInfo,
            List<DtoEmployeeDeviceRelatedData> userInfos,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(userInfos),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.SendWithoutFinger,
                Priority = priority.HasValue ? priority.Value : CommandPriorityEnumeration.Medium,
                MaxRetry = maxRetry,
                ResponseTime = null,
                ResponseValue = null,
                SendTime = null,
                DeviceNumber = deviceInfo.DeviceNumber,
                Deadline = deadline.HasValue ? DateTime.Now.AddMinutes(deadline.Value) : (DateTime?)null,
                DeviceContent = ObjectHelper.SerializeAsJson(deviceInfo),
                ProducerNumber = ProducerEnumeration.ProcessingWorld,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                VisiblilityTime = visibilityTime,
            };
        }


    }
}
