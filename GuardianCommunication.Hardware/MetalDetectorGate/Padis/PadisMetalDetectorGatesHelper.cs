using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GuardianCommunication.Hardware.MetalDetectorGate.Padis
{
    internal static class PadisMetalDetectorGatesHelper
    {
        //private string m_softVer = "Soft V1.0.000";
        public static int cur = 0;
        public static int total = 0;
        public static string m_appStartDateTime = string.Empty;
        public static DateTime m_appStartDtime = DateTime.Now;
        public static CommType CommType { get; set; } = CommType.Net;
        public static ComParam CurComParam { get; set; } = new ComParam();
        public static ProductType Product { get; set; } = ProductType.High;
        public static string Lan { get; set; } = "Lan_ch";

        //private string comsec = "ComParam";
        //private string lansec = "Language";

        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder builder = new StringBuilder(data.Length * 3);
            foreach (byte num in data)
            {
                builder.Append(Convert.ToString(num, 0x10).PadLeft(2, '0'));
                builder.Append(" ");
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString().ToUpper();
        }

        public static string GetAppRunDir() =>
            Environment.CurrentDirectory;
        

        public static List<string> GetIpList()
        {
            List<string> list = new List<string>();
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    list.Add(address.ToString());
                }
            }
            return list;
        }

        public static string GetProductByType(ProductType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case ProductType.High:
                    str = "高端";
                    break;

                case ProductType.Mid:
                    str = "中端";
                    break;

                case ProductType.Low:
                    str = "低端";
                    break;

                default:
                    str = "标准";
                    break;
            }
            return str;
        }

        public static ProductType GetProductTypeByInt(int type)
        {
            ProductType high = ProductType.High;
            switch (type)
            {
                case 1:
                    high = ProductType.High;
                    break;

                case 2:
                    high = ProductType.Mid;
                    break;

                case 3:
                    high = ProductType.Low;
                    break;

                default:
                    high = ProductType.None;
                    break;
            }
            return high;
        }
        
        public static byte[] HexStringToByteArray(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                int num2 = Convert.ToInt32(hexString.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            return buffer;
        }

        public static bool IsIpValid(string ip)
        {
            bool flag = false;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if ((address.AddressFamily == AddressFamily.InterNetwork) && (address.ToString() == ip))
                {
                    flag = true;
                }
            }
            return flag;
        }

        //public void ReadParams()
        //{
        //    ComParam param = new ComParam
        //    {
        //        com = AppInit.m_iniOp.ReadString(this.comsec, "Com", string.Empty),
        //        baud = AppInit.m_iniOp.ReadString(this.comsec, "Baud", string.Empty),
        //        databit = AppInit.m_iniOp.ReadString(this.comsec, "DataBit", string.Empty),
        //        stopbit = AppInit.m_iniOp.ReadString(this.comsec, "StopBit", string.Empty),
        //        parity = AppInit.m_iniOp.ReadString(this.comsec, "Parity", string.Empty),
        //        flow = AppInit.m_iniOp.ReadString(this.comsec, "Flow", string.Empty)
        //    };
        //    string str = AppInit.m_iniOp.ReadString(this.lansec, "Language", string.Empty);
        //    if (!string.IsNullOrEmpty(str))
        //    {
        //        Lan = str;
        //    }
        //    CurComParam = param;
        //}
        
        
    }
}
