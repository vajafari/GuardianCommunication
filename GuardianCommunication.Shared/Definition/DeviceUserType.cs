using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum DeviceUserTypeEnumeration
    {
        [EnumMember]
        PermanentUser,
        [EnumMember]
        TempUser
    }
}
