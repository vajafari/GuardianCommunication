using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class GetAttendanceImageModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public long EmployeeNumber { get; set; }
		[DataMember]
		public double AttendanceDateTime { get; set; }
	}
}
