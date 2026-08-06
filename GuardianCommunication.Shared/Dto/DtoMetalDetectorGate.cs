using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoMetalDetectorGate
	{
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Title { get; set; }
        public int ZoneCount { get; set; }
        public MetalDetectorGateTypeEnumeration DeviceType { get; set; }
        public string DeviceIp { get; set; }
        public int? TcpPort { get; set; }
        public int AreaNumber { get; set; }
        public MetalDetectorGateConnectionModeEnumeration ConnectionMode { get; set; }
        public bool IsActive { get; set; }
    }

}
