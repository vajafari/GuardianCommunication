using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Newtonsoft.Json;

namespace GuardianCommunication.Service.WCF
{
    // DataContractJsonSerializer (used by default for WebMessageFormat.Json) matches JSON
    // property names case-sensitively, so a camelCase request body (e.g. built by RestSharp)
    // never populates the PascalCase [DataMember] properties on our contracts and every field
    // comes back null/default. This formatter re-parses the incoming body with Newtonsoft.Json,
    // which matches property names case-insensitively, and leaves reply serialization untouched.
    public class CamelCaseTolerantMessageFormatter : IDispatchMessageFormatter
    {
        private readonly IDispatchMessageFormatter _innerFormatter;
        private readonly Type _parameterType;

        public CamelCaseTolerantMessageFormatter(OperationDescription operation, IDispatchMessageFormatter innerFormatter)
        {
            _innerFormatter = innerFormatter;

            var inputPart = operation.Messages[0].Body.Parts.FirstOrDefault();
            _parameterType = inputPart?.Type;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (_parameterType == null)
            {
                // Operation has no body parameter (e.g. HealthCheck, ResetDeviceCache): nothing to do.
                return;
            }

            var bodyReader = message.GetReaderAtBodyContents();

            string json;
            using (var stream = new MemoryStream())
            {
                using (var jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false))
                {
                    bodyReader.MoveToContent();
                    jsonWriter.WriteNode(bodyReader, false);
                    jsonWriter.Flush();
                }

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            parameters[0] = string.IsNullOrWhiteSpace(json) || json == "null"
                ? null
                : JsonConvert.DeserializeObject(json, _parameterType);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            return _innerFormatter.SerializeReply(messageVersion, parameters, result);
        }
    }
}
