using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class NewIrisModelEnrolled
    {
        [DataMember]
        public EmployeeIrisModel IrisInfo { get; set; }

        [DataMember]
        public int DeviceNumber { get; set; }
    }
}
