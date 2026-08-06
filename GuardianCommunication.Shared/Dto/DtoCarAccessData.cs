using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoCarAccessData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PlateString { get; set; }
        public CarPlateTypeEnumeration PlateType { get; set; }
        public List<DtoCarAccessInterval> Intervals { get; set; }
    }

}
