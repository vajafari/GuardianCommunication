using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerCalendar
    {
        public int CalendarNumber { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public List<DtoPadisControllerCalendarDetails> Details { get; set; }
    }
}