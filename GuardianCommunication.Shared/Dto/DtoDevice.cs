using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDevice
    {
        public int DeviceNumber { get; set; }
        public string Title { get; set; }
        public int DeviceTypeNumber { get; set; }
        public string SerialNumber { get; set; }
        public string DevicePassword { get; set; }
        public ConnectionTypeEnumeration ConnectionType { get; set; }
        public string DeviceIp { get; set; }
        public int? TcpPort { get; set; }
        public bool? BaudAdjust { get; set; }
        public BaudRateEnumeration? BaudRate { get; set; }
        public short? ComPort { get; set; }
        public int ConnectTimeout { get; set; }
        public int AreaNumber { get; set; }
        public DeviceConnectionModeEnumeration ConnectionMode { get; set; }
        public bool IsMasterDevice { get; set; }
        public bool HasAttendanceValidationCheck { get; set; }
        public bool HasVisibleLight { get; set; }
        public bool HasPalm { get; set; }
        public bool AutomaticDataCollect { get; set; }
        public bool IsOldVersion { get; set; }
        public DeviceSettingsEnumeration DeviceSettings { get; set; }
        public TimeZonesEnumeration TimeZone { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public bool SendProfileImage { get; set; }
        public bool HasSoftwareAccessControl { get; set; }
        public bool OnlineMonitoringMode { get; set; }
        public bool JustDoorControl { get; set; }
        public bool IsHookActive { get; set; }
        public bool IsActive { get; set; }
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        public int PwConnectionTimeout { get; set; }
        public int PwPcPort { get; set; }
        public int PwMaxRetry { get; set; }
        public bool PwAcceptValidList { get; set; }


        public DtoDeviceTypeSummary DeviceTypeSummary { get; set; }
    }

}
