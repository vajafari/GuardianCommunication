using GuardianCommunication.Shared.Definition;
using RestSharp;
using System;

namespace GuardianCommunication.ExternalServices.Hooks
{
    /// <summary>
    /// رکورد مربوط به سیستم هوک
    /// </summary>
    public class HookSystemModel
    {
        public string EndPointUrl { get; set; }
        public string QueryStringTemplate { get; set; }
        public string HeaderTemplate { get; set; }
        public string BodyTemplate { get; set; }

        public Method HttpMethod { get; set; }

        public AuthorizationTypeEnumeration AuthorizationType { get; set; }

        public string DateFormat { get; set; }

        public string AuthorizationUsername { get; set; }

        public string AuthorizationPassword { get; set; }
        
        public TimeSpan? RequestTimeout { get; set; }

    }
}
