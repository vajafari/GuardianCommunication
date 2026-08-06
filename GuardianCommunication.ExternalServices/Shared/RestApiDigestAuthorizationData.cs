using RestSharp;
using RestSharp.Authenticators.Digest;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiDigestAuthorizationData : IRestApiAuthorizationData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public void ApplyAuthorization(RestRequest request)
        {
            request.Authenticator = new DigestAuthenticator(Username, Password);
        }

    }
}