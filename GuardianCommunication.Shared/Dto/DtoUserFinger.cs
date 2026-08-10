namespace GuardianCommunication.Shared.Dto
{
	public class DtoUserFinger
	{
		public long UserIdOnDevice { get; set; }
		public byte[] TemplateData { get; set; }
		public int FingerIndex { get; set; }
		public uint CheckSum { get; set; }

	}
}
