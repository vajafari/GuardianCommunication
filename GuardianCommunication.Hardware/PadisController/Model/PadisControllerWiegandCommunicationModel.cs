namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerWiegandCommunicationModel
    {
        public int Id { get; set; }
        public int WiegandNumber { get; set; }
        public int WiegandFormat { get; set; }
        public int WiegandDataType { get; set; }
        public bool IsActive { get; set; }
    }
}
