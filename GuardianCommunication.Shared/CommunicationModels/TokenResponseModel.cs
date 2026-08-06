using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class TokenResponseModel
	{
		[DataMember]
		public string Token { get; set; }
		[DataMember]
		public string RefreshToken { get; set; }
	}

}
