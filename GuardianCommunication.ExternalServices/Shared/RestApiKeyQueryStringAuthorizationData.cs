using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiKeyQueryStringAuthorizationData : IRestApiAuthorizationData
    {
        public string KeyName { get; set; }
        public string ApiKey { get; set; }

        public void ApplyAuthorization(RestRequest request)
        {
            request.AddQueryParameter(KeyName, ApiKey);
        }
    }
}