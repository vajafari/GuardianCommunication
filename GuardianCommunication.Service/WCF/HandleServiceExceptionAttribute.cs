using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace GuardianCommunication.Service.WCF
{

	public class HandleServiceExceptionAttribute : Attribute, IServiceBehavior
	{

		public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase,
		  Collection<ServiceEndpoint> endpoints,
		  BindingParameterCollection parameters)
		{

		}


		public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
		{
			IErrorHandler errorHandler = new HandleServiceExceptionHandler();

			foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
				channelDispatcher?.ErrorHandlers.Add(errorHandler);
			}

		}

		public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
		{

		}

	}

}
