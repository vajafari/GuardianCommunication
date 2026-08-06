using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1AccessGroupDoorTimezoneModel
	{
		[DataMember]
		public int AccessGroupNumber { get; set; }
		[DataMember]
		public int DeviceDoorId { get; set; }
		[DataMember]
		public int TimezoneNumber { get; set; }

	}
}
