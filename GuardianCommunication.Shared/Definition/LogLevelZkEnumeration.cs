using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelZkEnumeration : long
    {
        None = (((long)1) << 0),
        AutoCollect = (((long)1) << 1),
        AgentCreate = (((long)1) << 2),
        AgentStartStopMonitoring = (((long)1) << 3),
        AgentOpenDoor = (((long)1) << 4),
        AgentLog = (((long)1) << 5),
        AgentPingReset = (((long)1) << 6),
        AgentPingNotSuccess = (((long)1) << 7),
        AgentLastDataTimer = (((long)1) << 8),
        OnDemandSetDateTime = (((long)1) << 9),
        OnDemandGetDateTime = (((long)1) << 10),
        OnDemandRebootDevice = (((long)1) << 11),
        ErrorCodes = (((long)1) << 12),
        OnDemandFunctionTitle = (((long)1) << 13),
        OnDemandFirmware = (((long)1) << 14),
        OnDemandClearData = (((long)1) << 15),
        OnDemandGetData = (((long)1) << 16),
        OnDemandStatistics = (((long)1) << 17),
        OnDemandDeleteUser = (((long)1) << 18),
        OnDemandGetUser = (((long)1) << 19),
        OnDemandSetUser = (((long)1) << 20),
        OnDemandScan = (((long)1) << 21),
        OnDemandDoorControl = (((long)1) << 22),
        ServerSetDeviceList = (((long)1) << 23),
        ServerPushBufferTextDataFromDevice = (((long)1) << 24),
        ServerPushBufferTextGetRequest = (((long)1) << 25),
        ServerPushBufferTextCommandResponse = (((long)1) << 26),
        ServerPushAttendanceImage = (((long)1) << 27),
        ServerPushConfig = (((long)1) << 28),
        ServerLogWholeMessageOnException = (((long)1) << 29),
        LogInvalidDeviceData = (((long)1) << 30),
        LogInvalidDevice = (((long)1) << 31),
        LogGetCommandTimersElapsed = (((long)1) << 32),
        ServerPushLogAttendanceString = (((long)1) << 33),

        All = (long)(long.MaxValue),

    }
}
