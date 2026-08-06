using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class PadisControllerDeviceAndWiegandModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public PadisControllerWiegandModel Wiegand { get; set; }
	}
}
