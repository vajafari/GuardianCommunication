using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class VirdiAccessControlDataModel
    {
        [DataMember]
        public VirdiAccessControlDataTypeEnumeration AccessDataType { get; set; }
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public List<VirdiHolidayGroupModel> Holidays { get; set; }
        [DataMember]
        public List<VirdiTimeZoneModel> Timezones { get; set; }
        [DataMember]
        public List<VirdiAccessTimeModel> AccessTimes { get; set; }
        [DataMember]
        public List<VirdiAccessGroupModel> AccessGroups { get; set; }
    }
}
