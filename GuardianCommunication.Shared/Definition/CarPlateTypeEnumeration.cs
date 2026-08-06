using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum CarPlateTypeEnumeration : short
    {
        /// <summary>
        /// پلاک ملی
        /// </summary>
        [DataMember]
        National = 0,

    }
}
