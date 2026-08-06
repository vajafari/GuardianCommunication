using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoSupremaSdk2AccessSchedule
    {
        public int AccessScheduleNumber { get; set; }

        public string Title { get; set; }

        public string TimezoneDescription { get; set; }

        public int? HolidayGroupNumber1 { get; set; }

        public int? HolidayGroupNumber2 { get; set; }

        public int? HolidayGroupNumber3 { get; set; }

        public int? HolidayGroupNumber4 { get; set; }

        public List<DtoSupremaSdk2AccessScheduleElement> Elements { get; set; }
    }
}
