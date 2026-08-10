using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class DeviceEventLogModel
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public Guid DeviceId { get; set; }
        [DataMember]
        public ProducerEnumeration Producer { get; set; }
        [DataMember]
        public SdkVersionEnumeration SdkVersion { get; set; }
        [DataMember]
        public double EventDateTime { get; set; }
        [DataMember]
        public int EventCode { get; set; }
        [DataMember]
        public long? UserIdOnDevice { get; set; }
    }
}
