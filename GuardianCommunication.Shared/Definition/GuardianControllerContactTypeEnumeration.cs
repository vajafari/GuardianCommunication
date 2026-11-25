using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    /// <summary>
    /// نوع کنتاکت رله یا ورودی/خروجی برد کنترلر گاردین - هم‌شماره با گاردین؛ معادل contact_type
    /// </summary>
    [DataContract]
    public enum GuardianControllerContactTypeEnumeration : short
    {
        /// <summary>
        /// نرمال باز
        /// </summary>
        [EnumMember]
        NormallyOpen = 1,
        /// <summary>
        /// نرمال بسته
        /// </summary>
        [EnumMember]
        NormallyClosed = 2,
    }
}
