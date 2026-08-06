using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewPalmModelEnrolled
    {
		[DataMember]
	    public EmployeePalmModel PalmInfo { get; set; }

	    [DataMember]
	    public int DeviceNumber { get; set; }
    }
}
