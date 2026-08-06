using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerUserAccessDataModel
    {
        [DataMember]
        public List<int> UserGroupNumbers { get; set; }
        [DataMember]
        public List<PadisControllerEasyPermissionModel> EasyPermissions { get; set; }
        [DataMember]
        public List<PadisControllerUserLimitationTimeModel> TimeLimitations { get; set; }
        [DataMember]
        public List<PadisControllerUserLimitationAttendanceModel> AttendanceLimitations { get; set; }
    }
}

