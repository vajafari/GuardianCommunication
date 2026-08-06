using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class TimeZoneIntervalModel
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public int TimeZoneNumber { get; set; }
		[DataMember]
		public int StartTime { get; set; }
		[DataMember]
		public int EndTime { get; set; }
		[DataMember]
		public TimeZoneDayTypeEnumeration DayType { get; set; }
	}
}
