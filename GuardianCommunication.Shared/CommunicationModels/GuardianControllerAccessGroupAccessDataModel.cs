using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// یک قاعده گروه: سطح دسترسی با تقویم
    /// </summary>
    [DataContract]
    public class GuardianControllerAccessGroupAccessDataModel
    {
        /// <summary>
        /// شناسه ردیف داده دسترسی در گاردین
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid AccessLevelId { get; set; }

        [DataMember]
        public Guid CalendarId { get; set; }
    }
}
