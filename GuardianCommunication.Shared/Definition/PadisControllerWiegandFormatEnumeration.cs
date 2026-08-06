using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum PadisControllerWiegandFormatEnumeration
    {
        [EnumMember]
        Auto,
        [EnumMember]
        Format26,
        [EnumMember]
        Format26A,
        [EnumMember]
        Format34,
        [EnumMember]
        Format34A,
        [EnumMember]
        Format36,
        [EnumMember]
        Format37,
        [EnumMember]
        Format37A,
        [EnumMember]
        Format50,
        [EnumMember]
        Format66
    }
}