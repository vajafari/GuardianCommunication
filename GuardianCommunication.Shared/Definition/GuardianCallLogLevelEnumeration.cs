using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum GuardianCallLogLevelEnumeration : long
	{
		None = 1 << 0,
        GeneralInfo = 1 << 1,
        EventLog = 1 << 2,
        SendAttendanceImage = 1 << 3,
        FetchBasicResourceData = 1 << 4,
        SubmitIoEvent = 1 << 5,
        SubmitSeverMatching = 1 << 6,
        ChangeConnectionStatus = 1 << 7,
        UserEnrollment = 1 << 8,
        SubmitInvalidIoEvent = 1 << 9,
        DeviceStatistics = 1 << 10,

        All = long.MaxValue

	}
}
