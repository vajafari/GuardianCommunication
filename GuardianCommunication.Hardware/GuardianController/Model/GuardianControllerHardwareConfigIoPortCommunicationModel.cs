using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// ورودی/خروجی برد؛ ورودی/خروجی که هیچ دربی به آن ارجاع نمی‌دهد مال کل برد است
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigIoPortCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// نشانی فیزیکی ورودی/خروجی روی برد
        /// </summary>
        public int io_number { get; set; }

        public GuardianControllerGpioTypeEnumeration io_type { get; set; }
        public GuardianControllerContactTypeEnumeration contact_type { get; set; }
        public string title { get; set; }
    }
}
