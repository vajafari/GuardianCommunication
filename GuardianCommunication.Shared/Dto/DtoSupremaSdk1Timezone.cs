using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk1Timezone
	{
		public int TimezoneNumber { get; set; }

		public string TimezoneTitle { get; set; }

		public string TimezoneDescription { get; set; }

		public int? HolidayGroupNumber1 { get; set; }

		public int? HolidayGroupNumber2 { get; set; }

		public List<DtoSupremaSdk1TimezoneElement> Elements { get; set; }
	}
}
