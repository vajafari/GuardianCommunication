using System.Runtime.Serialization;
using GuardianCommunication.Shared.Dto;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class VirdiAccessTimeModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public VirdiDayOfWeekEnumeration DayOfWeekEnum { get; set; }
        [DataMember]
        public string TimezoneCode { get; set; }
        [DataMember]
        public string HolidayCode { get; set; }
    }
}
