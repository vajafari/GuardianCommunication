using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ReadoutModel
	{
		[DataMember]
		public List<Guid> DeviceIds { get; set; }
		[DataMember]
		public List<long> UsersIdOnDevice { get; set; }
		[DataMember]
		public DateTime StartDate { get; set; }
		[DataMember]
		public DateTime EndDate { get; set; }
		[DataMember]
		public bool? IsSentToGuardian { get; set; }
	}
}
