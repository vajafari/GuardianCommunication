using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Filter
{
	public class DeviceFilter
	{
		public List<int> DeviceNumbers { get; set; }

		public int? DeviceNumberLike { get; set; }

		public string Title { get; set; }

		public List<int> DeviceTypeNumbers { get; set; }

		public string DeviceTypeTitle { get; set; }

		public List<int> AreaNumbers { get; set; }

		public string AreaTitle { get; set; }

		public List<ConnectionTypeEnumeration> ConnectionTypes { get; set; }

		public List<ProducerEnumeration> Producers { get; set; }

		public List<DeviceConnectionModeEnumeration> ConnectionModes { get; set; }

		public bool? IsActive { get; set; }

		public ApplicationTypeEnumeration? HasAtLeastOneApplication { get; set; }

		public ApplicationTypeEnumeration? HasAllApplication { get; set; }


	}
}
