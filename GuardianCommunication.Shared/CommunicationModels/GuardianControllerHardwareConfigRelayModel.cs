using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// رله برد
    /// </summary>
    [DataContract]
    public class GuardianControllerHardwareConfigRelayModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// شماره فیزیکی رله روی برد
        /// </summary>
        [DataMember]
        public int RelayNumber { get; set; }

        [DataMember]
        public GuardianControllerRelayTypeEnumeration RelayType { get; set; }

        [DataMember]
        public GuardianControllerContactTypeEnumeration ContactType { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
