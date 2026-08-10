using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoZkOperationLog
	{

		public int Id { get; set; }

		public Guid DeviceId { get; set; }

		public DateTime OperationTime { get; set; }

		public string Operator { get; set; }

		public string User { get; set; }

		public string OperationType { get; set; }

		public string Object1 { get; set; }

		public string Object2 { get; set; }

		public string Object3 { get; set; }

		public string Object4 { get; set; }
	}
}
