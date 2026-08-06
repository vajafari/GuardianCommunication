using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class OpenDoorWithDelayModel
    {
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }

		[DataMember]
		public DeviceDoorBaseModel DoorInfo { get; set; }

		[DataMember]
        public int DelayInSecond { get; set; }
    }
}
