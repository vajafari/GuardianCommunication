using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// سطح دسترسی برد کنترلر گاردین با درب‌های همین برد
    /// </summary>
    public class DtoGuardianControllerAccessLevel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// شماره سطح (برچسب)
        /// </summary>
        public int AccessLevelNumber { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// درب‌های همین برد
        /// </summary>
        public List<Guid> DoorIds { get; set; } = [];
    }
}
