namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceCommand : DtoDeviceCommandWithoutContent
	{
		public string CommandContent { get; set; }
		
		public string DeviceContent { get; set; }
	}
}
