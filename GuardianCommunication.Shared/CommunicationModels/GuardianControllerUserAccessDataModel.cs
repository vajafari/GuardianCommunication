using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// داده‌های دسترسی کاربر روی یک برد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerUserAccessDataModel
    {
        /// <summary>
        /// گروه‌هایی از کاربر که سطحشان روی همین برد درب دارد
        /// </summary>
        [DataMember]
        public List<Guid> UserGroupIds { get; set; }

        /// <summary>
        /// دسترسی‌های آسان پایان نیافته کاربر روی همین برد
        /// </summary>
        [DataMember]
        public List<GuardianControllerUserEasyAccessModel> EasyAccesses { get; set; }
    }
}
