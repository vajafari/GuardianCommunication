using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceUnsentCommand
	{

		public int Id { get; set; }

		public string DeviceSerialNumber { get; set; }

		public string CommandContent { get; set; }

		public DeviceCommandTypeEnumeration CommandType { get; set; }

		public Guid DeviceId { get; set; }

		public string DeviceContent { get; set; }
		
	}
}
