using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimyDayTimezoneIntervalModel
    {
        
        [DataMember]
        public int StartHourMinute { get; set; }

        [DataMember]
        public int EndHourMinute { get; set; }
    }
}
