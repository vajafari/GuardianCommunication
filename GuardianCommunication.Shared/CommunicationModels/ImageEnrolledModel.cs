using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class ImageEnrolledModel
    {
		[DataMember]
	    public UserFaceModel FaceInfo { get; set; }

		[DataMember]
	    public Guid DeviceId { get; set; }
    }
}
