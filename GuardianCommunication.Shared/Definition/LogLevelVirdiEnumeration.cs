using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelVirdiEnumeration : long
    {
        None = 1 << 0,
        AutoCollect = 1 << 1,
        ServerSetDeviceList = 1 << 2,
        ServerDeviceConnection = 1 << 3,
        ServerGetTime = 1 << 4,
        ServerCommandFetch = 1 << 6,
        ServerCommandFetchResult = 1 << 7,
        ServerCommandFetchStartSend = 1 << 8,
        ServerGetStatistics = 1 << 9,
        ServerGetData = 1 << 10,
        ServerAccessLog = 1 << 11,
        ServerRealTimeLog = 1 << 12,
        ServerGetUserData = 1 << 13,
        ServerDeleteUser = 1 << 14,
        ServerSetUser = 1 << 15,
        ServerScan = 1 << 16,
        DoorControl = 1 << 17,
        MatchOnServer = 1 << 18,
        SendAccessControlData = 1 << 19,
        All = long.MaxValue,

    }
}
