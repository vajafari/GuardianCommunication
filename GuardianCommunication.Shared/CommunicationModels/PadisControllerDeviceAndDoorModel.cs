using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{

	[DataContract]
	public class PadisControllerDeviceAndDoorModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public PadisControllerDeviceDoorModel Door { get; set; }
	}
}
