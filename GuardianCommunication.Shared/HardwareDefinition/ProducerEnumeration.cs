using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// تولید کنندگان ساعت
    /// </summary>
    [Flags]
    public enum ProducerEnumeration : short
    {
        /// <summary>
        /// زد-کا
        /// </summary>
        Zk = 1,

        /// <summary>
        /// ساپریما
        /// </summary>
        Suprema = 2,

        /// <summary>
        /// ویردی
        /// </summary>
        Virdi = 4,

        /// <summary>
        /// تیمی
        /// </summary>
        Timy = 8,

        /// <summary>
        /// پویا فناوران
        /// </summary>
        PouyaFanavaran = 16,

        /// <summary>
        /// اساگارد
        /// </summary>
        AsaGuard = 32,

        /// <summary>
        /// تمامی تولید کنندگان
        /// </summary>
        All = 32767
    }

}
