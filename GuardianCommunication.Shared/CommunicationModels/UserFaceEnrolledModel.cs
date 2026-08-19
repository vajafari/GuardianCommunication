using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class UserFaceEnrolledModel
    {
	    [DataMember]
	    public UserFaceModel FaceInfo { get; set; }
	    [DataMember]
	    public Guid DeviceId { get; set; }
    }
}
