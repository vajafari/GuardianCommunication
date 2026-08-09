using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.OperationResult;
using RestSharp;
using RestSharp.Authenticators.Digest;

namespace GuardianCommunication.ExternalServices.Shared
{
    public class RestSharpClient
    {
        private readonly RestClient _client;
        private readonly ConcurrentDictionary<string, RestClient> _digestClients = new ConcurrentDictionary<string, RestClient>();


        private RestSharpClient()
        {
            var options = new RestClientOptions
            {
                FailOnDeserializationError = false,
                ThrowOnDeserializationError = false,
                ThrowOnAnyError = false,
            };
            _client = new RestClient(options);


        }

        private static RestSharpClient _instance;

        public static RestSharpClient GetInstance()
        {
            return _instance ?? (_instance = new RestSharpClient()
            {
                
            });

        }

        public RestClient GetDigestRestClientInstance(string baseUrl, string username, string password)
        {
            if (_digestClients.TryGetValue(baseUrl, out var digestClient))
            {
                return digestClient;
            }

            var restOptions = new RestClientOptions(baseUrl)
            {
                Authenticator = new DigestAuthenticator(username, password)
            };
            var client = new RestClient(restOptions);
            _digestClients[baseUrl] = client;
            return client;
        }

        #region Private methods

        private static void AddAuthorization(RestRequest request, IRestApiAuthorizationData authorizationData, AuthorizationTypeEnumeration authorizationType)
        {
            if (authorizationData != null && authorizationType != AuthorizationTypeEnumeration.NoAuth)
            {
                authorizationData.ApplyAuthorization(request);
            }
        }

        private static void AddQueryStringParams(RestRequest request, Dictionary<string, string> queryStrings)
        {
            if (queryStrings == null) return;
            foreach (var header in queryStrings)
            {
                request.AddQueryParameter(header.Key, header.Value);
            }
        }

