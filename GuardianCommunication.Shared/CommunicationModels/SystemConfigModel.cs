using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SystemConfigModel
	{

		[DataMember]
		public string KarnamaAuthorizationToken { get; set; }
		[DataMember]
		public string KarnamaServiceUrl { get; set; }
		[DataMember]
		public int AutomaticCollectAttendanceTimerInterval { get; set; }
		[DataMember]
		public int OnlineMonitoringDevicesIntervalFromLastDataToReset { get; set; }
        [DataMember]
        public int OnlineMonitoringDevicesSleepAfterPingInSecond { get; set; }
        [DataMember]
        public int OnlineMonitoringDevicesPingTimeoutInMillisecond { get; set; }
		[DataMember]
        public int OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond { get; set; }
		[DataMember]
        public bool IsNetworkPingActive { get; set; }
		[DataMember]
		public int AttendanceSendToKarnamaTimerInterval { get; set; }
		[DataMember]
		public int AttendanceHookTimerInterval { get; set; }
        [DataMember]
        public int AttendanceSendToKarnamaTimerRecordCount { get; set; }
		[DataMember]
		public int AttendanceHookTimerRecordCount { get; set; }
		[DataMember]
		public int ResetRetryCountDayBefore { get; set; }
		[DataMember]
		public int OnlineDeviceTimerInterval { get; set; }
		[DataMember]
		public int AttendanceRegisterInterval { get; set; }
		[DataMember]
		public int AttendanceRegisterIntervalForParking { get; set; }
		[DataMember]
		public int AttendanceRegisterIntervalForTimeAttendance { get; set; }
		[DataMember]
		public int AttendanceRegisterIntervalForAccessControl { get; set; }
		[DataMember]
		public bool KeepCommandAfterResponse { get; set; }
		
		#region ZK
		
		[DataMember]
		public string ZkPushServerIp { get; set; }
		[DataMember]
		public int ZkPushServerPort { get; set; }
		[DataMember]
		public int MaxZkCommandCount { get; set; }
		[DataMember]
		public int MaxRetryForZkOtherCommand { get; set; }
		[DataMember]
		public int MaxRetryForZkUserCommands { get; set; }
		[DataMember]
		public int ZkDeadlineInMinutes { get; set; }
		
		#endregion

		#region PW

		
		[DataMember]
		public int PwDeadlineInMinutes { get; set; }
		[DataMember]
		public int MaxRetryForPwUserCommands { get; set; }
		[DataMember]
		public int MaxRetryForPwOtherCommands { get; set; }

		#endregion

		#region Suprema SDK 1
		
		[DataMember]
		public int SupremaSdk1DeadlineInMinutes { get; set; }
		[DataMember]
		public int MaxRetryForSupremaSdk1UserCommands { get; set; }
		[DataMember]
		public int MaxRetryForSupremaSdk1OtherCommands { get; set; }
		[DataMember]
		public int SupremaSdk1ServerMaxConnection { get; set; }
		[DataMember]
		public int SupremaSdk1ServerPort { get; set; }
		
		#endregion

		#region Suprema SDK 2
		
		[DataMember]
		public int SupremaSdk2ServerPort { get; set; }
		[DataMember]
		public int SupremaSdk2ServerReconnectTimerInterval { get; set; }
		[DataMember]
		public int SupremaSdk2DeadlineInMinutes { get; set; }
		[DataMember]
		public int MaxRetryForSupremaSdk2UserCommands { get; set; }
		[DataMember]
		public int MaxRetryForSupremaSdk2OtherCommands { get; set; }

		#endregion

		#region Virdi

		[DataMember]
		public int VirdiDeadlineInMinutes { get; set; }
		[DataMember]
		public int VirdiServerPort { get; set; }
		[DataMember]
		public int MaxRetryForVirdiUserCommands { get; set; }
		[DataMember]
		public int MaxRetryForVirdiOtherCommands { get; set; }

        #endregion

    }
}
