using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ScanFingerModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
        public UserModel EmployeeData { get; set; }
        [DataMember]
		public int FingerIndex { get; set; }
	}
}
