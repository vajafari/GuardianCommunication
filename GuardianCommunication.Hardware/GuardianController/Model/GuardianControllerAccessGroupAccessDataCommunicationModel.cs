using System;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// یک قاعده گروه دسترسی: یک سطح دسترسی با یک تقویم
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerAccessGroupAccessDataCommunicationModel
    {
        public Guid id { get; set; }
        public Guid access_level_id { get; set; }
        public Guid calendar_id { get; set; }
    }
}
