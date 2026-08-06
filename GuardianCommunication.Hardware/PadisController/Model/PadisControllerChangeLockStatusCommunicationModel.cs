namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerChangeLockStatusCommunicationModel
    {
        public int door_id { get; set; }
        /// <summary>
        /// 0 = Close
        /// 1 = Open
        /// </summary>
        public short lock_status { get; set; }
        /// <summary>
        /// null: Permanent
        /// Number = Delay
        /// </summary>
        public int? duration { get; set; }
        /// <summary>
        /// null: Permanent
        /// Number = Delay
        /// </summary>
        public bool force_override_emergency { get; set; } = false;
    }
}