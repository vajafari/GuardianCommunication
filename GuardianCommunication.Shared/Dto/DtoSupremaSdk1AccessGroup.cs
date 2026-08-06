using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk1AccessGroup
	{
		public int AccessGroupNumber { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public List<DtoSupremaSdk1AccessGroupDoorTimezone> DoorTimezones { get; set; }
	}
}
