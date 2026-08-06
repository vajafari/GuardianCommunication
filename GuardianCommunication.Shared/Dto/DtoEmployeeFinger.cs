namespace GuardianCommunication.Shared.Dto
{
	public class DtoEmployeeFinger
	{
		public long EmployeeNumber { get; set; }
		public byte[] TemplateData { get; set; }
		public int FingerIndex { get; set; }
		public uint CheckSum { get; set; }

	}
}
