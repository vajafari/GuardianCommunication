using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceEventLog
	{
		public long Id { get; set; }
		public long? UserIdOnDevice { get; set; }
		public DateTime EventDateTime { get; set; }
		public Guid DeviceId { get; set; }
        public ProducerEnumeration Producer { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public int EventCode { get; set; }
		public bool IsFromDevice { get; set; }
        
	}
}
