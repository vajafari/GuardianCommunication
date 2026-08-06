using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class EmployeeIrisModel
    {
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public string TemplateData { get; set; }
    }
}
