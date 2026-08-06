using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk2DeviceHoliday
	{
		public int Id { get; set; }

		public int HolidayGroupNumber { get; set; }

		public DateTime HolidayDate { get; set; }

		public bool IsRepeatYearly { get; set; }
	}
}
