using System;

namespace GuardianCommunication.Shared.Dto
{
	
	public class DtoAttendanceHookSystem
	{
        public long Id { get; set; }
        public long AttendanceId { get; set; }
		public int HookSystemId { get; set; }
		public bool IsSent { get; set; }
		public int RetryCount { get; set; }
		public DateTime? SentTime { get; set; }
	}
}
