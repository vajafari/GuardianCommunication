using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewCardEnrolledModel
    {
		[DataMember]
	    public string Card { get; set; }
		[DataMember]
	    public int DeviceNumber { get; set; }
        [DataMember]
        public long EmployeeNumber { get; set; }
    }
}
