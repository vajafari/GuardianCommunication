
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerDeviceDoor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DeviceNumber { get; set; }
        public string DeviceTitle { get; set; }
        public int DoorNumber { get; set; }
        public bool IsActive { get; set; }
        public int OpenDoorDelay { get; set; }
        public int? ReaderDeviceNumber { get; set; }
        public DeviceIoTypeEnumeration? ReaderIoType { get; set; }
        public int? WiegandId { get; set; }
        public int? PassVerificationIoPortId { get; set; }
        public int? OpenTimeCalendarNumber { get; set; }
        public string CombinationAccessGroupNumbersInJson { get; set; }
        public int DoorNumberOnDevice { get; set; }

    }
}