using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum DeviceCommandTypeEnumeration : short
    {
        [EnumMember]
        Other = 0,
        [EnumMember]
        SetUserInfo = 1,
        [EnumMember]
        SetFinger = 2,
        [EnumMember]
        SetFace = 3,
        [EnumMember]
        SetPalm = 4,
        [EnumMember]
        SetPhoto = 5,
        [EnumMember]
        DeleteUser = 6,
        [EnumMember]
        ReadAttendance = 7,
        [EnumMember]
        ReadoutAttendance = 8,
        [EnumMember]
        ReadUser = 9,
        [EnumMember]
        ReadFingerPrint = 10,
        [EnumMember]
        ClearData = 11,
        [EnumMember]
        ClearUser = 12,
        [EnumMember]
        Check = 13,
        [EnumMember]
        ScanFace = 14,
        [EnumMember]
        ScanFinger = 15,
        [EnumMember]
        Reboot = 16,
        [EnumMember]
        Unlock = 17,
        [EnumMember]
        EnrollUserWithTemplate = 18,
        [EnumMember]
        ScanCard = 19,
        [EnumMember]
        FaceCount = 20,
        [EnumMember]
        FingerCount = 21,
        [EnumMember]
        UserCount = 22,
        [EnumMember]
        AttendanceLogCount = 23,
        [EnumMember]
        SetDateAndTime = 24,
        [EnumMember]
        ScanIris = 25,
        [EnumMember]
        CancelOperation = 26,
        /// <summary>
        /// پیکربندی سخت‌افزار برد کنترلر گاردین (درب‌ها با رله، ویگند، ورودی/خروجی‌ها و تنظیمات درب)
        /// </summary>
        [EnumMember]
        GuardianControllerSetHardwareConfig = 27,
        /// <summary>
        /// ارسال قالب تقویم (چرخه چندروزه) به برد کنترلر گاردین
        /// </summary>
        [EnumMember]
        GuardianControllerSetCalendarTemplate = 28,
        /// <summary>
        /// حذف قالب‌های تقویم از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerDeleteCalendarTemplate = 29,
        /// <summary>
        /// ارسال تقویم به برد کنترلر گاردین
        /// </summary>
        [EnumMember]
        GuardianControllerSetCalendar = 30,
        /// <summary>
        /// حذف تقویم‌ها از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerDeleteCalendar = 31,
        /// <summary>
        /// ارسال سطح دسترسی (درب‌های همین برد) به برد کنترلر گاردین
        /// </summary>
        [EnumMember]
        GuardianControllerSetAccessLevel = 32,
        /// <summary>
        /// حذف سطح‌های دسترسی از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerDeleteAccessLevel = 33,
        /// <summary>
        /// ارسال گروه دسترسی به برد کنترلر گاردین
        /// </summary>
        [EnumMember]
        GuardianControllerSetAccessGroup = 34,
        /// <summary>
        /// حذف گروه‌های دسترسی از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerDeleteAccessGroup = 35,
        /// <summary>
        /// ثبت یا به‌روزرسانی جداگانه دسترسی‌های آسان روی برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerSetEasyAccess = 36,
        /// <summary>
        /// حذف جداگانه دسترسی‌های آسان از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [EnumMember]
        GuardianControllerDeleteEasyAccess = 37,
        /// <summary>
        /// پایان همگام‌سازی کامل برد کنترلر گاردین: برد هر چیزی را که در فهرست‌ها نیست حذف می‌کند
        /// </summary>
        [EnumMember]
        GuardianControllerKeepOnly = 38,
        /// <summary>
        /// فرمان درب برد کنترلر گاردین: باز کردن لحظه‌ای، باز یا بسته نگه داشتن دائم و برگشت به حالت عادی
        /// </summary>
        [EnumMember]
        GuardianControllerDoorControl = 39,
    }
}
