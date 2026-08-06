using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewImageEnrolledModel
    {

		[DataMember]
	    public EmployeeFaceModel FaceInfo { get; set; }

		[DataMember]
	    public int DeviceNumber { get; set; }
    }
}
