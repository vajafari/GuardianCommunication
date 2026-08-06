namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerDeviceDoorCommunicationModel
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? ReaderDeviceNumber { get; set; }
        public int? WiegandId { get; set; }
        public int? PassVerificationIoPortId { get; set; }
        public int? OpenTimeCalendarNumber { get; set; }
        public string CombinationAccessGroupNumbersInJson { get; set; }
        public int? ReaderIoType { get; set; }
        public int DoorNumberOnDevice { get; set; }
    }
}
