using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.OperationResult
{
	[DataContract]
	public class OperationResult
	{

		[DataMember]
		public readonly List<OperationResultEnumeration> Errors = [];

		[DataMember]
		public readonly string ContextDescription;

		public bool HasError => Errors.IsCollectionNullOrEmpty();


		public OperationResult(OperationResultEnumeration error, string contextDescription = "")
		{
			ContextDescription = contextDescription;
			Errors.Add(error);
		}

		public OperationResult(List<OperationResultEnumeration> errors, string contextDescription = "")
		{
			ContextDescription = contextDescription;
			if (errors.IsCollectionNotNullOrEmpty())
			{
				Errors.AddRange(errors);
			}
		}

		public OperationResult(params OperationResultEnumeration[] errors) : this("", errors)
		{

		}

		public OperationResult(string contextDescription = "", params OperationResultEnumeration[] errors)
		{
			ContextDescription = contextDescription;
			if (errors.IsCollectionNotNullOrEmpty())
			{
				Errors.AddRange(errors);
			}
		}


		public string[] GetErrorMessagesSperatly()
		{
			return Errors.Select(ResultMessageProvider.GetMessageFromResource).ToArray();
		}


		public string GetErrorMessage()
		{
			var allErrors = GetErrorMessagesSperatly();
			StringBuilder builder = new StringBuilder();
			foreach (var message in allErrors)
			{
				builder.AppendLine(message);
			}
			return builder.ToString();
		}

		public string GetErrorMessageWithAdditionalInfo()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(GetErrorMessage());
			if (ContextDescription.IsNotNullOrEmpty())
			{
				builder.AppendLine(ContextDescription);
			}
			return builder.ToString();
		}



	}
}
