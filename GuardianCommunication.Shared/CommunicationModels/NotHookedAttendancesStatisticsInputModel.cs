using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class NotHookedAttendancesStatisticsInputModel
	{
		[DataMember]
		public List<long> EmployeeNumbers { get; set; }
	}
}
