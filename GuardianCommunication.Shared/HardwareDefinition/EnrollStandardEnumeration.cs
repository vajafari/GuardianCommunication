using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
	/// <summary>
	/// تعیین کننده ی استاندارد نمونه برداری از اثر انگشت و یا فیس
	/// </summary>
	[DataContract]
	public enum EnrollStandardEnumeration : short
	{
		[EnumMember]
		DeviceStandard = 1,
	}
}
