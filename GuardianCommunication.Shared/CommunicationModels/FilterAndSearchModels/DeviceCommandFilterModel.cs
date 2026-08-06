using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels.FilterAndSearchModels
{
    [DataContract]
    public class DeviceCommandFilterModel
    {
        [DataMember]
        public List<long> Ids { get; set; }

        [DataMember]
        public List<long> EmployeeNumbers { get; set; }

        [DataMember]
        public long? EmployeeNumberLike { get; set; }

        [DataMember]
        public List<string> DeviceSerialNumbers { get; set; }

        [DataMember]
        public List<int> DeviceNumbers { get; set; }

        [DataMember]
        public double? CommitTimeFrom { get; set; }

        [DataMember]
        public double? CommitTimeTo { get; set; }

        [DataMember]
        public CommandPriorityEnumeration? Priority { get; set; }

        [DataMember]
        public DeviceCommandTypeEnumeration? CommandType { get; set; }

        [DataMember]
        public bool? IsSend { get; set; }

        [DataMember]
        public bool? VisiblilityTimeHasValue { get; set; }

        [DataMember]
        public double? VisiblilityTimeFrom { get; set; }

        [DataMember]
        public double? VisiblilityTimeTo { get; set; }



    }
}
