using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{
    /// <summary>
    /// نوع ورود و خروج دستگاه
    /// </summary>
    [Flags]
    [DataContract]
    public enum DeviceIoTypeEnumeration : short
    {
        /// <summary>
        /// هیچکدام
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// ورود
        /// </summary>
        [EnumMember] 
        Input = 1,

        /// <summary>
        /// خروج
        /// </summary>
        [EnumMember] 
        Output = 2,

        /// <summary>
        /// ورود و خروج
        /// </summary>
        [EnumMember] 
        InputOutput = 3,
    }
}
