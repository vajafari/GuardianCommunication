using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Hardware.PadisController.Model;

namespace GuardianCommunication.Hardware.PadisController
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class PadisControllerPushCommands
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
                CommandContent = null,
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

        public static DtoDeviceCommand GetChangeLockStatusCommand
            (DtoCommunicationDeviceData deviceInfo,
             int doorId,
             int lockStatus,
             int? delay,
             int maxRetry,
             int? deadline,
             DateTime? visibilityTime,
             CommandPriorityEnumeration? priority,
             Guid? commandIdentifier = null
            )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerChangeLockStatusCommunicationModel
                {
                    duration = delay,
                    door_id = doorId,
                    lock_status = (short)lockStatus,
                    force_override_emergency = false,
                }),
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

        public static DtoDeviceCommand GetDefineUserCommand(
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
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerUserCommunicationModel
                {
                    user_permissions = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.EasyPermissions.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.EasyPermissions.Select(up => new PadisControllerEasyPermissionCommunicationModel
                    {
                        calendar_number = up.CalendarNumber,
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime.ToEpochMillisecondsTime(),
                    }).ToList() : null,
                    attendance_limitations = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.AttendanceLimitations.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.AttendanceLimitations.Select(up => new PadisControllerUserLimitationAttendanceCommunicationModel
                    {
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime.ToEpochMillisecondsTime(),
                        attendance_count = up.AttendanceCount,
                        io_type = (int?)up.IoType,
                    }).ToList() : null,
                    time_limitations = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.TimeLimitations.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.TimeLimitations.Select(up => new PadisControllerUserLimitationTimeCommunicationModel
                    {
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime?.ToEpochMillisecondsTime(),
                    }).ToList() : null,
                    rf_card_numbers = userInfo.RfCardNumbers,
                    password = userInfo.Password,
                    user_id = userInfo.EmployeeNumber,
                    is_enable = userInfo.IsEnable,
                    user_type = userInfo.UserType,
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
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
            };
        }

        //Delete
        public static DtoDeviceCommand GetDeleteUserCommands(
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
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerUserIdCommunicationModel { UserId = employeeNumber }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
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
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerDateIntervalCommunicationModel
                {
                    StartTime = startDate.ToEpochMillisecondsTime(),
                    EndTime = endDate.ToEpochMillisecondsTime(),
                }),
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


        public static DtoDeviceCommand GetUserInfoCommand
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
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerUserIdCommunicationModel { UserId = employeeNumber }),
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
                        CommandContent = null,
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
                        CommandIdentifier = commandIdentifier
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = null,
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

        public static DtoDeviceCommand GetDeviceStatisticsCommand
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
                CommandContent = "INFO",
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.PadisControllerSetRelay,
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


        public static DtoDeviceCommand GetSetRelayCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoPadisControllerRelay relay,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerRelayCommunicationModel
                {
                    RelayNumber = relay.RelayNumber,
                    IsActive = relay.IsActive,
                    RelayType = relay.RelayType,
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.PadisControllerSetRelay,
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

        public static DtoDeviceCommand GetSetIoPortCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoPadisControllerIoPort ioPort,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerIoPortCommunicationModel
                {
                    IoNumber = ioPort.IoNumber,
                    IsActive = ioPort.IsActive,
                    Id = ioPort.Id
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.PadisControllerSetIoPort,
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

        public static DtoDeviceCommand GetSetWiegandCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoPadisControllerWiegand wiegand,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerWiegandCommunicationModel
                {
                    WiegandNumber = wiegand.WiegandNumber,
                    IsActive = wiegand.IsActive,
                    Id = wiegand.Id,
                    WiegandDataType = (int)wiegand.WiegandDataType,
                    WiegandFormat = (int)wiegand.WiegandFormat,
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.PadisControllerSetWiegand,
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

        public static DtoDeviceCommand GetSetDoorCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoPadisControllerDeviceDoor door,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerDeviceDoorCommunicationModel
                {
                    CombinationAccessGroupNumbersInJson = door.CombinationAccessGroupNumbersInJson,
                    Id = door.Id,
                    DoorNumberOnDevice = door.DoorNumberOnDevice,
                    IsActive = door.IsActive,
                    OpenTimeCalendarNumber = door.OpenTimeCalendarNumber,
                    PassVerificationIoPortId = door.PassVerificationIoPortId,
                    ReaderDeviceNumber = door.ReaderDeviceNumber,
                    WiegandId = door.WiegandId,
                    ReaderIoType = (int?)door.ReaderIoType
                }),
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
                ProducerNumber = deviceInfo.ProducerEnum,
                SdkVersion = deviceInfo.SdkVersionEnum,
                VisiblilityTime = visibilityTime,
                CommandIdentifier = commandIdentifier
            };
        }

        public static DtoDeviceCommand GetCalendarCommand
        (DtoCommunicationDeviceData deviceInfo,
            DtoPadisControllerCalendar calendar,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new PadisControllerCalendarCommunicationModel
                {
                    title = calendar.Title,
                    calendar_number = calendar.CalendarNumber,
                    details = calendar.Details?.Select(d => new PadisControllerCalendarDetailsCommunicationModel
                    {
                        date = d.Date.Date.ToEpochMillisecondsTime(),
                        end_time = d.EndTime,
                        start_time = d.StartTime,
                    }).ToList(),
                }),
                CommitTime = DateTime.Now,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                EmployeeNumber = null,
                CommandType = DeviceCommandTypeEnumeration.PadisControllerSetCalendar,
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

    }
}
