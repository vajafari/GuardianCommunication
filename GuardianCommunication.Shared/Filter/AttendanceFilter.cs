using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
	public class AttendanceFilter
	{
		public List<Guid> Ids { get; set; }

		public bool? IsHooked { get; set; }

		public DateTime? AttendanceDateFrom { get; set; }

		public DateTime? AttendanceDateTo { get; set; }

		public List<Guid> DeviceIds { get; set; }

		public List<long> UsersIdOnDevice { get; set; }

		public bool? IsSentToGuardian { get; set; }
	}
}
