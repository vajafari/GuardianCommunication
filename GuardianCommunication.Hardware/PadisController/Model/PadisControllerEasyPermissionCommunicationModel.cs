namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerEasyPermissionCommunicationModel
    {
        public long start_date { get; set; }
        public long end_date { get; set; }
        public int door_id { get; set; }
        public int? calendar_number { get; set; }
    }
}
