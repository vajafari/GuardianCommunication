using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class OpenDoorModel
	{

		[DataMember]
        public Guid DeviceId { get; set; }

		[DataMember]
        public Guid DoorId { get; set; }
		
        [DataMember]
        public int? DelayInSecond { get; set; }
	}
}
