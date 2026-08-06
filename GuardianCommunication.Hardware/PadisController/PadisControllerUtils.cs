using System;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.PadisController.Definition;
using GuardianCommunication.Hardware.PadisController.Model;
using GuardianCommunication.Hardware.Shared;

namespace GuardianCommunication.Hardware.PadisController
{
    public static class PadisControllerUtils 
    {

        public static InvalidAttendanceReasonEnumeration GetAuthFailReason(PadisControllerAttendanceStatusEnumeration reason)
        {
            switch (reason)
            {
                case PadisControllerAttendanceStatusEnumeration.FailedToAccessAtTime:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfSchedule;
                case PadisControllerAttendanceStatusEnumeration.FailedToAccessToDoor:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfDoorPermission;
                case PadisControllerAttendanceStatusEnumeration.FailedInvalidUser:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfInvalidUser;
                case PadisControllerAttendanceStatusEnumeration.FailedForAntiPassback:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfAntiPassback;
                case PadisControllerAttendanceStatusEnumeration.FailedForBlacklist:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfBlacklist;
                case PadisControllerAttendanceStatusEnumeration.FailedForPassAccuracy:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfPassAccuracy;
                case PadisControllerAttendanceStatusEnumeration.FailedForDoorCalendarLock:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfCalendarLock;
                case PadisControllerAttendanceStatusEnumeration.FailedForNotAccessToAnyGroup:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNotAccessToAnyGroup;
                case PadisControllerAttendanceStatusEnumeration.FailedForNoRelevantGroupFound:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNoRelevantGroupFound;
                case PadisControllerAttendanceStatusEnumeration.FailedForNoCoverAllGroup:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNoCoverAllGroup;
                case PadisControllerAttendanceStatusEnumeration.FailedForNoAccessSchedule:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNoAccessSchedule;
                case PadisControllerAttendanceStatusEnumeration.FailedForNoUserSchedule:
                    return InvalidAttendanceReasonEnumeration.InvalidBecauseOfNoUserSchedule;
                default:
                    return InvalidAttendanceReasonEnumeration.UnAuthorize;
            }
        }
        
        public static void ProcessOperationLog(DtoCommunicationDeviceData deviceInfo, PadisControllerOperationLogCommunicationModel[] models)
        {
            if (!deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
            {
                foreach (var item in models)
                {
                    var operationLog = new DtoDeviceEventLog
                    {
                        Id = (int)item.Id,
                        EmployeeNumber = item.EmployeeNumber,
                        EventDateTime = item.EventDateTime.ToDateTimeFromEpochMillisecondTime(true),
                        DeviceNumber = deviceInfo.DeviceNumber,
                        EventCode = item.EventCode,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        IsFromDevice = true,
                        Producer = ProducerEnumeration.Padis,
                    };
                    if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ServerRealTimeLog))
                    {
                        LoggingSystem.LogInfo("Padis controller operation lod", operationLog);
                    }
                    HardwareEventPublisher.Instance.PublishDeviceEventLogData(operationLog);
                }
            }
        }

        public static void ProcessAttendance(DtoCommunicationDeviceData deviceInfo, PadisControllerAttendanceCommunicationModel[] attendanceModel)
        {
            foreach (var item in attendanceModel)
            {
                if (item.IsAccessGranted)
                {
                    // تردد مجاز
                    if (!deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                    {
                        var currentAttendance = new DtoAttendance
                        {
                            EmployeeNumber = item.EmployeeNumber,
                            ApplicationId = deviceInfo.ApplicationId,
                            AttendanceDateTime = item.AttendanceDateTime.ToDateTimeFromEpochMillisecondTime(true),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            CameraId = null,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            DeviceNumber = deviceInfo.DeviceNumber,
                            DoorId = item.DoorId,
                            InsertDateTime = DateTime.Now,
                            IsSent = false,
                            IsInvalid = false,
                            RfCardNumber = item.RfCardNumber,
                            StatusCode = 0,
                            VerificationStyle = null,
                        };
                        HardwareEventPublisher.Instance.PublishAttendance(currentAttendance);
                    }
                }
                else
                {
                    // تردد نامجاز
                    var currentRecord = new DtoInvalidAttendance
                    {
                        AttendanceDateTime = item.AttendanceDateTime.ToDateTimeFromEpochMillisecondTime(true),
                        EmployeeNumber = item.EmployeeNumber,
                        StatusCode = 0,
                        DeviceNumber = deviceInfo.DeviceNumber,
                        VerificationStyle = null,
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                        RfCardNumber = item.RfCardNumber,
                        Reason = GetAuthFailReason(item.VerificationStatus),
                        DoorId = item.DoorId,
                    };
                    HardwareEventPublisher.Instance.PublishInvalidAttendance(currentRecord);

                }
            }
        }

    }
}