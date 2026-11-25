using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// اجازه مدیر درب: درب برای کاربران عادی بسته است تا عضوی از این گروه‌ها کارت بزند؛ فهرست خالی یعنی اجازه مدیر خاموش است
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigMasterAccessPrimerCommunicationModel
    {
        public List<Guid> master_groups { get; set; }
        public GuardianControllerMasterPrimerResetPeriodEnumeration reset_period { get; set; }

        /// <summary>
        /// ساعت ریست به شکل HHmm؛ در دوره Duration همین مقدار مدت است
        /// </summary>
        public short reset_time { get; set; }

        public bool apply_to_enter_only { get; set; }
        public bool toggle_mode { get; set; }
    }
}
