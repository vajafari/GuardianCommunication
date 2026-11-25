using System;

namespace GuardianCommunication.Shared.Definition
{
	[Flags]
	public enum HookTypeEnumeration
	{
		Attendance = 1,

		/// <summary>
		/// زمان بند: آدرس در زمان های عبارت Cron ردیف صدا زده می شود
		/// </summary>
		Cron = 2
	}

}
