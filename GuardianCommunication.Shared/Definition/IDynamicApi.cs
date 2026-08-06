
namespace GuardianCommunication.Shared.Definition
{
    public interface IDynamicApi
    {
        string AuthorizationUsername { get; set; }
        string AuthorizationPassword { get; set; }
        AuthorizationTypeEnumeration AuthorizationType { get; set; }
        string AuthorizationToken { get; set; }
        string EndPointUrl { get; set; }
        int HttpMethod { get; set; }
        string QueryString { get; set; }
        string Header { get; set; }
        string Body { get; set; }
        string DateFormat { get; set; }
        int RequestTimeoutInSeconds { get; set; }
    }
}
