using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	
	[DataContract]
	public class Int64Model
	{
		[DataMember]
		public long Value { get; set; }
	}
}
