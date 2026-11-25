using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Hardware.GuardianController.Model;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Hardware.GuardianController
{
    /// <summary>
    /// ساخت مدل‌های ارتباطی برد و پردازش مدل‌هایی که برد می‌فرستد.
    /// هر عملیات فقط یک مدل دارد که هم بدنه API در Standalone (GuardianControllerOnDemandAdapter) است
    /// و هم محتوای دستور در Push (GuardianControllerPushCommands)، و فقط همین‌جا ساخته می‌شود
    /// </summary>
    public static class GuardianControllerUtils
    {
        private static readonly DeviceTimeService TimeService = new DeviceTimeService();

        public static InvalidAttendanceReasonEnumeration GetAuthFailReason(GuardianControllerAttendanceStatusEnumeration reason)
        {
            switch (reason)
            {
                case GuardianControllerAttendanceStatusEnumeration.FailedInvalidUser:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfInvalidUser;
                case GuardianControllerAttendanceStatusEnumeration.FailedToAccessAtTime:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfSchedule;
                case GuardianControllerAttendanceStatusEnumeration.FailedToAccessToDoor:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfDoorPermission;
                case GuardianControllerAttendanceStatusEnumeration.FailedForDoorCalendarLock:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfCalendarLock;
                case GuardianControllerAttendanceStatusEnumeration.FailedForNotAccessToAnyGroup:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNotAccessToAnyGroup;
                case GuardianControllerAttendanceStatusEnumeration.FailedForAttendanceLimit:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfAttendanceLimit;
                case GuardianControllerAttendanceStatusEnumeration.FailedForMasterPrimerRequired:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfMasterPrimerRequired;
                case GuardianControllerAttendanceStatusEnumeration.FailedForUserInactive:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfDisabled;
                case GuardianControllerAttendanceStatusEnumeration.FailedForOutsideValidity:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfExpired;
                case GuardianControllerAttendanceStatusEnumeration.FailedForServerDenied:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfPermission;
                default:
                    return InvalidAttendanceReasonEnumeration.UnAuthorize;
            }
        }

        /// <summary>
        /// ترددهای برد در Push (پیام device_attendance)؛ برد درب را با شناسه درب (Guid) می‌فرستد و زمان‌ها UTC ذخیره می‌شوند
        /// </summary>
        public static void ProcessAttendance(DtoDevice deviceInfo, List<GuardianControllerAttendanceCommunicationModel> attendanceModel)
        {
            foreach (var item in attendanceModel)
            {
                if (item.IsAccessGranted)
                {
                    // تردد مجاز
                    if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveAttendance)
                    {
                        continue;
                    }
                    var currentAttendance = MapAttendanceFromCommunicationModel(deviceInfo, item, DeviceAttendanceIoRetrieveTypeEnumeration.Push);
                    if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ServerRealTimeLog))
                    {
                        LoggingSystem.LogInfo("Guardian controller attendance", currentAttendance);
                    }
                    HardwareEventPublisher.Instance.PublishAttendance(currentAttendance);
                }
                else
                {
                    // تردد نامجاز
                    if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveInvalidAttendance)
                    {
                        continue;
                    }
                    var currentRecord = new DtoInvalidAttendance
                    {
                        Id = item.id,
                        AttendanceDateTime = item.attendance_date_time.ToUtcDateTimeFromEpochMillisecondTime(),
                        UserIdOnDevice = item.user_id_on_device,
                        StatusCode = 0,
                        DeviceId = deviceInfo.Id,
                        DoorId = item.door_id,
                        VerificationStyle = null,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                        RfCardNumber = item.rf_card_number,
                        Reason = GetAuthFailReason(item.verification_status),
                    };
                    if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ServerRealTimeLog))
                    {
                        LoggingSystem.LogInfo("Guardian controller invalid attendance", currentRecord);
                    }
                    HardwareEventPublisher.Instance.PublishInvalidAttendance(currentRecord);
                }
            }
        }

        /// <summary>
        /// تردد مجاز برد؛ ترددهای GetAttendances در Standalone و ترددهای مجاز پیام device_attendance در Push با همین تبدیل ساخته می‌شوند
        /// </summary>
        internal static DtoAttendance MapAttendanceFromCommunicationModel(DtoDevice deviceInfo,
            GuardianControllerAttendanceCommunicationModel item, DeviceAttendanceIoRetrieveTypeEnumeration retrieveType)
        {
            return new DtoAttendance
            {
                UserIdOnDevice = item.user_id_on_device ?? 0,
                LogIdOnDevice = item.id,
                AttendanceDateTime = item.attendance_date_time.ToUtcDateTimeFromEpochMillisecondTime(),
                AttendanceSource = AttendanceSourceEnumeration.Device,
                DeviceAttendanceIoRetrieveType = retrieveType,
                DeviceId = deviceInfo.Id,
                DoorId = item.door_id,
                LocationId = deviceInfo.LocationId,
                CameraId = null,
                IsSentToGuardian = false,
                RfCardNumber = item.rf_card_number,
                StatusCode = 0,
                VerificationStyle = null,
                IoType = item.io_type,
            };
        }

        /// <summary>
        /// رویدادهای درب در Push (پیام device_event)؛ کد رویداد همان نوع رویداد درب است
        /// </summary>
        public static void ProcessDoorEvent(DtoDevice deviceInfo
            , List<GuardianControllerDoorEventCommunicationModel> models)
        {
            if (deviceInfo.DeviceSettings != null && deviceInfo.DeviceSettings.DontSaveEvents)
            {
                return;
            }
            foreach (var item in models)
            {
                HardwareEventPublisher.Instance.PublishDeviceEventLogData(new DtoDeviceEventLog
                {
                    Id = item.id,
                    UserIdOnDevice = item.user_id_on_device,
                    EventDateTime = item.event_date_time.ToUtcDateTimeFromEpochMillisecondTime(),
                    DeviceId = deviceInfo.Id,
                    EventCode = (int)item.event_type,
                    SdkVersion = SdkVersionEnumeration.SdkVersion1,
                    IsFromDevice = true,
                    Producer = ProducerEnumeration.Guardian,
                    DoorId = item.door_id ?? Guid.Empty,
                });
            }
        }

        /// <summary>
        /// شرح خطای برد؛ در Standalone متن خطا و در Push شرح دستور می‌شود
        /// </summary>
        public static string DescribeError(GuardianControllerErrorCommunicationModel model)
        {
            if (model == null)
            {
                return string.Empty;
            }
            var description = $"{model.error_code} {model.message}".Trim();
            return model.errors.IsCollectionNotNullOrEmpty()
                ? description + Environment.NewLine + string.Join(Environment.NewLine, model.errors)
                : description;
        }

        #region Date And Time

        /// <summary>
        /// ساعت با منطقه زمانی برد
        /// </summary>
        internal static GuardianControllerDateAndTimeCommunicationModel MapDateAndTimeToCommunicationModel(DtoDevice deviceInfo, DateTime date)
        {
            return new GuardianControllerDateAndTimeCommunicationModel
            {
                date_time_epoch = date.ToEpochMillisecondsTime(),
                time_zone = deviceInfo.IanaTimeZoneId,
            };
        }

        #endregion

        #region User And Easy Access

        /// <summary>
        /// کاربر با گروه‌ها و دسترسی‌های آسانش روی این برد؛ اعتبار از وقت محلی دستگاه به UTC می‌رود و کاربر بدون پایان، پایان خالی دارد
        /// </summary>
        internal static GuardianControllerUserCommunicationModel MapUserToCommunicationModel(DtoDevice deviceInfo, DtoUserDeviceRelatedData userInfo)
        {
            var userInfoForDevice = userInfo.WithDeviceLocalDates(deviceInfo);
            var accessData = userInfo.GuardianControllerUserAccessData ?? new DtoGuardianControllerUserAccessData();
            return new GuardianControllerUserCommunicationModel
            {
                user_id_on_device = userInfo.UserIdOnDevice,
                name = userInfo.UserName,
                gender = userInfo.Gender,
                is_enable = userInfo.IsEnable,
                start_date = ToEpochMilliseconds(deviceInfo, userInfoForDevice.StartDateTime),
                end_date = userInfo.EndDateTime.HasValue && userInfoForDevice.EndDateTime.HasValue
                    ? ToEpochMilliseconds(deviceInfo, userInfoForDevice.EndDateTime.Value)
                    : (long?)null,
                rf_card_numbers = (userInfo.RfCardNumbers ?? new List<string>()).Where(card => !string.IsNullOrWhiteSpace(card)).Distinct().ToList(),
                password = string.IsNullOrWhiteSpace(userInfo.Password) ? null : userInfo.Password,
                access_group_ids = (accessData.UserGroupIds ?? new List<Guid>()).Distinct().ToList(),
                easy_accesses = (accessData.EasyAccesses ?? new List<DtoGuardianControllerUserEasyAccess>())
                    .Select(easyAccess => MapEasyAccessToCommunicationModel(deviceInfo, new DtoGuardianControllerEasyAccess
                    {
                        EasyAccessId = easyAccess.EasyAccessId,
                        UserIdOnDevice = userInfo.UserIdOnDevice,
                        AccessLevelId = easyAccess.AccessLevelId,
                        CalendarId = easyAccess.CalendarId,
                        StartDateTime = easyAccess.StartDateTime,
                        EndDateTime = easyAccess.EndDateTime,
                        AttendanceCount = easyAccess.AttendanceCount,
                    }))
                    .ToList(),
            };
        }

        /// <summary>
        /// کاربری که برد برمی‌گرداند (همان مدل SetUserInfo)؛ اعتبار کاربر UTC و بازه دسترسی‌های آسان به وقت محلی دستگاه برمی‌گردد
        /// </summary>
        internal static DtoUserDeviceRelatedData MapUserFromCommunicationModel(DtoDevice deviceInfo, GuardianControllerUserCommunicationModel user)
        {
            return new DtoUserDeviceRelatedData
            {
                UserIdOnDevice = user.user_id_on_device,
                UserName = user.name,
                Gender = user.gender,
                IsEnable = user.is_enable,
                StartDateTime = user.start_date.ToUtcDateTimeFromEpochMillisecondTime(),
                EndDateTime = user.end_date?.ToUtcDateTimeFromEpochMillisecondTime(),
                StartAndEndHasTime = true,
                RfCardNumbers = user.rf_card_numbers ?? new List<string>(),
                Password = user.password,
                GuardianControllerUserAccessData = new DtoGuardianControllerUserAccessData
                {
                    UserGroupIds = user.access_group_ids ?? new List<Guid>(),
                    EasyAccesses = (user.easy_accesses ?? new List<GuardianControllerEasyAccessCommunicationModel>())
                        .Select(easyAccess => new DtoGuardianControllerUserEasyAccess
                        {
                            EasyAccessId = easyAccess.id,
                            AccessLevelId = easyAccess.access_level_id,
                            CalendarId = easyAccess.calendar_id,
                            StartDateTime = ToDeviceTime(deviceInfo, easyAccess.start_date),
                            EndDateTime = easyAccess.end_date.HasValue ? ToDeviceTime(deviceInfo, easyAccess.end_date.Value) : (DateTime?)null,
                            AttendanceCount = easyAccess.attendance_count,
                        })
                        .ToList(),
                },
            };
        }

        internal static GuardianControllerEasyAccessCommunicationModel MapEasyAccessToCommunicationModel(DtoDevice deviceInfo, DtoGuardianControllerEasyAccess easyAccess)
        {
            return new GuardianControllerEasyAccessCommunicationModel
            {
                id = easyAccess.EasyAccessId,
                user_id_on_device = easyAccess.UserIdOnDevice,
                access_level_id = easyAccess.AccessLevelId,
                calendar_id = easyAccess.CalendarId,
                start_date = ToEpochMilliseconds(deviceInfo, easyAccess.StartDateTime),
                end_date = easyAccess.EndDateTime.HasValue ? ToEpochMilliseconds(deviceInfo, easyAccess.EndDateTime.Value) : (long?)null,
                attendance_count = easyAccess.AttendanceCount,
            };
        }

        #endregion

        #region Access Control

        /// <summary>
        /// مدت فقط برای باز کردن لحظه‌ای فرستاده می‌شود
        /// </summary>
        internal static GuardianControllerChangeDoorStatusCommunicationModel MapChangeDoorStatusToCommunicationModel(Guid doorId,
            GuardianControllerDoorActionEnumeration action, int? duration)
        {
            return new GuardianControllerChangeDoorStatusCommunicationModel
            {
                door_id = doorId,
                action = action,
                duration = action == GuardianControllerDoorActionEnumeration.OpenMomentary ? duration : null,
            };
        }

        internal static GuardianControllerCalendarTemplateCommunicationModel MapCalendarTemplateToCommunicationModel(DtoGuardianControllerCalendarTemplate calendarTemplate)
        {
            return new GuardianControllerCalendarTemplateCommunicationModel
            {
                id = calendarTemplate.Id,
                calendar_template_number = calendarTemplate.CalendarTemplateNumber,
                title = calendarTemplate.Title,
                day_count = calendarTemplate.DayCount,
                details = (calendarTemplate.Details ?? new List<DtoGuardianControllerCalendarTemplateDetails>())
                    .OrderBy(detail => detail.DayIndex)
                    .ThenBy(detail => detail.StartTime)
                    .Select(detail => new GuardianControllerCalendarTemplateDetailsCommunicationModel
                    {
                        id = detail.Id,
                        day_index = detail.DayIndex,
                        start_time = detail.StartTime,
                        end_time = detail.EndTime,
                        audience = detail.Audience,
                        purpose = detail.Purpose,
                    })
                    .ToList(),
            };
        }

        internal static GuardianControllerCalendarCommunicationModel MapCalendarToCommunicationModel(DtoGuardianControllerCalendar calendar)
        {
            var isTemplateBased = calendar.CalendarType == 2;
            return new GuardianControllerCalendarCommunicationModel
            {
                id = calendar.Id,
                calendar_number = calendar.CalendarNumber,
                title = calendar.Title,
                is_active = calendar.IsActive,
                calendar_type = calendar.CalendarType,
                details = isTemplateBased
                    ? new List<GuardianControllerCalendarDetailsCommunicationModel>()
                    : (calendar.Details ?? new List<DtoGuardianControllerCalendarDetails>())
                    .OrderBy(detail => detail.Date)
                    .ThenBy(detail => detail.StartTime)
                    .Select(detail => new GuardianControllerCalendarDetailsCommunicationModel
                    {
                        id = detail.Id,
                        date = detail.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        start_time = detail.StartTime,
                        end_time = detail.EndTime,
                        audience = detail.Audience,
                        purpose = detail.Purpose,
                    })
                    .ToList(),
                calendar_template_id = isTemplateBased ? calendar.CalendarTemplateId : null,
                cycle_start_date = isTemplateBased && calendar.CycleStartDate.HasValue
                    ? calendar.CycleStartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : null,
            };
        }

        internal static GuardianControllerAccessLevelCommunicationModel MapAccessLevelToCommunicationModel(DtoGuardianControllerAccessLevel accessLevel)
        {
            return new GuardianControllerAccessLevelCommunicationModel
            {
                id = accessLevel.Id,
                access_level_number = accessLevel.AccessLevelNumber,
                title = accessLevel.Title,
                is_active = accessLevel.IsActive,
                door_ids = (accessLevel.DoorIds ?? new List<Guid>()).Distinct().ToList(),
            };
        }

        internal static GuardianControllerAccessGroupCommunicationModel MapAccessGroupToCommunicationModel(DtoGuardianControllerAccessGroup accessGroup)
        {
            return new GuardianControllerAccessGroupCommunicationModel
            {
                id = accessGroup.Id,
                access_group_number = accessGroup.AccessGroupNumber,
                title = accessGroup.Title,
                is_active = accessGroup.IsActive,
                access_data = (accessGroup.AccessData ?? new List<DtoGuardianControllerAccessGroupAccessData>())
                    .Select(accessData => new GuardianControllerAccessGroupAccessDataCommunicationModel
                    {
                        id = accessData.Id,
                        access_level_id = accessData.AccessLevelId,
                        calendar_id = accessData.CalendarId,
                    })
                    .ToList(),
            };
        }

        #endregion

        #region Hardware Config

        /// <summary>
        /// هر رله، ویگند و ورودی/خروجی یک بار تعریف می‌شود و درب‌ها با شناسه به آن ارجاع می‌دهند؛ ورودی/خروجی بدون درب هم با پیکربندی برد می‌رود
        /// </summary>
        internal static GuardianControllerHardwareConfigCommunicationModel MapHardwareConfigToCommunicationModel(
            List<DtoGuardianControllerHardwareConfigDoor> doors, List<DtoGuardianControllerHardwareConfigIoPort> ioPorts)
        {
            var doorsForSend = (doors ?? new List<DtoGuardianControllerHardwareConfigDoor>()).OrderBy(door => door.DoorNumber).ToList();
            var ioPortsForSend = (ioPorts ?? new List<DtoGuardianControllerHardwareConfigIoPort>()).OrderBy(ioPort => ioPort.IoNumber).ToList();
            return new GuardianControllerHardwareConfigCommunicationModel
            {
                relays = doorsForSend
                    .SelectMany(door => door.Relays ?? new List<DtoGuardianControllerHardwareConfigRelay>())
                    .GroupBy(relay => relay.Id)
                    .Select(group => group.First())
                    .OrderBy(relay => relay.RelayNumber)
                    .Select(relay => new GuardianControllerHardwareConfigRelayCommunicationModel
                    {
                        id = relay.Id,
                        relay_number = relay.RelayNumber,
                        relay_type = relay.RelayType,
                        contact_type = relay.ContactType,
                        title = relay.Title,
                    })
                    .ToList(),
                wiegands = doorsForSend
                    .Where(door => door.WiegandId.HasValue)
                    .GroupBy(door => door.WiegandId.Value)
                    .Select(group => group.First())
                    .OrderBy(door => door.WiegandNumber)
                    .Select(door => new GuardianControllerHardwareConfigWiegandCommunicationModel
                    {
                        id = door.WiegandId.Value,
                        wiegand_number = door.WiegandNumber.GetValueOrDefault(),
                        data_type = door.WiegandDataType ?? GuardianControllerWiegandDataTypeEnumeration.CardNumber,
                        title = door.WiegandTitle,
                    })
                    .ToList(),
                io_ports = ioPortsForSend
                    .Select(ioPort => new GuardianControllerHardwareConfigIoPortCommunicationModel
                    {
                        id = ioPort.Id,
                        io_number = ioPort.IoNumber,
                        io_type = ioPort.GpioType,
                        contact_type = ioPort.ContactType,
                        title = ioPort.Title,
                    })
                    .ToList(),
                doors = doorsForSend.Select(door => MapHardwareConfigDoorToCommunicationModel(door, ioPortsForSend)).ToList(),
            };
        }

        private static GuardianControllerHardwareConfigDoorCommunicationModel MapHardwareConfigDoorToCommunicationModel(
            DtoGuardianControllerHardwareConfigDoor door, List<DtoGuardianControllerHardwareConfigIoPort> ioPorts)
        {
            return new GuardianControllerHardwareConfigDoorCommunicationModel
            {
                id = door.DoorId,
                door_number = door.DoorNumber,
                title = door.Title,
                door_type = door.DoorType,
                unlock_duration = door.UnlockDurationInSecond,
                door_held_timeout = door.DoorHeldTimeoutInSecond,
                reader_io_type = (DeviceIoTypeEnumeration)door.WiegandDirection,
                match_on_server_enabled = door.MatchOnServerEnabled,
                match_on_server_local_check_enabled = door.MatchOnServerLocalCheckEnabled,
                relay_ids = (door.Relays ?? new List<DtoGuardianControllerHardwareConfigRelay>())
                    .OrderBy(relay => relay.RelayType)
                    .ThenBy(relay => relay.RelayNumber)
                    .Select(relay => relay.Id)
                    .ToList(),
                wiegand_id = door.WiegandId,
                io_port_ids = ioPorts
                    .Where(ioPort => ioPort.DoorId == door.DoorId)
                    .Select(ioPort => ioPort.Id)
                    .ToList(),
                door_calendar = new GuardianControllerHardwareConfigDoorCalendarCommunicationModel
                {
                    open_calendar_ids = door.OpenCalendarIds ?? new List<Guid>(),
                    close_calendars = (door.CloseCalendars ?? new List<DtoGuardianControllerHardwareConfigCloseCalendar>())
                        .Select(closeCalendar => new GuardianControllerHardwareConfigCloseCalendarCommunicationModel
                        {
                            id = closeCalendar.Id,
                            calendar_id = closeCalendar.CalendarId,
                            access_group_id = closeCalendar.AccessGroupId,
                        })
                        .ToList(),
                },
                master_access_primer = new GuardianControllerHardwareConfigMasterAccessPrimerCommunicationModel
                {
                    master_groups = door.MasterAccessPrimerGroupIds ?? new List<Guid>(),
                    reset_period = door.MasterAccessPrimerResetPeriod,
                    reset_time = door.MasterAccessPrimerResetTime,
                    apply_to_enter_only = door.MasterAccessPrimerApplyToEnterOnly,
                    toggle_mode = door.MasterAccessPrimerToggleMode,
                },
            };
        }

        internal static GuardianControllerKeepOnlyCommunicationModel MapKeepOnlyToCommunicationModel(DtoGuardianControllerKeepOnly keepOnly)
        {
            return new GuardianControllerKeepOnlyCommunicationModel
            {
                calendar_template_ids = (keepOnly.CalendarTemplateIds ?? new List<Guid>()).Distinct().ToList(),
                calendar_ids = (keepOnly.CalendarIds ?? new List<Guid>()).Distinct().ToList(),
                access_level_ids = (keepOnly.AccessLevelIds ?? new List<Guid>()).Distinct().ToList(),
                access_group_ids = (keepOnly.AccessGroupIds ?? new List<Guid>()).Distinct().ToList(),
                user_ids_on_device = (keepOnly.UserIdsOnDevice ?? new List<long>()).Distinct().OrderBy(number => number).ToList(),
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// وقت محلی دستگاه به میلی‌ثانیه epoch (UTC)
        /// </summary>
        private static long ToEpochMilliseconds(DtoDevice deviceInfo, DateTime deviceLocalTime)
        {
            return TimeService.DeviceTimeToUtc(deviceLocalTime, deviceInfo.IanaTimeZoneId).ToEpochMillisecondsTime();
        }

        /// <summary>
        /// میلی‌ثانیه epoch (UTC) به وقت محلی دستگاه
        /// </summary>
        private static DateTime ToDeviceTime(DtoDevice deviceInfo, long epochMilliseconds)
        {
            return TimeService.UtcToDeviceTime(epochMilliseconds.ToUtcDateTimeFromEpochMillisecondTime(), deviceInfo.IanaTimeZoneId);
        }

        #endregion
    }
}
