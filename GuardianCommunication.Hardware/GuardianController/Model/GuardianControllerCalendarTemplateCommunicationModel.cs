using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// قالب تقویم (چرخه چندروزه)؛ بدنه POST api/v1/calendar-templates/upsert در Standalone و محتوای دستور GuardianControllerSetCalendarTemplate در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerCalendarTemplateCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// فقط برای نمایش در UI برد؛ ارجاع به قالب همه‌جا با id است
        /// </summary>
        public int calendar_template_number { get; set; }

        public string title { get; set; }
        public int day_count { get; set; }
        public List<GuardianControllerCalendarTemplateDetailsCommunicationModel> details { get; set; }
    }
}
