using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class FingerEnrolledModel
    {
	    [DataMember]
	    public UserFingerModel FingerInfo { get; set; }
	    [DataMember]
	    public Guid UserId { get; set; }
    }
}
