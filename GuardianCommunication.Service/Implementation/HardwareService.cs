using System;
using System.Collections.Generic;
using System.ServiceModel;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Business.PrintService;
using GuardianCommunication.Service.WCF;

namespace GuardianCommunication.Service
{


    [ServiceBehavior(Name = "IHardwareService",
        Namespace = "http://www.emdad.com/IHardwareService"
        , InstanceContextMode = InstanceContextMode.PerCall
        , ConcurrencyMode = ConcurrencyMode.Multiple
    )]
    [ValidateInputMessage]
    [HandleServiceException]
    public class HardwareService : BaseService, IHardwareService
    {

        #region  Communication Service


        #region Bulk Operation

        public List<EmployeeAndDeviceResultModel> CommunicationEnrollUserWithTemplateBulk(EmployeeAndDeviceParamsModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationBulkEnrollUser(
                CommunicationModelMapper.MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(param.Records));
            return CommunicationModelMapper.MapDtoEmployeeAndDeviceResultToEmployeeAndDeviceResultModel(result);
        }

        public List<EmployeeAndDeviceResultModel> CommunicationDeleteUserBulk(EmployeeAndDeviceParamsModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationBulkDeleteUser(
                CommunicationModelMapper.MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(param.Records));
            return CommunicationModelMapper.MapDtoEmployeeAndDeviceResultToEmployeeAndDeviceResultModel(result);
        }

        #endregion


        public void CommunicationRebootDevice(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.RebootDevice(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));

        }

        public void CommunicationEnrollUserWithTemplate(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationEnrollUserWithTemplate(
                CommunicationModelMapper.MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(param));
        }

        public void CommunicationSendUser(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSendUser(
                CommunicationModelMapper.MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(param));
        }

        public EmployeeModel CommunicationGetUserById(GetUserByIdModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoEmployeeToEmployeeModel(component.CommunicationGetUserById(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                param.EmployeeNumber,
                param.TemplateType));
        }

