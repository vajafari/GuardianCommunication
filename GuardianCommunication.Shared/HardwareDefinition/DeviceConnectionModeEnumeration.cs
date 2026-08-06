using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum DeviceConnectionModeEnumeration : short
	{
		[EnumMember]
		Standalone = 1,
		[EnumMember]
		Push = 2,
	}
}
