using System.Runtime.InteropServices;

namespace GuardianCommunication.Hardware.MetalDetectorGate.Padis
{

    #region Delegate and Events

    public delegate void SetDataReceivedHandler(byte[] dat);

    public delegate void DataReceivedHandler(byte[] dat);

    #endregion


    #region Enums

    public enum CommType
    {
        Net,
        Com
    }

    public enum ListenState
    {
        UnListen,
        Listening
    }

    public enum ProductType
    {
        None,
        High,
        Mid,
        Low,
        Custom2900,
        Custom3900
    }

    public enum Command
    {
        M0 = 0,
        M10 = 10,
        M11 = 11,
        M12 = 12,
        M13 = 13,
        M14 = 14,
        M15 = 15,
        M16 = 0x10
    }

    #endregion


    #region Struct

    [StructLayout(LayoutKind.Sequential)]
    public struct UnitCountSt
    {
        public int id;
        public int pass;
        public int passF;
        public int passB;
        public int alarm;
        public int todayPass;
        public int todayAlarm;
        public int totalPassF;
        public int totalPassB;
        public int totalPass;
        public int totalAlarm;
        public string todayDate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClientInfoSt
    {
        public string ip;
        public int port;
        public int doorId;
        public string doorName;
        public string doorDesc;
        public int offJudgecnt;
        public int offlineFlag;
        public int sensitivity;
        public ProductType product;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ComParam
    {
        public string com;
        public string baud;
        public string databit;
        public string stopbit;
        public string parity;
        public string flow;
    }

    #endregion


}
