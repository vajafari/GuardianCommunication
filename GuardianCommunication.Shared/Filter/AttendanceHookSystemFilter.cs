using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
	public class AttendanceHookSystemFilter
	{
		public List<long> AttendanceIds { get; set; }
		public List<int> HookSystemIds { get; set; }
		public bool? IsSent { get; set; }
		public int? RetryCountFrom { get; set; }
		public int? RetryCountTo { get; set; }
	}
}
