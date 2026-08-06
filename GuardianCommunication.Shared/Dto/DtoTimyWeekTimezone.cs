using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoTimyWeekTimezone
    {
        public int WeekTimezoneGroupId { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public int DayTimezoneIndex { get; set; }
    }
}
