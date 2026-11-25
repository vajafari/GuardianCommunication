using System;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// یک بازه تاریخ‌دار تقویم به ساعت محلی برد؛ دقیقه پایان هم جزو بازه است
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerCalendarDetailsCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// تاریخ به شکل yyyy-MM-dd
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// ساعت به شکل HHmm (مثلا 630 یعنی 06:30)
        /// </summary>
        public short start_time { get; set; }

        /// <summary>
        /// ساعت به شکل HHmm
        /// </summary>
        public short end_time { get; set; }

        /// <summary>
        /// «مخصوص» بازه فضا: 1 آقایان، 2 بانوان، 3 هر دو؛ خالی در تقویم عمومی
        /// </summary>
        public short? audience { get; set; }

        /// <summary>
        /// «برای» بازه فضا: 1 عضویت، 2 رزرو؛ خالی در تقویم عمومی
        /// </summary>
        public short? purpose { get; set; }
    }
}
