using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Dto
{
    [DataContract]
    public enum VirdiDayOfWeekEnumeration
    {
        [EnumMember]
        Sunday = 0,
        [EnumMember]
        Monday = 1,
        [EnumMember]
        Tuesday = 2,
        [EnumMember]
        Wednesday = 3,
        [EnumMember]
        Thursday = 4,
        [EnumMember]
        Friday = 5,
        [EnumMember]
        Saturday = 6,
        [EnumMember]
        Holiday = 7,
    }
}
