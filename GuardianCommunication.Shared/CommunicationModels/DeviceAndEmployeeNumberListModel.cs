using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceAndEmployeeNumberListModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public List<long> EmployeeNumber { get; set; }
	}
}
