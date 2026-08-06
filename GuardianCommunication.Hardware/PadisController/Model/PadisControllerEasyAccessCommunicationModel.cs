namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerEasyAccessCommunicationModel
    {
        public int Id { get; set; }
        public long StartDateTime { get; set; }
        public long EndDateTime { get; set; }
        public int? CalendarNumber { get; set; }
        public int DoorId { get; set; }
    }
}
