using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class EmployeeImageEnrolledModel
    {
	    [DataMember]
		public UserImageModel EmployeeImage { get; set; }
		[DataMember]
		public Guid DeviceId { get; set; }

    }
}
