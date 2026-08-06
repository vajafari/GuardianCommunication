using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoHookSystem
    {
        public int Id { get; set; }

        public string SystemName { get; set; }

        public string AuthorizationUsername { get; set; }

        public string AuthorizationPassword { get; set; }

        public AuthorizationTypeEnumeration AuthorizationType { get; set; }

        public string AuthorizationToken { get; set; }

        public bool UseProxy { get; set; }

        public string ProxyUrl { get; set; }

        public string ProxyUsername { get; set; }

        public string ProxyPassword { get; set; }

        public bool BypassProxyOnLocal { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public List<DtoHookSystemDetail> Details { get; set; } = new List<DtoHookSystemDetail>();
    }
}
