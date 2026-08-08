using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum DeviceAttendanceIoRetrieveTypeEnumeration : short
	{
		[EnumMember]
		OnDemand = 1,
		[EnumMember]
		Push = 2,
    }
}
