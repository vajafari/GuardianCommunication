using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Filter
{
	public class HookDefinitionFilter
    {

		public List<Guid> Ids { get; set; }
		
		public List<HookTypeEnumeration> HookTypes { get; set; }

		public int? IsActive { get; set; }

		public List<int> HookTypesValues => HookTypes.IsCollectionNotNullOrEmpty()
			? HookTypes.ConvertAll(row => (int)row).ToList()
			: null;

	}
}
