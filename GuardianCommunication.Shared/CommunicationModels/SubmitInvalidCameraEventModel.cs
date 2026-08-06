using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public class SubmitInvalidCameraEventModel
    {
        public int CameraId { get; set; }
        public string PlateString { get; set; }
        public double AttendanceDateTimeNumeric { get; set; }
        public string CarImage { get; set; }
        public PlateRecognitionAcceptType AcceptType { get; set; }
    }
}
