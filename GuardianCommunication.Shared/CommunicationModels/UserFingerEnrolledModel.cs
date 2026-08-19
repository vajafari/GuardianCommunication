using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class UserFingerEnrolledModel
    {
	    [DataMember]
	    public UserFingerModel FingerInfo { get; set; }
	    [DataMember]
	    public Guid DeviceId { get; set; }
    }
}
