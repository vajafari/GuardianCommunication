using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class EmployeeAndDeviceModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public EmployeeModel EmployeeData { get; set; }
        [DataMember]
        public CommandPriorityEnumeration? CommandPriority { get; set; }
        [DataMember]
        public Guid? CommandIdentifier { get; set; }
    }
}
