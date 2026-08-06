using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class ServerMatchingResultModel
    {
        [DataMember]
        public bool IsSuccessfullyProcessed { get; set; }
        [DataMember]
        public int ResultCode { get; set; }
        [DataMember]
        public MatchOnServerDeviceAttendanceModel Attendance { get; set; }
    }
}
