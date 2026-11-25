using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// داده‌های دسترسی کاربر روی یک برد کنترلر گاردین
    /// </summary>
    public class DtoGuardianControllerUserAccessData
    {
        /// <summary>
        /// گروه‌هایی از کاربر که سطحشان روی همین برد درب دارد
        /// </summary>
        public List<Guid> UserGroupIds { get; set; } = [];

        /// <summary>
        /// دسترسی‌های آسان پایان نیافته کاربر روی همین برد
        /// </summary>
        public List<DtoGuardianControllerUserEasyAccess> EasyAccesses { get; set; } = [];
    }
}
