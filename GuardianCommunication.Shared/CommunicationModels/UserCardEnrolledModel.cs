using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class UserCardEnrolledModel
    {
		[DataMember]
	    public string Card { get; set; }
		[DataMember]
	    public Guid DeviceId { get; set; }
        [DataMember]
        public long UserIdOnDevice { get; set; }
    }
}
