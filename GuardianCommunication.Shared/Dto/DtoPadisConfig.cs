using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisConfig
    {
        public int ApplicationType { get; set; }
        public int SerialNumber { get; set; }
        public short CalendarType { get; set; }
        public int EmployeeCount { get; set; }
        public int TaSoftwareType { get; set; }
        public string CompanyName { get; set; }
        public DateTime? ExpireDate { get; set; }
        public short DeviceCount { get; set; }
    }
}
