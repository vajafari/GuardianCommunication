using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoTimyDayTimezoneGroup
    {
        public int Id { get; set; }
        public int DeviceIndex { get; set; }
        public string Title { get; set; }

        public List<DtoTimyDayTimezoneInterval> DayTimezoneIntervals { get; set; }

    }
}
