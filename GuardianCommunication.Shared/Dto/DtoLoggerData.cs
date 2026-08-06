using System;
using System.Globalization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoLoggerData : IChecksumEntity
    {
        public long Id { get; set; }
        public LogLevelCodeEnum LevelCode { get; set; }
        public string Name { get; set; }
        public Guid? UserId { get; set; }
        public string Source { get; set; }
        public int? EventId { get; set; }
        public string Message { get; set; }
        public DateTime RegisterDateTime { get; set; }
        public string Checksum { get; set; }

        public string GetChecksum()
        {
            var hashInput = $"{UserId}-" +
                            $"{LevelCode}-" +
                            $"{Name.ToNotNullString()}-" +
                            $"{(UserId.HasValue ? UserId.Value.ToString() : string.Empty)}-" +
                            $"{Source.ToNotNullString()}-" +
                            $"{(EventId.HasValue ? EventId.Value.ToString() : "NULL")}-" +
                            $"{Message}-" +
                            $"{RegisterDateTime.ToString("YYYY-MM-dd HH:mm:ss", new CultureInfo("en-US"))}";
            return hashInput.ToSha256CheckSum();
        }
    }
}
