using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigRelayCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// نشانی فیزیکی رله روی برد
        /// </summary>
        public int relay_number { get; set; }

        public GuardianControllerRelayTypeEnumeration relay_type { get; set; }
        public GuardianControllerContactTypeEnumeration contact_type { get; set; }
        public string title { get; set; }
    }
}
