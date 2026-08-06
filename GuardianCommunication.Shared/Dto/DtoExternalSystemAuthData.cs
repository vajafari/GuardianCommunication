using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoExternalSystemAuthData
	{
		public Guid SystemId { get; set; }
		public string SecurityToken { get; set; }

	}
}
