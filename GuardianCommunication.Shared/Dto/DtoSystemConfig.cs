using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Dto
{
    [DataContract]
    public class DtoSystemConfig
    {

        public string KarnamaAppUsername { get; set; }
        public string KarnamaAppPassword { get; set; }
        public string KarnamaAuthorizationToken { get; set; }
        public string KarnamaServiceUrl { get; set; }
        public int AutomaticCollectAttendanceTimerInterval { get; set; }
        public int OnlineMonitoringDevicesIntervalFromLastDataToReset { get; set; }
        public int OnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes { get; set; }
        public int OnlineMonitoringDevicesSleepAfterPingInSecond { get; set; }
        public int OnlineMonitoringDevicesPingTimeoutInMillisecond { get; set; }
        public int OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond { get; set; }
        public bool IsNetworkPingActive { get; set; }
        public int SleepBetweenSendsIfCommandNotExistsInMilliSeconds { get; set; }
        public int ResetRetryCountDayBefore { get; set; }
        public int OnlineDeviceTimerInterval { get; set; }
        public int DeleteUnsentCommandsIntervalInDays { get; set; }
        public int DeleteUnsentCommandsTimerIntervalInHours { get; set; }
        public bool DeleteUnsentCommandsJustDeleteFailed { get; set; }
        public bool KeepCommandAfterResponse { get; set; }
        public bool IsDeleteFailedCommandsActive { get; set; }


        #region Attendance

        public int AttendanceSendToKarnamaTimerInterval { get; set; }
        public int AttendanceHookTimerInterval { get; set; }
        public int AttendanceSendToKarnamaTimerRecordCount { get; set; }
        public int AttendanceHookTimerRecordCount { get; set; }
        public bool AttendanceSaveKarnamaSendResult { get; set; }
        public int AttendanceRegisterInterval { get; set; }
        public int AttendanceRegisterIntervalForParking { get; set; }
        public int AttendanceRegisterIntervalForTimeAttendance { get; set; }
        public int AttendanceRegisterIntervalForAccessControl { get; set; }


        #endregion

        #region Self

        public int SelfTimerIntervalForSendMealsToDeviceInMinute { get; set; }
        public int SelfOffsetForFutureMealsInMinute { get; set; }
        public int SelfPrinterPingTimeoutInSecond { get; set; }
        public int SelfPrinterSleepAfterPingCircleInMillisecond { get; set; }
        public int SelfPrinterSleepWhenQueueIsEmptyInMillisecond { get; set; }
        public int SelfPrinterPrinterPerQueue { get; set; }
        public bool SelfSendSingleFoodTitle { get; set; }

        #endregion

        #region ZK

        public string ZkPushServerIp { get; set; }
        public int ZkPushServerPort { get; set; }
        public int MaxZkCommandCount { get; set; }
        public int MaxRetryForZkOtherCommand { get; set; }
        public int MaxRetryForZkUserCommands { get; set; }
        public int ZkDeadlineInMinutes { get; set; }
        public int ZkWaitInCommandLoopInMilliseconds { get; set; }
        public int ZkPushSleepOnContinueInMillisecond { get; set; }
        public int ZkPushSleepBetweenSocketsInMillisecond { get; set; }
        public int ZkPushReceiveTimeoutInMillisecond { get; set; }
        public int ZkPushStamp { get; set; }
        public int ZkPushOpStamp { get; set; }
        public int ZkPushPhotoStamp { get; set; }
        public int ZkPushErrorDelay { get; set; }
        public int ZkPushDelay { get; set; }
        public string ZkPushTransTimes { get; set; }
        public int ZkPushTransInterval { get; set; }
        public int ZkPushSyncTime { get; set; }
        public int ZkPushRealtime { get; set; }
        public int ZkPushAttendanceLogStamp { get; set; }
        public int ZkPushOperationLogStamp { get; set; }
        public int ZkPushAttendancePhotoStamp { get; set; }
        public string ZkPushMultiBioDataSupport { get; set; }
        public string ZkPushMultiBioPhotoSupport { get; set; }
        public int ZkPushWaitForSocketData { get; set; }
        public int ZkIntervalForConsiderDeviceOnlineInSecond { get; set; }
        public int ZkPushGetCommandTimerIntervalInMillisecond { get; set; }

        #endregion

        #region PW

        public int PwDeadlineInMinutes { get; set; }
        public int MaxRetryForPwUserCommands { get; set; }
        public int MaxRetryForPwOtherCommands { get; set; }
        public int PwWaitInCommandLoopInMilliseconds { get; set; }
        public int PwSleepTimeInAutoCollectInSeconds { get; set; }
        public int PwSleepTimeBeforeClearDataInAutoCollectInSeconds { get; set; }

        #endregion

        #region Suprema SDK 1

        public int SupremaSdk1ServerMaxConnection { get; set; }
        public int SupremaSdk1ServerPort { get; set; }
        public int SupremaSdk1DeadlineInMinutes { get; set; }
        public int MaxRetryForSupremaSdk1UserCommands { get; set; }
        public int MaxRetryForSupremaSdk1OtherCommands { get; set; }
        public int SupremaSdk1WaitInCommandLoopInMilliseconds { get; set; }
        public int SupremaSdk1ServerCheckConnectionTimerIntervalInSeconds { get; set; }

        #endregion

        #region Suprema SDK 2

        public int SupremaSdk2ServerPort { get; set; }
        public int SupremaSdk2ServerReconnectTimerInterval { get; set; }
        public int SupremaSdk2DeadlineInMinutes { get; set; }
        public int MaxRetryForSupremaSdk2UserCommands { get; set; }
        public int MaxRetryForSupremaSdk2OtherCommands { get; set; }
        public int SupremaSdk2WaitInCommandLoopInMilliseconds { get; set; }

        #endregion

        #region Virdi

        public int VirdiDeadlineInMinutes { get; set; }
        public int MaxRetryForVirdiUserCommands { get; set; }
        public int MaxRetryForVirdiOtherCommands { get; set; }
        public int VirdiWaitInCommandLoopInMilliseconds { get; set; }
        public int VirdiServerPort { get; set; }
        public int VirdiSyncOperationTimeoutInMilliseconds { get; set; }
        public int VirdiMaxVisibleLightImageSizeInKb { get; set; }
        public int VirdiMaxVisibleLightImageSizeWidth { get; set; }
        public int VirdiMaxVisibleLightImageSizeHeight { get; set; }

        #endregion

        #region Padis Metal Detector Cache

        public int PadisMetalDetectorGateServerPushPort { get; set; }
        public string PadisMetalDetectorGateServerPushIp { get; set; }


        #endregion

        #region Karabin Camera Interval

        public int KarabinCameraIntervalForSendAccessListInSecond { get; set; }
        public int KarabinCameraAutoCollectIntervalInSecond { get; set; }
        public string KarabinCameraAccessFileBasePath { get; set; }

        #endregion

        #region XRay

        public int XRayIntervalToRetrySendInSecond { get; set; }
        public int XRaySleepAfterNoFileInMilliSecond { get; set; }
        public int XRayWaitBeforeAddToQueueInMilliSecond { get; set; }

        #endregion

        #region Camera

        public int FaceDetectionCameraMaxRetryForUserCommand { get; set; }
        public int FaceDetectionCameraMaxRetryForOtherCommand { get; set; }


        #endregion

        #region Data moon face detection camera

        public string FaceDetectionDataMoonServerBaseAddress { get; set; }
        public string FaceDetectionDataMoonUsername { get; set; }
        public string FaceDetectionDataMoonPassword { get; set; }
        public int FaceDetectionDataMoonSleepBetweenSendsIfCommandNotExistsInMilliSeconds { get; set; }
        public int FaceDetectionDataMoonSleepAfterACommandLoopInMilliSeconds { get; set; }
        public int FaceDetectionDataMoonWaitBetweenCommandSendInMilliseconds { get; set; }
        public int FaceDetectionDataMoonCommandCountInEachFetch { get; set; }


        #endregion
        
        #region Padis face detection camera

        public string FaceDetectionPadisServerBaseAddress { get; set; }
        public string FaceDetectionPadisUsername { get; set; }
        public string FaceDetectionPadisPassword { get; set; }
        public int FaceDetectionPadisSleepBetweenSendsIfCommandNotExistsInMilliSeconds { get; set; }
        public int FaceDetectionPadisSleepAfterACommandLoopInMilliSeconds { get; set; }
        public int FaceDetectionPadisWaitBetweenCommandSendInMilliseconds { get; set; }
        public int FaceDetectionPadisCommandCountInEachFetch { get; set; }

        #endregion

        #region Padis controller

        public string PadisControllerPushServerPushAddress { get; set; }
        public int PadisControllerGetCommandTimerIntervalInMillisecond { get; set; }

        #region Push

        public int PadisControllerPushServerMaxCommandCount { get; set; }
        public int PadisControllerPushServerMaxCommandLength { get; set; }
        public int PadisControllerPushServerIntervalForConsiderDeviceOnlineInSecond { get; set; }

        #endregion


        #region Grpc


        public int PadisControllerGrpcServerCommandCount { get; set; }
        public int PadisControllerGrpcServerPort { get; set; }
        public int PadisControllerGrpcCommandTimerIntervalInMillisecond { get; set; }
        public int PadisControllerGrpcServerTimeoutShortInMillisecond { get; set; }
        public int PadisControllerGrpcServerTimeoutLongInMillisecond { get; set; }
        public int PadisControllerGrpcServerTimeoutVeryLongInMillisecond { get; set; }

        #endregion


        #endregion


        #region Timy

        public int TimyNormalCommandTimeoutInSecond { get; set; }
        public int TimyLongCommandTimeoutInSecond { get; set; }
        public int TimyGetCommandTimerIntervalInMillisecond { get; set; }
        public int TimyWaitBetweenCommandSendInMilliseconds { get; set; }
        public int TimyMaxRetryForOtherCommand { get; set; }
        public int TimyMaxRetryForUserCommand { get; set; }

        #endregion

    }
}
