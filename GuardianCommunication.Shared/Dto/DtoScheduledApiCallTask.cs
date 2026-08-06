using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoScheduledApiCallTask : IDynamicApi
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string Cron { get; set; }
        public string AuthorizationUsername { get; set; }
        public string AuthorizationPassword { get; set; }
        public AuthorizationTypeEnumeration AuthorizationType { get; set; }
        public string AuthorizationToken { get; set; }
        public string EndPointUrl { get; set; }
        public int HttpMethod { get; set; }
        public string QueryString { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public string DateFormat { get; set; }
        public int RequestTimeoutInSeconds { get; set; }
    }
}