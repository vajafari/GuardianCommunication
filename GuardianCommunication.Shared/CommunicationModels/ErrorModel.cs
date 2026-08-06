using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ErrorModel
	{
		[DataMember]
		public int[] ErrorCodes { get; set; }
		[DataMember]
		public string StackTrace { get; set; }
		[DataMember]
		public string AdditionalInfo { get; set; }

	}
}
