using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.SharedSettings
{
    public static class ApplicationEmbeddedInfo
    {
        public static string CustomerName { get; set; }

        public static int SerialNumber { get; set; }

        public static DateTime? ExpireDate { get; set; }

        public static ApplicationTypeEnumeration ValidApplication { get; set; }

        public static int EmployeeCount { get; set; }

        public static int DeviceCount { get; set; }

        public static int TotalDeviceCount { get; set; }

        public static CalendarTypeEnumeration CalendarType { get; set; }

        public static ProducerEnumeration ActiveProducers { get; set; }

        public static SdkVersionEnumeration SupremaProducerVersions { get; set; }

        public static bool CheckDeviceSerialNumber { get; set; }

        public static string[] ValidDeviceSerialNumbers { get; set; }

        public static DateTime? EffectiveDateForValidDeviceSerialNumbers { get; set; }

        public static ValidSerialNumberCheckTypeEnumeration ValidSerialNumberCheckTypes { get; set; }

        public static AcFaceDetectionModuleTypeEnumeration FaceDetectionModuleType { get; set; }
    }

}

