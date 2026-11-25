using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// یک قاعده گروه: سطح دسترسی با تقویم
    /// </summary>
    public class DtoGuardianControllerAccessGroupAccessData
    {
        /// <summary>
        /// شناسه ردیف داده دسترسی در گاردین
        /// </summary>
        public Guid Id { get; set; }

        public Guid AccessLevelId { get; set; }

        public Guid CalendarId { get; set; }
    }
}
