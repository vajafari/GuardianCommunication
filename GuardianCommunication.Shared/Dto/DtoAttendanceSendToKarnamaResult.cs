using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoAttendanceSendToKarnamaResult
    {
        public int Id { get; set; }
        public long AttendanceId { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
