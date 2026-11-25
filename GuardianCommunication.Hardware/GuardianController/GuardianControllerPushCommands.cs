using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Hardware.GuardianController.Model;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Hardware.GuardianController
{
    /// <summary>
    /// دستورهای حالت Push برد کنترلر گاردین؛ محتوای هر دستور همان مدل ارتباطی است
    /// که GuardianControllerOnDemandAdapter در Standalone به‌عنوان بدنه API می‌فرستد.
    /// شناسه دستگاه هم ثبت می‌شود چون صف دستورها برای هر دستگاه با آن جدا و پاک‌سازی دستورهای قبلی با آن انجام می‌شود
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class GuardianControllerPushCommands
    {
        public static List<DeviceCommandTypeEnumeration> GetDefineAndDeleteUserCommandTypes()
        {
            return new List<DeviceCommandTypeEnumeration>
            {
                DeviceCommandTypeEnumeration.SetUserInfo,
                DeviceCommandTypeEnumeration.DeleteUser,
            };
        }

        //Control
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
                CommandContent = string.Empty,
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
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

        /// <summary>
        /// فرمان درب با شناسه درب: باز کردن لحظه‌ای (مدت خالی یعنی مدت باز ماندن قفل درب)، باز یا بسته نگه داشتن دائم، برگشت به حالت عادی
        /// </summary>
        public static DtoDeviceCommand GetChangeDoorStatusCommand
        (DtoDevice deviceInfo,
            Guid doorId,
            GuardianControllerDoorActionEnumeration action,
            int? duration,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapChangeDoorStatusToCommunicationModel(doorId, action, duration)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDoorControl,
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

        /// <summary>
        /// ثبت کاربر با گروه‌های دسترسی و دسترسی‌های آسانش روی همین برد؛ این فهرست‌ها جایگزین فهرست‌های قبلی کاربر روی برد می‌شوند
        /// </summary>
        public static DtoDeviceCommand GetDefineUserCommand
        (DtoDevice deviceInfo,
            DtoUserDeviceRelatedData userInfo,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapUserToCommunicationModel(deviceInfo, userInfo)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = userInfo.UserIdOnDevice,
                CommandType = DeviceCommandTypeEnumeration.SetUserInfo,
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
            };
        }

        //Delete
        /// <summary>
        /// حذف کاربر با گروه‌ها و دسترسی‌های آسانش از برد
        /// </summary>
        public static DtoDeviceCommand GetDeleteUserCommand
        (DtoDevice deviceInfo,
            long userIdOnDevice,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(new GuardianControllerUserIdOnDeviceCommunicationModel
                { user_id_on_device = userIdOnDevice }),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = userIdOnDevice,
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
            };
        }

        /// <summary>
        /// پاک کردن لاگ‌های برد (Attendance: ترددها و رویدادهای درب) یا همه کاربران برد (Users)
        /// </summary>
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
                        CommandContent = string.Empty,
                        CommitTime = DateTime.Now,
                        DeviceId = deviceInfo.Id,
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
                        CommandIdentifier = commandIdentifier
                    };
                case DeviceLogTypeEnumeration.Users:
                    return new DtoDeviceCommand
                    {
                        CommandContent = string.Empty,
                        CommitTime = DateTime.Now,
                        DeviceId = deviceInfo.Id,
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
                        ProducerNumber = deviceInfo.ProducerNumber,
                        SdkVersion = deviceInfo.SdkVersion,
                        VisiblilityTime = visibilityTime?.ToUniversalTime(),
                        CommandIdentifier = commandIdentifier
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }

        /// <summary>
        /// ثبت یا به‌روزرسانی جداگانه دسترسی‌های آسان روی برد (با شناسه)؛ شمارنده تردد هر کدام روی برد حفظ می‌شود
        /// </summary>
        public static DtoDeviceCommand GetSetEasyAccessCommand
        (DtoDevice deviceInfo,
            List<DtoGuardianControllerEasyAccess> easyAccesses,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(easyAccesses
                    .Select(easyAccess => GuardianControllerUtils.MapEasyAccessToCommunicationModel(deviceInfo, easyAccess))
                    .ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetEasyAccess,
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
            };
        }

        /// <summary>
        /// حذف جداگانه دسترسی‌های آسان از برد (با شناسه)
        /// </summary>
        public static DtoDeviceCommand GetDeleteEasyAccessCommand
        (DtoDevice deviceInfo,
            List<Guid> easyAccessIds,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(easyAccessIds.Distinct().ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDeleteEasyAccess,
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
            };
        }

        public static DtoDeviceCommand GetCalendarTemplateCommand
        (DtoDevice deviceInfo,
            DtoGuardianControllerCalendarTemplate calendarTemplate,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapCalendarTemplateToCommunicationModel(calendarTemplate)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetCalendarTemplate,
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
            };
        }

        public static DtoDeviceCommand GetDeleteCalendarTemplateCommand
        (DtoDevice deviceInfo,
            List<Guid> calendarTemplateIds,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(calendarTemplateIds.Distinct().ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDeleteCalendarTemplate,
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
            };
        }

        public static DtoDeviceCommand GetCalendarCommand
        (DtoDevice deviceInfo,
            DtoGuardianControllerCalendar calendar,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapCalendarToCommunicationModel(calendar)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetCalendar,
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
            };
        }

        public static DtoDeviceCommand GetDeleteCalendarCommand
        (DtoDevice deviceInfo,
            List<Guid> calendarIds,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(calendarIds.Distinct().ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDeleteCalendar,
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
            };
        }

        public static DtoDeviceCommand GetAccessGroupCommand
        (DtoDevice deviceInfo,
            DtoGuardianControllerAccessGroup accessGroup,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapAccessGroupToCommunicationModel(accessGroup)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetAccessGroup,
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
            };
        }

        public static DtoDeviceCommand GetDeleteAccessGroupCommand
        (DtoDevice deviceInfo,
            List<Guid> accessGroupIds,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(accessGroupIds.Distinct().ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDeleteAccessGroup,
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
            };
        }

        public static DtoDeviceCommand GetAccessLevelCommand
        (DtoDevice deviceInfo,
            DtoGuardianControllerAccessLevel accessLevel,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapAccessLevelToCommunicationModel(accessLevel)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetAccessLevel,
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
            };
        }

        public static DtoDeviceCommand GetDeleteAccessLevelCommand
        (DtoDevice deviceInfo,
            List<Guid> accessLevelIds,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(accessLevelIds.Distinct().ToList()),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerDeleteAccessLevel,
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
            };
        }

        /// <summary>
        /// پیکربندی کامل سخت‌افزار برد (درب‌ها با رله‌ها، ویگند و تنظیمات درب، و ورودی/خروجی‌های برد)؛ جایگزین پیکربندی قبلی می‌شود.
        /// محتوای دستور همان بدنه POST api/v1/config/hardware است
        /// </summary>
        public static DtoDeviceCommand GetHardwareConfigCommand
        (DtoDevice deviceInfo,
            List<DtoGuardianControllerHardwareConfigDoor> doors,
            List<DtoGuardianControllerHardwareConfigIoPort> ioPorts,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapHardwareConfigToCommunicationModel(doors, ioPorts)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerSetHardwareConfig,
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
            };
        }

        /// <summary>
        /// پایان همگام‌سازی کامل برد: برد هر چیزی را که در این فهرست‌ها نیست حذف می‌کند.
        /// اولویت آن با تعریف‌ها یکی است تا بعد از آن‌ها و پیش از دستورهای تازه‌تر اجرا شود
        /// </summary>
        public static DtoDeviceCommand GetKeepOnlyCommand
        (DtoDevice deviceInfo,
            DtoGuardianControllerKeepOnly keepOnly,
            int maxRetry,
            int? deadline,
            DateTime? visibilityTime,
            CommandPriorityEnumeration? priority,
            Guid? commandIdentifier = null
        )
        {
            return new DtoDeviceCommand
            {
                CommandContent = ObjectHelper.SerializeAsJson(
                    GuardianControllerUtils.MapKeepOnlyToCommunicationModel(keepOnly)),
                CommitTime = DateTime.Now,
                DeviceId = deviceInfo.Id,
                DeviceSerialNumber = deviceInfo.SerialNumber.ToNotNullString(),
                RetryCount = 0,
                UserIdOnDevice = null,
                CommandType = DeviceCommandTypeEnumeration.GuardianControllerKeepOnly,
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
            };
        }
    }
}
