using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoCommunicationDeviceTimeSettings
	{
		
		public bool IsDaylightActive { get; set; }
		public int DaylightStart { get; set; }
		public int DaylightEnd { get; set; }
		public int DaylightChangeTimeInSeconds { get; set; }
		public TimeZonesEnumeration TimeZone { get; set; }

		public DateTime CurrentYearDaylightStart => new DateTime(DateTime.Now.Year, DaylightStart / 100, DaylightStart % 100);
		public DateTime CurrentYearDaylightEnd => new DateTime(DateTime.Now.Year, DaylightEnd / 100, DaylightEnd % 100);

	}
}
