using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceModel
	{
		[DataMember]
		public int DeviceNumber { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public int DeviceTypeNumber { get; set; }
		[DataMember]
		public string SerialNumber { get; set; }
		[DataMember]
		public string DevicePassword { get; set; }
		[DataMember]
		public ConnectionTypeEnumeration ConnectionType { get; set; }
		[DataMember]
		public string DeviceIp { get; set; }
		[DataMember]
		public int? TcpPort { get; set; }
		[DataMember]
		public bool? BaudAdjust { get; set; }
		[DataMember]
		public BaudRateEnumeration? BaudRate { get; set; }
		[DataMember]
		public short? ComPort { get; set; }
		[DataMember]
		public int ConnectTimeout { get; set; }
		[DataMember]
		public int AreaNumber { get; set; }
		[DataMember]
		public DeviceConnectionModeEnumeration ConnectionMode { get; set; }
		[DataMember]
		public bool IsMasterDevice { get; set; }
		[DataMember]
		public bool HasAttendanceValidationCheck { get; set; }
		[DataMember]
		public bool HasVisibleLight { get; set; }
		[DataMember]
		public bool HasPalm { get; set; }
		[DataMember]
		public bool AutomaticDataCollect { get; set; }
		[DataMember]
		public bool IsOldVersion { get; set; }
        [DataMember]
		public DeviceSettingsEnumeration DeviceSettings { get; set; }
        [DataMember]
        public TimeZonesEnumeration TimeZone { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration IoType { get; set; }
        [DataMember]
        public bool SendProfileImage { get; set; }
        [DataMember]
        public bool HasSoftwareAccessControl { get; set; }
		[DataMember]
        public bool OnlineMonitoringMode { get; set; }
        [DataMember]
        public bool JustDoorControl { get; set; }
		[DataMember]
		public bool IsHookActive { get; set; }
		[DataMember]
		public bool IsActive { get; set; }
		[DataMember]
		public ApplicationTypeEnumeration ApplicationId { get; set; }
		[DataMember]
		public int PwMaxRetry { get; set; }
		[DataMember]
		public int PwConnectionTimeout { get; set; }
		[DataMember]
		public int PwPcPort { get; set; }
		[DataMember]
		public bool PwAcceptValidList { get; set; }


		[DataMember]
		public DeviceTypeSummaryModel DeviceTypeSummary { get; set; }
	}

}
