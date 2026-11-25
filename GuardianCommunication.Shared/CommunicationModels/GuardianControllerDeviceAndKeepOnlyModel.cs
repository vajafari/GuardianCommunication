using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// پایان همگام‌سازی کامل یک برد کنترلر گاردین: همه چیزهایی که روی برد می‌مانند؛ برد بقیه را حذف می‌کند
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndKeepOnlyModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        [DataMember]
        public List<Guid> CalendarTemplateIds { get; set; }

        [DataMember]
        public List<Guid> CalendarIds { get; set; }

        [DataMember]
        public List<Guid> AccessLevelIds { get; set; }

        [DataMember]
        public List<Guid> AccessGroupIds { get; set; }

        /// <summary>
        /// شماره اشخاص روی دستگاه
        /// </summary>
        [DataMember]
        public List<long> PersonNumbersOnDevice { get; set; }
    }
}
