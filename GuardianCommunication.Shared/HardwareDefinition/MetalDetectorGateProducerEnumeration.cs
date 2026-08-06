using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// مدل گیت فلزیاب
    /// </summary>
    [DataContract]
    public enum MetalDetectorGateTypeEnumeration : short
    {
        [EnumMember]
        Pd318 = 1,
    }
}