        private static void AddHeaders(RestRequest request, Dictionary<string, string> headers)
        {
            if (headers == null) return;
            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        private static void AddJsonBody(RestRequest request, RestApiRequestData requestParams)
        {
            if (requestParams.Body != null)
            {
                request.AddJsonBody(requestParams.Body);
            }
            else if (requestParams.BodyString.IsNotNullOrEmpty() && requestParams.BodyStringContentType.IsNotNullOrEmpty())
            {
                request.AddParameter(requestParams.BodyStringContentType, requestParams.BodyString, ParameterType.RequestBody);
            }
        }

        private static RestRequest GetRequest(RestApiRequestData requestParams)
        {
            var request = new RestRequest(requestParams.Uri);
            AddAuthorization(request, requestParams.AuthorizationData, requestParams.AuthorizationType);
            AddQueryStringParams(request, requestParams.QueryStringParameters);
            AddHeaders(request, requestParams.Headers);
            AddJsonBody(request, requestParams);
            if (requestParams.Method.HasValue)
            {
                request.Method = requestParams.Method.Value;
            }
            //if (requestParams.RequestTimeout.HasValue)
            //{
            //    // 
            //    //request.Timeout = ???
            //}

            return request;
        }

        private static Dictionary<string, string> ConvertToDictionary(string stringValue)
        {
            if (stringValue.IsNotNullOrEmpty())
            {
                var result = new Dictionary<string, string>();
                var valueConverted = ObjectHelper.DeserializeAsJson<string[][]>(stringValue);
                if (valueConverted.IsCollectionNotNullOrEmpty())
                {
                    foreach (var item in valueConverted)
                    {
                        if (item.Length == 2 && item.All(i => i.IsNotNullOrEmpty()))
                        {
                            if (result.All(r => r.Key != item[0]))
                            {
                                result.Add(item[0], item[1]);
                            }
                        }
                    }
                }

                return result;
            }

            return null;
        }


        #endregion


        public void CallDynamicApiAsVoid(IDynamicApi apiModel)
        {
            var response = _client.Execute(ConvertDynamicModelToRestRequest(apiModel));
            ThrowIfError(response);
        }

        public dynamic CallDynamicApi<T>(IDynamicApi apiModel)
        {

            var response = _client.Execute<T>(ConvertDynamicModelToRestRequest(apiModel));
            ThrowIfError(response);
            return response.Data;
        }



        public void ExecuteAsVoid(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            var response = _client.Execute(request);
            ThrowIfError(response);
        }

        public dynamic Execute<T>(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            var response = _client.Execute<T>(request);
            ThrowIfError(response);
            return response.Data;
        }

        public RestResponse ExecuteAndReturnResponse(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            return _client.Execute(request);
        }

        public byte[] DownloadFile(RestApiRequestData requestParams)
        {

            var request = GetRequest(requestParams);
            return _client.DownloadData(request);
        }


        public T PostAsJson<T>(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            return _client.Post<T>(request);
        }

        public void PostAsJsonVoid(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            _client.Post(request);
        }


        public T PutAsJson<T>(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            return _client.Put<T>(request);

        }

        public void PutAsJsonVoid(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            _client.Put(request);
        }


        public T DeleteAsJson<T>(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            return _client.Delete<T>(request);
        }

        public void DeleteAsJsonVoid(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            _client.Delete(request);
        }


        public T GetAsJson<T>(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            return _client.Get<T>(request);
        }

        public void GetAsJsonVoid(RestApiRequestData requestParams)
        {
            var request = GetRequest(requestParams);
            _client.Get(request);
        }




        public static void ThrowIfError(RestResponse restResponse)
        {
            if (!restResponse.IsSuccessful)
            {
                var sb = new StringBuilder();
                if (restResponse.ErrorException != null)
                {
                    sb.AppendLine($"Exception: {restResponse.ErrorException.GetFullExceptionMessage()}");
                }
                if (restResponse.ErrorMessage.IsNotNullOrEmpty())
                {
                    sb.AppendLine($"Error Message: {restResponse.ErrorMessage}");
                }
                if (restResponse.Content.IsNotNullOrEmpty())
                {
                    sb.AppendLine($"Body Content: {restResponse.Content}");
                }
                throw new Exception(sb.ToString());
            }
        }

        private static RestRequest ConvertDynamicModelToRestRequest(IDynamicApi apiModel)
        {
            var requestData = new RestApiRequestData
            {
                AuthorizationType = apiModel.AuthorizationType,
                BodyString = apiModel.Body,
                BodyStringContentType = "application/json",
                Headers = ConvertToDictionary(apiModel.Header),
                QueryStringParameters = ConvertToDictionary(apiModel.QueryString),
                Uri = apiModel.EndPointUrl,
                RequestTimeout = TimeSpan.FromSeconds(apiModel.RequestTimeoutInSeconds),
                Method = (Method)apiModel.HttpMethod
            };

            switch (apiModel.AuthorizationType)
            {
                case AuthorizationTypeEnumeration.NoAuth:
                    break;
                case AuthorizationTypeEnumeration.ApiKeyHeader:
                    requestData.AuthorizationData = new RestApiKeyHeaderAuthorizationData
                    {
                        ApiKey = apiModel.AuthorizationUsername,
                        KeyName = apiModel.AuthorizationPassword
                    };
                    break;
                case AuthorizationTypeEnumeration.ApiKeyQueryString:
                    requestData.AuthorizationData = new RestApiKeyQueryStringAuthorizationData
                    {
                        ApiKey = apiModel.AuthorizationUsername,
                        KeyName = apiModel.AuthorizationPassword
                    };
                    break;
                case AuthorizationTypeEnumeration.BearerToken:
                    requestData.AuthorizationData = new RestApiBearerAuthorizationData
                    {
                        Token = apiModel.AuthorizationToken
                    };
                    break;
                case AuthorizationTypeEnumeration.BasicAuth:
                    requestData.AuthorizationData = new RestApiBasicAuthorizationData
                    {
                        Username = apiModel.AuthorizationUsername,
                        Password = apiModel.AuthorizationPassword
                    };
                    break;
                case AuthorizationTypeEnumeration.Digest:
                    requestData.AuthorizationData = new RestApiDigestAuthorizationData
                    {
                        Username = apiModel.AuthorizationUsername,
                        Password = apiModel.AuthorizationPassword
                    };
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration
                     .CommunicationHookSystemDataValidationError);
            }

            return GetRequest(requestData);
        }


    }
}
