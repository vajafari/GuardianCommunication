using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2AccessScheduleElementModel
    {

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int AccessScheduleNumber { get; set; }

        [DataMember]
        public short ElementCode { get; set; }

        [DataMember]
        public short StartTime { get; set; }

        [DataMember]
        public short EndTime { get; set; }

    }
}
