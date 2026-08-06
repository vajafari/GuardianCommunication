using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoTimyWeekTimezoneGroup
    {
        public int Id { get; set; }
        public int DeviceIndex { get; set; }
        public string Title { get; set; }

        public List<DtoTimyWeekTimezone> Timezones { get; set; }
    }
}
