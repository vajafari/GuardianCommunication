using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.OperationResult
{
	public class OperationCannotBeDoneException : Exception
	{
		public OperationResult OperationResult { get; }

		public OperationCannotBeDoneException(OperationResult operationResult)
		{
			OperationResult = operationResult;
		}


		public OperationCannotBeDoneException(OperationResultEnumeration error, string contextDescription = "")
		{
			OperationResult = new OperationResult(error, contextDescription);
		}

		public OperationCannotBeDoneException(List<OperationResultEnumeration> errors, string contextDescription = "")
		{
			OperationResult = new OperationResult(errors, contextDescription);
		}

		public OperationCannotBeDoneException(params OperationResultEnumeration[] errors) : this("", errors)
		{
			OperationResult = new OperationResult(errors);
		}

		public OperationCannotBeDoneException(string contextDescription = "", params OperationResultEnumeration[] errors)
		{
			OperationResult = new OperationResult(contextDescription, errors);
		}


	}
}
