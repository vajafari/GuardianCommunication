using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GuardianCommunication.Data.Logger;

namespace GuardianCommunication.Service.WCF
{
    public class ValidateInputMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var context = OperationContext.Current;
            var messageProperties = context.IncomingMessageProperties;
            if (AppConfigs.ValidIpAddresses.IsCollectionNotNullOrEmpty())
            {
                var endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (!(endpointProperty != null && AppConfigs.ValidIpAddresses.Contains(endpointProperty.Address)))
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogInvalidIps))
                    {
                        LoggingSystem.LogWarning($"Address is {endpointProperty?.Address ?? "UNKNOWN IP"}",
                            "Invalid Ip message rejected");
                    }
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationNotValidRequest);
                }
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {

        }
    }
}