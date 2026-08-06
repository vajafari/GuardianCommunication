using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ExternalSystemAuthDataModel
	{
		[DataMember]
		public Guid SystemId { get; set; }
		[DataMember]
		public string SecurityToken { get; set; }

	}
}
