using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// پایان همگام‌سازی کامل برد: برد هر چیزی را که در این فهرست‌ها نیست حذف می‌کند؛
    /// بدنه POST api/v1/sync/keep-only در Standalone و محتوای دستور GuardianControllerKeepOnly در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerKeepOnlyCommunicationModel
    {
        public List<Guid> calendar_template_ids { get; set; }
        public List<Guid> calendar_ids { get; set; }
        public List<Guid> access_level_ids { get; set; }
        public List<Guid> access_group_ids { get; set; }
        public List<long> user_ids_on_device { get; set; }
    }
}
