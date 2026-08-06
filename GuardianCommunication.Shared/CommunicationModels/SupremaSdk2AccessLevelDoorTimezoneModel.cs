using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2AccessLevelDoorAccessScheduleModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int AccessLevelNumber { get; set; }
        [DataMember]
        public int DeviceDoorId { get; set; }
        [DataMember]
        public int AccessScheduleNumber { get; set; }
    }
}
