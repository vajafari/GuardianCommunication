using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelPrintServiceEnumeration : long
    {
        None = 0,
        PrintServiceCall = 1,
        PrintServiceQueueProcess = 2,
        PrintServicePrintProcess = 4,
        PrintServiceSendResult = 8,
        PrintServiceErrorsOnSend = 16,
       

        All = long.MaxValue,

    }
}
