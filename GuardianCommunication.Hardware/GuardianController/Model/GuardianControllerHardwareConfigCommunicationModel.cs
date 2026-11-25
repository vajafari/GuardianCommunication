using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// پیکربندی کامل سخت‌افزار برد؛ بدنه POST api/v1/config/hardware در Standalone و محتوای دستور GuardianControllerSetHardwareConfig در Push.
    /// جایگزین کل پیکربندی قبلی می‌شود؛ هر رله، ویگند و ورودی/خروجی یک بار تعریف می‌شود و درب‌ها با شناسه به آن‌ها ارجاع می‌دهند
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerHardwareConfigCommunicationModel
    {
        public List<GuardianControllerHardwareConfigRelayCommunicationModel> relays { get; set; }
        public List<GuardianControllerHardwareConfigWiegandCommunicationModel> wiegands { get; set; }
        public List<GuardianControllerHardwareConfigIoPortCommunicationModel> io_ports { get; set; }
        public List<GuardianControllerHardwareConfigDoorCommunicationModel> doors { get; set; }
    }
}
