using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.Dto
{

	public class DtoEmployeeAndDeviceResult
	{
		public int DeviceNumber { get; set; }
		public long EmployeeNumber { get; set; }
		public OperationResultEnumeration Result { get; set; }
	}

}
