using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    [DataContract]
    public enum InvalidAttendanceReasonEnumeration : short
    {

        /// <summary>
        /// دستگاه ثبت تردد
        /// </summary>
        [EnumMember]
        UnAuthorize = 1,
        /// <summary>
        /// نامجاز به خاطر گروه دسترسی
        /// </summary>
        InvalidBecauseOfAccessGroup = 2,
        /// <summary>
        /// نامجاز به خاطر غیر فعال بودن کاربر
        /// </summary>
        InvalidBecauseOfDisabled = 3,
        /// <summary>
        /// نامجاز به خاطر بازه تاریخ نامحاز
        /// </summary>
        InvalidBecauseOfExpired = 4,
        /// <summary>
        /// نامجاز به خاطر Antipassback
        /// </summary>
        InvalidBecauseOfAntiPassback = 5,
        /// <summary>
        /// نامجاز به خاطر لیست سیاه
        /// </summary>
        InvalidBecauseOfBlacklist = 6,
        /// <summary>
        /// نامجاز به خاطر برنامه زمانی
        /// </summary>
        InvalidBecauseOfSchedule = 7,
        /// <summary>
        /// نامجاز به خاطر بایومتریک جعلی
        /// </summary>
        InvalidBecauseOfFakeBiometric = 8,
        /// <summary>
        /// کاربر غیر مجاز
        /// </summary>
        InvalidBecauseOfInvalidUser = 9,
        /// <summary>
        /// کاربر غیر مجاز
        /// </summary>
        InvalidBecauseOfPermission = 10,
        /// <summary>
        /// غیر مجاز به خاطر نامجاز بودن در درب
        /// </summary>
        InvalidBecauseOfDoorPermission = 11,
        /// <summary>
        /// غیر مجاز به خاطر صحت عبور
        /// </summary>
        InvalidBecauseOfPassAccuracy = 12,
        /// <summary>
        /// دسترسی به هیچ گروهی ندارد 
        /// </summary>
        InvalidBecauseOfNotAccessToAnyGroup = 13,
        /// <summary>
        /// Multi person verfiy : هیچ گروه مرتبطی برای ورود برای این کاربر ثبت نشده 
        /// </summary>
        InvalidBecauseOfNoRelevantGroupFound = 14,
        /// <summary>
        /// "تأیید چندنفره کامل نشده است. هنوز همه گروه‌های دسترسی الزامی توسط پرسنل تأییدشده پوشش داده نشده‌اند."
        /// </summary>
        InvalidBecauseOfNoCoverAllGroup = 15,
        /// <summary>
        /// هیچ برنامه زمانی  تعریف نشده است
        /// </summary>
        InvalidBecauseOfNoAccessSchedule = 16,
        /// <summary>
        /// هیچ برنامه زمانی فعالی برای این کاربر تعریف نشده است
        /// </summary>
        InvalidBecauseOfNoUserSchedule = 17,
        /// <summary>
        /// محدودیت به دلیل محدودیت در بازه زمانی بیسته بودن درب
        /// </summary>
        InvalidBecauseOfCalendarLock = 18,

    }
}
