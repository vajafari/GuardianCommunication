namespace GuardianCommunication.Shared.HardwareDefinition
{
    public enum AttendanceSourceEnumeration : short
    {
        /// <summary>
        /// دستگاه ثبت تردد
        /// </summary>
        Device = 1,

        /// <summary>
        /// دوربین تشخیص پلاک
        /// </summary>
        ParkingPlateDetectionCamera = 2,

        /// <summary>
        /// تردد دستی
        /// </summary>
        Manual = 3,

        /// <summary>
        /// از دستگاه از طریق متد
        /// ServerMatch
        /// </summary>
        DeviceServerMatch = 4,

    }

}
