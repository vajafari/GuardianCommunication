using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2AccessLevelModel
    {
        [DataMember]
        public int AccessLevelNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<SupremaSdk2AccessLevelDoorAccessScheduleModel> DoorAccessSchedules { get; set; }

    }
}
