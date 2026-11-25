using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// گروه دسترسی؛ بدنه POST api/v1/access-groups/upsert در Standalone و محتوای دستور GuardianControllerSetAccessGroup در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerAccessGroupCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// فقط برای نمایش در UI برد؛ ارجاع به گروه دسترسی همه‌جا با id است
        /// </summary>
        public int access_group_number { get; set; }

        public string title { get; set; }
        public bool is_active { get; set; }
        public List<GuardianControllerAccessGroupAccessDataCommunicationModel> access_data { get; set; }
    }
}
