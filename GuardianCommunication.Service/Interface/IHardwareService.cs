using GuardianCommunication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.OperationResult;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace GuardianCommunication.Service
{

    [ServiceContract(Name = "IGuardianCommunication", Namespace = "http://www.emdad.com/IGuardianCommunicationService")]
    public interface IGuardianCommunication
    {

        #region  Communication Service


        #region Normal Communication


        #region Bulk Operation

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "EnrollUserWithTemplateBulk")]
        List<UserAndDeviceResultModel> CommunicationEnrollUserWithTemplateBulk(UserAndDeviceListModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserBulk")]
        List<UserAndDeviceResultModel> CommunicationDeleteUserBulk(UserAndDeviceListModel param);

        #endregion


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "RebootDevice")]
        void CommunicationRebootDevice(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "EnrollUserWithTemplate")]
        void CommunicationEnrollUserWithTemplate(UserAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendUser")]
        void CommunicationSendUser(UserAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetUserById")]
        UserModel CommunicationGetUserById(GetUserByIdModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetUsersInfoDefinedOnDevice")]
        List<UserInfoDefinedOnDeviceModel> CommunicationGetUsersInfoDefinedOnDevice(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetAttendance")]
        List<DeviceAttendanceModel> CommunicationGetAttendance(GetAttendanceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "Readout")]
        List<DeviceAttendanceModel> CommunicationReadout(ReadoutModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ReadoutFromDevice")]
        List<DeviceAttendanceModel> CommunicationReadoutFromDevice(ReadoutFromDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetFirmwareVersion")]
        string CommunicationGetFirmwareVersion(IdSingleModel model);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "UpgradeFirmware")]
        void CommunicationUpgradeFirmware(UpdateFirmwareModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SetDateAndTime")]
        void CommunicationSetDateAndTime(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetDateAndTime")]
        DateTime CommunicationGetDateAndTime(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ClearData")]
        void CommunicationClearData(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "RecordCount")]
        int CommunicationRecordCount(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "FaceCount")]
        int CommunicationFaceCount(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "FingerCount")]
        int CommunicationFingerCount(IdSingleModel model);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetSerialNumber")]
        string CommunicationGetSerialNumber(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "UserCount")]
        int CommunicationUserCount(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserByUserInfo")]
        void CommunicationDeleteUserByUserInfo(UserAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserByUserId")]
        void CommunicationDeleteUserByUserId(UserIdOnDeviceAndDeviceIdModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteAllUsers")]
        void CommunicationDeleteAllUsers(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "TestConnection")]
        OperationResultEnumeration CommunicationTestConnection(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetDeviceStatistics")]
        DeviceStatisticsModel CommunicationGetDeviceStatistics(IdSingleModel model);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanCard")]
        string CommunicationScanCard(UserAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFinger")]
        UserFingerModel CommunicationScanFinger(ScanFingerModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFace")]
        UserFaceModel CommunicationScanFace(UserAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFaceStandalone")]
        UserFaceModel CommunicationScanFaceStandalone(UserAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanIris")]
        UserIrisModel CommunicationScanIris(UserAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetAttendanceImage")]
        string GetAttendanceImage(GetAttendanceImageModel param);

        #endregion


        #region Access Control

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenCabinetDoor")]
        void CommunicationOpenCabinetDoor(OpenCabinetDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenDoor")]
        void CommunicationOpenDoor(OpenDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenDoorWithDelay")]
        void CommunicationOpenDoorWithDelay(OpenDoorModel param);


        #region Guardian Controller

        /// <summary>
        /// ارسال تقویم به برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetCalendar")]
        void GuardianControllerSetCalendar(GuardianControllerDeviceAndCalendarModel param);

        /// <summary>
        /// ارسال قالب تقویم (چرخه چندروزه) به برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetCalendarTemplate")]
        void GuardianControllerSetCalendarTemplate(GuardianControllerDeviceAndCalendarTemplateModel param);

        /// <summary>
        /// ثبت یا به روزرسانی جداگانه دسترسی های آسان روی برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetEasyAccess")]
        void GuardianControllerSetEasyAccess(GuardianControllerDeviceAndEasyAccessesModel param);

        /// <summary>
        /// حذف جداگانه دسترسی های آسان از برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerDeleteEasyAccess")]
        void GuardianControllerDeleteEasyAccess(GuardianControllerDeviceAndEasyAccessesModel param);

        /// <summary>
        /// ارسال سطح دسترسی به برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetAccessLevel")]
        void GuardianControllerSetAccessLevel(GuardianControllerDeviceAndAccessLevelModel param);

        /// <summary>
        /// ارسال گروه دسترسی به برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetAccessGroup")]
        void GuardianControllerSetAccessGroup(GuardianControllerDeviceAndAccessGroupModel param);

        /// <summary>
        /// ارسال پیکربندی سخت‌افزار به برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerSetHardwareConfig")]
        void GuardianControllerSetHardwareConfig(GuardianControllerDeviceAndHardwareConfigModel param);

        /// <summary>
        /// حذف قالب‌های تقویم از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerDeleteCalendarTemplate")]
        void GuardianControllerDeleteCalendarTemplate(GuardianControllerDeviceAndIdsModel param);

        /// <summary>
        /// حذف تقویم‌ها از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerDeleteCalendar")]
        void GuardianControllerDeleteCalendar(GuardianControllerDeviceAndIdsModel param);

        /// <summary>
        /// حذف سطح‌های دسترسی از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerDeleteAccessLevel")]
        void GuardianControllerDeleteAccessLevel(GuardianControllerDeviceAndIdsModel param);

        /// <summary>
        /// حذف گروه‌های دسترسی از برد کنترلر گاردین (با شناسه)
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerDeleteAccessGroup")]
        void GuardianControllerDeleteAccessGroup(GuardianControllerDeviceAndIdsModel param);

        /// <summary>
        /// پایان همگام‌سازی کامل برد کنترلر گاردین: برد هر چیزی را که در فهرست‌ها نیست حذف می‌کند
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GuardianControllerKeepOnly")]
        void GuardianControllerKeepOnly(GuardianControllerDeviceAndKeepOnlyModel param);

        /// <summary>
        /// باز کردن دائم درب برد کنترلر گاردین
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenGuardianControllerDoorPermanent")]
        void CommunicationOpenGuardianControllerDoorPermanent(OpenDoorModel param);

        /// <summary>
        /// برگرداندن درب برد کنترلر گاردین از حالت باز یا بسته دائم به حالت عادی
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationCloseGuardianControllerDoorPermanent")]
        void CommunicationCloseGuardianControllerDoorPermanent(OpenDoorModel param);

        /// <summary>
        /// بسته نگه داشتن دائم درب برد کنترلر گاردین (حتی با تردد مجاز باز نمی‌شود)
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationLockGuardianControllerDoorPermanent")]
        void CommunicationLockGuardianControllerDoorPermanent(OpenDoorModel param);

        #endregion


        #region Suprema SDK 1


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk1DoorPermanent")]
        void CommunicationOpenSupremaSdk1DoorPermanent(OpenDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationCloseSupremaSdk1DoorPermanent")]
        void CommunicationCloseSupremaSdk1DoorPermanent(OpenDoorModel param);


        #endregion


        #region Suprema SDK 2

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk2DoorPermanent")]
        void CommunicationOpenSupremaSdk2DoorPermanent(OpenDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationCloseSupremaSdk2DoorPermanent")]
        void CommunicationCloseSupremaSdk2DoorPermanent(OpenDoorModel param);


        #endregion


        #endregion


        #endregion


        #region Cache Reset

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ResetDeviceCache")]
        bool ResetDeviceCache();

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ResetControllerDeviceDoorCache")]
        bool ResetDeviceDoorCache();

        #endregion


        #region Health Check


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "/guardian-hw-service-health-check")]
        void HealthCheck();


        #endregion

    }

}
