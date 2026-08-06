using System.Collections.Generic;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerAccessLevelCommunicationModel
    {
        public int access_level_number { get; set; }
        public string title { get; set; }
        public bool is_active { get; set; }
        public List<int> door_ids { get; set; }
    }
}
