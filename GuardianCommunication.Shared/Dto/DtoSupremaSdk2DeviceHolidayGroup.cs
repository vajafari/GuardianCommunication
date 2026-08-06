using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk2DeviceHolidayGroup
	{
		public int GroupNumber { get; set; }

		public string GroupName { get; set; }

		public string GroupDescription { get; set; }

		public List<DtoSupremaSdk2DeviceHoliday> Holidays { get; set; }

	}
}
