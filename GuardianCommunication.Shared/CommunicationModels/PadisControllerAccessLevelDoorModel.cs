using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerAccessLevelDoorModel
    {
        [DataMember]
        public int AccessLevelNumber { get; set; }
        [DataMember]
        public int DoorId { get; set; }
    }
}