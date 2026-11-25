using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// گروه دسترسی برد کنترلر گاردین با قاعده‌های همین برد
    /// </summary>
    [DataContract]
    public class GuardianControllerAccessGroupModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// شماره گروه (برچسب)
        /// </summary>
        [DataMember]
        public int AccessGroupNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public List<GuardianControllerAccessGroupAccessDataModel> AccessData { get; set; }
    }
}
