using System;
using System.Text;
using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiBasicAuthorizationData : IRestApiAuthorizationData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public void ApplyAuthorization(RestRequest request)
        {
            var authenticationString = $"{Username}:{Password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            request.AddHeader(ServiceConstants.ApiAuthorizationHeader, $"Basic {base64EncodedAuthenticationString}");
        }

    }
}