using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// محتوای پیام send_command در Push
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerGrpcDeviceCommandModel
    {
        /// <summary>
        /// نوع دستور (DeviceCommandTypeEnumeration)
        /// </summary>
        public int command_type { get; set; }

        /// <summary>
        /// مدل ارتباطی همان فرمان (همان بدنه API در Standalone) به شکل JSON
        /// </summary>
        public string command_content { get; set; }
    }
}
