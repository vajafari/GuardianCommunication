using System;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.Dto
{

	public class DtoEmployeeAndDeviceResult
	{
		public Guid DeviceId { get; set; }
		public Guid EmployeeId { get; set; }
		public OperationResultEnumeration Result { get; set; }
	}

}
