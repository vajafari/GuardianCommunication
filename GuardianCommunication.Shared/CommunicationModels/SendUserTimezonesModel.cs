using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SendUserTimezonesModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }

		[DataMember]
		public long EmployeeNumber { get; set; }
		
		[DataMember]
		public List<int> TimeZoneNumbers { get; set; }
	}
}
