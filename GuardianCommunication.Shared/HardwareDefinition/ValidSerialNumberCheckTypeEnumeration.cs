using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    [Flags]
    [DataContract]
    public enum ValidSerialNumberCheckTypeEnumeration : short
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        User = 1,
        [EnumMember]
        Attendance = 2
    }
}
