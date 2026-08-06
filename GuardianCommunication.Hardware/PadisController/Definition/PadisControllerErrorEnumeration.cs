namespace GuardianCommunication.Hardware.PadisController.Definition
{
    public enum PadisControllerErrorEnumeration
    {
        Success = 0,
        // Validation Errors (1000-1099)
        ValidationError = 1000,
        MissingRequiredField = 1001,
        InvalidEpochValue = 1002,
        DateTimeFieldConflict = 1003,
        InvalidDateTimeFormat = 1004,
        InvalidInput = 1005,
        InvalidInputFormat = 1006,
        InvalidInputRange = 1007,
        InvalidInputType = 1008,
        MissingStartTime = 1009,
        MissingEndTime = 1010,
        InvalidTimeRange = 1011,

        // DateTime Setting Errors (2000-2099)
        PlatformNotSupported = 2000,
        RtcSetFailed = 2001,
        RtcSyncFailed = 2002,
        SubprocessTimeout = 2003,
        SudoPermissionDenied = 2004,
        InternalServerError = 2005,

        // System Reboot Errors (2100-2199)
        RebootFailed = 2100,
        RebootCommandError = 2101,
        RebootTimeout = 2102,
        RebootPermissionDenied = 2103,

        // Device Info Errors (2200-2299)
        DeviceInfoFailed = 2200,

        // System Health Errors (2300-2399)
        SystemHealthFailed = 2300,

        // User Management Errors (3000-3099)
        UserNotFound = 3000,
        UserAlreadyExists = 3001,
        UserCreateFailed = 3002,
        UserUpdateFailed = 3003,
        UserDeleteFailed = 3004,
        UserIdRequired = 3005,
        UserInvalidType = 3006,
        UserAccessDenied = 3007,
        UserClearAllFailed = 3008,
        UserAccountExpired = 3009,
        UserAccountNotActiveYet = 3010,

        // Card Management Errors (3100-3199)
        CardNotFound = 3100,
        CardAlreadyExists = 3101,
        CardAddFailed = 3102,
        CardDeleteFailed = 3103,
        CardInvalidNumber = 3104,
        CardUserIdRequired = 3105,
        CardNumberRequired = 3106,

        // Authentication & Authorization Errors (4000-4099)
        AuthenticationRequired = 4000,
        InvalidCredentials = 4001,
        AccessDenied = 4002,
        InsufficientPermissions = 4003,
        InvalidUserType = 4004,
        AccountExpired = 4005,
        AccountNotActive = 4006,
        RestApiAccessOnly = 4009,

        // Access
        DoorIdNotFound = 5000,
        CalendarNotFound = 5001,
        AccessLevelNotFound = 5002,





        LineIsBusy = 1_000_000,
        MessageTimeout = 1_000_001,
        UnknownError = 1_000_002,
    }
}
