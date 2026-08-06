using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class MetalDetectorGateModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string DeviceId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int ZoneCount { get; set; }
        [DataMember]
        public MetalDetectorGateTypeEnumeration DeviceType { get; set; }
        [DataMember]
        public string DeviceIp { get; set; }
        [DataMember]
        public int? TcpPort { get; set; }
        [DataMember]
        public int AreaNumber { get; set; }
        [DataMember]
        public MetalDetectorGateConnectionModeEnumeration ConnectionMode { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}
