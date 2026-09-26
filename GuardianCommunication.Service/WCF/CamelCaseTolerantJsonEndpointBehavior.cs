using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace GuardianCommunication.Service.WCF
{
    // Wires CamelCaseTolerantMessageFormatter into every operation of an endpoint, so all
    // WebInvoke JSON operations accept camelCase request bodies regardless of app.config.
    public class CamelCaseTolerantJsonEndpointBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                var dispatchOperation = endpointDispatcher.DispatchRuntime.Operations
                    .FirstOrDefault(o => o.Name == operation.Name);

                if (dispatchOperation != null)
                {
                    dispatchOperation.Formatter = new CamelCaseTolerantMessageFormatter(operation, dispatchOperation.Formatter);
                }
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
