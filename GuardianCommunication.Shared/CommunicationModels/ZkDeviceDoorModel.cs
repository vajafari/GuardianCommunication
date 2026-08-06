using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class ZkDeviceDoorModel
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public int DeviceNumber { get; set; }

        [DataMember]
        public int DoorNumber { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public int VerificationStyle { get; set; }

        [DataMember]
        public short WiegandFormat { get; set; }

        [DataMember]
        public bool CardNumberReversal { get; set; }

        [DataMember]
        public int OperateInterval { get; set; }

        [DataMember]
        public short DoorSensorType { get; set; }

        [DataMember]
        public bool ReverseLockStateOnDoorClose { get; set; }

        [DataMember]
        public int DoorSensorDelay { get; set; }

        [DataMember]
        public int? PassageModeTimeZoneNumber { get; set; }

        [DataMember]
        public int PassageDelay { get; set; }

        [DataMember]
        public int MultiPersonOperationInterval { get; set; }

        [DataMember]
        public int ActiveTimeZoneNumber { get; set; }

        [DataMember]
        public int LockOpenDuration { get; set; }

        [DataMember]
        public int AntiPassBackDurationOfEntrance { get; set; }

        [DataMember]
        public string DuressPassword { get; set; }

        [DataMember]
        public string EmergencyPassword { get; set; }

        [DataMember]
        public bool DisableAlarmSounds { get; set; }

        [DataMember]
        public int OpenDoorDelay { get; set; }

        [DataMember]
        public bool AllowSuperuserAccessWhenLockDown { get; set; }
    }
}
