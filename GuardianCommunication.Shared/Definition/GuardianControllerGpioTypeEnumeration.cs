using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    /// <summary>
    /// نقش ورودی/خروجی برد کنترلر گاردین - هم‌شماره با گاردین؛ معادل io_type
    /// </summary>
    [DataContract]
    public enum GuardianControllerGpioTypeEnumeration : short
    {
        /// <summary>
        /// کنتاکت درب
        /// </summary>
        [EnumMember]
        DoorContact = 1,
        /// <summary>
        /// سنسور حرکت
        /// </summary>
        [EnumMember]
        MotionSensor = 2,
        /// <summary>
        /// دتکتور آتش
        /// </summary>
        [EnumMember]
        FireDetector = 3,
        /// <summary>
        /// سنسور شکست شیشه
        /// </summary>
        [EnumMember]
        GlassBreak = 4,
        /// <summary>
        /// شستی خروج
        /// </summary>
        [EnumMember]
        ExitButton = 5,
        /// <summary>
        /// دتکتور دود
        /// </summary>
        [EnumMember]
        SmokeDetector = 6,
        /// <summary>
        /// خروجی جانبی
        /// </summary>
        [EnumMember]
        AuxOutput = 7,
    }
}
