using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ReadoutModel
	{
		[DataMember]
		public List<int> DeviceNumbers { get; set; }
		[DataMember]
		public List<long> EmployeeNumbers { get; set; }
		[DataMember]
		public double StartDate { get; set; }
		[DataMember]
		public double EndDate { get; set; }
		[DataMember]
		public bool? IsSent { get; set; }
	}
}
