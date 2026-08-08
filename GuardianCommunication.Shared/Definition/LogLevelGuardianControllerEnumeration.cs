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
        PushLogInvalidDevice = ((long)1) << 7,
        PushLogCommandContent = ((long)1) << 8,
        ChangeDoorStatus = ((long)1) << 9,
        ServerSetDeviceList = ((long)1) << 10,
        LogGetCommandTimersElapsed = ((long)1) << 11,
        ServerRealTimeLog = ((long)1) << 12,
        GrpcCreateAgent = ((long)1) << 13,
        GrpcAgentSendCommand = ((long)1) << 14,
        GrpcAgentReceiveCommand = ((long)1) << 15,
        GrpcServerConnect = ((long)1) << 16,
        GrpcServerCommandFetch = ((long)1) << 17,
        GrpcServerCommandFetchResult = ((long)1) << 18,
        GrpcServerCommandBeforeSend = ((long)1) << 19,
    }
}
