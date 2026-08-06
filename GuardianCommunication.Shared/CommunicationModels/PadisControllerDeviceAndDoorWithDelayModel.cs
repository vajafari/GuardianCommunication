using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class PadisControllerDeviceAndDoorWithDelayModel
    {
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public PadisControllerDeviceDoorModel Door { get; set; }
		[DataMember]
        public int DelayInSecond { get; set; }

    }
}
