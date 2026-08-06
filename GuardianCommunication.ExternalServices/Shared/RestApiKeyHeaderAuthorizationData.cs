using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiKeyHeaderAuthorizationData : IRestApiAuthorizationData
    {
        public string KeyName { get; set; }
        public string ApiKey { get; set; }

        public void ApplyAuthorization(RestRequest request)
        {
            request.AddHeader(KeyName, ApiKey);
        }
    }
}