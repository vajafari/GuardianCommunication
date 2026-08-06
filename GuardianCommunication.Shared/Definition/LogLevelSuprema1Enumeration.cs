using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum LogLevelSuprema1Enumeration : long
	{
		None = 1 << 0,
		AutoCollect = 1 << 1,
        ErrorCodes = 1 << 2,
        SetDateTime = 1 << 3,
        GetDateTime = 1 << 4,
        Reboot = 1 << 5,
        Firmware = 1 << 6,
        SerialNumber = 1 << 7,
        ClearData = 1 << 8,
        GetData = 1 << 9,
        DeleteUser = 1 << 10,
        SetUser = 1 << 11,
        GetUser = 1 << 12,
        GetStatistics = 1 << 13,
        Scan = 1 << 14,
        ServerSetDeviceList = 1 << 15,
        ServerConnection = 1 << 16,
        ServerRealTimeAttendance = 1 << 17,
        ServerRealTimeLog = 1 << 18,
        ServerCommandParams = 1 << 19,
        ServerCommandResult = 1 << 20,
        ServerCommandException = 1 << 21,
        ServerCommandConnectionException = 1 << 22,

        All = long.MaxValue,

    }
}
