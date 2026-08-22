
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Shared.Definition
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class ServiceConstants
    {
        public const string WcfNamsepace = @"http://www.emdad.com/";

        //public const string EncrptionKey = "Emdad@MoVj$1399@";
        //public static readonly byte[] EncrptionSalt = Encoding.ASCII.GetBytes("o6806642kbM7c5");


        public const int GeneralMaxValueForSecond = 59;
        public const int GeneralMinValueForSecond = 0;


        public const int GeneralMaxValueForMinute = 59;
        public const int GeneralMinValueForMinute = 0;

        public const int GeneralDayDurationInMinute = 1440;

        public const int GeneralStartOfDayTotalMinute = 0;
        public const int GeneralStartOfDayTimeDuration = 0;
        public const int GeneralEndOfDayTotalMinute = 1439;
        public const int GeneralEndOfDayTimeDuration = 2359;


        public const int GeneralMinValueForPort = ushort.MinValue;
        public const int GeneralMaxValueForPort = ushort.MaxValue;

        public const int ServerDelayStart = 20;

        public const string LogSource = "Padis Communication";
        public const string LogNameHookError = "Hook Error";
        public const string NationalPlateRegex = @"(^\d+)\s*(\D+)\s*(\d+)\s*\D+\s*(\d+$)";

        public const int GeneralMaxCountForUseInString = 100;
        public const int GeneralMaxCountForUseParsString = 300;
        public const int GeneralMaxCountForUseOpenJson = 2000;


        #region Database Names

        public const string RowNumberColumnName = " RowNumber  ";

        #endregion


        #region Auth

        public const string ApiAuthorizationHeader = "Authorization";

        #endregion


        #region Device and Communication

        public const string NotSupportedCommandText = "NOT SUPPORTED";
        public const string UserNotFoundCommandText = "USER NOT DEFINED IN DEVICE";

        public const int DevicePw1600DeviceNumber = 23;
        public const string DeviceDefaultValueForPw1600CommunicationPassword = "@ABCDEFGHIJKLMNO";


        public const int UnlockDoorTime = 5;

        #endregion


        #region Comminucation

        public const string DevicePersonalImageFolder = "\\Device\\PersonalImages";
        public const string DeviceVisibleLightImageFolder = "\\Device\\VisibleLightImage";

        public const int MaxReadoutDays = 7;



        #endregion


        #region Hardware Service Config

        public const int HardwareServiceMaxZkCommands = 20;

        public const int HardwareServiceMinValueForAttendanceSendToGuardianTimerInterval = 5;
        public const int HardwareServiceMaxValueForAttendanceSendToGuardianTimerInterval = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceHookTimerInterval = 5;
        public const int HardwareServiceMaxValueForAttendanceHookTimerInterval = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceSendToGuardianTimerRecordCount = 5;
        public const int HardwareServiceMaxValueForAttendanceSendToGuardianTimerRecordCount = 2000;

        public const int HardwareServiceMinValueForAttendanceHookTimerRecordCount = 5;
        public const int HardwareServiceMaxValueForAttendanceHookTimerRecordCount = 2000;

        public const int HardwareServiceMinValueForAutomaticCollectAttendanceTimerInterval = 10;
        public const int HardwareServiceMaxValueForAutomaticCollectAttendanceTimerInterval = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineMonitoringDevicesIntervalFromLastDataToReset = 5;
        public const int HardwareServiceMaxValueForOnlineMonitoringDevicesIntervalFromLastDataToReset = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes = 5;
        public const int HardwareServiceMaxValueForOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineMonitoringDevicesSleepAfterPingInSecond = 0;
        public const int HardwareServiceMaxValueForOnlineMonitoringDevicesSleepAfterPingInSecond = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = 5;
        public const int HardwareServiceMaxValueForOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineMonitoringDevicesPingTimeoutInMillisecond = 100;
        public const int HardwareServiceMaxValueForOnlineMonitoringDevicesPingTimeoutInMillisecond = int.MaxValue;

        public const int HardwareServiceMinValueForSupremaSdk1ServerMaxConnection = 1;
        public const int HardwareServiceMaxValueForSupremaSdk1ServerMaxConnection = 1000;


        public const int HardwareServiceMinValueForSupremaSdk2ServerReconnectTimerInterval = 1;
        public const int HardwareServiceMaxValueForSupremaSdk2ServerReconnectTimerInterval = int.MaxValue;

        public const int HardwareServiceMinValueForResetRetryCountDayBefore = 1;
        public const int HardwareServiceMaxValueForResetRetryCountDayBefore = int.MaxValue;

        public const int HardwareServiceMinValueForOnlineDeviceTimerInterval = 10;
        public const int HardwareServiceMaxValueForOnlineDeviceTimerInterval = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceRegisterInterval = 0;
        public const int HardwareServiceMaxValueForAttendanceRegisterInterval = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceRegisterIntervalForParking = 0;
        public const int HardwareServiceMaxValueForAttendanceRegisterIntervalForParking = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceRegisterIntervalForTimeAttendance = 0;
        public const int HardwareServiceMaxValueForAttendanceRegisterIntervalForTimeAttendance = int.MaxValue;

        public const int HardwareServiceMinValueForAttendanceRegisterIntervalForAccessControl = 0;
        public const int HardwareServiceMaxValueForAttendanceRegisterIntervalForAccessControl = int.MaxValue;

        public const int HardwareServiceMinValueForMaxZkCommandCount = 0;
        public const int HardwareServiceMaxValueForMaxZkCommandCount = 20;

        public const int HardwareServiceMinValueForMaxRetryForZkOtherCommand = 0;
        public const int HardwareServiceMaxValueForMaxRetryForZkOtherCommand = 100;

        public const int HardwareServiceMinValueForMaxRetryForZkUserCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForZkUserCommands = 300;

        public const int HardwareServiceMinValueForZkDeadlineInMinutes = 0;
        public const int HardwareServiceMaxValueForZkDeadlineInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForVirdiDeadlineInMinutes = 0;
        public const int HardwareServiceMaxValueForVirdiDeadlineInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForMaxRetryForVirdiUserCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForVirdiUserCommands = 20;

        public const int HardwareServiceMinValueForMaxRetryForVirdiOtherCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForVirdiOtherCommands = 10;


        public const int HardwareServiceMinValueForPwDeadlineInMinutes = 0;
        public const int HardwareServiceMaxValueForPwDeadlineInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForMaxRetryForPwUserCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForPwUserCommands = 20;

        public const int HardwareServiceMinValueForMaxRetryForPwOtherCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForPwOtherCommands = 10;


        public const int HardwareServiceMinValueForSupremaSdk1DeadlineInMinutes = 0;
        public const int HardwareServiceMaxValueForSupremaSdk1DeadlineInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForMaxRetryForSupremaSdk1UserCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForSupremaSdk1UserCommands = 20;

        public const int HardwareServiceMinValueForMaxRetryForSupremaSdk1OtherCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForSupremaSdk1OtherCommands = 10;


        public const int HardwareServiceMinValueForSupremaSdk2DeadlineInMinutes = 0;
        public const int HardwareServiceMaxValueForSupremaSdk2DeadlineInMinutes = int.MaxValue;

        public const int HardwareServiceMinValueForMaxRetryForSupremaSdk2UserCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForSupremaSdk2UserCommands = 20;

        public const int HardwareServiceMinValueForMaxRetryForSupremaSdk2OtherCommands = 0;
        public const int HardwareServiceMaxValueForMaxRetryForSupremaSdk2OtherCommands = 10;



        #endregion

    }
}
