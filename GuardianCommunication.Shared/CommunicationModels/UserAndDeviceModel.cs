using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserAndDeviceModel
    {
        [DataMember]
        public Guid DeviceId { get; set; }
        [DataMember]
        public UserModel UserData { get; set; }
        [DataMember]
        public CommandPriorityEnumeration? CommandPriority { get; set; }
        [DataMember]
        public Guid? CommandIdentifier { get; set; }
    }
}
