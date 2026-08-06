using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewFaceEnrolledModel
    {
		[DataMember]
	    public EmployeeFaceModel FaceInfo { get; set; }
		[DataMember]
	    public int DeviceNumber { get; set; }
    }
}
