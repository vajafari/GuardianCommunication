using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoTimeZoneInterval
	{
		public int Id { get; set; }
		public int TimeZoneNumber { get; set; }
		public int StartTime { get; set; }
		public int EndTime { get; set; }
		public TimeZoneDayTypeEnumeration DayType { get; set; }
	}
}
