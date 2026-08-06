using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class NotSendCommandsStatisticsModel
	{
		[DataMember]
		public int DeviceNumber { get; set; }
		[DataMember]
		public int MaxAttemptCommandCount { get; set; }
		[DataMember]
		public int NotSendCommandCounts { get; set; }
	}
}
