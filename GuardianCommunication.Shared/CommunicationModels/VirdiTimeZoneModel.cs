using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class VirdiTimeZoneModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int StartHour { get; set; }
        [DataMember]
        public int StartMinute { get; set; }
        [DataMember]
        public int EndHour { get; set; }
        [DataMember]
        public int EndMinute { get; set; }
    }
}
