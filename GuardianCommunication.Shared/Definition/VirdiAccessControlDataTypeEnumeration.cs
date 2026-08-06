using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    [DataContract]
    public enum VirdiAccessControlDataTypeEnumeration
    {
        [EnumMember]
        Holiday = 1,
        [EnumMember]
        TimeZone = 2,
        [EnumMember]
        AccessTimes = 4,
        [EnumMember]
        AccessGroup = 8
    }
}
