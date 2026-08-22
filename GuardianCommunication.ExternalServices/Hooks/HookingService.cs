using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.ExternalServices.Hooks
{
    public class HookingService
    {

        public dynamic CallHook<T>(HookSystemModel hookSystemModel, T dtoData)
        {
            dynamic result;
            var dtoEventType = typeof(T);
            var dtoEventPropertyInfoList = dtoEventType.GetProperties().ToList();
            //hookSystemModel.EndPointUrl = hookSystemModel.EndPointUrl.Remove(hookSystemModel.EndPointUrl.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase));
            var header = hookSystemModel.HeaderTemplate;
            var body = hookSystemModel.BodyTemplate;
            var queryString = hookSystemModel.QueryStringTemplate;
            var endPoint = hookSystemModel.EndPointUrl;
            foreach (var dtoEventPropertyInfo in dtoEventPropertyInfoList)
            {
                dynamic propertyValue = dtoEventPropertyInfo.GetValue(dtoData);
                if (dtoEventPropertyInfo.PropertyType == typeof(bool))
                    propertyValue = propertyValue?.ToString().Trim().ToLowerInvariant();
                else if (dtoEventPropertyInfo.PropertyType == typeof(DateTime))
                    propertyValue = (propertyValue as DateTime?)?.ToString(hookSystemModel.DateFormat ?? "", CultureInfo.InvariantCulture);
                else
                    propertyValue = propertyValue?.ToString(CultureInfo.InvariantCulture).Trim();
                header = header?.Replace($"<<{dtoEventPropertyInfo.Name}>>", propertyValue);
                queryString = queryString?.Replace($"<<{dtoEventPropertyInfo.Name}>>", propertyValue);
                body = body?.Replace($"<<{dtoEventPropertyInfo.Name}>>", propertyValue);
                endPoint = endPoint.Replace($"<<{dtoEventPropertyInfo.Name}>>", $"{propertyValue}/");
            }

            var requestData = new RestApiRequestData
            {
                AuthorizationType = hookSystemModel.AuthorizationType,
                BodyString = body,
                BodyStringContentType = "application/json",
                Headers = ConvertToDictionary(header),
                QueryStringParameters = ConvertToDictionary(queryString),
                Uri = endPoint,
                RequestTimeout = hookSystemModel.RequestTimeout,
            };

            switch (hookSystemModel.AuthorizationType)
            {
                case AuthorizationTypeEnumeration.NoAuth:
                    break;
                case AuthorizationTypeEnumeration.ApiKeyHeader:
                    requestData.AuthorizationData = new RestApiKeyHeaderAuthorizationData
                    {
                        ApiKey = hookSystemModel.AuthorizationUsername,
                        KeyName = hookSystemModel.AuthorizationPassword
                    };
                    break;
                case AuthorizationTypeEnumeration.ApiKeyQueryString:
                    requestData.AuthorizationData = new RestApiKeyQueryStringAuthorizationData
                    {
                        ApiKey = hookSystemModel.AuthorizationUsername,
                        KeyName = hookSystemModel.AuthorizationPassword
                    };
                    break;
                case AuthorizationTypeEnumeration.BasicAuth:
                    requestData.AuthorizationData = new RestApiBasicAuthorizationData
                    {
                        Username = hookSystemModel.AuthorizationUsername,
                        Password = hookSystemModel.AuthorizationPassword
                    };
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration
                     .CommunicationHookSystemDataValidationError);
            }

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookRestRequestAndResult))
            {
                LoggingSystem.LogInfo("Hook Service called", new
                {
                    RequestData = requestData,
                    HookSystemModel = hookSystemModel,
                    Data = dtoData
                });
            }

            requestData.Method = hookSystemModel.HttpMethod;
            
            result = RestSharpClient.GetInstance().Execute<dynamic>(requestData);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookRestRequestAndResult))
            {
                LoggingSystem.LogInfo("Hook Service Result", new
                {
                    Result = result,
                });
            }
            return result;
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

    }
}