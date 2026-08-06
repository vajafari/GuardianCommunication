using System;
using System.Collections.Generic;
using RestSharp;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestApiRequestData
    {
        public string Uri { get; set; }

        public object Body { get; set; }

        public string BodyString { get; set; }

        public string BodyStringContentType { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public Dictionary<string, string> QueryStringParameters { get; set; }

        public AuthorizationTypeEnumeration AuthorizationType { get; set; }

        public IRestApiAuthorizationData AuthorizationData { get; set; }

        public TimeSpan? RequestTimeout { get; set; }
        
        public Method? Method { get; set; }

    }
}

