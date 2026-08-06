using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceDoorModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }

		[DataMember]
		public ZkDeviceDoorModel ZkDeviceDoorInfo { get; set; }
	}
}
