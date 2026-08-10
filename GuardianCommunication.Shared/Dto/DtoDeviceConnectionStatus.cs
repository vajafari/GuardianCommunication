using System;

namespace GuardianCommunication.Shared.Dto
{

	public class DtoDeviceConnectionStatus
	{
		public Guid DeviceId { get; set; }
        public bool IsConnected { get; set; }
    }
}
