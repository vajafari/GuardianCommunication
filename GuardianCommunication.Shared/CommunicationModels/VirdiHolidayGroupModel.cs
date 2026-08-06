using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class VirdiHolidayGroupModel
    {
        [DataMember]
        public string GroupCode { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int Month { get; set; }
        [DataMember]
        public int Day { get; set; }
    }
}
