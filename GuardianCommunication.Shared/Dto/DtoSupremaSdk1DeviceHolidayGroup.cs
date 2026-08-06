using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk1DeviceHolidayGroup
	{
		public int GroupNumber { get; set; }

		public string GroupName { get; set; }

		public string GroupDescription { get; set; }

		public List<DtoSupremaSdk1DeviceHoliday> Holidays { get; set; }

	}
}
