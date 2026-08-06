using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelPwEnumeration : long
    {
        None = 1 << 0,
        AutoCollect = 1 << 1,
        SetDateTime = 1 << 2,
        ChangeDevicePassword = 1 << 3,
        ChangeCommunicationKey = 1 << 4,
        GetUser = 1 << 5,
        SetUser = 1 << 6,
        DeleteUser = 1 << 7,
        ClearData = 1 << 8,
        GetData = 1 << 9,
        ErrorCodes = 1 << 10,
        ConnectProcess = 1 << 11,
        All = long.MaxValue,

    }
}
