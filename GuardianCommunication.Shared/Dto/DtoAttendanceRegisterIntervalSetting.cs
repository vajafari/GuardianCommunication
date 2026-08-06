using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoAttendanceRegisterIntervalSetting
    {
		public AttendanceRegisterIntervalTypeEnumeration ApplicationType { get; set; }
		public int Interval { get; set; }
	}
}
