using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerUserEasyPermission
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int DoorId { get; set; }
        public int? CalendarNumber { get; set; }
    }
}
