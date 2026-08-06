using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class NewEmployeeImageEnrolledModel
    {
	    [DataMember]
		public EmployeeImageModel EmployeeImage { get; set; }
		[DataMember]
		public int DeviceNumber { get; set; }

    }
}
