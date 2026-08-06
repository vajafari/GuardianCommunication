using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk1DeviceHoliday
	{
		public int Id { get; set; }

		public int HolidayGroupNumber { get; set; }

		public DateTime HolidayDate { get; set; }

		public int HollidayDuration { get; set; }

		public bool IsRepeatYearly { get; set; }
	}
}
