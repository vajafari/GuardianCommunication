
using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ZkOperationLogModel
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
		public double OperationTime { get; set; }
		[DataMember]
		public string Operator { get; set; }
		[DataMember]
		public string User { get; set; }
		[DataMember]
		public string OperationType { get; set; }
		[DataMember]
		public string Object1 { get; set; }
		[DataMember]
		public string Object2 { get; set; }
		[DataMember]
		public string Object3 { get; set; }
		[DataMember]
		public string Object4 { get; set; }
	}
}
