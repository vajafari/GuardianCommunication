using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class OpenDoorModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }

		[DataMember]
		public DeviceDoorBaseModel DoorInfo { get; set; }
	}
}
