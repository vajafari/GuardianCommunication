using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// بازه زمانی و صفحه‌بندی خواندن ترددها؛ بدنه POST api/v1/logs/attendances
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerDateIntervalCommunicationModel
    {
        /// <summary>
        /// شروع بازه به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long start_time { get; set; }

        /// <summary>
        /// پایان بازه به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long end_time { get; set; }

        public int limit { get; set; }
        public int offset { get; set; }
    }
}
