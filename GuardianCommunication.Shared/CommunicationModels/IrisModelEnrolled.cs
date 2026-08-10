using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class IrisModelEnrolled
    {
        [DataMember]
        public UserIrisModel IrisInfo { get; set; }

        [DataMember]
        public Guid DeviceId { get; set; }
    }
}
