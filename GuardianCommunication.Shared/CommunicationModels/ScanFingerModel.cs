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
        public UserModel UserData { get; set; }
        [DataMember]
		public int FingerIndex { get; set; }
	}
}
