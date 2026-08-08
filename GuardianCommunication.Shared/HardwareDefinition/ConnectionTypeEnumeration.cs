using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum ConnectionTypeEnumeration : short
	{
		[EnumMember]
		Ethernet = 1,
	}
}
