using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	
	[DataContract]
	public class ListInt32Model
	{
		[DataMember]
		public List<int> Items { get; set; }
	}
}
