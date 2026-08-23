using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoAttendanceSaveResult
	{
        public List<DtoAttendance> AllRecords { get; set; } = [];
        public List<DtoAttendance> Successful { get; set; } = [];
        public List<DtoAttendance> ExistingRecords { get; set; } = [];
        /// <summary>
        /// تردد های که معتبر نیستن و نباید ذخیره سازی شوند و هوک شوند و هیچ عملیاتی بر روی آنها انجام شود
        /// </summary>
        public List<DtoAttendance> InvalidAttendances { get; set; } = [];
        public List<DtoAttendance> UnknownErrorSave { get; set; } = [];
        public List<DtoAttendance> InvalidDeviceSerialNumberRecords { get; set; } = [];
    }
}
