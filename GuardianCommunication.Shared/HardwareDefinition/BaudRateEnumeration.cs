using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	[DataContract]
	public enum BaudRateEnumeration
	{
		[EnumMember]
		BaudRate2400 = 2400,
		[EnumMember]
		BaudRate4800 = 4800,
		[EnumMember]
		BaudRate9600 = 9600,
		[EnumMember]
		BaudRate19200 = 19200,
		[EnumMember]
		BaudRate38400 = 38400,
		[EnumMember]
		BaudRate57600 = 57600,
		[EnumMember]
		BaudRate115200 = 115200,
		[EnumMember]
		BaudRate230400 = 230400,
		[EnumMember]
		BaudRate460800 = 460800,
		[EnumMember]
		BaudRate921600 = 921600
	}
}
