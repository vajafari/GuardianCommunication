using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoVirdiAccessControlData
    {
        public VirdiAccessControlDataTypeEnumeration AccessDataType { get; set; }
        public List<DtoVirdiHolidayGroup> Holidays { get; set; }
        public List<DtoVirdiTimeZone> Timezones { get; set; }
        public List<DtoVirdiAccessTime> AccessTimes { get; set; }
        public List<DtoVirdiAccessGroup> AccessGroups { get; set; }
    }
}
