using System;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoUnsentCommandCountByDeviceId
	{
		public Guid DeviceId { get; set; }

		public int CommandCount { get; set; }
		
	}
}
