using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SupremaSdk1DeviceDoorModel
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
		public int Relay { get; set; }

		[DataMember]
		public int OpenEvent { get; set; }

		[DataMember]
		public int OpenTime { get; set; }

		[DataMember]
		public int HeldOpenTime { get; set; }

		[DataMember]
		public int ForcedOpenSchedule { get; set; }

		[DataMember]
		public int ForcedCloseSchedule { get; set; }

		[DataMember]
		public int RteType { get; set; }

		[DataMember]
		public int SensorType { get; set; }

		[DataMember]
		public int Reader1 { get; set; }

		[DataMember]
		public int Reader2 { get; set; }

		[DataMember]
		public bool UseRteEx { get; set; }

		[DataMember]
		public bool UseSoundForcedOpen { get; set; }

		[DataMember]
		public bool UseSoundHeldOpen { get; set; }

		[DataMember]
		public bool OpenOnce { get; set; }

		[DataMember]
		public int Rte { get; set; }

		[DataMember]
		public bool UseDoorSensorEx { get; set; }

		[DataMember]
		public bool AlarmStatus { get; set; }

		[DataMember]
		public int DoorSensor { get; set; }

		[DataMember]
		public int RelayDeviceId { get; set; }
	}
}
