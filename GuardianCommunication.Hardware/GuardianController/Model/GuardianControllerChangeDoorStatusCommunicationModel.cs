using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Hardware.GuardianController.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// فرمان درب؛ بدنه POST api/v1/doors/control در Standalone و محتوای دستور GuardianControllerDoorControl در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerChangeDoorStatusCommunicationModel
    {
        public Guid door_id { get; set; }
        public GuardianControllerDoorActionEnumeration action { get; set; }

        /// <summary>
        /// فقط برای باز کردن لحظه‌ای (ثانیه)؛ خالی یعنی مدت باز ماندن قفل درب
        /// </summary>
        public int? duration { get; set; }
    }
}
