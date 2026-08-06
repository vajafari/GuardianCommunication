using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewFingerEnrolledModel
    {
	    [DataMember]
	    public EmployeeFingerModel FingerInfo { get; set; }
	    [DataMember]
	    public int DeviceNumber { get; set; }
    }
}
