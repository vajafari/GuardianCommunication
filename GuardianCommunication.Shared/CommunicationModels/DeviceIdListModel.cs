using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceIdListModel
	{
		[DataMember]
		public List<Guid> DeviceIds { get; set; }
	}
}
