using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigDoorCommunicationModel
    {
        /// <summary>
        /// شناسه درب در گاردین؛ برد همین را در تردد و رویداد درب می‌فرستد
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// فقط برای نمایش در UI برد؛ در ارتباط استفاده نمی‌شود و درب همه‌جا با id شناخته می‌شود
        /// </summary>
        public int door_number { get; set; }

        public string title { get; set; }

        /// <summary>
        /// 1 عادی، 2 آسانسور، 3 پارکینگ
        /// </summary>
        public short door_type { get; set; }

        /// <summary>
        /// مدت باز ماندن قفل پس از تردد مجاز (ثانیه)
        /// </summary>
        public int unlock_duration { get; set; }

        /// <summary>
        /// مهلت باز ماندن درب پیش از هشدار (ثانیه)؛ خالی یعنی پیش‌فرض برد
        /// </summary>
        public int? door_held_timeout { get; set; }

        /// <summary>
        /// جهت کارتخوان درب
        /// </summary>
        public DeviceIoTypeEnumeration reader_io_type { get; set; }

        public bool match_on_server_enabled { get; set; }

        /// <summary>
        /// برد پیش از پرسیدن از سرور دسترسی را خودش هم بررسی می‌کند
        /// </summary>
        public bool match_on_server_local_check_enabled { get; set; }

        public List<Guid> relay_ids { get; set; }
        public Guid? wiegand_id { get; set; }
        public List<Guid> io_port_ids { get; set; }
        public GuardianControllerHardwareConfigDoorCalendarCommunicationModel door_calendar { get; set; }
        public GuardianControllerHardwareConfigMasterAccessPrimerCommunicationModel master_access_primer { get; set; }
    }
}
