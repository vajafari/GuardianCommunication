using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserIrisModel
    {
        [DataMember]
        public long UserIdOnDevice { get; set; }
        [DataMember]
        public string TemplateData { get; set; }
    }
}
