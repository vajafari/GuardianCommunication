namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceDoorBase
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int DeviceNumber { get; set; }

		public int DoorNumber { get; set; }

		public bool IsActive { get; set; }

		public int OpenDoorDelay { get; set; }

	}
}
