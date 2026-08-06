using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1DeviceHolidayGroupModel
	{
		[DataMember]
		public int GroupNumber { get; set; }

		[DataMember]
		public string GroupName { get; set; }

		[DataMember]
		public string GroupDescription { get; set; }

		[DataMember]
		public List<SupremaSdk1DeviceHolidayModel> Holidays { get; set; }

	}
}
