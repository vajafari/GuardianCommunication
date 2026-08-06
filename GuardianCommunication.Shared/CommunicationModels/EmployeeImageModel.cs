using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class EmployeeImageModel
    {
	    [DataMember]
		public string EmployeeImage { get; set; }
		[DataMember]
		public long EmployeeNumber { get; set; }

    }
}
