using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum OtherHardwareCommandTypeEnumeration : short
    {
        [EnumMember]
        Other = 0,
        [EnumMember]
        FaceDetectionCameraSetUserInfo = 1,
        [EnumMember]
        FaceDetectionCameraDeleteUser = 2,
        [EnumMember]
        FaceDetectionCameraSetCameraData = 3,
        [EnumMember]
        FaceDetectionCameraDeleteCamera = 4,
    }
}