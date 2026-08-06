using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class NotHookedAttendancesStatisticsModel
	{
		[DataMember]
		public long EmployeeNumber { get; set; }
		[DataMember]
		public int AttendanceCount { get; set; }
	}
}
