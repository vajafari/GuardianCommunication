using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [DataContract]
    public enum DeviceCommandTypeEnumeration : short
    {
        [EnumMember]
        Other = 0,
        [EnumMember]
        SetUserInfo = 1,
        [EnumMember]
        SetFinger = 2,
        [EnumMember]
        SetFace = 3,
        [EnumMember]
        SetPalm = 4,
        [EnumMember]
        SetPhoto = 5,
        [EnumMember]
        SetTimeZone = 6,
        [EnumMember]
        SetHoliday = 7,
        [EnumMember]
        DeleteUser = 8,
        [EnumMember]
        ReadAttendance = 9,
        [EnumMember]
        ReadoutAttendance = 10,
        [EnumMember]
        ReadUser = 11,
        [EnumMember]
        ReadFingerPrint = 12,
        [EnumMember]
        ClearData = 13,
        [EnumMember]
        ClearUser = 14,
        [EnumMember]
        Check = 15,
        [EnumMember]
        ScanFace = 16,
        [EnumMember]
        ScanFinger = 17,
        [EnumMember]
        Reboot = 18,
        [EnumMember]
        Unlock = 19,
        [EnumMember]
        EnrollUserWithTemplate = 20,
        [EnumMember]
        ScanCard = 21,
        [EnumMember]
        FaceCount = 22,
        [EnumMember]
        FingerCount = 23,
        [EnumMember]
        UserCount = 24,
        [EnumMember]
        AttendanceLogCount = 25,
        [EnumMember]
        SendWithoutFinger = 26,
        [EnumMember]
        SendValidInvalid = 27,
        [EnumMember]
        SetDateAndTime = 28,
        //[EnumMember]
        //OpenDoor = 29,
        [EnumMember]
        SendHolidays = 30,
        [EnumMember]
        SendAccessGroup = 31,
        [EnumMember]
        SetDoorInfo = 32,
        [EnumMember]
        VirdiAccessControlData = 33,
        [EnumMember]
        SupremaSdk2HolidayGroup = 34,
        [EnumMember]
        SupremaSdk2AccessSchedule = 35,
        [EnumMember]
        SupremaSdk2AccessLevel = 36,
        [EnumMember]
        SupremaSdk2AccessGroup = 37,
        [EnumMember]
        SupremaSdk2DoorInfo = 38,
        [EnumMember]
        ScanIris = 39,
        [EnumMember]
        CancelOperation = 40,
        [EnumMember]
        PadisControllerSetRelay = 41,       
        [EnumMember]
        PadisControllerSetIoPort = 42,
        [EnumMember]
        PadisControllerSetWiegand = 43,
        [EnumMember]
        PadisControllerSetCalendar = 44,
        [EnumMember]
        TimySetDayTimezone = 45,
        [EnumMember]
        TimySetWeekTimezone = 45,
        [EnumMember]
        TimySetHoliday = 46,

    }
}
