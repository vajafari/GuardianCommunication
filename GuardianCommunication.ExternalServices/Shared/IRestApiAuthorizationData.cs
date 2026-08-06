using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public interface IRestApiAuthorizationData
    {
        void ApplyAuthorization(RestRequest request);
    }
}

