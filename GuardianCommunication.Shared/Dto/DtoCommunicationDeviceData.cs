using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoCommunicationDeviceData
    {
        public int DeviceNumber { get; set; }
        public int DeviceTypeCode { get; set; }
        public int DeviceTypeNumber { get; set; }
        public EnrollStandardEnumeration EnrollStandardEnum { get; set; }
        public ProducerEnumeration ProducerEnum { get; set; }
        public DoorTypeEnumeration DoorTypeEnum { get; set; }
        public SdkVersionEnumeration SdkVersionEnum { get; set; }
        public ConnectionTypeEnumeration ConnectionTypeEnum { get; set; }
        public DeviceConnectionModeEnumeration ConnectionMode { get; set; }
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        public bool IsMasterDevice { get; set; }
        public bool HasVisibleLight { get; set; }
        public bool HasPalm { get; set; }
        public bool HasIris { get; set; }
        public bool AutomaticDataCollect { get; set; }
        public bool IsOldVersion { get; set; }
        public DeviceSettingsEnumeration DeviceSettings { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public bool SendProfileImage { get; set; }
        public bool HasSoftwareAccessControl { get; set; }
        public bool OnlineMonitoringMode { get; set; }
        public bool JustDoorControl { get; set; }
        public bool IsHookActive { get; set; }
        public int? ComPort { get; set; }
        public BaudRateEnumeration? BuadRate { get; set; }
        public string Ip { get; set; }
        public int? TcpPort { get; set; }
        public string CommunicationPassword { get; set; }
        public int ConnectTimeout { get; set; }
        public bool HasFace { get; set; }
        public bool HasFinger { get; set; }
        public bool HasRfCard { get; set; }
        public bool HasAttendanceValidationCheck { get; set; }
        public string SerialNumber { get; set; }
        public int PwConnectionTimeout { get; set; }
        public int PwPcPort { get; set; }
        public int PwMaxRetry { get; set; }
        public bool PwAcceptValidList { get; set; }
        public DtoCommunicationDeviceTimeSettings TimeSetting { get; set; }
    }
}
