using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// ساعت برد؛ بدنه POST و پاسخ GET api/v1/system/time در Standalone و پاسخ پیام sync_datetime در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerDateAndTimeCommunicationModel
    {
        /// <summary>
        /// ساعت به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long date_time_epoch { get; set; }

        /// <summary>
        /// منطقه زمانی IANA برد، مثلا Asia/Tehran
        /// </summary>
        public string time_zone { get; set; }
    }
}
