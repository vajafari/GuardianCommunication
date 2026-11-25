namespace GuardianCommunication.Hardware.GuardianController.Definition
{
    /// <summary>
    /// نتیجه رساندن یک فرمان صف به برد در حالت Push؛ خطاهای خود برد رشته‌ای‌اند و شرحشان در پیام نتیجه می‌آید
    /// </summary>
    public enum GuardianControllerErrorEnumeration
    {
        /// <summary>
        /// برد فرمان را کامل اجرا کرد
        /// </summary>
        Success = 0,

        /// <summary>
        /// برد هنوز جواب فرمان قبلی را نداده است؛ فرمان دور بعد فرستاده می‌شود
        /// </summary>
        LineIsBusy = 1,

        /// <summary>
        /// جواب برد در مهلت نرسید یا اتصال قطع شد؛ فرمان دور بعد دوباره فرستاده می‌شود
        /// </summary>
        MessageTimeout = 2,

        /// <summary>
        /// خطای پیش‌بینی‌نشده در سرور
        /// </summary>
        UnknownError = 3,

        /// <summary>
        /// برد فرمان یا بخشی از قلم‌های آن را نپذیرفت (نتیجه error یا partial)
        /// </summary>
        CommandRejected = 4,
    }
}
