using System;

namespace GuardianCommunication.Shared.Dto
{
	
	public class DtoUnhookedAttendances : DtoAttendance
	{
		public Guid HookDefinitionId { get; set; }
	}
}
