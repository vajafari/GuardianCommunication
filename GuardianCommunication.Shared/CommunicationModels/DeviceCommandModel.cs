using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class DeviceCommandModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string DeviceSerialNumber { get; set; }

        //[DataMember]
        //public string CommandContent { get; set; }

        [DataMember]
        public double CommitTime { get; set; }

        [DataMember]
        public double? SendTime { get; set; }

        [DataMember]
        public double? ResponseTime { get; set; }

        [DataMember]
        public string ResponseValue { get; set; }

        [DataMember]
        public DeviceCommandTypeEnumeration CommandType { get; set; }

        [DataMember]
        public CommandPriorityEnumeration Priority { get; set; }

        [DataMember]
        public long? EmployeeNumber { get; set; }

        [DataMember]
        public int RetryCount { get; set; }

        [DataMember]
        public int MaxRetry { get; set; }

    }
}
