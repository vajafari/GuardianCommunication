using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimySetDayTimezoneModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }

        [DataMember]
        public List<TimyDayTimezoneGroupModel> DayTimezoneGroups { get; set; }
    }
}
