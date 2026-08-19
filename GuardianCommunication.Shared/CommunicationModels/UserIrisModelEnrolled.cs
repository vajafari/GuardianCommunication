using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserIrisModelEnrolled
    {
        [DataMember]
        public UserIrisModel IrisInfo { get; set; }

        [DataMember]
        public Guid DeviceId { get; set; }
    }
}
