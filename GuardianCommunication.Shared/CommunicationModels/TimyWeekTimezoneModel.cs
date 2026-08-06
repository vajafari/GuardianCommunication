using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimyWeekTimezoneModel
    {
        [DataMember]
        public int WeekTimezoneGroupId { get; set; }

        [DataMember]
        public DayOfWeek WeekDay { get; set; }

        [DataMember]
        public int DayTimezoneIndex { get; set; }
    }
}
