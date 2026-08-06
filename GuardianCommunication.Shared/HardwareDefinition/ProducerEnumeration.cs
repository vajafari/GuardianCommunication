using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// تولید کنندگان ساعت
    /// </summary>
    [Flags]
    [DataContract]
    public enum ProducerEnumeration : short
    {
        [EnumMember]
        Zk = 1,
        [EnumMember]
        ProcessingWorld = 2,
        [EnumMember]
        Suprema = 4,
        [EnumMember]
        Virdi = 8,
        [EnumMember]
        ElmOSanat = 16,
        [EnumMember]
        Timy = 32,
        [EnumMember]
        JahanGostar = 64,
        [EnumMember]
        PouyaFanavaran = 128,
        [EnumMember]
        Padis = 256,
        [EnumMember]
        Other = 16384,
    }
}
