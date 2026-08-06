using GuardianCommunication.Hardware.PadisController.Definition;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerGrpcDeviceCommandResultModel
    {
        public int CommandId { get; set; }
        public string DeviceSerialNumber { get; set; }
        public PadisControllerErrorEnumeration ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
