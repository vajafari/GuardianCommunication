using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum PadisControllerWiegandDataTypeEnumeration
    {
        [EnumMember]
        CardNumber = 1,
        [EnumMember]
        UserId = 2,
    }
}