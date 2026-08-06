using System;
using System.Collections.Generic;

namespace GuardianCommunication.Hardware.Shared
{
    public class HardwareEventPublisher
    {
        public event Action<DtoAttendance> AttendanceReceived;
        public event Action<DtoInvalidAttendance> InvalidAttendanceReceived;
        public event Action<DtoPlateDetectionCameraCarAttendance> NotLivePlateDetectionCameraCarAttendanceReceived;
        public event Action<DtoPlateDetectionCameraCarAttendance> LivePlateDetectionCameraCarAttendanceReceived;
        public event Action<DtoDeviceAttendanceImage> AttendanceImageReceived;
        public event Action<DtoDeviceUnauthorizedAttendanceImage> UnauthorizedAttendanceImageReceived;
        public event Action<DtoMetalDetectorPersonPassedData> MetalDetectorPersonPassed;
        public event Action<string, DateTime, byte[]> XRayDeviceDataReceived;
        public event Action<List<DtoDeviceConnectionStatus>> DeviceConnectionStatusChanged;
        public event Action<DtoEmployeeDeviceRelatedData, int, DtoEmployeeEnrolledSetting> NewUserEnrolled;
        public event Action<DtoEmployeeFace, int> NewFaceEnrolled;
        public event Action<string, int, long> NewCardEnrolled;
        public event Action<DtoZkOperationLog> ZkOperationLogDataReceived;
        public event Action<DtoDeviceEventLog> DeviceEventLogDataReceived;
        public event Action<DtoEmployeePalm, int> NewPalmEnrolled;
        public event Action<DtoEmployeeIris, int> NewIrisEnrolled;
        public event Action<DtoEmployeeFinger, int> NewFingerEnrolled;
        public event Action<List<int>> CommandSentToDevice;
        public event Action<DtoDeviceCommandProcessingResult> CommandResponseReceived;
        public event Action<DtoDeviceCommandProcessingDescription> CommandDescriptionReceived;
        public event Action<DtoEmployeeImage, int> UserProfileImageReceived;
        public event Action<int, int> UserCountReceived;
        public event Action<int, int> FaceCountReceived;
        public event Action<int, int> FingerCountReceived;
        public event Action<int, int> AccessLogCountReceived;
        public event Action<int, long> UserChanged;
        public event Action<List<int>> FaceDetectionCameraCommandSent;
        public event Action<DtoOtherHardwareCommandProcessingResult> FaceDetectionCameraCommandResponseReceived;
        public event Action<DtoOtherHardwareCommandProcessingDescription> FaceDetectionCameraCommandSetDescription;

        #region Singleton

        public static HardwareEventPublisher Instance { get; }

        private HardwareEventPublisher()
        {

        }

        static HardwareEventPublisher()
        {
            Instance = new HardwareEventPublisher();
        }

        #endregion


        public void PublishFaceDetectionCameraCommandSent(List<int> ids)
        {
            FaceDetectionCameraCommandSent?.Invoke(ids);
        }

        public void PublishFaceDetectionCameraCommandResponseReceived(DtoOtherHardwareCommandProcessingResult param)
        {
            FaceDetectionCameraCommandResponseReceived?.Invoke(param);
        }

        public void PublishFaceDetectionCameraCommandSetDescription(DtoOtherHardwareCommandProcessingDescription param)
        {
            FaceDetectionCameraCommandSetDescription?.Invoke(param);
        }

        public void PublishInvalidAttendance(DtoInvalidAttendance attendance)
        {
            InvalidAttendanceReceived?.Invoke(attendance);
        }

        public void PublishAttendance(DtoAttendance attendance)
        {
            AttendanceReceived?.Invoke(attendance);
        }

        public void PublishNotLivePlateDetectionCameraCarAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            NotLivePlateDetectionCameraCarAttendanceReceived?.Invoke(attendance);
        }

        public void LivePublishPlateDetectionCameraCarAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            LivePlateDetectionCameraCarAttendanceReceived?.Invoke(attendance);
        }

        public void PublishAttendanceImage(DtoDeviceAttendanceImage image)
        {
            AttendanceImageReceived?.Invoke(image);
        }

        public void PublishUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {
            UnauthorizedAttendanceImageReceived?.Invoke(image);
        }

        public void PublishMetalDetectorPersonPassed(DtoMetalDetectorPersonPassedData passData)
        {
            MetalDetectorPersonPassed?.Invoke(passData);
        }

        public void PublishXRayDeviceDataReceived(string deviceId, DateTime date, byte[] scanData)
        {
            XRayDeviceDataReceived?.Invoke(deviceId, date, scanData);
        }

        public void PublishDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            DeviceConnectionStatusChanged?.Invoke(deviceConnectionStatuses);
        }

        public void PublishNewUserEnrolled(DtoEmployeeDeviceRelatedData userInfo, int deviceNumber, DtoEmployeeEnrolledSetting setting)
        {
            NewUserEnrolled?.Invoke(userInfo, deviceNumber, setting);
        }

        public void PublishNewFaceEnrolled(DtoEmployeeFace face, int deviceNumber)
        {
            NewFaceEnrolled?.Invoke(face, deviceNumber);
        }

        public void PublishZkOperationLogData(DtoZkOperationLog operationLog)
        {
            ZkOperationLogDataReceived?.Invoke(operationLog);
        }

        public void PublishDeviceEventLogData(DtoDeviceEventLog eventLog)
        {
            DeviceEventLogDataReceived?.Invoke(eventLog);
        }

        public void PublishNewPalmEnrolled(DtoEmployeePalm palm, int deviceNumber)
        {
            NewPalmEnrolled?.Invoke(palm, deviceNumber);
        }

        public void PublishNewIrisEnrolled(DtoEmployeeIris palm, int deviceNumber)
        {
            NewIrisEnrolled?.Invoke(palm, deviceNumber);
        }

        public void PublishNewFingerEnrolled(DtoEmployeeFinger finger, int deviceNumber)
        {
            NewFingerEnrolled?.Invoke(finger, deviceNumber);
        }

        public void PublishNewCardEnrolled(string card, int deviceNumber, long userId)
        {
            NewCardEnrolled?.Invoke(card, deviceNumber, userId);
        }

        public void PublishCommandSentToDevice(List<int> commandIds)
        {
            CommandSentToDevice?.Invoke(commandIds);
        }

        public void PublishUserProfileImageReceived(DtoEmployeeImage employeeImage, int deviceNumber)
        {
            UserProfileImageReceived?.Invoke(employeeImage, deviceNumber);
        }

        public void PublishCommandResponseReceived(DtoDeviceCommandProcessingResult deviceResponse)
        {
            CommandResponseReceived?.Invoke(deviceResponse);
        }

        public void PublishCommandDescriptionReceived(DtoDeviceCommandProcessingDescription commandDescription)
        {
            CommandDescriptionReceived?.Invoke(commandDescription);
        }

        public void PublishUserCountReceived(int deviceNumber, int count)
        {
            UserCountReceived?.Invoke(deviceNumber, count);
        }

        public void PublishAccessLogCountReceived(int deviceNumber, int count)
        {
            AccessLogCountReceived?.Invoke(deviceNumber, count);
        }

        public void PublishUserChangedReceived(int deviceNumber, long employeeNumber)
        {
            UserChanged?.Invoke(deviceNumber, employeeNumber);
        }

        public void PublishFaceCountReceived(int deviceNumber, int count)
        {
            FaceCountReceived?.Invoke(deviceNumber, count);
        }

        public void PublishFingerCountReceived(int deviceNumber, int count)
        {
            FingerCountReceived?.Invoke(deviceNumber, count);
        }

    }
}
