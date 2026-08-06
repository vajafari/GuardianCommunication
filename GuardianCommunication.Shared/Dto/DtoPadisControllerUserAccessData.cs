using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerUserAccessData
    {
        public List<int> UserGroupNumbers { get; set; }
        public List<DtoPadisControllerUserEasyPermission> EasyPermissions { get; set; } = new List<DtoPadisControllerUserEasyPermission>();
        public List<DtoPadisControllerUserLimitationTime> TimeLimitations { get; set; } = new List<DtoPadisControllerUserLimitationTime>();
        public List<DtoPadisControllerUserLimitationAttendance> AttendanceLimitations { get; set; } = new List<DtoPadisControllerUserLimitationAttendance>();
    }
}
