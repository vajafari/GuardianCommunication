using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Dto
{
    [DataContract]
    public enum OtherHardwareCommandSortEnumeration
    {
        [EnumMember]
        Id,
        [EnumMember]
        HardwareSerialNumber,
        [EnumMember]
        HardwareId,
        [EnumMember]
        CommandContent,
        [EnumMember]
        HardwareType,
        [EnumMember]
        CommitTime,
        [EnumMember]
        SendTime,
        [EnumMember]
        ResponseTime,
        [EnumMember]
        ResponseValue,
        [EnumMember]
        CommandType,
        [EnumMember]
        ObjectId,
        [EnumMember]
        RetryCount,
        [EnumMember]
        Priority,
        [EnumMember]
        MaxRetry,
        [EnumMember]
        HardwareContent,
        [EnumMember]
        Deadline,
        [EnumMember]
        VisiblilityTime,
        [EnumMember]
        Description,
        [EnumMember]
        CommandIdentifier,
    }
}