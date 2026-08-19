namespace GuardianCommunication.Shared.Dto
{
	public class DtoUserFace
    {
		public long UserIdOnDevice { get; set; }
		public byte[] TemplateData { get; set; }
		public int FaceIndex { get; set; }
		public uint CheckSum { get; set; }
		public int Length { get; set; }

        public DtoSupremaSdk2FaceTemplateAdditionalData SupremaSdk2AdditionalData { get; set; }

	}
}
