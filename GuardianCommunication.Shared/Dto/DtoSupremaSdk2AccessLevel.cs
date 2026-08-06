using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoSupremaSdk2AccessLevel
    {
        public int AccessLevelNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<DtoSupremaSdk2AccessLevelDoorAccessSchedule> DoorAccessSchedules { get; set; }
    }
}
