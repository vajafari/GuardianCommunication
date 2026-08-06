using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoApplicationEncodedConfig
    {

        public string CustomerName { get; set; }
        public int SerialNumber { get; set; }
        public DateTime? ExpireDate { get; set; }
        public ApplicationTypeEnumeration ValidApplication { get; set; }
        public int EmployeeCount { get; set; }
        public int DeviceCount { get; set; }
        public int TotalDeviceCount { get; set; }
        public CalendarTypeEnumeration CalendarType { get; set; }
        public ProducerEnumeration ActiveProducers { get; set; }
        public SdkVersionEnumeration SupremaProducerVersions { get; set; }
        public bool CheckDeviceSerialNumber { get; set; }
        public string[] ValidDeviceSerialNumbers { get; set; }
        public DateTime? EffectiveDateForValidDeviceSerialNumbers { get; set; }
        public ValidSerialNumberCheckTypeEnumeration ValidSerialNumberCheckTypes { get; set; }
        public AcFaceDetectionModuleTypeEnumeration FaceDetectionModuleType { get; set; }
    }
}
