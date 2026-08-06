namespace GuardianCommunication.Shared.Dto
{
    public class DtoZkDeviceDoor : DtoDeviceDoorBase
    {
        
        public int VerificationStyle { get; set; }

        public short WiegandFormat { get; set; }

        public bool CardNumberReversal { get; set; }

        public int OperateInterval { get; set; }

        public short DoorSensorType { get; set; }

        public bool ReverseLockStateOnDoorClose { get; set; }

        public int DoorSensorDelay { get; set; }

        public int? PassageModeTimeZoneNumber { get; set; }

        public int PassageDelay { get; set; }

        public int MultiPersonOperationInterval { get; set; }

        public int ActiveTimeZoneNumber { get; set; }

        public int LockOpenDuration { get; set; }

        public int AntiPassBackDurationOfEntrance { get; set; }

        public string DuressPassword { get; set; }

        public string EmergencyPassword { get; set; }

        public bool DisableAlarmSounds { get; set; }
        
        public bool AllowSuperuserAccessWhenLockDown { get; set; }
    }
}
