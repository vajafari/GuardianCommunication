using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1DeviceAndDoorModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public SupremaSdk1DeviceDoorModel Door { get; set; }
	}
}
