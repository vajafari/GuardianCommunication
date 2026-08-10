using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class UserAndDeviceResultModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
		public long UserIdOnDevice { get; set; }
		[DataMember]
		public OperationResultEnumeration Result { get; set; }
	}
}
