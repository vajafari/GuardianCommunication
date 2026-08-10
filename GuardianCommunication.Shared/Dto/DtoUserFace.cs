namespace GuardianCommunication.Shared.Dto
{
	public class DtoUserFace
    {
		public long UserIdOnDevice { get; set; }
		public byte[] TemplateData { get; set; }
		public int FaceIndex { get; set; }
		public uint CheckSum { get; set; }
		public int Length { get; set; }

        #region SupremaSdk2

        public byte? SupremaSdk2FaceFlag { get; set; }
		public int? SupremaSdk2FaceImageLen { get; set; }
		public byte[] SupremaSdk2FaceImageData { get; set; }
		public byte? SupremaSdk2FaceNumOfTemplate { get; set; }

		#endregion

	}
}
