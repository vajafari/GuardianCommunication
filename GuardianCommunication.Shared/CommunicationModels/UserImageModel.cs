using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class UserImageModel
    {
	    [DataMember]
		public string UserImage { get; set; }
		[DataMember]
		public long UserIdOnDevice { get; set; }

    }
}
