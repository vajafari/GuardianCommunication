using System;
using System.Collections.Generic;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.KarnamaApi;
using GuardianCommunication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;

namespace GuardianCommunication.Business.Component
{
    public class GuardianComponent : BaseComponent
    {

        public GuardianComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        #region Basic Info

        public AttendanceProcessResultModel SubmitIoEvent(DtoAttendance entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            return proxy.SubmitIoEvent(entity);
        }

        public void SubmitInvalidIoEvent(DtoInvalidAttendance entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitInvalidIoEvent(entity);
        }

        public ServerMatchingResultModel SubmitServerMatching(DtoServerMatchData entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            var result = proxy.SubmitServerMatching(entity);
            return result;
        }

        public void SendDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitConnectionStatusChanged(deviceConnectionStatuses);
        }

        public void SendUserEnrolled(DtoUserDeviceRelatedData userInfo, Guid deviceId, DtoUserEnrolledSetting setting)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitUserEnrolled(userInfo, deviceId, setting);
        }

        public void SendFaceEnrolled(DtoUserFace faceInfo, Guid deviceId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitFaceEnrolled(faceInfo, deviceId);
        }

        public void SendCardEnrolled(string cardNumber, Guid deviceId, long userId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitCardEnrolled(cardNumber, deviceId, userId);

        }

        public void SendPalmEnrolled(DtoUserPalm palmInfo, Guid deviceId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitPalmEnrolled(palmInfo, deviceId);
        }

        public void SendIrisEnrolled(DtoUserIris irisInfo, Guid deviceId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitIrisEnrolled(irisInfo, deviceId);
        }

        public void SendFingerEnrolled(DtoUserFinger fingerInfo, Guid deviceId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitFingerEnrolled(fingerInfo, deviceId);

        }

        public void SendUserProfileImage(DtoUserImage employeeImage, Guid deviceId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitUserProfileImage(employeeImage, deviceId);

        }

        public void SendZkOperationLog(DtoZkOperationLog operationLog)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitZkOperationLog(operationLog);

        }

        public void SendDeviceEventLog(DtoDeviceEventLog eventLog)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitDeviceEventLog(eventLog);

        }

        public void SendUserCount(Guid deviceId, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitUserCount(deviceId, count);
        }

        public void SendAccessLogCount(Guid deviceId, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitAccessLogCount(deviceId, count);
        }

        public void SendFaceCount(Guid deviceId, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitFaceCount(deviceId, count);
        }

        public void SendFingerCount(Guid deviceId, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SubmitFingerCount(deviceId, count);
        }


        public void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SendAttendanceImage(image);
        }

        public void SendUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            proxy.SendUnauthorizedAttendanceImage(image);
        }

        public DtoApplicationEncodedConfig GetSoftwareEncodedConfig()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new GuardianProxy(systemConfigComponent.GetGuardianApiConfig());
            return proxy.GetSoftwareEncodedConfig();
        }



        #endregion


    }
}
