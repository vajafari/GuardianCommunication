using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// پاسخ تطبیق روی سرور؛ محتوای پیام match_on_server_response در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerMatchOnServerResponseModel
    {
        public bool is_authorize { get; set; }

        /// <summary>
        /// شماره کاربری که سرور برای کارت پیدا کرد
        /// </summary>
        public long? user_id_on_device { get; set; }
    }
}
