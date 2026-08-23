using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class IdListModel
	{
		[DataMember]
		public List<Guid> Ids { get; set; }
	}
}
