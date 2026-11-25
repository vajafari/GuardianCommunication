using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// خطای اجرای فرمان روی برد؛ بدنه پاسخ ناموفق API در Standalone و محتوای پیام response_command با error_code غیرصفر در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerErrorCommunicationModel
    {
        public string error_code { get; set; }
        public string message { get; set; }

        /// <summary>
        /// ریز خطاها (مثلا هر ارجاع نامعتبر در پیکربندی)
        /// </summary>
        public List<string> errors { get; set; }
    }
}
