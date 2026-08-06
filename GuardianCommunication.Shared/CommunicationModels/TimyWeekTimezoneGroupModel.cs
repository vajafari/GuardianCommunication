using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimyWeekTimezoneGroupModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int DeviceIndex { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public List<TimyWeekTimezoneModel> Timezones { get; set; }
    }
}
