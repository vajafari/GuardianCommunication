using GuardianCommunication.Shared.ExtensionsAndUtilities;
using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceCommunicationData : DtoDatabaseEntityBase
    {
        public Guid DeviceId { get; set; }
        public string DeviceCommunicationDataInJson { get; set; }


        private DtoDeviceCommunicationDataJsonEntity _deviceCommunicationData;

        public DtoDeviceCommunicationDataJsonEntity DeviceCommunicationData
        {
            get
            {
                if (_deviceCommunicationData == null && DeviceCommunicationDataInJson.IsNotNullOrEmpty())
                {
                    _deviceCommunicationData = ObjectHelper.DeserializeAsJson<DtoDeviceCommunicationDataJsonEntity>(DeviceCommunicationDataInJson)
                                      ?? new DtoDeviceCommunicationDataJsonEntity();
                }

                return _deviceCommunicationData;
            }
            set => _deviceCommunicationData = value;
        }

        public void UpdateDeviceSettings()
        {
            if (_deviceCommunicationData != null)
            {
                DeviceCommunicationDataInJson = ObjectHelper.SerializeAsJson(_deviceCommunicationData);
            }
        }

    }

    public class DtoDeviceCommunicationDataJsonEntity
    {
        public DtoDeviceCommunicationDataZk Zk { get; set; }
        public DtoDeviceCommunicationDataSuprema1 Suprema1 { get; set; }
        public DtoDeviceCommunicationDataSuprema2 Suprema2 { get; set; }
    }


    public class DtoDeviceCommunicationDataZk
    {
        public DateTime? LastAttendanceLogDateTime { get; set; }
    }

    public class DtoDeviceCommunicationDataSuprema1
    {
        public DateTime? LastAttendanceLogDateTime { get; set; }
        public DateTime? LastLogDateTime { get; set; }
    }

    public class DtoDeviceCommunicationDataSuprema2
    {
        public long LastAttendanceLogId { get; set; }
        public long LastLogId { get; set; }
        public DateTime? LastAttendanceLogDateTime { get; set; }
        public DateTime? LastLogDateTime { get; set; }
    }

}
