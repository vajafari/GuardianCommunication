namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerRelayCommunicationModel
    {
        public int Id { get; set; }
        public int RelayNumber { get; set; }
        public PadisControllerRelayTypeEnumeration RelayType { get; set; }
        public bool IsActive { get; set; }
    }
}
