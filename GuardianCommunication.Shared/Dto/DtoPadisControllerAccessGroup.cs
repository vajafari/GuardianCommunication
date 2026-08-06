using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerAccessGroup
    {
        public int AccessGroupNumber { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public List<DtoPadisControllerAccessGroupAccessData> AccessData { get; set; }
    }
}