using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class UserPalmModel
	{
		[DataMember]
		public long UserIdOnDevice { get; set; }
		[DataMember]
		public string TemplateData { get; set; }
		[DataMember]
		public int Index { get; set; }
		[DataMember]
		public uint CheckSum { get; set; }
		[DataMember]
		public int Length { get; set; }
	}
}
