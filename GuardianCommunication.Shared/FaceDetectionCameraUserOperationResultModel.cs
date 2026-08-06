using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared
{

    public class FaceDetectionCameraUserOperationResultModel
    {
        public long EmployeeNumber { get; set; }
        public OperationResultEnumeration OperationResult { get; set; }
    }
}
