using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoTimezone
	{
		public int TimeZoneNumber { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public List<DtoTimeZoneInterval> Intervals { get; set; }
	}
}
