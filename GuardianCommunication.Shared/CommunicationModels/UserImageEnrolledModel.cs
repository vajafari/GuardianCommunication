using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserImageEnrolledModel
    {
        [DataMember]
        public UserImageModel UserImage { get; set; }
        [DataMember]
        public Guid DeviceId { get; set; }
    }
}
