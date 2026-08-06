using GuardianCommunication.Hardware.PadisController.Definition;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerAttendanceCommunicationModel
    {
        public int Id { get; set; }
        public long EmployeeNumber { get; set; }
        public long AttendanceDateTime { get; set; }
        public int DoorId { get; set; }
        public PadisControllerAttendanceStatusEnumeration VerificationStatus { get; set; }
        public string RfCardNumber { get; set; }


        public bool IsAccessGranted
        {
            get
            {
                if (VerificationStatus == PadisControllerAttendanceStatusEnumeration.Success || VerificationStatus == PadisControllerAttendanceStatusEnumeration.FailedForPassAccuracy)
                {
                    return true;
                }
                return false;
            }
        }

    }
}
