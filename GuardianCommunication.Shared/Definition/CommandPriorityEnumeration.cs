using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum CommandPriorityEnumeration : short
    {
        [EnumMember]
        VeryLow = 1,
        [EnumMember]
        Low = 2,
        [EnumMember]
        Medium = 3,
        [EnumMember]
        High = 4,
        [EnumMember]
        VeryHigh = 5,
    }
}
