using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Filter
{
	[DataContract]
	public enum DeviceCommandSortEnumeration
	{
		[EnumMember]
		Id,
		[EnumMember]
		DeviceSerialNumber,
		[EnumMember]
		CommitTime,
		[EnumMember]
		SendTime,
		[EnumMember]
		ResponseTime,
		[EnumMember]
		CommandType,
		[EnumMember]
		Priority,
		[EnumMember]
		EmployeeNumber,
		[EnumMember]
		RetryCount,
		[EnumMember]
		MaxRetry,
		[EnumMember]
		DeviceNumber,
		[EnumMember]
		Deadline,
		[EnumMember]
		ProducerNumber,
		[EnumMember]
		SdkVersion,
		[EnumMember]
		VisiblilityTime,
		[EnumMember]
        Description,

	}
}
