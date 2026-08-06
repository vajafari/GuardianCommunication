namespace GuardianCommunication.Hardware.PadisController.Definition
{
    public enum PadisControllerAttendanceStatusEnumeration
    {
        Success = 0,
        FailedInvalidUser = 1,
        FailedToAccessAtTime = 2,
        FailedToAccessToDoor = 3,
        FailedForPassAccuracy = 4,
        FailedForAntiPassback = 5,
        FailedForBlacklist = 6,
        FailedForDoorCalendarLock = 7,
        FailedForNotAccessToAnyGroup = 8,
        FailedForNoRelevantGroupFound = 9,
        FailedForNoCoverAllGroup = 10,
        FailedForNoAccessSchedule = 11,
        FailedForNoUserSchedule = 12,
    }
}
