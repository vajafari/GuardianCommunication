using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;

namespace GuardianCommunication.Hardware.Shared
{
    public class HardwareEventPublisher
    {
        public event Action<DtoAttendance> AttendanceReceived;
        public event Action<DtoInvalidAttendance> InvalidAttendanceReceived;
        public event Action<DtoDeviceAttendanceImage> AttendanceImageReceived;
        public event Action<DtoDeviceUnauthorizedAttendanceImage> UnauthorizedAttendanceImageReceived;
        public event Action<string, DateTime, byte[]> XRayDeviceDataReceived;
        public event Action<List<DtoDeviceConnectionStatus>> DeviceConnectionStatusChanged;
        public event Action<DtoUserDeviceRelatedData, int, DtoUserEnrolledSetting> NewUserEnrolled;
        public event Action<DtoUserFace, int> NewFaceEnrolled;
        public event Action<string, int, long> NewCardEnrolled;
        public event Action<DtoZkOperationLog> ZkOperationLogDataReceived;
        public event Action<DtoDeviceEventLog> DeviceEventLogDataReceived;
        public event Action<DtoUserPalm, int> NewPalmEnrolled;
        public event Action<DtoUserIris, int> NewIrisEnrolled;
        public event Action<DtoUserFinger, int> NewFingerEnrolled;
        public event Action<List<int>> CommandSentToDevice;
        public event Action<DtoDeviceCommandProcessingResult> CommandResponseReceived;
        public event Action<DtoDeviceCommandProcessingDescription> CommandDescriptionReceived;
        public event Action<DtoUserImage, int> UserProfileImageReceived;
        public event Action<int, int> UserCountReceived;
        public event Action<int, int> FaceCountReceived;
        public event Action<int, int> FingerCountReceived;
        public event Action<int, int> AccessLogCountReceived;
        public event Action<int, long> UserChanged;
        public event Action<List<int>> FaceDetectionCameraCommandSent;

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


        public void PublishInvalidAttendance(DtoInvalidAttendance attendance)
        {
            InvalidAttendanceReceived?.Invoke(attendance);
        }

        public void PublishAttendance(DtoAttendance attendance)
        {
            AttendanceReceived?.Invoke(attendance);
        }

        public void PublishAttendanceImage(DtoDeviceAttendanceImage image)
        {
            AttendanceImageReceived?.Invoke(image);
        }

        public void PublishUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {
            UnauthorizedAttendanceImageReceived?.Invoke(image);
        }

        public void PublishXRayDeviceDataReceived(string deviceId, DateTime date, byte[] scanData)
        {
            XRayDeviceDataReceived?.Invoke(deviceId, date, scanData);
        }

        public void PublishDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            DeviceConnectionStatusChanged?.Invoke(deviceConnectionStatuses);
        }

        public void PublishNewUserEnrolled(DtoUserDeviceRelatedData userInfo, int deviceNumber, DtoUserEnrolledSetting setting)
        {
            NewUserEnrolled?.Invoke(userInfo, deviceNumber, setting);
        }

        public void PublishNewFaceEnrolled(DtoUserFace face, int deviceNumber)
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

        public void PublishNewPalmEnrolled(DtoUserPalm palm, int deviceNumber)
        {
            NewPalmEnrolled?.Invoke(palm, deviceNumber);
        }

        public void PublishNewIrisEnrolled(DtoUserIris palm, int deviceNumber)
        {
            NewIrisEnrolled?.Invoke(palm, deviceNumber);
        }

        public void PublishNewFingerEnrolled(DtoUserFinger finger, int deviceNumber)
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

        public void PublishUserProfileImageReceived(DtoUserImage employeeImage, int deviceNumber)
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
