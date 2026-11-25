using System;
using GuardianCommunication.Shared.Definition;
using RestSharp;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoHookDefinition : DtoDatabaseEntityBase
	{
		public HookTypeEnumeration HookType { get; set; }

		public string EndPointUrl { get; set; }

		public Method HttpMethod { get; set; }

		public AuthorizationTypeEnumeration AuthorizationType { get; set; }

		public string AuthorizationUsername { get; set; }

		public string AuthorizationPassword { get; set; }

		public string QueryStringTemplate { get; set; }

		public string HeaderTemplate { get; set; }

		public string BodyTemplate { get; set; }

		public string DateFormat { get; set; }

		public int RetryCount { get; set; }

		public int RequestTimeoutInSeconds { get; set; }

		public bool IsActive { get; set; }

		/// <summary>
		/// عبارت Cron ردیف زمان بند (فقط نوع Cron): پنج بخش دقیقه، ساعت، روز ماه، ماه و روز هفته به وقت محلی سرویس
		/// </summary>
		public string Cron { get; set; }

		/// <summary>
		/// زمان شروع آخرین اجرای ردیف زمان بند، به وقت محلی (در دیتابیس UTC)؛ خالی یعنی هنوز اجرا نشده است
		/// </summary>
		public DateTime? LastRunAt { get; set; }
	}
}
