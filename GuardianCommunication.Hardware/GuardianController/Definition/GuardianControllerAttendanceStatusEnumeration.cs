namespace GuardianCommunication.Hardware.GuardianController.Definition
{
    /// <summary>
    /// نتیجه بررسی تردد روی برد
    /// </summary>
    public enum GuardianControllerAttendanceStatusEnumeration
    {
        Success = 0,

        /// <summary>
        /// کارت یا کاربر روی برد تعریف نشده است
        /// </summary>
        FailedInvalidUser = 1,

        /// <summary>
        /// خارج از بازه‌های تقویم دسترسی
        /// </summary>
        FailedToAccessAtTime = 2,

        /// <summary>
        /// کاربر به این درب دسترسی ندارد
        /// </summary>
        FailedToAccessToDoor = 3,

        /// <summary>
        /// درب در حالت بسته تقویمی است
        /// </summary>
        FailedForDoorCalendarLock = 7,

        /// <summary>
        /// کاربر عضو هیچ گروه دسترسی نیست
        /// </summary>
        FailedForNotAccessToAnyGroup = 8,

        /// <summary>
        /// سقف تردد دسترسی آسان تمام شده است
        /// </summary>
        FailedForAttendanceLimit = 14,

        /// <summary>
        /// درب منتظر اجازه مدیر است
        /// </summary>
        FailedForMasterPrimerRequired = 15,

        /// <summary>
        /// کاربر غیرفعال است
        /// </summary>
        FailedForUserInactive = 18,

        /// <summary>
        /// خارج از بازه اعتبار کاربر
        /// </summary>
        FailedForOutsideValidity = 19,

        /// <summary>
        /// تطبیق روی سرور اجازه نداد
        /// </summary>
        FailedForServerDenied = 20,
    }
}
