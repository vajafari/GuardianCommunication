using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class TimySetHolidaysModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }

        [DataMember]
        public List<TimyHolidayModel> Holidays { get; set; }
    }
}
