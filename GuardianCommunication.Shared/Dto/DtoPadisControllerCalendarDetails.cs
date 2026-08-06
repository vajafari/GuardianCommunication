using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerCalendarDetails
    {
        public int Id { get; set; }
        public int PadisControllerCalendarNumber { get; set; }
        public DateTime Date { get; set; }
        public short StartTime { get; set; }
        public short EndTime { get; set; }
    }
}


