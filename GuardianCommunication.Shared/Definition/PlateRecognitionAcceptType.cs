using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum PlateRecognitionAcceptType : short
    {
        [EnumMember]
        None,
        [EnumMember]
        Accepted,
        [EnumMember]
        Rejected,
        [EnumMember]
        AcceptedVip,
        [EnumMember]
        Blacklist
    }
}
