using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum PadisControllerRelayTypeEnumeration
    {
        [EnumMember]
        Single = 1,
        [EnumMember]
        Double = 2,
    }
}