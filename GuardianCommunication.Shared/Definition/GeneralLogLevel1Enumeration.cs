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
        LogXRayFetch = 1 << 7,
        LogXRayCacheProcessFetch = 1 << 8,
        LogMetalDetectionGateFetch = 1 << 9,
        LogMetalDetectionGateCacheProcessFetch = 1 << 10,
        LogHookRestRequestAndResult = 1 << 11,
        LogHookProcess = 1 << 12,
        HardwarePublisherSendAttendanceImage = 1 << 13,
        HardwarePublisherAttendanceProcess = 1 << 14,
        HardwarePublisherCameraEvent = 1 << 15,
        ScheduledApiCallAgent = 1 << 16,
        AttendanceHookTask = 1 << 17,
        AttendanceSendToKarnamaTask = 1 << 18,
        SendOnlineStatusTimer = 1 << 19,
        SelfSendMealsTask = 1 << 20,
        LogInvalidIps = 1 << 21,
        HardwarePublisherXRayPublish = 1 << 22,
        LogDeleteUnsentCommandTask = 1 << 23,
        HardwarePublisherInvalidAttendanceProcess = 1 << 24,

        All = long.MaxValue,
        LogControllerDeviceDoorFetch = long.MinValue,
        LogAttendanceDoorCacheProcessFetch = -9223372036854775807,
    }
}
