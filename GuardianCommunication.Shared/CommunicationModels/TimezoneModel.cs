using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class TimezoneModel
	{
		[DataMember]
		public int TimeZoneNumber { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<TimeZoneIntervalModel> Intervals { get; set; }
	}
}
