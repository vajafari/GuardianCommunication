using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoAttendanceSaveResult
	{
        public List<DtoAttendance> AllRecords { get; set; } = new List<DtoAttendance>();
        public List<DtoAttendance> Successful { get; set; } = new List<DtoAttendance>();
        public List<DtoAttendance> ExistingRecords { get; set; } = new List<DtoAttendance>();
        /// <summary>
        /// تردد های که معتبر نیستن و نباید ذخیره سازی شوند و هوک شوند و هیچ عملیاتی بر روی آنها انجام شود
        /// </summary>
        public List<DtoAttendance> InvalidAttendances { get; set; } = new List<DtoAttendance>();
        public List<DtoAttendance> UnknownErrorSave { get; set; } = new List<DtoAttendance>();
        public List<DtoAttendance> InvalidDeviceSerialNumberRecords { get; set; } = new List<DtoAttendance>();
    }
}
