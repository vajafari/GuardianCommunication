using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// یک بازه از یک روز تقویم خاص
    /// </summary>
    public class DtoGuardianControllerCalendarDetails
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// ساعت شروع HHmm
        /// </summary>
        public short StartTime { get; set; }

        /// <summary>
        /// ساعت پایان HHmm (دقیقه پایان هم جزو بازه است)
        /// </summary>
        public short EndTime { get; set; }

        /// <summary>
        /// «مخصوص» بازه تقویم یا قالب فضا: 1 آقایان، 2 بانوان، 3 هر دو؛ خالی در تقویم و قالب عمومی
        /// </summary>
        public short? Audience { get; set; }

        /// <summary>
        /// «برای» بازه تقویم یا قالب فضا: 1 عضویت، 2 رزرو؛ خالی در تقویم و قالب عمومی
        /// </summary>
        public short? Purpose { get; set; }
    }
}
