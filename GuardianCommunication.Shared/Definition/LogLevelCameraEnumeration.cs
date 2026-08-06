using System;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    public enum LogLevelCameraEnumeration : long
    {
        None = 1 << 0,
        KarabinAutoCollect = 1 << 1,
        KarabinAgent = 1 << 2,
        KarabinSendValidList = 1 << 3,
        KarabinAgentReadCameraAttendance = 1 << 4,
        KarabinAgentReadCameraAttendanceImage = 1 << 5,
        KarabinServer = 1 << 6,
        ServerCommandResult = 1 << 7,
        SetCameraProcess = 1 << 8,
        //KarabinServer = 1 << 10,
        //KarabinServer = 1 << 10,
        //KarabinServer = 1 << 10,
        //KarabinServer = 1 << 10,


        All = long.MaxValue,


    }
}
