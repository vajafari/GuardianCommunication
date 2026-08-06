using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerAccessLevel
    {
        public int AccessLevelNumber { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public List<DtoPadisControllerAccessLevelDoor> Doors { get; set; }
    }
}