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

        public HttpMethodEnumeration HttpMethod { get; set; }

        public AuthorizationTypeEnumeration AuthorizationType { get; set; }

        public string DateFormat { get; set; }

        public string AuthorizationUsername { get; set; }

        public string AuthorizationPassword { get; set; }
        
        public string AuthorizationBearerToken { get; set; }

        public TimeSpan? RequestTimeout { get; set; }

    }
}
