using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// سطح دسترسی برد کنترلر گاردین با درب‌های همین برد
    /// </summary>
    [DataContract]
    public class GuardianControllerAccessLevelModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// شماره سطح (برچسب)
        /// </summary>
        [DataMember]
        public int AccessLevelNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// درب‌های همین برد
        /// </summary>
        [DataMember]
        public List<Guid> DoorIds { get; set; } = [];
    }
}
