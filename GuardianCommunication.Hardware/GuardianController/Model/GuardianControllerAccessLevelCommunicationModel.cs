using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// سطح دسترسی (درب‌های همین برد)؛ بدنه POST api/v1/access-levels/upsert در Standalone و محتوای دستور GuardianControllerSetAccessLevel در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerAccessLevelCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// فقط برای نمایش در UI برد؛ ارجاع به سطح دسترسی همه‌جا با id است
        /// </summary>
        public int access_level_number { get; set; }

        public string title { get; set; }
        public bool is_active { get; set; }
        public List<Guid> door_ids { get; set; }
    }
}
