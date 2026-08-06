using System.Runtime.Serialization;
using GuardianCommunication.Shared.OperationResult;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class AttendanceProcessResultModel
    {
        [DataMember]
        public bool IsSuccessfullyProcessed { get; set; }
        [DataMember]
        public int ResultCode { get; set; }


        public bool IsAttendanceSavedAtKarnama()
        {
            if (IsSuccessfullyProcessed)
            {
                return true;
            }
            if (ResultCode == (int)OperationResultEnumeration.AttendanceStatusAttendanceAlreadyExist)
            {
                return true;
            }
            return false;
        }
    }
}
