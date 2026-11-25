using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// پایان همگام‌سازی کامل برد کنترلر گاردین: همه چیزهایی که روی برد می‌مانند؛ برد بقیه را حذف می‌کند. دسترسی‌های آسان همراه کاربرشان می‌مانند یا حذف می‌شوند
    /// </summary>
    public class DtoGuardianControllerKeepOnly
    {
        public List<Guid> CalendarTemplateIds { get; set; } = [];

        public List<Guid> CalendarIds { get; set; } = [];

        public List<Guid> AccessLevelIds { get; set; } = [];

        public List<Guid> AccessGroupIds { get; set; } = [];

        /// <summary>
        /// شماره کاربران روی برد
        /// </summary>
        public List<long> UserIdsOnDevice { get; set; } = [];
    }
}
