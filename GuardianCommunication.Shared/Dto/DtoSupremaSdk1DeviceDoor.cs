namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk1DeviceDoor : DtoDeviceDoorBase
	{

		public int Relay { get; set; }

		public int OpenEvent { get; set; }

		public int OpenTime { get; set; }

		public int HeldOpenTime { get; set; }

		public int ForcedOpenSchedule { get; set; }

		public int ForcedCloseSchedule { get; set; }

		public int RteType { get; set; }

		public int SensorType { get; set; }

		public int Reader1 { get; set; }

		public int Reader2 { get; set; }

		public bool UseRteEx { get; set; }

		public bool UseSoundForcedOpen { get; set; }

		public bool UseSoundHeldOpen { get; set; }

		public bool OpenOnce { get; set; }

		public int Rte { get; set; }

		public bool UseDoorSensorEx { get; set; }

		public bool AlarmStatus { get; set; }

		public int DoorSensor { get; set; }

		public int RelayDeviceId { get; set; }

	}
}
