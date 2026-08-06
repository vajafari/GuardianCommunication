using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{

	[DataContract]
	public class PadisControllerDeviceAndIoPortModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public PadisControllerIoPortModel IoPort { get; set; }
	}
}
