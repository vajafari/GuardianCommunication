using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2SendAccessLevelModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public List<SupremaSdk2AccessLevelModel> AccessLevels { get; set; }
    }
}
