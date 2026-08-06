using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum LogLevelSuprema2Enumeration : long
	{
		None = 1 << 0,
        AutoCollect = 1 << 1,
        ErrorCodes = 1 << 2,
        SetDateTime = 1 << 3,
        GetDateTime = 1 << 4,
        SerialNumber = 1 << 5,
        Reboot = 1 << 6,
        Firmware = 1 << 7,
        FunctionTitle = 1 << 8,
        ClearData = 1 << 9,
        GetData = 1 << 10,
        Statistics = 1 << 11,
        DeleteUser = 1 << 12,
        GetUser = 1 << 13,
        Scan = 1 << 14,
        SetUser = 1 << 15,
        ServerSetDeviceList = 1 << 16,
        ServerConnection = 1 << 17,
        ServerCommandParams = 1 << 18,
        ServerCommandResult = 1 << 19,
        ServerCommandSend = 1 << 20,
        ServerReconnectError = 1 << 21,
        ServerCommandSendException = 1 << 22,
        ServerRealTimeAttendance = 1 << 23,
        ServerRealTimeLog = 1 << 24,
        OpenDoor = 1 << 25,
        CloseDoor = 1 << 26,
        All = long.MaxValue,

    }
}
