using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelXRayEnumeration : long
    {
        None = 1 << 0,
        DeviceList = 1 << 1,
        NewFile = 1 << 2,
        Publish = 1 << 3,
        All = long.MaxValue,


    }
}
