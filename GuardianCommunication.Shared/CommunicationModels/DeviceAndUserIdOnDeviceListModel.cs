using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceAndUserIdOnDeviceListModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
		public List<long> UserIdsOnDevice { get; set; }
	}
}
