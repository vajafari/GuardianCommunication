using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoUnsentCommandCountByDeviceNumber
	{
		public Guid DeviceId { get; set; }

		public int CommandCount { get; set; }
		
	}
}
