using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    [DataContract]
    public enum AttendanceSourceEnumeration : short
    {
        /// <summary>
        /// دستگاه ثبت تردد
        /// </summary>
        [EnumMember]
        Device = 1,

        /// <summary>
        /// دوربین تشخیص پلاک
        /// </summary>
        [EnumMember]
        ParkingPlateDetectionCamera = 2,

        /// <summary>
        /// تردد دستی
        /// </summary>
        [EnumMember]
        Manual = 3,

        /// <summary>
        /// دریافت تردد های دستگاه توسط API 
        /// </summary>
        [EnumMember]
        DeviceFromApi = 4,

        /// <summary>
        /// از دستگاه از طریق متد
        /// ServerMatch
        /// </summary>
        [EnumMember]
        DeviceServerMatch = 5,

        /// <summary>
        /// فایل
        /// </summary>
        File = 6,

        /// <summary>
        /// دوربین تشخیص چهره
        /// </summary>
        FaceDetectionCamera = 7,
    }
}
