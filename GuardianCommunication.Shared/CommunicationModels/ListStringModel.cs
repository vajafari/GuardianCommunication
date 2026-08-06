using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	
	[DataContract]
	public class ListStringModel
	{
		[DataMember]
		public List<string> Items { get; set; }
	}
}
