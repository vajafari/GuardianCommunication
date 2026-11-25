namespace GuardianCommunication.Hardware.GuardianController.Definition
{
    /// <summary>
    /// فرمان درب
    /// </summary>
    public enum GuardianControllerDoorActionEnumeration
    {
        /// <summary>
        /// باز کردن لحظه‌ای؛ مدت خالی یعنی مدت باز ماندن قفل درب
        /// </summary>
        OpenMomentary = 1,

        /// <summary>
        /// باز نگه داشتن دائم
        /// </summary>
        Unlock = 2,

        /// <summary>
        /// بسته نگه داشتن دائم؛ با تردد مجاز هم باز نمی‌شود
        /// </summary>
        Lock = 3,

        /// <summary>
        /// برگشت به حالت عادی
        /// </summary>
        Normal = 4,
    }
}
