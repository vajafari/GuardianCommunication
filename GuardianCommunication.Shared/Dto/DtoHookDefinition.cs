using GuardianCommunication.Shared.Definition;
using RestSharp;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoHookDefinition : DtoDatabaseEntityBase
	{
		public HookTypeEnumeration DetailType { get; set; }

		public string EndPointUrl { get; set; }

		public Method HttpMethod { get; set; }

		public string QueryStringTemplate { get; set; }

		public string HeaderTemplate { get; set; }

		public string BodyTemplate { get; set; }

		public string DateFormat { get; set; }

		public int RetryCount { get; set; }

		public int RequestTimeoutInSeconds { get; set; }

		public bool IsActive { get; set; }
	}
}
