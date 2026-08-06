using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoSupremaSdk2AccessGroup
	{
		public int AccessGroupNumber { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		public List<int> AccessLevelNumbers { get; set; }
	}
}
