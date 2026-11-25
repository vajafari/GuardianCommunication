using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// حذف کاربر با گروه‌ها و دسترسی‌های آسانش؛ بدنه POST api/v1/users/delete در Standalone و محتوای دستور DeleteUser در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerUserIdOnDeviceCommunicationModel
    {
        public long user_id_on_device { get; set; }
    }
}
