using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// ردیف حالت بسته تقویمی درب: در بازه‌های تقویم درب قفل است و فقط اعضای گروه با کارت رد می‌شوند
    /// </summary>
    public class DtoGuardianControllerHardwareConfigCloseCalendar
    {
        /// <summary>
        /// شناسه ردیف در گاردین
        /// </summary>
        public Guid Id { get; set; }

        public Guid CalendarId { get; set; }

        public Guid AccessGroupId { get; set; }
    }
}
