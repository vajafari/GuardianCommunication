using System;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Shared.HardwareDefinition;
using Newtonsoft.Json;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// تردد برد؛ پاسخ POST api/v1/logs/attendances در Standalone و محتوای پیام device_attendance و match_on_server در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerAttendanceCommunicationModel
    {
        /// <summary>
        /// شناسه لاگ روی برد؛ یکتا و رو به افزایش
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// شماره کاربر روی برد؛ خالی یعنی کارت ناشناخته
        /// </summary>
        public long? user_id_on_device { get; set; }

        public string rf_card_number { get; set; }

        /// <summary>
        /// شناسه درب (id درب در پیکربندی سخت‌افزار)، نه شماره درب
        /// </summary>
        public Guid? door_id { get; set; }

        public DeviceIoTypeEnumeration io_type { get; set; }

        /// <summary>
        /// زمان تردد به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long attendance_date_time { get; set; }

        public GuardianControllerAttendanceStatusEnumeration verification_status { get; set; }

        [JsonIgnore]
        public bool IsAccessGranted => verification_status == GuardianControllerAttendanceStatusEnumeration.Success;
    }
}
