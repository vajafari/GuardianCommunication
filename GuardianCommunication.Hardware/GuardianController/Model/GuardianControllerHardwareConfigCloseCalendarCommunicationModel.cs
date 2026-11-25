using System;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigCloseCalendarCommunicationModel
    {
        public Guid id { get; set; }
        public Guid calendar_id { get; set; }

        /// <summary>
        /// گروهی که در بازه‌های این تقویم با کارت رد می‌شود
        /// </summary>
        public Guid access_group_id { get; set; }
    }
}
