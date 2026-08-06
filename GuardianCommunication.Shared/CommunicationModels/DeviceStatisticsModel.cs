using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceStatisticsModel
	{
		[DataMember]
		public bool IsConnected { get; set; }
		[DataMember]
		public int CountOfUsers { get; set; }
		[DataMember]
		public int CountOfFaces { get; set; }
		[DataMember]
		public int CountOfFingers { get; set; }
		[DataMember]
		public int CountOfUnreadAttendance { get; set; }
	}
}
