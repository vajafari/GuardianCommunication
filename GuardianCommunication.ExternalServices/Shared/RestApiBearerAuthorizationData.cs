using GuardianCommunication.Shared.Definition;
using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiBearerAuthorizationData: IRestApiAuthorizationData
    {
        public string Token{ get; set; }
        
        public void ApplyAuthorization(RestRequest request)
        {
            request.AddHeader(ServiceConstants.ApiAuthorizationHeader, $"Bearer {Token}");
        }
    }
}
