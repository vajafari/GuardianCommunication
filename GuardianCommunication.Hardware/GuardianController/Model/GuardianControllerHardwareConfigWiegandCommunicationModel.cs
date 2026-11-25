using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigWiegandCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// نشانی فیزیکی ویگند روی برد
        /// </summary>
        public int wiegand_number { get; set; }

        public GuardianControllerWiegandDataTypeEnumeration data_type { get; set; }
        public string title { get; set; }
    }
}
