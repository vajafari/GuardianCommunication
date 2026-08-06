using System;
using System.Data.SqlClient;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
	public static class ExceptionHelper
	{
		public static string GetFullExceptionMessage(this Exception exp)
		{
			Exception exception = exp;
			string exceptionMessage = $"Exception at {Environment.NewLine}{exp.Message}";
			while (exception.InnerException != null)
			{
				exceptionMessage += $"{Environment.NewLine}InnerMessage: {exception.InnerException.Message}";
				exception = exception.InnerException;
			}
			exceptionMessage += Environment.NewLine;
			return exceptionMessage;
		}


		public static OperationResultEnumeration TranslateSqlExceptionToOperationResultStatus(Exception exp)
		{
			var result = OperationResultEnumeration.CommunicationStatusUnknownError;
			var exceptionToWorkOn = exp;

			while (!(exceptionToWorkOn == null || exceptionToWorkOn is SqlException))
			{
				exceptionToWorkOn = exceptionToWorkOn.InnerException;
			}

			var exceptionToTranslate = exceptionToWorkOn as SqlException;
			if (exceptionToTranslate == null)
			{
				return result;
			}

			switch (exceptionToTranslate.Number)
			{
				case -2:
				case 2:
					result = OperationResultEnumeration.CommunicationObjectRequestTimeout;
					break;
				case 18456:
				case 1326:
				case 229:
					result = OperationResultEnumeration.CommunicationObjectLogOnFailed;
					break;
				case 4060:
					result = OperationResultEnumeration.CommunicationObjectDatabaseNotAvailable;
					break;
				case 2601:
				case 2627:
					result = OperationResultEnumeration.CommunicationObjectUniqueKeyConstraint;
					break;
				case 547:
					result = OperationResultEnumeration.CommunicationObjectHasForeignKeyConstraint;
					break;
				case 8152:
					result = OperationResultEnumeration.CommunicationStringOrBinaryMustBeTruncated;
					break;
				default:
					result = OperationResultEnumeration.CommunicationStatusUnknownError;
					break;
			}

			return result;
		}
	}
}
