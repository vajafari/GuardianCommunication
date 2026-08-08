using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoHookSystemDetail
	{
		public int Id { get; set; }

		public int HookSystemId { get; set; }

		public ApplicationTypeEnumeration ApplicationId { get; set; }

		public HookTypeEnumeration DetailType { get; set; }

		public string EndPointUrl { get; set; }

		public HttpMethodEnumeration HttpMethod { get; set; }

		public string QueryStringTemplate { get; set; }

		public string HeaderTemplate { get; set; }

		public string BodyTemplate { get; set; }

		public string DateFormat { get; set; }

		public int RetryCount { get; set; }

		public int RequestTimeoutInSeconds { get; set; }

		public string ResponseResultJsonPath { get; set; }

		public bool IsActive { get; set; }
	}
}
