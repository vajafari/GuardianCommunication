namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerDeviceInfoCommunicationModel
    {
        public string FirmwareInfo { get; set; }
        public string SerialNumber { get; set; }
        public int UserCount { get; set; }
        public int DoorCount { get; set; }
        public int SuccessAttendanceCount { get; set; }
        public int FailedAttendanceCount { get; set; }
        public int EventCount { get; set; }
    }
}
