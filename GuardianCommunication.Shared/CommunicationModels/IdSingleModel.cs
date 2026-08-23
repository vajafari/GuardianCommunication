using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class IdSingleModel
    {
		[DataMember]
		public Guid Id { get; set; }
	}
}
