using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceCommunicationData
	{
		public int DeviceNumber { get; set; }
		public long? LastLogId { get; set; }
		public long? LastAttendanceLogId { get; set; }
		public DateTime? LastLogDateTime { get; set; }
		public DateTime? LastAttendanceLogDateTime { get; set; }
		
	}
}
