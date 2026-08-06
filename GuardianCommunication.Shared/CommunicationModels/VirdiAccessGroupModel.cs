using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class VirdiAccessGroupModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string AccessTimeCode { get; set; }
    }
}
