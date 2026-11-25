using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// تطبیق روی سرور: برد کنترلر گاردین در لحظه تردد از گاردین می پرسد که تردد مجاز است یا نه؛ دستگاه دیگری تطبیق روی سرور ندارد
    /// </summary>
    public class DtoServerMatchData
    {
        public Guid DeviceId { get; set; }

        public DateTime EventDateTime { get; set; }

        public string RfCardNumber { get; set; }

        public long? UserIdOnDevice { get; set; }

        /// <summary>
        /// شناسه درب گاردین که برد فرستاده است، فقط اگر درب همین دستگاه باشد
        /// </summary>
        public Guid? DoorId { get; set; }

    }
}
