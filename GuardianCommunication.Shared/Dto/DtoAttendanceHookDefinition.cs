using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoAttendanceHookDefinition : DtoDatabaseEntityBase
    {
        public Guid AttendanceId { get; set; }
		public Guid HookDefinitionId { get; set; }
		public bool IsSent { get; set; }
		public int RetryCount { get; set; }
		public DateTime? SentTime { get; set; }
	}
}
