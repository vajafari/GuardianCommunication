using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceDoor
	{
        public int Id { get; set; }

        public int DeviceNumber { get; set; }

        public int? ReaderCameraId { get; set; }

        public int? ReaderDeviceNumber { get; set; }

        public int ReaderAreaumber { get; set; }

        public DeviceIoTypeEnumeration ReaderIoType { get; set; }

        public ApplicationTypeEnumeration ReaderApplicationId { get; set; }
    }
}
