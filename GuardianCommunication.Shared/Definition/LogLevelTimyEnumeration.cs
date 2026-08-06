using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum LogLevelTimyEnumeration : long
	{

		None = 1 << 0,
        AutoCollect = 1 << 1,
        SetDateTime = 1 << 2,
        GetDateTime = 1 << 3,
        GetSerialNumber = 1 << 4,
        RebootDevice = 1 << 5,
        GetData = 1 << 6,
        DeleteUser = 1 << 7,
        GetUser = 1 << 8,
        SetUser = 1 << 9,
        GetStatistics = 1 << 10,
        ServerSetDeviceList = 1 << 11,
        ServerDeviceConnection = 1 << 12,
        ServerMessage = 1 << 13,
        ErrorCodes = 1 << 14,
        ServerRealTimeAttendance = 1 << 15,
        OpenDoor = 1 << 16,
        Scan = 1 << 17
    }
}
