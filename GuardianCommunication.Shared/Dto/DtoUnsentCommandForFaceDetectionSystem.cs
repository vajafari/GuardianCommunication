using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoUnsentCommandForFaceDetectionSystem
    {
        public int Id { get; set; }

        public string CommandContent { get; set; }

        public OtherHardwareCommandTypeEnumeration CommandType { get; set; }
    }
}
