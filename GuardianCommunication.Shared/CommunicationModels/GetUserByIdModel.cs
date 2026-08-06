using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class GetUserByIdModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public long EmployeeNumber { get; set; }
		[DataMember]
		public TemplateTypeEnumeration TemplateType { get; set; }

	}
}
