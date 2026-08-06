using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class DeviceCommunicationModel
    {
        [DataMember]
        public int DeviceNumber { get; set; }
        [DataMember]
        public int DeviceTypeCode { get; set; }
        [DataMember]
        public int DeviceTypeNumber { get; set; }
        [DataMember]
        public EnrollStandardEnumeration EnrollStandardEnum { get; set; }
        [DataMember]
        public ProducerEnumeration ProducerEnum { get; set; }
        [DataMember]
        public DoorTypeEnumeration DoorTypeEnum { get; set; }
        [DataMember]
        public SdkVersionEnumeration SdkVersionEnum { get; set; }
        [DataMember]
        public ConnectionTypeEnumeration ConnectionTypeEnum { get; set; }
        [DataMember]
        public DeviceConnectionModeEnumeration ConnectionMode { get; set; }
        [DataMember]
        public bool IsMasterDevice { get; set; }
        [DataMember]
        public bool HasVisibleLight { get; set; }
        [DataMember]
        public bool HasPalm { get; set; }
        [DataMember]
        public bool HasIris { get; set; }
        [DataMember]
        public bool AutomaticDataCollect { get; set; }
        [DataMember]
        public bool IsOldVersion { get; set; }
        [DataMember]
        public DeviceSettingsEnumeration DeviceSettings { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration IoType { get; set; }
        [DataMember]
        public bool SendProfileImage { get; set; }
        [DataMember]
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        [DataMember]
        public bool HasSoftwareAccessControl { get; set; }
        [DataMember]
        public bool OnlineMonitoringMode { get; set; }
        [DataMember]
        public bool JustDoorControl { get; set; }
        [DataMember]
        public bool IsHookActive { get; set; }
        [DataMember]
        public int? ComPort { get; set; }
        [DataMember]
        public BaudRateEnumeration? BuadRate { get; set; }
        [DataMember]
        public string Ip { get; set; }
        [DataMember]
        public int? TcpPort { get; set; }
        [DataMember]
        public string CommunicationPassword { get; set; }
        [DataMember]
        public int ConnectTimeout { get; set; }
        [DataMember]
        public bool HasFace { get; set; }
        [DataMember]
        public bool HasFinger { get; set; }
        [DataMember]
        public bool HasRfCard { get; set; }
        [DataMember]
        public bool HasAttendanceValidationCheck { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public int PwConnectionTimeout { get; set; }
        [DataMember]
        public int PwPcPort { get; set; }
        [DataMember]
        public int PwMaxRetry { get; set; }
        [DataMember]
        public bool PwAcceptValidList { get; set; }
        [DataMember]
        public TimeZonesEnumeration TimeZone { get; set; }
        [DataMember]
        public bool IsDaylightActive { get; set; }
        [DataMember]
        public int DaylightStart { get; set; }
        [DataMember]
        public int DaylightEnd { get; set; }
        [DataMember]
        public int DaylightChangeTimeInSeconds { get; set; }
    }
}
