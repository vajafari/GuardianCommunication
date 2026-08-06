using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerUserLimitationTime
    {
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? DoorId { get; set; }
    }
}
