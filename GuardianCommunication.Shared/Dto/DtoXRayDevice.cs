using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoXRayDevice
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AreaNumber { get; set; }
        public string DeviceId { get; set; }
        public XRayDeviceTypeEnumeration DeviceType { get; set; }
        public XRayDeviceConnectionModeEnumeration ConnectionMode { get; set; }
        public string DeviceIp { get; set; }
        public int? TcpPort { get; set; }
        public string FilePath { get; set; }
        public bool IsActive { get; set; }
    }

}
