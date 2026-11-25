using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// تقویم برد کنترلر گاردین
    /// </summary>
    public class DtoGuardianControllerCalendar
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// شماره تقویم (برچسب)
        /// </summary>
        public int CalendarNumber { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// نوع تقویم: 1 خاص (با روزها)، 2 بر اساس الگو
        /// </summary>
        public short CalendarType { get; set; }

        /// <summary>
        /// قالب تقویم بر اساس الگو
        /// </summary>
        public Guid? CalendarTemplateId { get; set; }

        /// <summary>
        /// تاریخ شروع چرخه تقویم بر اساس الگو
        /// </summary>
        public DateTime? CycleStartDate { get; set; }

        public List<DtoGuardianControllerCalendarDetails> Details { get; set; } = [];
    }
}
