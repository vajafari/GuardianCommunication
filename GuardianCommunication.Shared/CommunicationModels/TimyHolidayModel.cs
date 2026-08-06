using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimyHolidayModel
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public double StartDate { get; set; }

        [DataMember]
        public double EndDate { get; set; }

        [DataMember]
        public int DayTimezoneIndex { get; set; }
    }
}
