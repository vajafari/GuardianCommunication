using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class GetAttendanceImageModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
		public long UserIdOnDevice { get; set; }
		[DataMember]
		public double AttendanceDateTime { get; set; }
	}
}
