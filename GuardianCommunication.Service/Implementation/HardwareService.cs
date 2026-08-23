using GuardianCommunication.Business.Component;
using GuardianCommunication.Service.WCF;
using GuardianCommunication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SearchDataWrapper;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace GuardianCommunication.Service
{


    [ServiceBehavior(Name = "IGuardianCommunication",
        Namespace = "http://www.emdad.com/IHardwareService"
        , InstanceContextMode = InstanceContextMode.PerCall
        , ConcurrencyMode = ConcurrencyMode.Multiple
    )]
    [HandleServiceException]
    public class HardwareService : BaseService, IGuardianCommunication
    {

        #region  Communication Service


        #region Bulk Operation

        public List<UserAndDeviceResultModel> CommunicationEnrollUserWithTemplateBulk(UserAndDeviceListModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationBulkEnrollUser(
                CommunicationModelMapper.MapUserAndDeviceModelToDtoUserAndDeviceParam(param.Records));
            return CommunicationModelMapper.MapDtoUserAndDeviceResultToUserAndDeviceResultModel(result);
        }

        public List<UserAndDeviceResultModel> CommunicationDeleteUserBulk(UserAndDeviceListModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationBulkDeleteUser(
                CommunicationModelMapper.MapUserAndDeviceModelToDtoUserAndDeviceParam(param.Records));
            return CommunicationModelMapper.MapDtoUserAndDeviceResultToUserAndDeviceResultModel(result);
        }

        #endregion


        public void CommunicationRebootDevice(IdSingleModel deviceInfo)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.RebootDevice(deviceInfo.Id);

        }

        public void CommunicationEnrollUserWithTemplate(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationEnrollUserWithTemplate(
                CommunicationModelMapper.MapUserAndDeviceModelToDtoUserAndDeviceParam(param));
        }

        public void CommunicationSendUser(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSendUser(
                CommunicationModelMapper.MapUserAndDeviceModelToDtoUserAndDeviceParam(param));
        }

        public UserModel CommunicationGetUserById(GetUserByIdModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserToUserModel(component.CommunicationGetUserById(
                param.DeviceId,
                param.UserIdOnDevice,
                param.TemplateType));
        }

        public List<UserInfoDefinedOnDeviceModel> CommunicationGetUsersInfoDefinedOnDevice
            (IdSingleModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserInfoOnDeviceToUserInfoOnDeviceModel(
                component.CommunicationGetUsersInfoDefinedOnDevice(param.Id));
        }

        public List<DeviceAttendanceModel> CommunicationGetAttendance(GetAttendanceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationGetUnreadAttendanceForClientFromSdk(
                param.DeviceId,
                param.DeleteAttendance);
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public List<DeviceAttendanceModel> CommunicationReadout(ReadoutModel param)
        {
            var component = new AttendanceComponent(GetRepositoryFactory());

            if (Math.Abs(param.EndDate.Subtract(param.StartDate).TotalDays) > ServiceConstants.MaxReadoutDays)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationMaxReadoutDaysIsNotValid);
            }

            var result = component.Search(new PagingData<AttendanceFilter, AttendanceSortEnumeration>
            {
                Filter = new AttendanceFilter
                {
                    AttendanceDateFrom = param.StartDate,
                    AttendanceDateTo = param.EndDate,
                    DeviceIds = param.DeviceIds,
                    UsersIdOnDevice = param.UsersIdOnDevice,
                    IsSentToGuardian = param.IsSentToGuardian
                }
            });
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public List<DeviceAttendanceModel> CommunicationReadoutFromDevice(ReadoutFromDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            var result = component.CommunicationReadoutFromDevice(
                param.DeviceId,
                param.StartDate, param.EndDate);
            return CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(result);
        }

        public string CommunicationGetFirmwareVersion(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetFirmwareVersion(model.Id);
        }

        public void CommunicationUpgradeFirmware(UpdateFirmwareModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationUpgradeFirmware(param.DeviceId, param.FileName
                , Convert.FromBase64String(param.FirmwareData));
        }

        public void CommunicationSetDateAndTime(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationSetDateAndTime(model.Id);
        }

        public DateTime CommunicationGetDateAndTime(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetDateAndTime(model.Id);
        }

        public void CommunicationClearData(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationClearData(model.Id);
        }

        public int CommunicationRecordCount(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationRecordCount(model.Id);
        }

        public int CommunicationFaceCount(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationFaceCount(model.Id);
        }

        public int CommunicationFingerCount(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationFingerCount(model.Id);
        }

        public int CommunicationUserCount(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationUserCount(model.Id);
        }

        public void CommunicationDeleteUserByUserInfo(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteUserByInfo(
                CommunicationModelMapper.MapUserAndDeviceModelToDtoUserAndDeviceParam(param));
        }

        public void CommunicationDeleteUserByUserId(UserIdOnDeviceAndDeviceIdModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteUserByUserId(model.DeviceId, model.UserIdOnDevice);
        }

        public void CommunicationDeleteAllUsers(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CommunicationDeleteAllUsers(model.Id);
        }

        public string CommunicationGetSerialNumber(IdSingleModel model)
        {
            var component =
                new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationGetSerialNumber(model.Id);
        }

        public OperationResultEnumeration CommunicationTestConnection(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.CommunicationTestConnection(model.Id);
        }

        public DeviceStatisticsModel CommunicationGetDeviceStatistics(IdSingleModel model)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoDeviceStatisticsToDeviceStatisticsModel(
                component.CommunicationGetDeviceStatistics(model.Id));
        }

        public string CommunicationScanCard(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return component.ScanCard(param.DeviceId
                , CommunicationModelMapper.MapUserModelToDtoUser(param.UserData));
        }

        public UserFingerModel CommunicationScanFinger(ScanFingerModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserFingerToUserFingerModel(component.ScanFinger(
                param.DeviceId
                , CommunicationModelMapper.MapUserModelToDtoUser(param.UserData)
                , param.FingerIndex));
        }

        public UserFaceModel CommunicationScanFace(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserFaceToUserFaceModel(component.ScanFace(
                param.DeviceId
                , CommunicationModelMapper.MapUserModelToDtoUser(param.UserData)));
        }

        public UserFaceModel CommunicationScanFaceStandalone(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserFaceToUserFaceModel(component.ScanFaceStandalone(
                param.DeviceId
                , CommunicationModelMapper.MapUserModelToDtoUser(param.UserData)));
        }

        public UserIrisModel CommunicationScanIris(UserAndDeviceModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            return CommunicationModelMapper.MapDtoUserIrisToUserIrisModel(component.ScanIris(
                param.DeviceId
                , CommunicationModelMapper.MapUserModelToDtoUser(param.UserData)));
        }

        public string GetAttendanceImage(GetAttendanceImageModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());

            var result = component.CommunicationGetAttendanceImage(
                param.DeviceId
                , param.UserIdOnDevice
                , DateTime.FromOADate(param.AttendanceDateTime));
            if (result.IsCollectionNotNullOrEmpty())
            {
                return Convert.ToBase64String(result);
            }
            return null;
        }



        #endregion


        #region Access control

        public void CommunicationOpenCabinetDoor(OpenCabinetDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenCabinetDoor(
                param.DeviceId
                , param.CabinetNumber);
        }

        public void CommunicationOpenDoor(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenDoor(param.DeviceId, param.DoorId);
        }

        public void CommunicationOpenDoorWithDelay(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenDoorWithDelay(param.DeviceId, param.DoorId, param.DelayInSecond ?? 5);

        }


        #region Suprema SDK 1


        public void CommunicationOpenSupremaSdk1DoorPermanent(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk1DoorPermanent(param.DeviceId, param.DoorId);
        }

        public void CommunicationCloseSupremaSdk1DoorPermanent(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CloseSupremaSdk1DoorPermanent(param.DeviceId, param.DoorId);
        }


        #endregion


        #region Suprema SDK 2

        public void CommunicationOpenSupremaSdk2DoorPermanent(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.OpenSupremaSdk2DoorPermanent(param.DeviceId, param.DoorId);
        }

        public void CommunicationCloseSupremaSdk2DoorPermanent(OpenDoorModel param)
        {
            var component = new CommunicationComponent(GetRepositoryFactory());
            component.CloseSupremaSdk2DoorPermanent(param.DeviceId, param.DoorId);
        }


        #endregion



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


        #region Cache Reset
        //TODO: Check this

        public bool ResetDeviceCache()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            //component.();
            return true;
        }

        public bool ResetDeviceDoorCache()
        {
            var component = new DeviceComponent(GetRepositoryFactory());
            //component.ResetDeviceDoorCache();
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






