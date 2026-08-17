
namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceStatistics
    {
        public bool IsConnected { get; set; }
        public int CountOfUsers { get; set; }
        public int CountOfFaces { get; set; }
        public int CountOfFingers { get; set; }
        public int CountOfUnreadAttendance { get; set; }
    }

}
