namespace GuardianCommunication.Shared.Dto
{
	public class DtoUserPalm
	{
		public long UserIdOnDevice { get; set; }
		public byte[] TemplateData { get; set; }
		public int Index { get; set; }
		public uint CheckSum { get; set; }
		public int Length { get; set; }

	}
}
