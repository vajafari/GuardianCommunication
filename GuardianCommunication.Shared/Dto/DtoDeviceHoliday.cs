using System;

namespace GuardianCommunication.Shared.Dto
{

	public class DtoDeviceHoliday
	{
		public int Id { get; set; }
		public int DeviceNumber { get; set; }
		public int TimeZoneNumber { get; set; }
		public int HolidayIndex { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

}
