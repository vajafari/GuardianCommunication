using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// تقویم برد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerCalendarModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// شماره تقویم (برچسب)
        /// </summary>
        [DataMember]
        public int CalendarNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// نوع تقویم: 1 خاص (با روزها)، 2 بر اساس الگو
        /// </summary>
        [DataMember]
        public short CalendarType { get; set; }

        /// <summary>
        /// قالب تقویم بر اساس الگو
        /// </summary>
        [DataMember]
        public Guid? CalendarTemplateId { get; set; }

        /// <summary>
        /// تاریخ شروع چرخه تقویم بر اساس الگو
        /// </summary>
        [DataMember]
        public DateTime? CycleStartDate { get; set; }

        [DataMember]
        public List<GuardianControllerCalendarDetailsModel> Details { get; set; }
    }
}
