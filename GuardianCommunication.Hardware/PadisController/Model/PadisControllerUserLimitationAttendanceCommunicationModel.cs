namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerUserLimitationAttendanceCommunicationModel
    {
        public long start_date { get; set; }
        public long end_date { get; set; }
        public int attendance_count { get; set; }
        public int? door_id { get; set; }
        public int? io_type { get; set; }
    }
}