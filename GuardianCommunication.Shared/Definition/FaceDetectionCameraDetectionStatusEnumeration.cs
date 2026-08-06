using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum FaceDetectionCameraDetectionStatusEnumeration
    {
        [EnumMember]
        Valid = 1,
        [EnumMember]
        Invalid = 2,
        [EnumMember]
        Unknown = 3
    }
}
