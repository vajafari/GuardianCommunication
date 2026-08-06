using System;
using System.Collections.Generic;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.KarnamaApi;

namespace GuardianCommunication.Business.Component
{
    public class KarnamaComponent : BaseComponent
    {

        public KarnamaComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }

        public List<DtoDevice> FetchAllDevices()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var deviceList = proxy.GetAllDevices(new DeviceSearchParams
            {
                GetArea = true,
                GetDeviceTypeSummary = true,
                Filter = new DeviceFilter
                {
                    IsActive = true
                }
            });

            return deviceList;
        }

        public List<DtoDeviceDoor> FetchAllDeviceDoors()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            return proxy.GetAllDeviceDoors();
        }

        public List<DtoMetalDetectorGate> FetchAllMetalDetectorGates()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var metalDetectorGatesList = proxy.GetAllActiveMetalDetectorGates();

            return metalDetectorGatesList;
        }

        public List<DtoXRayDevice> FetchAllXRayDevices()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var xRayDevicesList = proxy.GetAllActiveXRayDevices();
            return xRayDevicesList;
        }

        public List<DtoCamera> FetchAllCameras()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var cameraList = proxy.GetAllCameras();
            return cameraList;
        }

        #region Basic Info

        public AttendanceProcessResultModel SubmitIoEvent(DtoAttendance entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            return proxy.SubmitIoEvent(entity);
        }

        public void SubmitInvalidIoEvent(DtoInvalidAttendance entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitInvalidIoEvent(entity);
        }

        public void SubmitNotLiveValidPlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitNotLiveValidPlateDetectionCameraAttendance(attendance);
        }

        public void SubmitNotLiveInvalidPlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitNotLiveInvalidPlateDetectionCameraAttendance(attendance);
        }

        public void SubmitLivePlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitLivePlateDetectionCameraAttendance(attendance);
        }

        public ServerMatchingResultModel SubmitServerMatching(DtoServerMatchData entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var result = proxy.SubmitServerMatching(entity);

            return result;
        }

        public void SendDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitConnectionStatusChanged(deviceConnectionStatuses);
        }

        public void SendUserEnrolled(DtoEmployeeDeviceRelatedData userInfo, int deviceNumber, DtoEmployeeEnrolledSetting setting)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitUserEnrolled(userInfo, deviceNumber, setting);
        }

        public void SendFaceEnrolled(DtoEmployeeFace faceInfo, int deviceNumber)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitFaceEnrolled(faceInfo, deviceNumber);
        }

        public void SendCardEnrolled(string cardNumber, int deviceNumber, long userId)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitCardEnrolled(cardNumber, deviceNumber, userId);

        }

        public void SendPalmEnrolled(DtoEmployeePalm palmInfo, int deviceNumber)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitPalmEnrolled(palmInfo, deviceNumber);
        }

        public void SendIrisEnrolled(DtoEmployeeIris irisInfo, int deviceNumber)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitIrisEnrolled(irisInfo, deviceNumber);
        }

        public void SendFingerEnrolled(DtoEmployeeFinger fingerInfo, int deviceNumber)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitFingerEnrolled(fingerInfo, deviceNumber);

        }

        public void SendUserProfileImage(DtoEmployeeImage employeeImage, int deviceNumber)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitUserProfileImage(employeeImage, deviceNumber);

        }

        public void SendZkOperationLog(DtoZkOperationLog operationLog)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitZkOperationLog(operationLog);

        }

        public void SendDeviceEventLog(DtoDeviceEventLog eventLog)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitDeviceEventLog(eventLog);

        }

        public void SendUserCount(int deviceNumber, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitUserCount(deviceNumber, count);
        }

        public void SendAccessLogCount(int deviceNumber, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitAccessLogCount(deviceNumber, count);
        }

        public void SendFaceCount(int deviceNumber, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitFaceCount(deviceNumber, count);
        }

        public void SendFingerCount(int deviceNumber, int count)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitFingerCount(deviceNumber, count);
        }

        public void SendMetalDetectorPersonPassed(DtoMetalDetectorPersonPassedData passData)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SendMetalDetectorPersonPassed(passData);

        }

        public void SendXRayDeviceScanData(string deviceId, DateTime date, byte[] scanData)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SendXRayDeviceScanData(deviceId, date, scanData);

        }

        public void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SendAttendanceImage(image);
        }

        public void SendUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SendUnauthorizedAttendanceImage(image);
        }

        public DtoApplicationEncodedConfig GetSoftwareEncodedConfig()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            return proxy.GetSoftwareEncodedConfig();
        }



        #endregion


        #region Self


        public void SendSelfEvent(DtoAttendance entity)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            proxy.SubmitSelfEvent(entity);
        }

        public void ReportSelfPrintResult(DtoSelfPrintResult entity)
        {
            try
            {
                var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
                var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
                proxy.SubmitSelfPrintResult(entity);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }

        }

        public List<DtoDeviceMealOrderData> GetMealOrderMealsInfo(int time)
        {

            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var result = proxy.GetMealOrderMealsInfo(time);
            return result;
        }


        #endregion


        #region Access Control

        public List<DtoCarAccessData> GetCameraCarAccessData(int cameraId, DateTime startDateTime, DateTime endDateTime)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var proxy = new PadisProxy(systemConfigComponent.GetPadisApiConfig());
            var carAccessDataList = proxy.GetCameraCarAccessData(cameraId, startDateTime, endDateTime);
            return carAccessDataList;
        }

        #endregion
    }
}
