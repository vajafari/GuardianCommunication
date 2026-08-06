using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoTimyHoliday
    {
        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int DayTimezoneIndex { get; set; }
    }

}
