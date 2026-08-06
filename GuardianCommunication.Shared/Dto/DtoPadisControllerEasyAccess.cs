using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerEasyAccess
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int? CalendarNumber { get; set; }
        public int DoorId { get; set; }
    }
}

