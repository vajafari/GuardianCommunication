using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DataCountModel
	{
		public int Count { get; set; }
		public Guid DeviceId { get; set; }
	}
}
