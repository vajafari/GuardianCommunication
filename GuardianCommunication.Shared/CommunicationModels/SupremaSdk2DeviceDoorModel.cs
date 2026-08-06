using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk2DeviceDoorModel
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public int DeviceNumber { get; set; }

		[DataMember]
		public int DoorNumber { get; set; }

		[DataMember]
		public bool IsActive { get; set; }

		[DataMember]
		public int OpenDoorDelay { get; set; }

		[DataMember]
		public int DeviceDoorId { get; set; }

		[DataMember]
		public int AutoLockTimeout { get; set; }

		[DataMember]
		public int HeldOpenTimeout { get; set; }

		[DataMember]
		public bool UnconditionalLock { get; set; }

		[DataMember]
		public byte ForceOpenAlarmType { get; set; }

		[DataMember]
		public byte HeldOpenAlarmType { get; set; }

		[DataMember]
		public bool InstantLock { get; set; }

		[DataMember]
		public byte LockFlags { get; set; }

		[DataMember]
		public byte UnlockFlag { get; set; }

		// Door Sensor
		[DataMember]
		public bool HasDoorSensor { get; set; }

		[DataMember]
		public byte DoorSensor { get; set; }

		[DataMember]
		public byte SensorType { get; set; }

		[DataMember]
		public bool ApbUseDoorSensor { get; set; }

		// Exit Button
		[DataMember]
		public bool HasExitButton { get; set; }

		[DataMember]
		public byte ExitButton { get; set; }

		[DataMember]
		public byte ExitButtonType { get; set; }

		// Relay
		[DataMember]
		public bool HasRelay { get; set; }

		[DataMember]
		public int Relay { get; set; }
	}
}
