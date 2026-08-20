using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
	public class AttendanceFilter
	{
		public List<Guid> Ids { get; set; }
        
		public bool? IsHooked { get; set; }
	}
}
