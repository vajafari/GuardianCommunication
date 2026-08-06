using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceHolidayModel
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int DeviceNumber { get; set; }

		[DataMember]
		public int TimeZoneNumber { get; set; }

		[DataMember]
		public int HolidayIndex { get; set; }

		[DataMember]
		public double StartDate { get; set; }

		[DataMember]
		public double EndDate { get; set; }
	}

}
