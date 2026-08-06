using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ScanFingerModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
        public EmployeeModel EmployeeData { get; set; }
        [DataMember]
		public int FingerIndex { get; set; }
	}
}
