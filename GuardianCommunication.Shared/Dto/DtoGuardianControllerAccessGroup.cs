using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// گروه دسترسی برد کنترلر گاردین با قاعده‌های همین برد
    /// </summary>
    public class DtoGuardianControllerAccessGroup
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// شماره گروه (برچسب)
        /// </summary>
        public int AccessGroupNumber { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        public List<DtoGuardianControllerAccessGroupAccessData> AccessData { get; set; } = [];
    }
}
