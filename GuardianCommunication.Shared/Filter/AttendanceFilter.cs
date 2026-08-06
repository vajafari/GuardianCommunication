using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
	public class AttendanceFilter
	{

		public List<long> Ids { get; set; }

		public List<long> EmployeeNumbers { get; set; }

		public DateTime? AttendanceDate { get; set; }

		public DateTime? AttendanceDateFrom { get; set; }

		public DateTime? AttendanceDateTo { get; set; }

		public bool? IsSent { get; set; }

		public bool? IsHooked { get; set; }

		public List<int> DeviceNumbers { get; set; }


	}
}
