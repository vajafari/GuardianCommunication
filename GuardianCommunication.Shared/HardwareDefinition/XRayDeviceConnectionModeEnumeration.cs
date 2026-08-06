using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// نوع اتصال
    /// </summary>
    [DataContract]
	public enum XRayDeviceConnectionModeEnumeration : short
	{
        [EnumMember]
		File = 1,
	}
}
