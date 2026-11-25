using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Hardware.GuardianController.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// رویداد درب؛ محتوای پیام device_event در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerDoorEventCommunicationModel
    {
        /// <summary>
        /// شناسه رویداد روی برد؛ یکتا و رو به افزایش
        /// </summary>
        public long id { get; set; }

        public Guid? door_id { get; set; }
        public GuardianControllerDoorEventTypeEnumeration event_type { get; set; }
        public long? user_id_on_device { get; set; }

        /// <summary>
        /// زمان رویداد به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long event_date_time { get; set; }
    }
}
