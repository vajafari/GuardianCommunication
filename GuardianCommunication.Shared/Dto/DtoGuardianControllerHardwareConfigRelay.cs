using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// رله برد
    /// </summary>
    public class DtoGuardianControllerHardwareConfigRelay
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// شماره فیزیکی رله روی برد
        /// </summary>
        public int RelayNumber { get; set; }

        public GuardianControllerRelayTypeEnumeration RelayType { get; set; }

        public GuardianControllerContactTypeEnumeration ContactType { get; set; }

        public string Title { get; set; }
    }
}
