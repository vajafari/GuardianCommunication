using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
	public enum TimeZoneDayTypeEnumeration
	{
		Sunday = 0,
		Monday,
		Tuesday,
		Wednesday,
		Thursday,
		Friday,
		Saturday,

		/// <summary>
		/// تعطیل رسمی
		/// </summary>
		[EnumMember]
		Official = 5000,

		/// <summary>
		/// تعطیلات آخر هفته مثل جمعه ها
		/// </summary>
		[EnumMember]
		Weekends = 5001,

		/// <summary>
		/// تعطیلاتی که در اثر مناسبت های خاص و به صورت پیش بینی نشده تعطیل می گردند
		/// </summary>
		[EnumMember]
		Special = 5002
	}
}
