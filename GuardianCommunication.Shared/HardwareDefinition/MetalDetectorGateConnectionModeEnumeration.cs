using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// نوع اتصال
    /// </summary>
    [DataContract]
    public enum MetalDetectorGateConnectionModeEnumeration : short
    {
        [EnumMember]
        Standalone = 1,
        [EnumMember]
        Push = 2,
    }
}
