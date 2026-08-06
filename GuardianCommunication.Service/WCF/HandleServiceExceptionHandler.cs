using System;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using GuardianCommunication.Data.Logger;

namespace GuardianCommunication.Service.WCF
{
	public class HandleServiceExceptionHandler : IErrorHandler
	{
		public bool HandleError(Exception error)
		{
			return true;
		}

		// This is a trivial implementation that converts Exception to FaultException<GreetingFault>. 
		public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
		{
			ErrorModel model;
			if (error is OperationCannotBeDoneException exception)
			{
				model = new ErrorModel
				{
					ErrorCodes = exception.OperationResult.Errors.ConvertAll(row => (int)row).ToArray(),
					StackTrace = AppConfigs.IncludeStack ? exception.StackTrace : string.Empty,
					AdditionalInfo = exception.OperationResult.ContextDescription
				};

			}
			else
			{
				model = new ErrorModel
				{
					ErrorCodes = new[] { (int)OperationResultEnumeration.CommunicationStatusUnknownError },
					StackTrace = AppConfigs.IncludeStack ? error.StackTrace : string.Empty,
					AdditionalInfo = error.GetFullExceptionMessage()
				};
				LoggingSystem.LogError(error, "Unknown Exception");
			}

			msg = Message.CreateMessage(version, "", model, new DataContractJsonSerializer(model.GetType()));
			var wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);
			msg.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);
			// return custom error code.
			var rmp = new HttpResponseMessageProperty
			{
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				StatusDescription = "See fault object for more information."
			};
			// put appropraite description here..
			msg.Properties.Add(HttpResponseMessageProperty.Name, rmp);


			//if (error is OperationCannotBeDoneException exception)
			//{
			//	var model = new ErrorModel
			//	{
			//		ErrorCodes = exception.OperationResult.Errors.ConvertAll(row => (int)row).ToArray(),
			//		StackTrace = AppSettings.IncludeStack ? exception.StackTrace : string.Empty,
			//		AdditionalInfo = exception.OperationResult.ContextDescription
			//	};
			//	FaultException<ErrorModel> fault = new FaultException<ErrorModel>(model);
			//	MessageFault faultMessage = fault.CreateMessageFault();
			//	msg = Message.CreateMessage(ver, faultMessage, Constants.WcfNamsepace);
			//}
			//else
			//{
			//	var model = new ErrorModel
			//	{
			//		ErrorCodes = new []{ (int)OperationResultEnumeration.UnknownError },
			//		StackTrace = AppSettings.IncludeStack ? error.StackTrace : string.Empty,
			//		AdditionalInfo = error.GetFullExceptionMessage()
			//	};
			//	LoggingSystem.LogError(error, "Unknown Exception");
			//	FaultException<ErrorModel> fault = new FaultException<ErrorModel>(model);
			//	MessageFault faultMessage = fault.CreateMessageFault();
			//	msg = Message.CreateMessage(ver, faultMessage, Constants.WcfNamsepace);
			//}

		}

	}


}
