using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1DeviceHolidayModel
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int HolidayGroupNumber { get; set; }

		[DataMember]
		public double HolidayDate { get; set; }

		[DataMember]
		public int HollidayDuration { get; set; }

		[DataMember]
		public bool IsRepeatYearly { get; set; }

	}
}
