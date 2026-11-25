using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelGuardianControllerEnumeration : long
    {
        None = ((long)1) << 0,
        LogGeneralMethodsCall = ((long)1) << 1,
        ClearData = ((long)1) << 2,
        ReadLog = ((long)1) << 3,
        UserInfo = ((long)1) << 4,
        ErrorCodes = ((long)1) << 5,
        AccessControl = ((long)1) << 6,
        ChangeDoorStatus = ((long)1) << 7,
        ServerSetDeviceList = ((long)1) << 8,
        LogGetCommandTimersElapsed = ((long)1) << 9,
        ServerRealTimeLog = ((long)1) << 10,
        GrpcCreateAgent = ((long)1) << 11,
        GrpcAgentSendCommand = ((long)1) << 12,
        GrpcAgentReceiveCommand = ((long)1) << 13,
        GrpcServerConnect = ((long)1) << 14,
        GrpcServerCommandFetch = ((long)1) << 15,
        GrpcServerCommandFetchResult = ((long)1) << 16,
        GrpcServerCommandBeforeSend = ((long)1) << 17,
    }
}
