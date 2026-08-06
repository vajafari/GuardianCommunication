using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Communication.Shared.ExtensionsAndUtilities;

namespace Communication.Service.WCF
{
    public class DoLoggingMessageInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            LoggingSystem.LogIncomingRequest(request.ToString(), "Message Log");
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }
}