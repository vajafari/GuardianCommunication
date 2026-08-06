using System.Collections.Generic;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public class FaceDetectionCameraAttendanceReportFilterModel
    {
        public CurrentPageInfo PageInfo { get; set; }

        public List<int> CameraIds { get; set; }

        public double StartDateTime { get; set; }

        public double EndDateTime { get; set; }

    }
}
