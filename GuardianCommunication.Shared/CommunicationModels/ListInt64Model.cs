using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	
	[DataContract]
	public class ListInt64Model
	{
		[DataMember]
		public List<long> Items { get; set; }
	}
}
