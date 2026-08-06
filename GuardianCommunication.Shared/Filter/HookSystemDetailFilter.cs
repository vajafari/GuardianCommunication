using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Filter
{
	public class HookSystemDetailFilter
	{

		public List<int> Ids { get; set; }
		
		public List<int> HookSystemIds { get; set; }

		public List<HookDetailTypeEnumeration> DetailTypes { get; set; }

		public int? IsActive { get; set; }



		public List<int> DetailTypesValues => DetailTypes.IsCollectionNotNullOrEmpty()
			? DetailTypes.ConvertAll(row => (int)row).ToList()
			: null;

	}
}
