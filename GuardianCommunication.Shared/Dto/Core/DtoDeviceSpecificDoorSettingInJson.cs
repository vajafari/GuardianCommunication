namespace GuardianCommunication.Shared.Dto.Core
{
    public class DtoDeviceSpecificDoorSettingInJson
    {
        public SupremaSdk1DeviceDoorSettings Suprema1DoorSetting { get; set; }
        public SupremaSdk2DeviceDoorSettings Suprema2DoorSetting { get; set; }
    }

    public class SupremaSdk1DeviceDoorSettings
    {
        public int RelayDeviceId { get; set; }
        public int DoorSensor { get; set; }
    }

    public class SupremaSdk2DeviceDoorSettings
    {
        public int DeviceDoorId { get; set; }
        public byte UnlockFlag { get; set; }
        public byte LockFlag { get; set; }
        public int AutoLockTimeout { get; set; }
    }
}
