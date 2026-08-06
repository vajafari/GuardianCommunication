using System.Runtime.Serialization;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class EmployeeAndDeviceResultModel
	{
		[DataMember]
		public int DeviceNumber { get; set; }
		[DataMember]
		public long EmployeeNumber { get; set; }
		[DataMember]
		public OperationResultEnumeration Result { get; set; }
	}
}
