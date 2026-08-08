using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum GuardianControllerWiegandDataTypeEnumeration
    {
        [EnumMember]
        CardNumber = 1,
        [EnumMember]
        UserId = 2,
    }
}