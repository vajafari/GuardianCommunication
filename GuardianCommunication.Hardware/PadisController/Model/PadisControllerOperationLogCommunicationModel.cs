namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerOperationLogCommunicationModel
    {
        public long Id { get; set; }
        public long? EmployeeNumber { get; set; }
        public long EventDateTime { get; set; }
        public int DeviceNumber { get; set; }
        public int EventCode { get; set; }

    }
}
