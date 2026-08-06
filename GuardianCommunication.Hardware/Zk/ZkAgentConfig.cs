namespace GuardianCommunication.Hardware.Zk
{
	public class ZkAgentConfig
	{
		public int IntervalFromLastDataToReset { get; set; }
		public int TimerCheckLastDataIntervalInMinutes { get; set; }
		public int SleepAfterPingInSecond { get; set; }
		public int PingTimeoutInMillisecond { get; set; }
		public int WaitAfterPingIsConnectedAgainInSecond { get; set; }
		public bool IsNetworkPingActive { get; set; }
	}
}
