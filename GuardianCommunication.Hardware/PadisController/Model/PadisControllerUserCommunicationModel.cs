using System.Collections.Generic;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerUserCommunicationModel
    {
        public long user_id { get; set; }
        public List<string> rf_card_numbers { get; set; }
        public string password { get; set; }
        public bool is_enable { get; set; }
        public DeviceUserTypeEnumeration user_type { get; set; }
        public long start_date { get; set; }
        public long end_date { get; set; }

        public List<int> user_group_numbers { get; set; }

        
        public List<PadisControllerEasyPermissionCommunicationModel> user_permissions { get; set; }

        public List<PadisControllerUserLimitationAttendanceCommunicationModel> attendance_limitations { get; set; }

        public List<PadisControllerUserLimitationTimeCommunicationModel> time_limitations { get; set; }

    }
}
