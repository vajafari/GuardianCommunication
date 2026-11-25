using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// قالب تقویم برد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerCalendarTemplateModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// شماره قالب (برچسب)
        /// </summary>
        [DataMember]
        public int CalendarTemplateNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// تعداد روزهای چرخه
        /// </summary>
        [DataMember]
        public int DayCount { get; set; }

        [DataMember]
        public List<GuardianControllerCalendarTemplateDetailsModel> Details { get; set; }
    }
}
