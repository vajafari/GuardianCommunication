namespace GuardianCommunication.Shared.Dto
{
    public class DtoVirdiAccessTime
    {
        public string Code { get; set; }
        public VirdiDayOfWeekEnumeration DayOfWeekEnum { get; set; }
        public string TimezoneCode { get; set; }
        public string HolidayCode { get; set; }
    }
}
