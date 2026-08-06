using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPlateDetectionCameraCarAttendance
    {
        public string IdOnCamera { get; set; }
        public int CameraId { get; set; }
        public string PlateString { get; set; }
        public CarPlateTypeEnumeration PlateType { get; set; }
        public PlateRecognitionAcceptType AcceptType { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public byte[] CarImage { get; set; }
        public byte[] PlateImage { get; set; }
    }
}
