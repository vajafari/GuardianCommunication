namespace GuardianCommunication.Hardware.Virdi.VirdiConcepts
{
    public enum VirdiErrorEnum
    {
        // -------- General --------
        BaseGeneral = 0,

        Success = 0,
        InvalidPointer = 1,
        InvalidType = 2,
        InvalidParameter = 3,
        InvalidData = 4,
        FunctionFail = 5,
        NotServerActive = 6,
        InvalidTerminal = 7,
        ProcessFail = 8,
        UserCancel = 9,
        UnknownReason = 16,
        NotConnected = 17,

        // -------- Base Init --------
        BaseInit = 256,

        // -------- Base Memory / Data Size --------
        BaseMemory = 512,

        CodeSize = 513,
        UserIdSize = 514,
        UserNameSize = 515,
        UniqueIdSize = 516,
        InvalidSecurityLevel = 517,
        PasswordSize = 518,
        PictureSize = 519,
        InvalidPictureType = 520,
        RfidSize = 521,
        InvalidIndex = 528,
        MaxCardNumber = 529,
        MaxFingerNumber = 530,
        MaxTimezoneNumber = 531,
        MaxHolidayNumber = 532,
        MaxAccessTimeNumber = 533,
        MaxAccessGroupNumber = 534,

        // -------- Authentication --------
        BaseAuthentication = 768,

        InvalidUser = 769,
        Unauthorized = 770,
        Permission = 771,
        FingerCaptureFail = 772,
        DupAuthenticaion = 773,
        Antipassbsck = 774,
        Network = 775,
        ServerBusy = 776,
        FaceDetection = 777,
        Blacklist = 778,
        MealPay = 779,
        MealType = 780,
        MealCode = 781,
        Period = 782,
        MealLimit = 783,
        DayLimit = 784,
        MonthLimit = 785,
        SoftPassback = 800,
        DuressFinger = 801,

        // -------- Terminal --------
        BaseTerminal = 1024,

        UserRegistration = 1025,
        FwResource = 1026,
        FwUpgrade = 1027,
        FwVersion = 1028,
        FreshMemory = 1029,
        NotFoundUser = 1030,
        SimilarFp = 1031,
        DuplicateUser = 1032,
        DuplicateRfid = 1033,

        // -------- VD Service --------
        BaseVdService = 2304,

        ParameterIsNotValid = 2305,
        UnknownError = 2306,
        UserNotFound = 2307
    }
}
