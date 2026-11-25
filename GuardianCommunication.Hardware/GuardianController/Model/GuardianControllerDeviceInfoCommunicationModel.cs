using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// اطلاعات برد؛ پاسخ GET api/v1/system/info
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerDeviceInfoCommunicationModel
    {
        public string serial_number { get; set; }
        public string firmware_version { get; set; }
        public int user_count { get; set; }
        public int door_count { get; set; }
        public int success_attendance_count { get; set; }
        public int failed_attendance_count { get; set; }
        public int event_count { get; set; }
    }
}
