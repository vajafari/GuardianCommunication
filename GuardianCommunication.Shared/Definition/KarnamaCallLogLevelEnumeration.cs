using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum KarnamaCallLogLevelEnumeration : long
	{
		None = 1 << 0,
        DeviceStatistics = 1 << 1,
        EventLog = 1 << 2,
        MetalDetectorLog = 1 << 2,
        XRayLog = 1 << 3,
        SendAttendanceImage = 1 << 4,
        FetchBasicResourceData = 1 << 5,
        SendAttendanceBulk = 1 << 6,
        SubmitIoEvent = 1 << 7,
        SubmitSeverMatching = 1 << 8,
        ChangeConnectionStatus = 1 << 9,
        UserEnrollment = 1 << 10,
        PlateDetection = 1 << 11,
        SelfAttendance = 1 << 12,
        SelfPrintResult = 1 << 13,
        GetMealOrder = 1 << 14,
        CameraCarAccessData = 1 << 15,
        GeneralInfo = 1 << 16,
        SubmitInvalidIoEvent = 1 << 17,

        All = long.MaxValue

	}
}
