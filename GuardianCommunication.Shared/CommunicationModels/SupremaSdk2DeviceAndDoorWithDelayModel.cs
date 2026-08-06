using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk2DeviceAndDoorWithDelayModel
    {
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public SupremaSdk2DeviceDoorModel Door { get; set; }
		[DataMember]
        public int DelayInSecond { get; set; }

    }
}
