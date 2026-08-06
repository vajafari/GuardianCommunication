using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace GuardianCommunication.Service
{

    [ServiceContract(Name = "IHardwareService", Namespace = "http://www.emdad.com/IHardwareService")]
    public interface IHardwareService
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
        List<EmployeeAndDeviceResultModel> CommunicationEnrollUserWithTemplateBulk(EmployeeAndDeviceParamsModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserBulk")]
        List<EmployeeAndDeviceResultModel> CommunicationDeleteUserBulk(EmployeeAndDeviceParamsModel param);

        #endregion


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "RebootDevice")]
        void CommunicationRebootDevice(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "EnrollUserWithTemplate")]
        void CommunicationEnrollUserWithTemplate(EmployeeAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendWithoutFingers")]
        void CommunicationSendWithoutFingers(DeviceAndEmployeeListModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SetValidInvalid")]
        void CommunicationSetValidInvalid(DeviceAndEmployeeListModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendUser")]
        void CommunicationSendUser(EmployeeAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetUserById")]
        EmployeeModel CommunicationGetUserById(GetUserByIdModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetUsersInfoDefinedOnDevice")]
        List<UserInfoDefinedOnDeviceModel> CommunicationGetUsersInfoDefinedOnDevice(DeviceCommunicationModel deviceInfo);


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
        string CommunicationGetFirmwareVersion(DeviceCommunicationModel deviceInfo);

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
            , UriTemplate = "CancelOperation")]
        void CommunicationCancelOperation(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SetDateAndTime")]
        void CommunicationSetDateAndTime(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetDateAndTime")]
        double CommunicationGetDateAndTime(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ClearData")]
        void CommunicationClearData(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "RecordCount")]
        int CommunicationRecordCount(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "FaceCount")]
        int CommunicationFaceCount(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "FingerCount")]
        int CommunicationFingerCount(DeviceCommunicationModel deviceInfo);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetSerialNumber")]
        string CommunicationGetSerialNumber(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "UserCount")]
        int CommunicationUserCount(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserByUserInfo")]
        void CommunicationDeleteUserByUserInfo(EmployeeAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteUserByUserId")]
        void CommunicationDeleteUserByUserId(DeviceAndEmployeeNumberListModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeleteAllUsers")]
        void CommunicationDeleteAllUsers(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "TestConnection")]
        OperationResultEnumeration CommunicationTestConnection(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetDeviceStatistics")]
        DeviceStatisticsModel CommunicationGetDeviceStatistics(DeviceCommunicationModel deviceInfo);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanCard")]
        string CommunicationScanCard(EmployeeAndDeviceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFinger")]
        EmployeeFingerModel CommunicationScanFinger(ScanFingerModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFace")]
        EmployeeFaceModel CommunicationScanFace(EmployeeAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanFaceStandalone")]
        EmployeeFaceModel CommunicationScanFaceStandalone(EmployeeAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ScanIris")]
        EmployeeIrisModel CommunicationScanIris(EmployeeAndDeviceModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "Check")]
        void CommunicationCheck(DeviceCommunicationModel deviceInfo);

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


        #region Suprema SDK 1

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk1Holidays")]
        void CommunicationSendSupremaSdk1Holidays(SupremaSdk1SendHolidaysModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk1Timezones")]
        void CommunicationSendSupremaSdk1Timezones(SupremaSdk1SendTimezomesModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk1AccessGroups")]
        void CommunicationSendSupremaSdk1AccessGroups(SupremaSdk1SendAccessGroupModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk1DoorInfo")]
        void CommunicationSendSupremaSdk1DoorInfo(SupremaSdk1DeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk1Door")]
        void CommunicationOpenSupremaSdk1Door(SupremaSdk1DeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk1DoorWithDelay")]
        void CommunicationOpenSupremaSdk1DoorWithDelay(SupremaSdk1DeviceAndDoorWithDelayModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk1DoorPermanent")]
        void CommunicationOpenSupremaSdk1DoorPermanent(SupremaSdk1DeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationCloseSupremaSdk1DoorPermanent")]
        void CommunicationCloseSupremaSdk1DoorPermanent(SupremaSdk1DeviceAndDoorModel param);


        #endregion


        #region Suprema SDK 2


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk2Holidays")]
        void CommunicationSendSupremaSdk2Holidays(SupremaSdk2SendHolidaysModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk2AccessSchedules")]
        void CommunicationSendSupremaSdk2AccessSchedules(SupremaSdk2SendAccessSchedulesModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk2AccessLevels")]
        void CommunicationSendSupremaSdk2AccessLevels(SupremaSdk2SendAccessLevelModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk2AccessGroups")]
        void CommunicationSendSupremaSdk2AccessGroups(SupremaSdk2SendAccessGroupModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendSupremaSdk2DoorInfo")]
        void CommunicationSendSupremaSdk2DoorInfo(SupremaSdk2SendDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk2Door")]
        void CommunicationOpenSupremaSdk2Door(SupremaSdk2DeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk2DoorWithDelay")]
        void CommunicationOpenSupremaSdk2DoorWithDelay(SupremaSdk2DeviceAndDoorWithDelayModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationOpenSupremaSdk2DoorPermanent")]
        void CommunicationOpenSupremaSdk2DoorPermanent(SupremaSdk2DeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "CommunicationCloseSupremaSdk2DoorPermanent")]
        void CommunicationCloseSupremaSdk2DoorPermanent(SupremaSdk2DeviceAndDoorModel param);


        #endregion


        #region ZK


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendHolidays")]
        void CommunicationSendHolidays(SendHolidaysModel param);


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendTimezone")]
        void CommunicationSendTimezone(SendTimezoneModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendUserTimezones")]
        void CommunicationSendUserTimezones(SendUserTimezonesModel param);

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
        void CommunicationOpenDoorWithDelay(OpenDoorWithDelayModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendDoorInfo")]
        void CommunicationSendDoorInfo(DeviceDoorModel param);

        #endregion


        #region Virdi


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendVirdiAccessControlData")]
        void CommunicationSendVirdiAccessControlData(VirdiAccessControlDataModel param);


        #endregion


        #region Timy


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "TimySetDayTimezone")]
        void CommunicationTimySetDayTimezone(TimySetDayTimezoneModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "TimySetWeekTimezone")]
        void CommunicationTimySetWeekTimezone(TimySetWeekTimezoneModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "TimySetHolidays")]
        void CommunicationTimySetHolidays(TimySetHolidaysModel param);


        #endregion


        #region Padis Controller

        
        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenPadisControllerDoor")]
        void CommunicationOpenPadisControllerDoor(PadisControllerDeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenPadisControllerDoorWithDelay")]
        void CommunicationOpenPadisControllerDoorWithDelay(PadisControllerDeviceAndDoorWithDelayModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "OpenPadisControllerDoorPermanent")]
        void CommunicationOpenPadisControllerDoorPermanent(PadisControllerDeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ClosePadisControllerDoorPermanent")]
        void CommunicationClosePadisControllerDoorPermanent(PadisControllerDeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetDoor")]
        void CommunicationPadisControllerSetDoor(PadisControllerDeviceAndDoorModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
             , RequestFormat = WebMessageFormat.Json
             , ResponseFormat = WebMessageFormat.Json
             , BodyStyle = WebMessageBodyStyle.Bare
             , UriTemplate = "PadisControllerSetIoPort")]
        void CommunicationPadisControllerSetIoPort(PadisControllerDeviceAndIoPortModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetWiegand")]
        void CommunicationPadisControllerSetWiegand(PadisControllerDeviceAndWiegandModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetRelay")]
        void CommunicationPadisControllerSetRelay(PadisControllerDeviceAndRelayModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetCalendar")]
        void CommunicationPadisControllerSetCalendar(PadisControllerDeviceAndCalendarModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetAccessLevel")]
        void CommunicationPadisControllerSetAccessLevel(PadisControllerDeviceAndAccessLevelModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PadisControllerSetAccessGroup")]
        void CommunicationPadisControllerSetAccessGroup(PadisControllerDeviceAndAccessGroupModel param);
        
        #endregion


        #endregion


        #endregion


        #region System Config 

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SystemConfigGet")]
        SystemConfigModel SystemConfigGet();


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SystemConfigUpdate")]
        void SystemConfigUpdate(SystemConfigModel entity);


        #endregion


        #region Device Commands


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandsGetNotSendStatistics")]
        List<NotSendCommandsStatisticsModel> DeviceCommandsGetNotSendStatistics(ListInt32Model param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandSearch")]
        List<DeviceCommandModel> DeviceCommandSearch(DeviceCommandSearchModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandGetCount")]
        int DeviceCommandGetCount(DeviceCommandSearchModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandsResendById")]
        void DeviceCommandsResendById(ListInt32Model param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandsDeleteByIds")]
        void DeviceCommandsDeleteByIds(ListInt32Model param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandsDeleteByIdentifiers")]
        void DeviceCommandsDeleteByIdentifiers(ListGuidModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DeviceCommandsDeleteNotSendByDeviceNumbers")]
        void DeviceCommandsDeleteNotSendByDeviceNumbers(ListInt32Model param);

        #endregion


        #region Attendance

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SaveOtherResourcesAttendanceForHook")]
        void SaveOtherResourcesAttendanceForHook(OtherResourcesAttendanceModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "SendFunctionTitles")]
        void CommunicationSendFunctionTitles(FunctionTitleModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ReconnectOnlineMonitoringDevice")]
        void CommunicationReconnectOnlineMonitoringDevice(DeviceCommunicationModel deviceInfo);

        #endregion


        #region PrintService

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "DirectPrintSelfBill")]
        SelfPrintResultModel DirectPrintSelfBill(SelfBillInfoModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "PrintSelfBill")]
        void PrintSelfBill(ListSelfBillInfoModel param);

        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "GetPrinterNames")]
        ListStringModel GetPrinterNames();

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

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ResetMetalDetectorGateCache")]
        bool ResetMetalDetectorGateCache();

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ResetApplicationEmbeddedInfoCache")]
        bool ResetApplicationEmbeddedInfoCache();

        [OperationContract]
        [WebInvoke(Method = "GET"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "ResetCameraCache")]
        bool ResetCameraCache();

        #endregion


        #region Health Check


        [OperationContract]
        [FaultContract(typeof(ErrorModel), Namespace = ServiceConstants.WcfNamsepace)]
        [WebInvoke(Method = "POST"
            , RequestFormat = WebMessageFormat.Json
            , ResponseFormat = WebMessageFormat.Json
            , BodyStyle = WebMessageBodyStyle.Bare
            , UriTemplate = "/padis-hw-service-health-check")]
        void HealthCheck();


        #endregion

    }

}
