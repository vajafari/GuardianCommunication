using System.Collections.Generic;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Shared.Filter
{
	public class DeviceSearchParams
	{
		public DeviceFilter Filter { get; set; }

		public List<SortInfo<DeviceSortEnumeration>> Sort { get; set; }

		public CurrentPageInfo CurrentPage { get; set; }

		public bool GetDeviceTypeSummary { get; set; }

		public bool GetArea { get; set; }


	}
}
