using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum DoorTypeEnumeration : short
	{
		[EnumMember]
		NotSupport = 0,

		[EnumMember]
		Standalone = 1,

		[EnumMember]
		TwoDoor = 2,

		[EnumMember]
		ThreeDoor = 4,
	}
}
