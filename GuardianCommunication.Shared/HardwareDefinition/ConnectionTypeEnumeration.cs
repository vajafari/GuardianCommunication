using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum ConnectionTypeEnumeration : short
	{
		[EnumMember]
		Rs232 = 1,
		[EnumMember]
		Ethernet = 2,
		[EnumMember]
		Rs485 = 3
	}
}
