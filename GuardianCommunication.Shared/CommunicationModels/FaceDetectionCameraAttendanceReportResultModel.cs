using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public class FaceDetectionCameraAttendanceReportResultModel
    {
        public long? EmployeeNumber { get; set; }

        public int? CameraId { get; set; }

        public string FaceImage { get; set; }

        public string AttendanceImage { get; set; }

        public double AttendanceDateTime { get; set; }

        public FaceDetectionCameraDetectionStatusEnumeration DetectionType { get; set; }
    }
}
