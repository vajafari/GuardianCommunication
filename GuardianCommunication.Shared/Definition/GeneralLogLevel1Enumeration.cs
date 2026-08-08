using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum GeneralLogLevel1Enumeration : long
    {
        None = 1 << 0,
        StartupLog = 1 << 1,
        LogAttendanceSaveProcess = 1 << 2,
        LogCameraFetch = 1 << 3,
        LogCameraCacheProcessFetch = 1 << 4,
        LogDeviceFetch = 1 << 5,
        LogDeviceCacheProcessFetch = 1 << 6,
        LogHookRestRequestAndResult = 1 << 7,
        LogHookProcess = 1 << 8,
        LogDeleteUnsentCommandTask = 1 << 9,
        HardwarePublisherSendAttendanceImage = 1 << 10,
        HardwarePublisherAttendanceProcess = 1 << 11,
        HardwarePublisherCameraEvent = 1 << 12,
        HardwarePublisherInvalidAttendanceProcess = 1 << 13,
        AttendanceHookTask = 1 << 14,
        AttendanceSendToGuardianTask = 1 << 15,
        SendOnlineStatusTimer = 1 << 16,
        LogControllerDeviceDoorFetch = 1 << 17,
        LogAttendanceDoorCacheProcessFetch = 1 << 18,
        All = long.MaxValue,
       
    }
}
