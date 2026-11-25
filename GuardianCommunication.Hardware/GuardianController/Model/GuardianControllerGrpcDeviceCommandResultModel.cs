using GuardianCommunication.Hardware.GuardianController.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// نتیجه ارسال یک فرمان صف روی استریم gRPC برد
    /// </summary>
    public class GuardianControllerGrpcDeviceCommandResultModel
    {
        public long CommandId { get; set; }
        public string DeviceSerialNumber { get; set; }
        public GuardianControllerErrorEnumeration ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
