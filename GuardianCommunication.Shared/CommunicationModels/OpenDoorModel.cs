using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class OpenDoorModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }

		[DataMember]
		public DeviceDoorBaseModel DoorInfo { get; set; }
	}
}
