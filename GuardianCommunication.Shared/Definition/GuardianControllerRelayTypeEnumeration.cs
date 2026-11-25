using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    /// <summary>
    /// نقش رله برد کنترلر گاردین - هم‌شماره با گاردین؛ معادل relay_type
    /// </summary>
    [DataContract]
    public enum GuardianControllerRelayTypeEnumeration : short
    {
        /// <summary>
        /// قفل درب
        /// </summary>
        [EnumMember]
        MainLock = 1,
        /// <summary>
        /// آژیر هشدار
        /// </summary>
        [EnumMember]
        AlarmBuzzer = 2,
        /// <summary>
        /// دستگاه جانبی
        /// </summary>
        [EnumMember]
        AuxDevice = 3,
    }
}
