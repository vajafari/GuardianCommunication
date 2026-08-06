namespace GuardianCommunication.Hardware.Zk
{
	public class ZkPushConfig
	{
		public string PushServerIp { get; set; }
		public int PushServerPort{ get; set; }
		public int MaxZkCommandCount { get; set; }
        public int SleepBetweenSocketsInMillisecond { get; set; }
        public int SleepOnContinueInMillisecond { get; set; }
        public int ReceiveTimeoutInMillisecond { get; set; }
        public int Stamp { get; set; }
        public int OpStamp { get; set; }
        public int PhotoStamp { get; set; }
        public int ErrorDelay { get; set; }
        public int Delay { get; set; }
        public string TransTimes { get; set; }
        public int TransInterval { get; set; }
        public int SyncTime { get; set; }
        public int Realtime { get; set; }
        public int AttendanceLogStamp { get; set; }
        public int OperationLogStamp { get; set; }
        public int AttendancePhotoStamp { get; set; }
        public string MultiBioDataSupport { get; set; }
        public string MultiBioPhotoSupport { get; set; }
        public int WaitForSocketData { get; set; }
        public int IntervalForConsiderDeviceOnlineInSecond { get; set; }
        public int GetCommandTimerIntervalInMillisecond { get; set; }
    }
}
