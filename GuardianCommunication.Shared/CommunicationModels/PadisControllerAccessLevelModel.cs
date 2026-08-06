using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerAccessLevelModel
    {
        [DataMember]
        public int AccessLevelNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public List<PadisControllerAccessLevelDoorModel> Doors { get; set; }

    }
}
