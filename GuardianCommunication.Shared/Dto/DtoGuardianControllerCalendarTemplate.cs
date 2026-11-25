using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// قالب تقویم (چرخه چندروزه) برد کنترلر گاردین
    /// </summary>
    public class DtoGuardianControllerCalendarTemplate
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// شماره قالب (برچسب)
        /// </summary>
        public int CalendarTemplateNumber { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// تعداد روزهای چرخه
        /// </summary>
        public int DayCount { get; set; }

        public List<DtoGuardianControllerCalendarTemplateDetails> Details { get; set; } = [];
    }
}
