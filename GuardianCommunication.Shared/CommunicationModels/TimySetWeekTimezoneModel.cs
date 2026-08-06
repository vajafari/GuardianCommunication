using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimySetWeekTimezoneModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }

        [DataMember]
        public List<TimyWeekTimezoneGroupModel> WeekTimezoneGroups { get; set; }
    }
}
