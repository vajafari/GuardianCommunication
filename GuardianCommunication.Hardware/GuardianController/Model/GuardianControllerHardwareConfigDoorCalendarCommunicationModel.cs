using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// حالت باز و بسته تقویمی درب؛ فهرست خالی یعنی آن حالت خاموش است
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigDoorCalendarCommunicationModel
    {
        /// <summary>
        /// در بازه‌های این تقویم‌ها درب باز می‌ماند و همه رد می‌شوند
        /// </summary>
        public List<Guid> open_calendar_ids { get; set; }

        /// <summary>
        /// در بازه‌های هر تقویم بسته درب قفل است و فقط اعضای گروه همان ردیف با کارت رد می‌شوند
        /// </summary>
        public List<GuardianControllerHardwareConfigCloseCalendarCommunicationModel> close_calendars { get; set; }
    }
}
