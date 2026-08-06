using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.HardwareDefinition
{

    [DataContract]
    [Flags]
    public enum TemplateTypeEnumeration : short
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        FingerPrint = 1,

        [EnumMember]
        Face = 2,

        [EnumMember]
        Palm = 4,

        [EnumMember]
        Iris = 8,

        [EnumMember]
        All = short.MaxValue

    }
}
