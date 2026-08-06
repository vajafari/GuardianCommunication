using System;

namespace GuardianCommunication.Hardware.Pw.PwConcepts
{
    public class PwDeviceConnectResult
    {
        public bool IsConnected { get; set; }

        public int BiosVersion { get; set; }

        public int RecordCount { get; set; }

        public DateTime DeviceDateTime { get; set; }

    }
}
