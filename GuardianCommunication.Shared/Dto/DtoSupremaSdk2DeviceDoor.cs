namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk2DeviceDoor : DtoDeviceDoorBase
	{
		public int DeviceDoorId { get; set; }

		public int AutoLockTimeout { get; set; }

		public int HeldOpenTimeout { get; set; }
		
		public bool UnconditionalLock { get; set; }

		public byte ForceOpenAlarmType { get; set; }

		public byte HeldOpenAlarmType { get; set; }

		public bool InstantLock { get; set; }

		public byte LockFlags { get; set; }

		public byte UnlockFlag { get; set; }

		// Door Sensor
		public bool HasDoorSensor { get; set; }
		
		public byte DoorSensor { get; set; }

		public byte SensorType { get; set; }

		public bool ApbUseDoorSensor { get; set; }

		// Exit Button
		public bool HasExitButton { get; set; }

		public byte ExitButton { get; set; }

		public byte ExitButtonType { get; set; }

		// Relay
		public bool HasRelay { get; set; }
		
		public int Relay { get; set; }
	}
}
