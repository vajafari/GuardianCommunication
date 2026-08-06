namespace GuardianCommunication.Shared.Dto
{
	public class DtoFailedCommandStatistics
	{
		public int DeviceNumber { get; set; }

		public int MaxAttemptCommandCount { get; set; }

		public int NotSendCommandCounts { get; set; }
	}
}
