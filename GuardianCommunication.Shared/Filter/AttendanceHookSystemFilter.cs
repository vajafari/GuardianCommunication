using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
	public class AttendanceHookSystemFilter
	{
		public List<Guid> AttendanceIds { get; set; }
		public List<Guid> HookDefinitionIds { get; set; }
		public bool? IsSent { get; set; }
		public int? RetryCountFrom { get; set; }
		public int? RetryCountTo { get; set; }
	}
}
