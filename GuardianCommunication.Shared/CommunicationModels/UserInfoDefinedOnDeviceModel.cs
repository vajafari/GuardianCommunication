using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserInfoDefinedOnDeviceModel
    {
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int? Privilege { get; set; }
    }
}
