using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoAttendanceRegisterIntervalSetting
    {
		public AttendanceRegisterIntervalTypeEnumeration ApplicationType { get; set; }

		/// <summary>
		/// بازه عدم ثبت تردد تکراری به ثانیه، پیش و پس از لحظه تردد
		/// </summary>
		public int Interval { get; set; }
	}
}
