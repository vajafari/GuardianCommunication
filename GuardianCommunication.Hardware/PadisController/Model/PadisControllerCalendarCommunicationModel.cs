using System.Collections.Generic;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerCalendarCommunicationModel
    {
        public int calendar_number { get; set; }
        public string title { get; set; }
        public bool is_active { get; set; }

        public List<PadisControllerCalendarDetailsCommunicationModel> details { get; set; }
    }
}
