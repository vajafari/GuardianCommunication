using System;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.Dto
{

	public class DtoUserAndDeviceResult
	{
		public Guid DeviceId { get; set; }
		public long UserIdOnDevice { get; set; }
		public OperationResultEnumeration Result { get; set; }
	}

}
