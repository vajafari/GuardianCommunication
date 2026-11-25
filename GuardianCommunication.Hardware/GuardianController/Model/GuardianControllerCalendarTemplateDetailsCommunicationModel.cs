using System;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// یک بازه از یک روز چرخه قالب تقویم به ساعت محلی برد؛ دقیقه پایان هم جزو بازه است
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerCalendarTemplateDetailsCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// شماره روز چرخه از صفر
        /// </summary>
        public short day_index { get; set; }

        /// <summary>
        /// ساعت به شکل HHmm (مثلا 630 یعنی 06:30)
        /// </summary>
        public short start_time { get; set; }

        /// <summary>
        /// ساعت به شکل HHmm
        /// </summary>
        public short end_time { get; set; }

        /// <summary>
        /// «مخصوص» بازه فضا: 1 آقایان، 2 بانوان، 3 هر دو؛ خالی در قالب عمومی
        /// </summary>
        public short? audience { get; set; }

        /// <summary>
        /// «برای» بازه فضا: 1 عضویت، 2 رزرو؛ خالی در قالب عمومی
        /// </summary>
        public short? purpose { get; set; }
    }
}
