using System.Collections.Generic;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerAccessGroupCommunicationModel
    {
        public int access_group_number { get; set; }
        public string title { get; set; }
        public bool is_active { get; set; }

        public List<PadisControllerAccessGroupAccessDataCommunicationModel> access_data { get; set; }
    }
}
