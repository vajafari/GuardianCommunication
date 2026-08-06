using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2AccessScheduleModel
    {

        [DataMember]
        public int AccessScheduleNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string TimezoneDescription { get; set; }

        [DataMember]
        public int? HolidayGroupNumber1 { get; set; }

        [DataMember]
        public int? HolidayGroupNumber2 { get; set; }

        [DataMember]
        public int? HolidayGroupNumber3 { get; set; }

        [DataMember]
        public int? HolidayGroupNumber4 { get; set; }

        [DataMember]
        public List<SupremaSdk2AccessScheduleElementModel> Elements { get; set; }

    }
}
