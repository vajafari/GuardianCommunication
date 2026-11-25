using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    /// <summary>
    /// دوره ریست اجازه مدیر درب برد کنترلر گاردین - هم‌شماره با گاردین؛ معادل reset_period
    /// </summary>
    [DataContract]
    public enum GuardianControllerMasterPrimerResetPeriodEnumeration : short
    {
        /// <summary>
        /// بدون ریست
        /// </summary>
        [EnumMember]
        Never = 1,
        /// <summary>
        /// پس از مدت
        /// </summary>
        [EnumMember]
        Duration = 2,
        /// <summary>
        /// روزانه
        /// </summary>
        [EnumMember]
        Daily = 3,
        /// <summary>
        /// هفتگی
        /// </summary>
        [EnumMember]
        Weekly = 4,
    }
}
