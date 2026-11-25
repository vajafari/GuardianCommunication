using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// تقویم؛ بدنه POST api/v1/calendars/upsert در Standalone و محتوای دستور GuardianControllerSetCalendar در Push.
    /// تقویم بر اساس قالب (نوع 2) بازه ندارد و برد روز چرخه را از قالب و تاریخ شروع چرخه حساب می‌کند
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerCalendarCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// فقط برای نمایش در UI برد؛ ارجاع به تقویم همه‌جا با id است
        /// </summary>
        public int calendar_number { get; set; }

        public string title { get; set; }
        public bool is_active { get; set; }

        /// <summary>
        /// 1 با تاریخ‌های مشخص، 2 بر اساس قالب
        /// </summary>
        public short calendar_type { get; set; }

        public List<GuardianControllerCalendarDetailsCommunicationModel> details { get; set; }
        public Guid? calendar_template_id { get; set; }

        /// <summary>
        /// تاریخ شروع چرخه به شکل yyyy-MM-dd
        /// </summary>
        public string cycle_start_date { get; set; }
    }
}
