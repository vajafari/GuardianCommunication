using System;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// تنظیمات دستگاه ها
    /// </summary>
    [Flags]
    public enum DeviceSettingsEnumeration : long
    {
        None = 0,

        /// <summary>
        /// آیا Server Matching در دستگاه فعال است
        /// </summary>
        ServerMatch = 1,

        /// <summary>
        /// سیستم ارسال نام قدیمی برای دستگاه های ZK
        /// </summary>
        ZkOldName = 2,

        /// <summary>
        /// جمع آوری رویداد های دستگاه های ساپریما 2
        /// </summary>
        SupremaAutoCollectEvents = 4,

        /// <summary>
        /// عدم ذخیره سازی تردد ها
        /// </summary>
        DontSaveAttendance = 16,

        /// <summary>
        /// عدم ذخیره سازی تردد های نامجاز
        /// </summary>
        DontSaveInvalidAttendance = 32,

        /// <summary>
        /// عدم ذخیره سازی رویدادها
        /// </summary>
        DontSaveEvents = 64,

        /// <summary>
        /// دستگاه پادیس از پروتکل https استفاده می کند.
        /// </summary>
        PadisControllerUseHttps = 128,


    }
}
