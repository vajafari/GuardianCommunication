using System;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// درخواست تطبیق روی سرور برد کنترلر گاردین برای گاردین (HardwareEvent/SubmitServerMatching)؛ زمان به UTC
    /// </summary>
    public class ServerMatchDataModel
    {
        public Guid DeviceId { get; set; }

        public DateTime EventDateTime { get; set; }

        public string RfCardNumber { get; set; }

        public long? UserIdOnDevice { get; set; }

        /// <summary>
        /// شناسه درب تردد (درب همین دستگاه)
        /// </summary>
        public Guid? DoorId { get; set; }

    }
}
