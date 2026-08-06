using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1AccessGroupModel
	{
		[DataMember]
		public int AccessGroupNumber { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public string Description { get; set; }

		public List<SupremaSdk1AccessGroupDoorTimezoneModel> DoorTimezones { get; set; }
	}
}