        public List<UserInfoDefinedOnDeviceModel> CommunicationGetUsersInfoDefinedOnDevice(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserInfoOnDeviceToUserInfoOnDeviceModel(
                component.CommunicationGetUsersInfoDefinedOnDevice(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo)));
        }

        public List<DeviceAttendanceModel> CommunicationGetAttendance(GetAttendanceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationGetUnreadAttendanceForClientFromSdk(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                param.DeleteAttedance);
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public List<DeviceAttendanceModel> CommunicationReadout(ReadoutModel param)
        {
            var component = new AttendanceComponent(GetRepositoryFactory());
            var startDate = param.StartDate.FromNumericDateTime();
            var endDate = param.EndDate.FromNumericDateTime();

            if (Math.Abs(endDate.Subtract(startDate).TotalDays) > ServiceConstants.MaxReadoutDays)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationMaxReadoutDaysIsNotValid);
            }

            var result = component.Search(new PagingData<AttendanceFilter, AttendanceSortEnumeration>()
            {
                Filter = new AttendanceFilter
                {
                    AttendanceDateFrom = startDate,
                    AttendanceDateTo = endDate,
                    DeviceNumbers = param.DeviceNumbers,
                    EmployeeNumbers = param.EmployeeNumbers,
                    IsSent = param.IsSent
                }
            });
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public List<DeviceAttendanceModel> CommunicationReadoutFromDevice(ReadoutFromDeviceModel param)
        {
            var startDate = param.StartDate.FromNumericDateTime();
            var endDate = param.EndDate.FromNumericDateTime();
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationReadoutFromDevice(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                startDate, endDate);
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public string CommunicationGetFirmwareVersion(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetFirmwareVersion(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public void CommunicationUpgradeFirmware(UpdateFirmwareModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationUpgradeFirmware(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.FileName
                , Convert.FromBase64String(param.FirmwareData));
        }

        public void CommunicationCancelOperation(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationCancelOperation(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public void CommunicationSetDateAndTime(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSetDateAndTime(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public double CommunicationGetDateAndTime(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetDateAndTime(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo)).ToNumericDateTime();
        }

        public void CommunicationClearData(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationClearData(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public int CommunicationRecordCount(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationRecordCount(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public int CommunicationFaceCount(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationFaceCount(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public int CommunicationFingerCount(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationFingerCount(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public int CommunicationUserCount(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationUserCount(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public void CommunicationDeleteUserByUserInfo(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteUserByInfo(
                CommunicationModelMapper.MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(param));
        }

        public void CommunicationDeleteUserByUserId(DeviceAndEmployeeNumberListModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteUserByUserId(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.EmployeeNumber.First());
        }

        public void CommunicationSendWithoutFingers(DeviceAndEmployeeListModel param)
        {
            if (param.EmployeeInfos.IsCollectionNullOrEmpty())
            {
                return;
            }
            var component =
                new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSendWithoutFinger(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeInfos));
        }

        public void CommunicationSetValidInvalid(DeviceAndEmployeeListModel param)
        {
            if (param.EmployeeInfos.IsCollectionNullOrEmpty())
            {
                return;
            }
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSendValidInvalid(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeInfos));
        }

        public void CommunicationDeleteAllUsers(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteAllUsers(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public string CommunicationGetSerialNumber(DeviceCommunicationModel deviceInfo)
        {
            var component =
                new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetSerialNumber(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public OperationResultEnumeration CommunicationTestConnection(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationTestConnection(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public DeviceStatisticsModel CommunicationGetDeviceStatistics(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoDeviceStatisticsToDeviceStatisticsModel(
            component.CommunicationGetDeviceStatistics(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo)));
        }

        public string CommunicationScanCard(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.ScanCard(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeData));
        }

        public EmployeeFingerModel CommunicationScanFinger(ScanFingerModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoEmployeeFingerToEmployeeFingerModel(component.ScanFinger(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeData)
                , param.FingerIndex));
        }

        public EmployeeFaceModel CommunicationScanFace(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoEmployeeFaceToEmployeeFaceModel(component.ScanFace(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeData)));
        }

        public EmployeeFaceModel CommunicationScanFaceStandalone(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoEmployeeFaceToEmployeeFaceModel(component.ScanFaceStandalone(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeData)));
        }

        public EmployeeIrisModel CommunicationScanIris(EmployeeAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoEmployeeIrisToEmployeeIrisModel(component.ScanIris(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapEmployeeModelToDtoEmployee(param.EmployeeData)));
        }

        public void CommunicationCheck(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationCheck(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }

        public string GetAttendanceImage(GetAttendanceImageModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());

            var result = component.CommunicationGetAttendanceImage(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.EmployeeNumber
                , DateTime.FromOADate(param.AttendanceDateTime));
            if (result.IsCollectionNotNullOrEmpty())
            {
                return Convert.ToBase64String(result);
            }
            return null;
        }

        public void CommunicationSendFunctionTitles(FunctionTitleModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSendFunctionTitles(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                param.Titles);
        }

        public void CommunicationReconnectOnlineMonitoringDevice(DeviceCommunicationModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationReconnectOnlineMonitoringDevice(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(deviceInfo));
        }



        #endregion


        #region Access control

        public void CommunicationOpenCabinetDoor(OpenCabinetDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenCabinetDoor(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.CabinetNumber);
        }



        #region Suprema SDK 1 

        public void CommunicationSendSupremaSdk1Holidays(SupremaSdk1SendHolidaysModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk1Holidays(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapDeviceHolidayModelToDtoAcDeviceHoliday(param.Holidays));
        }

        public void CommunicationSendSupremaSdk1Timezones(SupremaSdk1SendTimezomesModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk1Timezones(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1TimezoneModelToDtoSupremaSdk1Timezone(param.Timezones));
        }

        public void CommunicationSendSupremaSdk1AccessGroups(SupremaSdk1SendAccessGroupModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk1AccessGroups(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1AccessGroupModelToDtoSupremaSdk1AccessGroup(param.AccessGroups));
        }

        public void CommunicationSendSupremaSdk1DoorInfo(SupremaSdk1DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk1DoorInfo(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(param.Door));
        }

        public void CommunicationOpenSupremaSdk1Door(SupremaSdk1DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk1Door(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(param.Door));
        }

        public void CommunicationOpenSupremaSdk1DoorWithDelay(SupremaSdk1DeviceAndDoorWithDelayModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk1DoorWithDelay(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(param.Door)
                , param.DelayInSecond);
        }

        public void CommunicationOpenSupremaSdk1DoorPermanent(SupremaSdk1DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk1DoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(param.Door));
        }

        public void CommunicationCloseSupremaSdk1DoorPermanent(SupremaSdk1DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CloseSupremaSdk1DoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk1DoorModelToDtoSupremaSdk1Door(param.Door));
        }

        #endregion


        #region Suprema SDK 2

        public void CommunicationSendSupremaSdk2Holidays(SupremaSdk2SendHolidaysModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk2Holidays(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapDeviceHolidayModelToDtoAcDeviceHoliday(param.Holidays));
        }

        public void CommunicationSendSupremaSdk2AccessSchedules(SupremaSdk2SendAccessSchedulesModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk2AccessSchedules(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2AccessScheduleModelToDtoSupremaSdk2AccessSchedule(param.AccessSchedules));
        }

        public void CommunicationSendSupremaSdk2AccessGroups(SupremaSdk2SendAccessGroupModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk2AccessGroups(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2AccessGroupModelToDtoSupremaSdk2AccessGroup(param.AccessGroups));
        }

        public void CommunicationSendSupremaSdk2AccessLevels(SupremaSdk2SendAccessLevelModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk2AccessLevels(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2AccessLevelModelToDtoSupremaSdk2AccessLevel(param.AccessLevels));
        }

        public void CommunicationSendSupremaSdk2DoorInfo(SupremaSdk2SendDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendSupremaSdk2DoorInfo(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(param.Door));
        }

        public void CommunicationOpenSupremaSdk2Door(SupremaSdk2DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk2Door(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(param.Door));
        }

        public void CommunicationOpenSupremaSdk2DoorWithDelay(SupremaSdk2DeviceAndDoorWithDelayModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk2DoorWithDelay(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(param.Door)
                , param.DelayInSecond);
        }

        public void CommunicationOpenSupremaSdk2DoorPermanent(SupremaSdk2DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk2DoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(param.Door));
        }

        public void CommunicationCloseSupremaSdk2DoorPermanent(SupremaSdk2DeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CloseSupremaSdk2DoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapSupremaSdk2DoorModelToDtoSupremaSdk2Door(param.Door));
        }

        #endregion


        #region Zk

        public void CommunicationSendHolidays(SendHolidaysModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendHolidays(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapDeviceHolidayModelToDtoAcDeviceHoliday(param.Holidays));
        }

        public void CommunicationSendTimezone(SendTimezoneModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendTimezone(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                CommunicationModelMapper.MapTimezoneModelToDtoTimezone(param.TimeZone));
        }

        public void CommunicationSendUserTimezones(SendUserTimezonesModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.SendUserTimeZones(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.EmployeeNumber
                , param.TimeZoneNumbers);
        }

        public void CommunicationOpenDoor(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenDoor(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapDeviceDoorBaseModelToDtoAcDeviceDoorBase(param.DoorInfo));
        }

        public void CommunicationOpenDoorWithDelay(OpenDoorWithDelayModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenDoorWithDelay(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , param.DelayInSecond);
        }

        public void CommunicationSendDoorInfo(DeviceDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            if (param.ZkDeviceDoorInfo != null)
            {
                component.SetZkDeviceDoorInfo(
                    CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo),
                    CommunicationModelMapper.MapZkDeviceDoorModelToDtoZkDeviceDoor(param.ZkDeviceDoorInfo));
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #region Virdi

        public void CommunicationSendVirdiAccessControlData(VirdiAccessControlDataModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.VirdiSendAccessControlData(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapVirdiAccessControlDataModelToDtoVirdiAccessControlData(param));
        }

        #endregion


        #region Timy

        public void CommunicationTimySetDayTimezone(TimySetDayTimezoneModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.TimySetDayTimezone(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapTimyDayTimezoneGroupModelToDtoTimyDayTimezoneGroup(param.DayTimezoneGroups));
        }

        public void CommunicationTimySetWeekTimezone(TimySetWeekTimezoneModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.TimySetWeekTimezone(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapTimyWeekTimezoneGroupModelToDtoTimyWeekTimezoneGroup(param.WeekTimezoneGroups));
        }

        public void CommunicationTimySetHolidays(TimySetHolidaysModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.TimySetHolidays(CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapTimyHolidayModelToDtoTimyHoliday(param.Holidays));
        }

        #endregion


        #region Padis Controller


        public void CommunicationOpenPadisControllerDoor(PadisControllerDeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenPadisControllerDoor(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerDoorModelToDtoPadisControllerDoor(param.Door));
        }

        public void CommunicationOpenPadisControllerDoorWithDelay(PadisControllerDeviceAndDoorWithDelayModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenPadisControllerDoorWithDelay(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerDoorModelToDtoPadisControllerDoor(param.Door)
                , param.DelayInSecond);
        }

        public void CommunicationOpenPadisControllerDoorPermanent(PadisControllerDeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenPadisControllerDoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerDoorModelToDtoPadisControllerDoor(param.Door));
        }

        public void CommunicationClosePadisControllerDoorPermanent(PadisControllerDeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.ClosePadisControllerDoorPermanent(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerDoorModelToDtoPadisControllerDoor(param.Door));
        }

        public void CommunicationPadisControllerSetDoor(PadisControllerDeviceAndDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetDoor(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerDoorModelToDtoPadisControllerDoor(param.Door));
        }

        public void CommunicationPadisControllerSetIoPort(PadisControllerDeviceAndIoPortModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetIoPort(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerIoPortModelToDtoPadisControllerIoPort(param.IoPort));
        }

        public void CommunicationPadisControllerSetWiegand(PadisControllerDeviceAndWiegandModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetWiegand(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerWiegandModelToDtoPadisControllerWiegand(param.Wiegand));
        }

        public void CommunicationPadisControllerSetRelay(PadisControllerDeviceAndRelayModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetRelay(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerRelayModelToDtoPadisControllerRelay(param.Relay));
        }

        public void CommunicationPadisControllerSetCalendar(PadisControllerDeviceAndCalendarModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetCalendar(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerCalendarModelToDtoPadisControllerCalendar(param.Calendar));
        }

        public void CommunicationPadisControllerSetAccessLevel(PadisControllerDeviceAndAccessLevelModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetAccessLevel(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerAccessLevelModelToDtoPadisControllerAccessLevel(param.AccessLevel));
        }

        public void CommunicationPadisControllerSetAccessGroup(PadisControllerDeviceAndAccessGroupModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.PadisControllerSetAccessGroup(
                CommunicationModelMapper.MapDeviceCommunicationModelToDtoDeviceCommunication(param.DeviceInfo)
                , CommunicationModelMapper.MapPadisControllerAccessGroupModelToDtoPadisControllerAccessGroup(param.AccessGroup));
        }



        #endregion

        #endregion


        #region System Config 

        public SystemConfigModel SystemConfigGet()
        {
            var component = new SystemConfigComponent(GetRepositoryFactory());
            var result = CommunicationModelMapper.MapDtoSystemConfigToSystemConfigModel(component.GetSystemConfig());
            return result;
        }

        public void SystemConfigUpdate(SystemConfigModel entity)
        {
            var component = new SystemConfigComponent(GetRepositoryFactory());
            component.Update(CommunicationModelMapper.MapSystemConfigModelToDtoSystemConfig(entity));
        }

        #endregion


        #region Device Commands

        public List<NotSendCommandsStatisticsModel> DeviceCommandsGetNotSendStatistics(ListInt32Model param)
        {
            var deviceCommandComponent = new DeviceCommandComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoNotSendStatisticsToNotSendStatisticsModel
                (deviceCommandComponent.GetDeviceNotSendCommandsStatistics(param.Items));
        }

        public List<DeviceCommandModel> DeviceCommandSearch(DeviceCommandSearchModel param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoDeviceCommandToDeviceCommandModel(
            component.SearchWithoutContent(CommunicationModelMapper.MapDeviceCommandSearchModelToSearchInfo(param)));
        }

        public int DeviceCommandGetCount(DeviceCommandSearchModel param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            return
                component.GetCount(CommunicationModelMapper.MapDeviceCommandSearchModelToSearchInfo(param));
        }

        public void DeviceCommandsResendById(ListInt32Model param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            component.ResetSendData(param.Items);
        }

        public void DeviceCommandsDeleteByIds(ListInt32Model param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            component.DeleteByIds(param.Items);
        }

        public void DeviceCommandsDeleteNotSendByDeviceNumbers(ListInt32Model param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            component.DeleteNotSendByDeviceNumbers(param.Items);
        }

        public void DeviceCommandsDeleteByIdentifiers(ListGuidModel param)
        {
            var component = new DeviceCommandComponent(GetRepositoryFactory());
            component.DeleteByCommandIdentifiers(param.Items);
        }

        #endregion


        #region Attendance

        public void SaveOtherResourcesAttendanceForHook(OtherResourcesAttendanceModel param)
        {
            if (param == null)
            {
                return;
            }
            var component = new AttendanceComponent(GetRepositoryFactory());
            component.HookOtherResourcesAttendance(CommunicationModelMapper.MapOtherResourcesAttendanceModelToDtoAttendance(param),
                param.CheckDuplicateInterval);
        }

        #endregion

        
        #region PrintService

        public SelfPrintResultModel DirectPrintSelfBill(SelfBillInfoModel param)
        {
            return CommunicationModelMapper.MapDtoSelfPrintResultToSelfPrintResultModel(
                    PrintService.Instance.DirectPrintSelfBill(CommunicationModelMapper.MapSelfBillInfoModelToDtoSelfBillInfo(param)));
        }

        public void PrintSelfBill(ListSelfBillInfoModel param)
        {
            if (param != null && param.Items.IsCollectionNotNullOrEmpty())
            {
                foreach (var item in param.Items)
                {
                    PrintService.Instance.AddToSelfBillQueue(CommunicationModelMapper.MapSelfBillInfoModelToDtoSelfBillInfo(item));
                }
            }
        }


        public ListStringModel GetPrinterNames()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            return new ListStringModel { Items = component.GetPrinterNames() };
        }



        #endregion


        #region Cache Reset

        public bool ResetDeviceCache()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            component.ResetDeviceCacheAndSetDeviceConnectionModes();
            return true;
        }

        public bool ResetDeviceDoorCache()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            component.ResetDeviceDoorCache();
            return true;
        }

        public bool ResetMetalDetectorGateCache()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            component.ResetMetalDetectorGateCacheAndSetConnectionModes();
            return true;
        }

        public bool ResetCameraCache()
        {
            var component = new CameraComponent(GetRepositoryFactory());
            component.ResetCameraCacheAndSetConnectionModes();
            return true;
        }

        public bool ResetApplicationEmbeddedInfoCache()
        {
            var component = new SystemConfigComponent(GetRepositoryFactory());
            component.ConfigureApplicationEmbeddedInfo();
            return true;
        }

        #endregion


        #region Health Check


        public void HealthCheck()
        {

        }

        #endregion

    }
}






