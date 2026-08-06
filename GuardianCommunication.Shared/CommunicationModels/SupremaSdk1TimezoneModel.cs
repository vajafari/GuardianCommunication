using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1TimezoneModel
	{

		[DataMember]
		public int TimezoneNumber { get; set; }

		[DataMember]
		public string TimezoneTitle { get; set; }

		[DataMember]
		public string TimezoneDescription { get; set; }

		[DataMember]
		public int? HolidayGroupNumber1 { get; set; }

		[DataMember]
		public int? HolidayGroupNumber2 { get; set; }

		[DataMember]
		public List<SupremaSdk1TimezoneElementModel> Elements { get; set; }

	}
}
