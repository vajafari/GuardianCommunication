using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[Flags]
	[DataContract]
	public enum SdkVersionEnumeration : short
	{
		[EnumMember]
		SdkVersion1 = 1,
		[EnumMember]
		SdkVersion2 = 2
	}
}
