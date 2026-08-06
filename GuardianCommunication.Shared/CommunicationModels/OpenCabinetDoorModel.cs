using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class OpenCabinetDoorModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public int CabinetNumber { get; set; }
	}
}
