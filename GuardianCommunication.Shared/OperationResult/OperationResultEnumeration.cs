

// ReSharper disable InconsistentNaming

using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.OperationResult
{
    [DataContract]
    public enum OperationResultEnumeration
    {
        /// <summary>
        /// عملیات موفقیت امیز انجام شد
        /// </summary>
        [EnumMember]
        CommunicationStatusSuccessful = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        AttendanceStatusAttendanceAlreadyExist = 12501,

        /// <summary>
        /// خطای ناشناخته در سیستم رخ داده است
        /// </summary>
        [EnumMember]
        CommunicationStatusUnknownError = 10000001,
        /// <summary>
        /// هیچ مقداری به پارامتر نسبت داده نشده است
        /// </summary>
        [EnumMember]
        CommunicationStatusObjectIsNull = 10000002,
        [EnumMember]
        CommunicationStatusGeneralErrorOnApiCall = 10000003,
        [EnumMember]
        CommunicationStatusFaultCausedCacheDataReset = 10000004,
        [EnumMember]
        CommunicationStatusNotSupport = 10000005,
        [EnumMember]
        CommunicationStatusConnectTheDeviceFirst = 10000006,
        [EnumMember]
        CommunicationStatusCannotConnect = 10000007,
        [EnumMember]
        CommunicationStatusGeneralEndDateMustBeGreaterThanOrEqualStartDate = 10000008,
        [EnumMember]
        CommunicationMaxFingerExceeded = 10000009,
        [EnumMember]
        CommunicationMaxHolidayExceeded = 10000010,
        [EnumMember]
        CommunicationMaxReadoutDaysIsNotValid = 10000011,
        [EnumMember]
        CommunicationHookSystemDataValidationError = 10000012,
        [EnumMember]
        CommunicationHookSystemCallError = 10000013,
        [EnumMember]
        CommunicationObjectRequestTimeout = 10000014,
        [EnumMember]
        CommunicationObjectLogOnFailed = 10000015,
        [EnumMember]
        CommunicationObjectDatabaseNotAvailable = 10000016,
        [EnumMember]
        CommunicationObjectUniqueKeyConstraint = 10000017,
        [EnumMember]
        CommunicationObjectHasForeignKeyConstraint = 10000018,
        [EnumMember]
        CommunicationStringOrBinaryMustBeTruncated = 10000019,
        [EnumMember]
        CommunicationNotFoundOnlineMonitoringDevice = 10000020,
        [EnumMember]
        CommunicationStatusProducerNotSupport = 10000021,
        [EnumMember]
        CommunicationNotValidRequest = 10000022,
        [EnumMember]
        CommunicationHardwareServiceTimeout = 10000023,
        [EnumMember]
        DeviceNotFoundInCache = 10000024,
        [EnumMember]
        DeviceDoorSettingIsNotValid = 10000025,
        [EnumMember]
        DeviceSettingIsNotValid = 10000026,
        [EnumMember]
        DeviceDoorNotFoundInCache = 10000027,
        [EnumMember]
        CommunicationStatusDeviceAttendanceCollectionIsNotActive = 10000029,
        [EnumMember]
        CommunicationStatusDeviceEventCollectionIsNotActive = 10000030,
        [EnumMember]
        CommunicationStatusTempUserMustHaveEndDate = 10000031,


        /// <summary>
        /// ادرس ای پی معتبر نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorIpIsNotValid = 10001000,
        /// <summary>
        /// پئرت معتبر نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorTcpPortIsNotValid = 10001001,
        /// <summary>
        /// کد دستگله معتبر نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorMachineNumberIsNotValid = 10001002,
        /// <summary>
        /// کانال ارتباطی در دسترس نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative100 = 10001003,
        /// <summary>
        /// کانال ارتباطی سریال در دسترس نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative101 = 10001004,
        /// <summary>
        /// امکان نوشتن بر روی کانال ارتباطی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative102 = 10001005,
        /// <summary>
        /// مهلت زمانی نوشتن بر روی کانال ارتباطی منقضی شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative103 = 10001006,
        /// <summary>
        /// امکان خواندن از روی کانال ارتباطی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative104 = 10001007,
        /// <summary>
        /// مهلت زمانی خواندن از روی کانال ارتباطی منقضی شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative105 = 10001008,
        /// <summary>
        /// CHANNEL_OVERFLOW
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative106 = 10001009,
        /// <summary>
        /// کانال ارتباطی بسته شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative107 = 10001010,
        /// <summary>
        /// امکان مقداردهی اولیه به کانال ارتباطی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative200 = 10001011,
        /// <summary>
        /// امکان بازکردن کانال ارتباطی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative201 = 10001012,
        /// <summary>
        /// امکان اتصال به کانال ارتباطی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative202 = 10001013,
        /// <summary>
        /// کانال ارتباطی بسته شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative203 = 10001014,
        /// <summary>
        /// امکام باز کردن ارتباط سریال وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative220 = 10001015,
        /// <summary>
        /// امکان بازکردن ارتباط یو اس بی وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative240 = 10001016,
        /// <summary>
        /// حافظه یو اس بی نامعتبر
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative260 = 10001017,
        /// <summary>
        /// کمبود فضا در حافظه یو اس بی
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative261 = 10001018,
        /// <summary>
        /// حافظه یو اس بی یافت نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative262 = 10001019,
        /// <summary>
        /// VT_EXIST_IN_MEMORY
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative263 = 10001020,
        /// <summary>
        /// حافظه یو اس بی پر شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative264 = 10001021,
        /// <summary>
        /// ERR_NO_MORE_VT
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative265 = 10001022,
        /// <summary>
        /// کانال ارتباطی شلوغ است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative300 = 10001023,
        /// <summary>
        /// INVALID_PACKET
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative301 = 10001024,
        /// <summary>
        /// خطای CHECKSUM
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative302 = 10001025,
        /// <summary>
        /// عملیات پشتیبانی نمی شود
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative303 = 10001026,
        /// <summary>
        /// خطا در خواندن و نوشتن
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative304 = 10001027,
        /// <summary>
        /// حافظه پر است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative305 = 10001028,
        /// <summary>
        /// پارامتر های نامعتبر
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative307 = 10001029,
        /// <summary>
        /// ERR_RTC
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative308 = 10001030,
        /// <summary>
        /// حافظه پر است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative309 = 10001031,
        /// <summary>
        /// حافظه پر است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative310 = 10001032,
        /// <summary>
        /// شناسه نامعتبر
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative311 = 10001033,
        /// <summary>
        /// پورت یو اس بی غیر فعال شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative312 = 10001034,
        /// <summary>
        /// پورت سریال غیرفعال شده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative313 = 10001035,
        /// <summary>
        /// کلمه عبور نامعتبر
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative314 = 10001036,
        /// <summary>
        /// مجددا سعی نمایید
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative315 = 10001037,
        /// <summary>
        /// اثر انگشت وجود دارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative316 = 10001038,
        /// <summary>
        /// هیچ کاربری بر روی دستگاه وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative320 = 10001039,
        /// <summary>
        /// CANNOT_CHANGE_IMG_VIEW
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative321 = 10001040,
        /// <summary>
        /// NO_MORE_TERMINAL
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative400 = 10001041,
        /// <summary>
        /// ترمینال یافت نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative401 = 10001042,
        /// <summary>
        /// خطای ارتباط با ترمینال
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative402 = 10001043,
        /// <summary>
        /// TERMINAL_NOT_AUTHORIZED
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative403 = 10001044,
        /// <summary>
        /// TERMINAL_BUSY
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative404 = 10001045,
        /// <summary>
        /// BS_ERR_DB_NOT_EXIST
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative500 = 10001046,
        /// <summary>
        /// BS_ERR_CANNOT_CONNECT_TO_DB
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative501 = 10001047,
        /// <summary>
        /// BS_ERR_DB_INTERNAL_ERROR
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative502 = 10001048,
        /// <summary>
        /// BS_ERR_CANNOT_INIT_SSL
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative601 = 10001049,
        /// <summary>
        /// BS_ERR_SSL_INVALID_CTX
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative602 = 10001050,
        /// <summary>
        /// BS_ERR_SSL_INVALID_CERTFILE
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative603 = 10001051,
        /// <summary>
        /// BS_ERR_SSL_INVALID_KEYFILE
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative604 = 10001052,
        /// <summary>
        /// BS_ERR_SSL_INVALID_CAFILE
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative605 = 10001053,
        /// <summary>
        /// BS_ERR_SSL_INVALID_PATH
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative606 = 10001054,
        /// <summary>
        /// امکان ارتباط بر روی خط امن وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative607 = 10001055,
        /// <summary>
        /// داده نامعتبر
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative608 = 10001056,
        /// <summary>
        /// خطای ناشناخته
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative9999 = 10001057,
        /// <summary>
        /// کاربر در دستگاه تعریف نشده است
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk1ErrorCodeNegative306 = 10001058,



        /// <summary>
        /// خطا در تخصیص کانتکس به واسط سخت افزار
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk2CantAllocateSdkContext = 10001500,
        /// <summary>
        /// کد کاربر معتبر نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusSupremaSdk2UserIdIsNotValid = 10001501,
        [EnumMember]
        CommunicationStatusSupremaSdk2ErrorFromDeviceDriver = 10001502,
        // Communication errors
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenSocket = 10001503,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotConnectSocket = 10001504,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotListenSocket = 10001505,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotAcceptSocket = 10001506,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadSocket = 10001507,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotWriteSocket = 10001508,
        [EnumMember]
        CommunicationStatusSupremaSdk2SocketIsNotConnected = 10001509,
        [EnumMember]
        CommunicationStatusSupremaSdk2SocketIsNotOpened = 10001510,
        [EnumMember]
        CommunicationStatusSupremaSdk2SocketIsNotListened = 10001511,
        [EnumMember]
        CommunicationStatusSupremaSdk2SocketInProgress = 10001512,
        [EnumMember]
        CommunicationStatusSupremaSdk2Ip4IsNotEnabled = 10001513,
        [EnumMember]
        CommunicationStatusSupremaSdk2Ip6IsNotEnabled = 10001514,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotSupportedSpecifiedDeviceInfo = 10001515,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotEnoughBuffer = 10001516,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidAddress = 10001517,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidParam = 10001518,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPacket = 10001519,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidDeviceId = 10001520,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidDeviceType = 10001521,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPacketChecksum = 10001522,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPacketIndex = 10001523,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPacketCommand = 10001524,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPacketSequence = 10001525,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoPacket = 10001526,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidCodeSign = 10001527,
        [EnumMember]
        CommunicationStatusSupremaSdk2ExtractionFail = 10001528,
        [EnumMember]
        CommunicationStatusSupremaSdk2VerifyFail = 10001529,
        [EnumMember]
        CommunicationStatusSupremaSdk2IdentifyFail = 10001530,
        [EnumMember]
        CommunicationStatusSupremaSdk2IdentifyTimeout = 10001531,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintCaptureFailed = 10001532,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintScanTimeout = 10001533,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintScanCanceled = 10001534,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotSameFingerPrint = 10001535,
        [EnumMember]
        CommunicationStatusSupremaSdk2ExtractionLowQuality = 10001536,
        [EnumMember]
        CommunicationStatusSupremaSdk2CaptureLowQuality = 10001537,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindFingerPrint = 10001538,
        [EnumMember]
        CommunicationStatusSupremaSdk2FakeFingerPrintDetected = 10001539,
        [EnumMember]
        CommunicationStatusSupremaSdk2FakeFingerPrintTryAgain = 10001540,
        [EnumMember]
        CommunicationStatusSupremaSdk2FakeFingerPrintSensor = 10001541,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindFace = 10001542,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceCaptureFail = 10001543,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceScanTimeout = 10001544,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceScanCancelled = 10001545,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceScanFailed = 10001546,
        [EnumMember]
        CommunicationStatusSupremaSdk2UnmaskedFaceDetected = 10001547,
        [EnumMember]
        CommunicationStatusSupremaSdk2FakeFaceDetected = 10001548,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotEstimate = 10001549,
        [EnumMember]
        CommunicationStatusSupremaSdk2NormalizeFace = 10001550,
        [EnumMember]
        CommunicationStatusSupremaSdk2SmallDetection = 10001551,
        [EnumMember]
        CommunicationStatusSupremaSdk2LargeDetection = 10001552,
        [EnumMember]
        CommunicationStatusSupremaSdk2BiasedDetection = 10001553,
        [EnumMember]
        CommunicationStatusSupremaSdk2RotatedFace = 10001554,
        [EnumMember]
        CommunicationStatusSupremaSdk2OverlappedFace = 10001555,
        [EnumMember]
        CommunicationStatusSupremaSdk2UnopenedFace = 10001556,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotLookingFront = 10001557,
        [EnumMember]
        CommunicationStatusSupremaSdk2OccludedMouth = 10001558,
        [EnumMember]
        CommunicationStatusSupremaSdk2MatchFailed = 10001559,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenDir = 10001560,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenFile = 10001561,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotWriteFile = 10001562,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSeekFile = 10001563,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadFile = 10001564,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotGetStat = 10001565,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotGetSystemInfo = 10001566,
        [EnumMember]
        CommunicationStatusSupremaSdk2DataMismatch = 10001567,
        [EnumMember]
        CommunicationStatusSupremaSdk2AlreadyOpenedDir = 10001568,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidRelay = 10001569,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotWriteIoPacket = 10001570,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadIoPacket = 10001571,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadInput = 10001572,
        [EnumMember]
        CommunicationStatusSupremaSdk2ReadInputTimeout = 10001573,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotEnableInput = 10001574,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSetInputDuration = 10001575,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPort = 10001576,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidInterPhoneType = 10001577,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidLcdParams = 10001578,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotWriteLcdPacket = 10001579,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadLcdPacket = 10001580,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidLcdPacket = 10001581,
        [EnumMember]
        CommunicationStatusSupremaSdk2InputQueueFull = 10001582,
        [EnumMember]
        CommunicationStatusSupremaSdk2WiegandQueueFull = 10001583,
        [EnumMember]
        CommunicationStatusSupremaSdk2MISCInputQueueFull = 10001584,
        [EnumMember]
        CommunicationStatusSupremaSdk2WiegandDataQueueFull = 10001585,
        [EnumMember]
        CommunicationStatusSupremaSdk2WiegandDataQueueEmpty = 10001586,
        [EnumMember]
        CommunicationStatusSupremaSdk2SdkNotSupported = 10001587,
        [EnumMember]
        CommunicationStatusSupremaSdk2Timeout = 10001588,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSetTime = 10001589,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidDataFile = 10001590,
        [EnumMember]
        CommunicationStatusSupremaSdk2ToLargeDataForSlot = 10001591,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidSlotNumber = 10001592,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidSlotData = 10001593,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotInitDb = 10001594,
        [EnumMember]
        CommunicationStatusSupremaSdk2DuplicateId = 10001595,
        [EnumMember]
        CommunicationStatusSupremaSdk2UserFull = 10001596,
        [EnumMember]
        CommunicationStatusSupremaSdk2DuplicateTemplate = 10001597,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintFull = 10001598,
        [EnumMember]
        CommunicationStatusSupremaSdk2DuplicateCard = 10001599,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotValidHdrFile = 10001600,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidLogFile = 10001601,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindUser = 10001602,
        [EnumMember]
        CommunicationStatusSupremaSdk2AccessLevelFull = 10001603,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidUserId = 10001604,
        [EnumMember]
        CommunicationStatusSupremaSdk2BlacklistFull = 10001605,
        [EnumMember]
        CommunicationStatusSupremaSdk2UsernameFull = 10001606,
        [EnumMember]
        CommunicationStatusSupremaSdk2UserImageFull = 10001607,
        [EnumMember]
        CommunicationStatusSupremaSdk2UserImageIsTooBig = 10001608,
        [EnumMember]
        CommunicationStatusSupremaSdk2SlotDataCheckSum = 10001609,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotUpdateFingerPrint = 10001610,
        [EnumMember]
        CommunicationStatusSupremaSdk2TemplateFormatMismatch = 10001611,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotAdminUser = 10001612,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindLog = 10001613,
        [EnumMember]
        CommunicationStatusSupremaSdk2DoorScheduleFull = 10001614,
        [EnumMember]
        CommunicationStatusSupremaSdk2DbSlotFull = 10001615,
        [EnumMember]
        CommunicationStatusSupremaSdk2AccessGroupFull = 10001616,
        [EnumMember]
        CommunicationStatusSupremaSdk2FloorLevelFull = 10001617,
        [EnumMember]
        CommunicationStatusSupremaSdk2AccessScheduleFull = 10001619,
        [EnumMember]
        CommunicationStatusSupremaSdk2HolidayGroupFull = 10001620,
        [EnumMember]
        CommunicationStatusSupremaSdk2HolidayFull = 10001621,
        [EnumMember]
        CommunicationStatusSupremaSdk2TimePeriodFull = 10001622,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoCredential = 10001623,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoBiometricCredential = 10001624,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoCardCredential = 10001625,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoPinCredential = 10001626,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoBiometricPinCredential = 10001627,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoUsername = 10001628,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoUserImage = 10001629,
        [EnumMember]
        CommunicationStatusSupremaSdk2ReaderFull = 10001630,
        [EnumMember]
        CommunicationStatusSupremaSdk2CacheMissed = 10001631,
        [EnumMember]
        CommunicationStatusSupremaSdk2OperatorFull = 10001632,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidLinkId = 10001633,
        [EnumMember]
        CommunicationStatusSupremaSdk2TimerCanceled = 10001634,
        [EnumMember]
        CommunicationStatusSupremaSdk2UserJobFull = 10001635,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotUpdateFace = 10001636,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceFull = 10001637,
        [EnumMember]
        CommunicationStatusSupremaSdk2FloorScheduleFull = 10001638,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindAuthGroup = 10001639,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthGroupFull = 10001640,
        [EnumMember]
        CommunicationStatusSupremaSdk2UserPhraseFull = 10001641,
        [EnumMember]
        CommunicationStatusSupremaSdk2DstPhraseFull = 10001642,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindDstModule = 10001643,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidSchedule = 10001644,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindOperator = 10001645,
        [EnumMember]
        CommunicationStatusSupremaSdk2DuplicateFingerPrint = 10001646,
        [EnumMember]
        CommunicationStatusSupremaSdk2DuplicateFace = 10001647,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotFaceCredential = 10001648,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoFingerPrintCredential = 10001649,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoFacePinCredential = 10001650,
        [EnumMember]
        CommunicationStatusSupremaSdk2NoFingerPrintPinCredential = 10001651,
        [EnumMember]
        CommunicationStatusSupremaSdk2userImageExFull = 10001652,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidConfig = 10001653,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenConfigFile = 10001654,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReadConfigFile = 10001655,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidConfigFile = 10001656,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidConfigData = 10001657,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotWriteConfigData = 10001658,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidConfigIndex = 10001659,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotScanFinger = 10001660,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotScanCard = 10001661,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenRtc = 10001662,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSetRtc = 10001663,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotGetRtc = 10001664,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSetLed = 10001665,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotOpenDeviceDriver = 10001666,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindDevice = 10001667,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotScanFace = 10001668,
        [EnumMember]
        CommunicationStatusSupremaSdk2SlaveFull = 10001669,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotAddDevice = 10001670,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindDoor = 10001671,
        [EnumMember]
        CommunicationStatusSupremaSdk2DoorFull = 10001672,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotLockDoor = 10001673,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotUnlockDoor = 10001674,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotReleaseDoor = 10001675,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindLift = 10001676,
        [EnumMember]
        CommunicationStatusSupremaSdk2LiftFull = 10001677,
        [EnumMember]
        CommunicationStatusSupremaSdk2AccessRuleViolation = 10001678,
        [EnumMember]
        CommunicationStatusSupremaSdk2Disabled = 10001679,
        [EnumMember]
        CommunicationStatusSupremaSdk2NotYetValid = 10001680,
        [EnumMember]
        CommunicationStatusSupremaSdk2Expired = 10001681,
        [EnumMember]
        CommunicationStatusSupremaSdk2Blacklist = 10001682,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindAccessGroup = 10001683,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindAccessLevel = 10001684,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindAccessSchedule = 10001685,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindHolidayGroup = 10001686,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindBlackList = 10001687,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthTimeout = 10001688,
        [EnumMember]
        CommunicationStatusSupremaSdk2DualAuthTimeout = 10001689,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidAuthMode = 10001691,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthUnexpectedUser = 10001692,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthUnexpectedCredential = 10001693,
        [EnumMember]
        CommunicationStatusSupremaSdk2DualAuthFailed = 10001694,
        [EnumMember]
        CommunicationStatusSupremaSdk2BiometricAuthRequired = 10001695,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardAuthRequired = 10001696,
        [EnumMember]
        CommunicationStatusSupremaSdk2PinAuthRequired = 10001697,
        [EnumMember]
        CommunicationStatusSupremaSdk2BiometricOrPinAuthRequired = 10001698,
        [EnumMember]
        CommunicationStatusSupremaSdk2TnaCodeRequired = 10001699,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthServerMatchRefusal = 10001700,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindFloorLevel = 10001701,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthFail = 10001702,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthGroupRequired = 10001703,
        [EnumMember]
        CommunicationStatusSupremaSdk2IdentificationRequired = 10001704,
        [EnumMember]
        CommunicationStatusSupremaSdk2AntiTailgateViolation = 10001705,
        [EnumMember]
        CommunicationStatusSupremaSdk2HighTemperatureViolation = 10001706,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotMeasureTemperature = 10001707,
        [EnumMember]
        CommunicationStatusSupremaSdk2UnmaskedFaceViolation = 10001708,
        [EnumMember]
        CommunicationStatusSupremaSdk2MaskCheckRequired = 10001709,
        [EnumMember]
        CommunicationStatusSupremaSdk2TerminalCheckRequired = 10001710,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceAuthRequired = 10001711,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintAuthRequired = 10001712,
        [EnumMember]
        CommunicationStatusSupremaSdk2FaceOrOrPinAuthRequired = 10001713,
        [EnumMember]
        CommunicationStatusSupremaSdk2FingerPrintOrPinAuthRequired = 10001714,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindZone = 10001715,
        [EnumMember]
        CommunicationStatusSupremaSdk2ErrorSetZone = 10001716,
        [EnumMember]
        CommunicationStatusSupremaSdk2HardApbViolation = 10001717,
        [EnumMember]
        CommunicationStatusSupremaSdk2SoftApbViolation = 10001718,
        [EnumMember]
        CommunicationStatusSupremaSdk2HardTimedApbViolation = 10001719,
        [EnumMember]
        CommunicationStatusSupremaSdk2SoftTimedApbViolation = 10001720,
        [EnumMember]
        CommunicationStatusSupremaSdk2ScheduledLockViolation = 10001721,
        [EnumMember]
        CommunicationStatusSupremaSdk2ErrorScheduledUnlockViolation = 10001722,
        [EnumMember]
        CommunicationStatusSupremaSdk2IntrusionAlarmViolation = 10001723,
        [EnumMember]
        CommunicationStatusSupremaSdk2SetFireAlarm = 10001724,
        [EnumMember]
        CommunicationStatusSupremaSdk2ApbZoneFull = 10001725,
        [EnumMember]
        CommunicationStatusSupremaSdk2FireAlarmZoneFull = 10001726,
        [EnumMember]
        CommunicationStatusSupremaSdk2ScheduledLockUnlockZoneFull = 10001727,
        [EnumMember]
        CommunicationStatusSupremaSdk2InactiveZone = 10001728,
        [EnumMember]
        CommunicationStatusSupremaSdk2IntrusionAlarmZoneFull = 10001729,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotArm = 10001730,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotDisarm = 10001731,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindArmCard = 10001732,
        [EnumMember]
        CommunicationStatusSupremaSdk2HardEntranceLimitCountViolation = 10001733,
        [EnumMember]
        CommunicationStatusSupremaSdk2SoftEntranceLimitCountViolation = 10001734,
        [EnumMember]
        CommunicationStatusSupremaSdk2HardEntranceLimitTimeViolation = 10001735,
        [EnumMember]
        CommunicationStatusSupremaSdk2SoftEntranceLimitTimeViolation = 10001736,
        [EnumMember]
        CommunicationStatusSupremaSdk2InterlockZoneDoorViolation = 10001737,
        [EnumMember]
        CommunicationStatusSupremaSdk2IntervalZoneInputViolation = 10001738,
        [EnumMember]
        CommunicationStatusSupremaSdk2InterlockZoneFull = 10001739,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthLimitScheduleViolation = 10001740,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthLimitCountViolation = 10001741,
        [EnumMember]
        CommunicationStatusSupremaSdk2AuthLimitUserViolation = 10001742,
        [EnumMember]
        CommunicationStatusSupremaSdk2SoftAuthLimitViolation = 10001743,
        [EnumMember]
        CommunicationStatusSupremaSdk2HardAuthLimitViolation = 10001744,
        [EnumMember]
        CommunicationStatusSupremaSdk2LiftLockUnlockZoneFull = 10001745,
        [EnumMember]
        CommunicationStatusSupremaSdk2LiftLockViolation = 10001746,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardIo = 10001747,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardInitFail = 10001748,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardNotActivated = 10001749,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardCannotReadData = 10001750,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardCisCrc = 10001751,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardCannotWriteData = 10001752,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardReadTimeout = 10001753,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardReadCanceled = 10001754,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardCannotSendData = 10001755,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotFindCard = 10001756,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidPassword = 10001757,
        [EnumMember]
        CommunicationStatusSupremaSdk2CameraInitFail = 10001758,
        [EnumMember]
        CommunicationStatusSupremaSdk2JpegEncoderInitFail = 10001759,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotEncodeJpeg = 10001760,
        [EnumMember]
        CommunicationStatusSupremaSdk2JpegEncoderNotInitialized = 10001770,
        [EnumMember]
        CommunicationStatusSupremaSdk2JpegEncoderUninitFail = 10001771,
        [EnumMember]
        CommunicationStatusSupremaSdk2CameraCaptureFail = 10001772,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotDetectFace = 10001773,
        [EnumMember]
        CommunicationStatusSupremaSdk2FileIo = 10001774,
        [EnumMember]
        CommunicationStatusSupremaSdk2AllocMemory = 10001775,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotUpgrade = 10001776,
        [EnumMember]
        CommunicationStatusSupremaSdk2DeviceLocked = 10001777,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotSendToServer = 10001778,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslInit = 10001779,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslNotSupported = 10001780,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslCannotConnect = 10001781,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslAlreadyConnected = 10001782,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslInvalidCertification = 10001783,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslVerifyCertification = 10001784,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslInvalidKey = 10001785,
        [EnumMember]
        CommunicationStatusSupremaSdk2SslVerifyKey = 10001786,
        [EnumMember]
        CommunicationStatusSupremaSdk2MobilePortal = 10001787,
        [EnumMember]
        CommunicationStatusSupremaSdk2NullPointer = 10001788,
        [EnumMember]
        CommunicationStatusSupremaSdk2ErrorUninitilized = 10001789,
        [EnumMember]
        CommunicationStatusSupremaSdk2CannotRunService = 10001790,
        [EnumMember]
        CommunicationStatusSupremaSdk2Canceled = 10001791,
        [EnumMember]
        CommunicationStatusSupremaSdk2Exist = 10001792,
        [EnumMember]
        CommunicationStatusSupremaSdk2Encrypt = 10001793,
        [EnumMember]
        CommunicationStatusSupremaSdk2Decrypt = 10001794,
        [EnumMember]
        CommunicationStatusSupremaSdk2DeviceBusy = 10001795,
        [EnumMember]
        CommunicationStatusSupremaSdk2Internal = 10001796,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidFileFormat = 10001797,
        [EnumMember]
        CommunicationStatusSupremaSdk2InvalidScheduleId = 10001798,
        [EnumMember]
        CommunicationStatusSupremaSdk2UnknownFingerTemplate = 10001799,
        [EnumMember]
        CommunicationStatusSupremaSdk2SdkErrorDisabled = 10001800,
        [EnumMember]
        CommunicationStatusSupremaSdk2CardFull = 10001801,

        /// <summary>
        /// خطا در اتصال
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCode1 = 10002001,
        /// <summary>
        /// ایندکس اثر انگشت وجود دارد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCode2 = 10002002,
        /// <summary>
        ///خطای تخصیص بافر
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCode101 = 10002003,
        /// <summary>
        /// تماس مكرر
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCode102 = 10002004,
        /// <summary>
        /// مقداردهی اولیه انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative1 = 10002005,
        /// <summary>
        /// خطا در خواندن و نوشتن فایل
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2 = 10002006,
        /// <summary>
        ///  اندازه اشتباه
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative3 = 10002007,
        /// <summary>
        /// داده از قبل وجود دارد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative5 = 10002008,
        /// <summary>
        /// گذرواژه نادرست است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative6 = 10002009,
        /// <summary>
        /// خطای پاسخ
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative7 = 10002010,
        /// <summary>
        /// مهلت دریافت کنید
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative8 = 10002011,
        /// <summary>
        /// طول داده های ارسال شده نادرست است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative10 = 10002012,
        /// <summary>
        /// عملیات پشتیبانی نمی شود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative100 = 10002013,
        /// <summary>
        /// خطای نسخه داده
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative102 = 10002014,
        /// <summary>
        /// دستگاه نسخه اشتباه الگوی صورت را برمی گرداند
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative103 = 10002015,
        /// <summary>
        /// دستگاه مشغول است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative201 = 10002016,
        /// <summary>
        /// وقفه اتصال
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative307 = 10002017,
        /// <summary>
        /// بازگشت برای اجرای دستور ناموفق است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2001 = 10002018,
        /// <summary>
        /// بازگشت داده ها
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2002 = 10002019,
        /// <summary>
        /// رویداد ثبت شده رخ داده است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2003 = 10002020,
        /// <summary>
        /// دستور REPEAT را برگردانید
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2004 = 10002021,
        /// <summary>
        /// بازگشت فرماندهی UNAUTH
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative2005 = 10002022,
        /// <summary>
        /// تخصیص حافظه انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4982 = 10002023,
        /// <summary>
        /// محاسبه مقدار Hash انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4983 = 10002024,
        /// <summary>
        /// نوشتن پرونده ناموفق بود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4984 = 10002025,
        /// <summary>
        /// خواندن پرونده ناموفق بود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4985 = 10002026,
        /// <summary>
        /// پرونده وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4986 = 10002027,
        /// <summary>
        /// سرریز حافظه اختصاص جلسه
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4987 = 10002028,
        /// <summary>
        /// فضای حافظه کافی برای جلسه اختصاص داده نشده است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4988 = 10002029,
        /// <summary>
        /// جلسه قادر به تخصیص حافظه نبود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4989 = 10002030,
        /// <summary>
        /// حجم داده پایگاه داده به حد دستگاه رسیده است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4990 = 10002031,
        /// <summary>
        /// هیچ داده مرتبطی در پایگاه داده وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4991 = 10002032,
        /// <summary>
        /// عملیات حذف پایگاه داده انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4992 = 10002033,
        /// <summary>
        /// عملیات خواندن پایگاه داده انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4993 = 10002034,
        /// <summary>
        ///  عملیات به روز رسانی پایگاه داده انجام نشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4994 = 10002035,
        /// <summary>
        /// عملیات افزودن پایگاه داده ناموفق بود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4995 = 10002036,
        /// <summary>
        /// پارامتر ارسالی توسط نرم افزار به دستگاه اشتباه است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4996 = 10002037,
        /// <summary>
        ///  طول داده های ارسال شده توسط نرم افزار به دستگاه اشتباه است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4997 = 10002038,
        /// <summary>
        /// خطای پارامتر دستگاه را بنویسید
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4998 = 10002039,
        /// <summary>
        /// خطا در خواندن پارامترهای دستگاه
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative4999 = 10002040,
        /// <summary>
        ///  ایجاد وقفه سوکت (وقفه اتصال)
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12001 = 10002041,
        /// <summary>
        /// حافظه کافی نیست
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12002 = 10002042,
        /// <summary>
        /// خطای نسخه سوکت
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12003 = 10002043,
        /// <summary>
        /// پروتکل غیر TCP
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12004 = 10002044,
        /// <summary>
        /// مهلت انتظار
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12005 = 10002045,
        /// <summary>
        /// ارسال مهلت زمانی داده
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12006 = 10002046,
        /// <summary>
        /// خواندن مهلت زمانی داده
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12007 = 10002047,
        /// <summary>
        /// SOCKET قابل خواندن نیست
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative12008 = 10002048,
        /// <summary>
        /// در انتظار خطای سمافر
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13009 = 10002049,
        /// <summary>
        /// تعداد تلاشهای مجدد بیشتر شد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13010 = 10002050,
        /// <summary>
        /// REPLYID
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13011 = 10002051,
        /// <summary>
        /// خطای Checksum
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13012 = 10002052,
        /// <summary>
        /// منتظر مهلت زمانی semaphore
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13013 = 10002053,
        /// <summary>
        /// DIRTY_DATA
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13014 = 10002054,
        /// <summary>
        ///  اندازه بافر خیلی کوچک است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13015 = 10002055,
        /// <summary>
        /// طول داده های خوانده شده اشتباه است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13016 = 10002056,
        /// <summary>
        /// داده خواندن نامعتبر است 1
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13017 = 10002057,
        /// <summary>
        /// داده خواندن نامعتبر است 2
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13018 = 10002058,
        /// <summary>
        /// داده خواندن نامعتبر است 3
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13019 = 10002059,
        /// <summary>
        /// داده از دست رفته
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13020 = 10002060,
        /// <summary>
        /// خطای مقداردهی اولیه حافظه
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative13021 = 10002061,
        /// <summary>
        /// مقدار وضعیت صادر شده توسط رابط SetShortkey تکرار می شود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15001 = 10002062,
        /// <summary>
        /// توضیحات تکراری فراخوانی رابط SetShortkey
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15002 = 10002063,
        /// <summary>
        /// منوی ثانویه در دستگاه باز نمی شود و نیازی به صدور آن نیست
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15003 = 10002064,
        /// <summary>
        /// خطا در دریافت ساختار جدول
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15100 = 10002065,
        /// <summary>
        /// قسمت شرط در ساختار جدول وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15101 = 10002066,
        /// <summary>
        /// تعداد کل قسمت ها متناقض است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15102 = 10002067,
        /// <summary>
        /// مرتب سازی درست متناقض است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15103 = 10002068,
        /// <summary>
        /// خطای تخصیص حافظه
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15104 = 10002069,
        /// <summary>
        /// خطای داده هنگام تجزیه داده ها
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15105 = 10002070,
        /// <summary>
        /// داده های صادر شده بیش از 4 است ، سرریز داده ها
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15106 = 10002071,
        /// <summary>
        /// گزینه OPTIONS نامعتبر است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative15108 = 10002072,
        /// <summary>
        /// خطای داده هنگام تجزیه داده ها ، شناسه جدول یافت نمی شود
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative5113 = 10002073,
        /// <summary>
        /// تعداد قسمتها کمتر از یا برابر با 0 است و داده های برگشتی غیر عادی هستند
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative5114 = 10002074,
        /// <summary>
        /// عداد کل قسمتهای جدول با تعداد کل قسمتهای تعیین شده توسط خود داده مغایرت دارد و داده ها غیر عادی است
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorCodeNegative5115 = 10002075,
        /// <summary>
        /// پورت ارتباطات معتبر نمی باشد
        /// </summary>
        [EnumMember]
        CommunicationStatusZkTcpPortIsNotValid = 10002076,
        /// <summary>
        /// خطا در ثبت رویداد دریافت تردد های انلاین
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorOnRegEvent = 10002077,
        /// <summary>
        /// خطا در ریست کردم مانیتورینگ ZK
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorOnResetOnlineMonitoringBecauseOfLockIsTaken = 10002078,
        /// <summary>
        /// خطا در ریست کردم مانیتورینگ ZK به دلیل عدم وجود پینگ
        /// </summary>
        [EnumMember]
        CommunicationStatusZkErrorOnResetOnlineMonitoringBecauseNoPing = 10002078,




        /// <summary>
        // خطا در اتصال
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorCommunication = 10002501,
        /// <summary>
        /// خطا در نوشتن داده ها
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorWriteFail = 10002502,
        /// <summary>
        /// خطا در خواندن داده ها
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorReadFail = 10002503,
        /// <summary>
        /// خطای تنظیمات
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorInvalidParam = 10002504,
        /// <summary>
        /// داده مورد نظر وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorCarryOut = 10002505,
        /// <summary>
        /// اتمام رکورد ها
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorLogEnd = 10002506,
        /// <summary>
        /// خطای حافظه
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorMemory = 10002507,
        /// <summary>
        /// خطای چند کاربری
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorMultiUser = 10002508,
        /// <summary>
        /// خطای ناشناخته در ازتباط با دستگاه های ویردی
        /// </summary>
        CommunicationStatusTimyUnknownError = 10002509,
        /// <summary>
        /// طول چهره VisibleLight بیش از جد مجاز است
        /// </summary>
        [EnumMember]
        CommunicationStatusTimyErrorVisibleLightImageLengthIsTooLong = 10002510,






        [EnumMember]
        CommunicationStatusPwAuthenticationFailed = 10003000,
        [EnumMember]
        CommunicationStatusPwAuthenticationMaxTries = 10003001,
        [EnumMember]
        CommunicationStatusPwNoRecordToReadout = 10003002,
        [EnumMember]
        CommunicationStatusPwLinkTo = 10003003,
        [EnumMember]
        CommunicationStatusPwLinkError = 10003004,
        [EnumMember]
        CommunicationStatusPwBaudSet = 10003005,
        [EnumMember]
        CommunicationStatusPwBadModemId = 10003006,
        [EnumMember]
        CommunicationStatusPwBonesCreate = 10003007,
        [EnumMember]
        CommunicationStatusPwBonesOpen = 10003008,
        [EnumMember]
        CommunicationStatusPwBonesWrite = 10003009,
        [EnumMember]
        CommunicationStatusPwNowEqualLen = 10003010,
        [EnumMember]
        CommunicationStatusPwNoRecords = 10003011,
        [EnumMember]
        CommunicationStatusPwTextCreate = 10003012,
        [EnumMember]
        CommunicationStatusPwFileCreate = 10003013,
        [EnumMember]
        CommunicationStatusPwBadSub = 10003014,
        [EnumMember]
        CommunicationStatusPwBadPointers = 10003015,
        [EnumMember]
        CommunicationStatusPwFileOpen = 10003016,
        [EnumMember]
        CommunicationStatusPwTextFile = 10003017,
        [EnumMember]
        CommunicationStatusPwMoreRecords = 10003018,
        [EnumMember]
        CommunicationStatusPwSendData = 10003019,
        [EnumMember]
        CommunicationStatusPwSocketGet = 10003020,
        [EnumMember]
        CommunicationStatusPwSocketSend = 10003021,
        [EnumMember]
        CommunicationStatusPwBadDevice = 10003022,
        [EnumMember]
        CommunicationStatusPwOldVersion = 10003023,
        [EnumMember]
        CommunicationStatusPwGetDaysFailed = 10003024,
        [EnumMember]
        CommunicationStatusPwDataLen = 10003025,
        [EnumMember]
        CommunicationStatusPwSaveInStruct = 10003026,
        [EnumMember]
        CommunicationStatusPwBadCoolLen = 10003027,
        [EnumMember]
        CommunicationStatusPwEmptyCoolDisk = 10003028,
        [EnumMember]
        CommunicationStatusPwBadCoolDisk = 10003029,
        [EnumMember]
        CommunicationStatusPwMediaOpenError = 10003030,
        [EnumMember]
        CommunicationStatusPwMediaCloseError = 10003031,
        [EnumMember]
        CommunicationStatusPwBadSequenceValue = 10003032,
        [EnumMember]
        CommunicationStatusPwBadFileLen = 10003033,
        [EnumMember]
        CommunicationStatusPwRegistered = 10003034,
        [EnumMember]
        CommunicationStatusPwBadTemplateLen = 10003035,
        [EnumMember]
        CommunicationStatusPwInvalidFile = 10003036,
        [EnumMember]
        CommunicationStatusPwNotRegistered = 10003037,
        [EnumMember]
        CommunicationStatusPwBadRepeatSecs = 10003038,
        [EnumMember]
        CommunicationStatusPwNoITemToSend = 10003039,
        [EnumMember]
        CommunicationStatusPwBadGrpCode = 10003040,
        [EnumMember]
        CommunicationStatusPwInvalidId = 10003041,
        [EnumMember]
        CommunicationStatusPwInvalidIndex = 10003042,
        [EnumMember]
        CommunicationStatusPwReadingData = 10003043,
        [EnumMember]
        CommunicationStatusPwMvTimeout = 10003044,
        [EnumMember]
        CommunicationStatusPwFileChecksum = 10003045,
        [EnumMember]
        CommunicationStatusPwFileNameLen = 10003046,
        [EnumMember]
        CommunicationStatusPwFileNumberLen = 10003047,
        [EnumMember]
        CommunicationStatusPwFileCheckLen = 10003048,
        [EnumMember]
        CommunicationStatusPwFileBadAmount = 10003049,
        [EnumMember]
        CommunicationStatusPwNotEqualCheck = 10003050,
        [EnumMember]
        CommunicationStatusPwNotEqualFNumber = 10003051,
        [EnumMember]
        CommunicationStatusPwPoorSecurity = 10003052,
        [EnumMember]
        CommunicationStatusPwAuthenticationNeeded = 10003053,
        [EnumMember]
        CommunicationStatusPwChangeDefaultKey = 10003054,
        [EnumMember]
        CommunicationStatusPwAuthenticationNeedPass = 10003055,
        [EnumMember]
        CommunicationStatusPwInvalidKeyAmount = 10003056,
        [EnumMember]
        CommunicationStatusPwFileNotExist = 10003057,
        [EnumMember]
        CommunicationStatusPwKeyLengthIsNotValid = 10003058,
        [EnumMember]
        CommunicationStatusPwUserNotFound = 10003059,


        [EnumMember]
        CommunicationStatusVirdiErrorInvalidClientId = 10003500,


        [EnumMember]
        CommunicationStatusTimyCannotEnableDevice = 10004000,
        [EnumMember]
        CommunicationStatusTimyCannotDisableDevice = 10004001,




        [EnumMember]
        CommunicationStatusSettingErrorEndPointUrlStatusEndPointUrlNumberIsNotValid = 10005000,
        [EnumMember]
        CommunicationStatusSettingErrorEndPointUrlStatusEndPointUrlNotFoundInDatabase = 10005001,



        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceHookTimerIntervalIsNotValid = 10006000,
        [EnumMember]
        SystemConfigStatusHardwareConfigAutomaticCollectAttendanceTimerIntervalIsNotValid = 10006001,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineMonitoringDevicesIntervalFromLastDataToResetIsNotValid = 10006002,
        [EnumMember]
        SystemConfigStatusHardwareConfigGuardianServiceUrlIsNotValid = 10006004,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceHookTimerRecordCountIsNotValid = 10006005,
        [EnumMember]
        SystemConfigStatusHardwareConfigZkPushServerPortIsNotValid = 10006006,
        [EnumMember]
        SystemConfigStatusHardwareConfigZkPushServerIpIsNotValid = 10006007,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk1ServerMaxConnectionIsNotValid = 10006008,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk1ServerPortIsNotValid = 10006009,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk2ServerPortIsNotValid = 10006011,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk2ServerReconnectTimerIntervalIsNotValid = 10006012,
        [EnumMember]
        SystemConfigStatusHardwareConfigResetRetryCountDayBeforeIsNotValid = 10006013,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineDeviceTimerIntervalIsNotValid = 10006014,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceRegisterIntervalIsNotValid = 10006015,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForZkOtherCommandIsNotValid = 10006016,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForZkUserCommandsIsNotValid = 10006017,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxZkCommandCountIsNotValid = 10006018,
        [EnumMember]
        SystemConfigStatusHardwareConfigVirdiDeadlineInMinutesIsNotValid = 10006020,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForVirdiUserCommandsIsNotValid = 10006021,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForVirdiOtherCommandsIsNotValid = 10006022,
        [EnumMember]
        SystemConfigStatusHardwareConfigPwDeadlineInMinutesIsNotValid = 10006024,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForPwUserCommandsIsNotValid = 10006025,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForPwOtherCommandsIsNotValid = 10006026,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk1DeadlineInMinutesIsNotValid = 10006028,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForSupremaSdk1UserCommandsIsNotValid = 10006029,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForSupremaSdk1OtherCommandsIsNotValid = 10006030,
        [EnumMember]
        SystemConfigStatusHardwareConfigSupremaSdk2DeadlineInMinutesIsNotValid = 10006032,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForSupremaSdk2UserCommandsIsNotValid = 10006033,
        [EnumMember]
        SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForSupremaSdk2OtherCommandsIsNotValid = 10006034,
        [EnumMember]
        SystemConfigStatusHardwareConfigZkDeadlineInMinutesIsNotValid = 10006035,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineMonitoringDevicesSleepAfterPingInSecondIsNotValid = 10006036,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineMonitoringDevicesPingTimeoutInMillisecondIsNotValid = 10006037,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecondIsNotValid = 10006038,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceSendToGuardianTimerIntervalIsNotValid = 10006039,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceSendToGuardianTimerRecordCountIsNotValid = 10006040,
        [EnumMember]
        SystemConfigStatusHardwareConfigPadisMetalDetectorGateServerPushPortIsNotValid = 10006041,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForParkingIsNotValid = 10006042,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForTimeAttendanceIsNotValid = 10006043,
        [EnumMember]
        SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForAccessControlIsNotValid = 10006044,
        [EnumMember]
        SystemConfigStatusHardwareConfigOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes = 10006002,

        /// <summary>
        /// در بازه مورد نظر تعطیلی یافت نشده است
        /// </summary>
        [EnumMember]
        CommunicationStatusAccessControlNoHolidayFound = 10007000,

        /// <summary>
        /// تعداد بازه های زمانی پرسنل بیش از تعداد مجاز این دستگاه است
        /// </summary>
        [EnumMember]
        CommunicationStatusAccessControlInvalidTimeZoneCount = 10007001,

        /// <summary>
        /// تاریخ پایان استخدام کاربر گذشته است
        /// </summary>
        [EnumMember]
        CommunicationStatusUserEndTimeIsInPast = 10007002,

        /// <summary>
        /// امکان گرفتن عکس وجود ندارد
        /// </summary>
        [EnumMember]
        CommunicationStatusCannotTakePhoto = 10009000,


        [EnumMember]
        CommunicationStatusPadisControllerErrorCommunicationSecurityDataIsNotValid = 10010001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCommunicationAccessDenied = 10010002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCommunicationResourceNotFound = 10010003,
        [EnumMember]
        CommunicationStatusPadisControllerResponseIsNotValid = 10010004,
        [EnumMember]
        CommunicationStatusPadisControllerErrorIpIsNotValid = 10010005,
        [EnumMember]
        CommunicationStatusPadisControllerErrorTcpPortIsNotValid = 10010006,

        [EnumMember]
        CommunicationStatusPadisControllerErrorValidationError = 10011007,
        [EnumMember]
        CommunicationStatusPadisControllerErrorMissingRequiredField = 10011008,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidEpochValue = 1001109,
        [EnumMember]
        CommunicationStatusPadisControllerErrorDateTimeFieldConflict = 10011010,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidDateTimeFormat = 10011011,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidInput = 10011012,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidInputFormat = 10011013,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidInputRange = 10011014,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidInputType = 10011015,
        [EnumMember]
        CommunicationStatusPadisControllerErrorMissingStartTime = 10011016,
        [EnumMember]
        CommunicationStatusPadisControllerErrorMissingEndTime = 10011017,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidTimeRange = 10011018,

        // DateTime Setting Errors (2000-2099)
        [EnumMember]
        CommunicationStatusPadisControllerErrorPlatformNotSupported = 10012000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRtcSetFailed = 10012001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRtcSyncFailed = 10012002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorSubprocessTimeout = 10012003,
        [EnumMember]
        CommunicationStatusPadisControllerErrorSudoPermissionDenied = 10012004,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInternalServerError = 10012005,

        // System Reboot Errors (2100-2199)
        [EnumMember]
        CommunicationStatusPadisControllerErrorRebootFailed = 10013000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRebootCommandError = 10013001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRebootTimeout = 10013002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRebootPermissionDenied = 10013003,

        // Device Info Errors (2200-2299)
        [EnumMember]
        CommunicationStatusPadisControllerErrorDeviceInfoFailed = 10014000,

        // System Health Errors (2300-2399)
        [EnumMember]
        CommunicationStatusPadisControllerErrorSystemHealthFailed = 10015000,

        // User Management Errors (3000-3099)
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserNotFound = 10016000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserAlreadyExists = 10016001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserCreateFailed = 10016002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserUpdateFailed = 10016003,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserDeleteFailed = 10016004,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserIdRequired = 10016005,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserInvalidType = 10016006,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserAccessDenied = 10016007,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserClearAllFailed = 10016008,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserAccountExpired = 10016009,
        [EnumMember]
        CommunicationStatusPadisControllerErrorUserAccountNotActiveYet = 10016010,

        // Card Management Errors (3100-3199)
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardNotFound = 10017000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardAlreadyExists = 10017001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardAddFailed = 10017002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardDeleteFailed = 10017003,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardInvalidNumber = 10017004,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardUserIdRequired = 10017005,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCardNumberRequired = 10017006,
        
        // Authentication & Authorization Errors (4000-4099)
        [EnumMember]
        CommunicationStatusPadisControllerErrorAuthenticationRequired = 10018000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidCredentials = 10018001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorAccessDenied = 10018002,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInsufficientPermissions = 10018003,
        [EnumMember]
        CommunicationStatusPadisControllerErrorInvalidUserType = 10018004,
        [EnumMember]
        CommunicationStatusPadisControllerErrorAccountExpired = 10018005,
        [EnumMember]
        CommunicationStatusPadisControllerErrorAccountNotActive = 10018006,
        [EnumMember]
        CommunicationStatusPadisControllerErrorRestApiAccessOnly = 10018007,

        // Access
        [EnumMember]
        CommunicationStatusPadisControllerErrorDoorIdNotFound = 10019000,
        [EnumMember]
        CommunicationStatusPadisControllerErrorCalendarNotFound = 10019001,
        [EnumMember]
        CommunicationStatusPadisControllerErrorAccessLevelNotFound = 10019002,

    }
}

// ReSharper restore InconsistentNaming
