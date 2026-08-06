using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum LogLevelElmoSanatEnumeration : long
	{
		None = 1 << 0,
		AutoCollect = 1 << 1,
        SetDateTime = 1 << 2,
        GetDateTime = 1 << 3,
        GetData = 1 << 4,
        RecordCount = 1 << 5,
        All = long.MaxValue,

    }
}
