using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserIdOnDeviceAndDeviceIdModel
    {
        [DataMember]
        public Guid DeviceId { get; set; }
        [DataMember]
        public long UserIdOnDevice { get; set; }
    }
}
