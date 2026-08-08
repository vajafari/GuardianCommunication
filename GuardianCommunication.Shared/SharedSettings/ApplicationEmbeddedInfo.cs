using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.SharedSettings
{
    public static class ApplicationEmbeddedInfo
    {
        public static string CustomerName { get; set; }

        public static int SerialNumber { get; set; }

        public static DateTime? ExpireDate { get; set; }

        public static ModuleEnumeration Modules { get; set; }

        public static int PersonCount { get; set; }

        public static int DeviceCount { get; set; }

        public static int TotalDeviceCount { get; set; }

        public static ProducerEnumeration ActiveProducers { get; set; }

        public static List<SdkVersionEnumeration> SupremaProducerVersions { get; set; }

        public static void CheckProducerValidity(ProducerEnumeration producer, SdkVersionEnumeration sdkVersion)
        {
            if (ActiveProducers.HasFlag(producer))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            if (producer == ProducerEnumeration.Suprema)
            {
                if (!SupremaProducerVersions.Contains(sdkVersion))
                {
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                }
            }
        }


    }

}

