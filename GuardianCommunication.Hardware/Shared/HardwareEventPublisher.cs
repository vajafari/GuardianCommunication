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
        public event Action<Guid, DateTime, byte[]> XRayDeviceDataReceived;
        public event Action<List<DtoDeviceConnectionStatus>> DeviceConnectionStatusChanged;
        public event Action<DtoUserDeviceRelatedData, Guid, DtoUserEnrolledSetting> NewUserEnrolled;
        public event Action<DtoUserFace, Guid> NewFaceEnrolled;
        public event Action<string, Guid, long> NewCardEnrolled;
        public event Action<DtoZkOperationLog> ZkOperationLogDataReceived;
        public event Action<DtoDeviceEventLog> DeviceEventLogDataReceived;
        public event Action<DtoUserPalm, Guid> NewPalmEnrolled;
        public event Action<DtoUserIris, Guid> NewIrisEnrolled;
        public event Action<DtoUserFinger, Guid> NewFingerEnrolled;
        public event Action<List<int>> CommandSentToDevice;
        public event Action<DtoDeviceCommandProcessingResult> CommandResponseReceived;
        public event Action<DtoDeviceCommandProcessingDescription> CommandDescriptionReceived;
        public event Action<DtoUserImage, Guid> UserProfileImageReceived;
        public event Action<Guid, int> UserCountReceived;
        public event Action<Guid, int> FaceCountReceived;
        public event Action<Guid, int> FingerCountReceived;
        public event Action<Guid, int> AccessLogCountReceived;
        public event Action<Guid, long> UserChanged;

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

        public void PublishXRayDeviceDataReceived(Guid deviceId, DateTime date, byte[] scanData)
        {
            XRayDeviceDataReceived?.Invoke(deviceId, date, scanData);
        }

        public void PublishDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            DeviceConnectionStatusChanged?.Invoke(deviceConnectionStatuses);
        }

        public void PublishNewUserEnrolled(DtoUserDeviceRelatedData userInfo, Guid deviceId, DtoUserEnrolledSetting setting)
        {
            NewUserEnrolled?.Invoke(userInfo, deviceId, setting);
        }

        public void PublishNewFaceEnrolled(DtoUserFace face, Guid deviceId)
        {
            NewFaceEnrolled?.Invoke(face, deviceId);
        }

        public void PublishZkOperationLogData(DtoZkOperationLog operationLog)
        {
            ZkOperationLogDataReceived?.Invoke(operationLog);
        }

        public void PublishDeviceEventLogData(DtoDeviceEventLog eventLog)
        {
            DeviceEventLogDataReceived?.Invoke(eventLog);
        }

        public void PublishNewPalmEnrolled(DtoUserPalm palm, Guid deviceId)
        {
            NewPalmEnrolled?.Invoke(palm, deviceId);
        }

        public void PublishNewIrisEnrolled(DtoUserIris palm, Guid deviceId)
        {
            NewIrisEnrolled?.Invoke(palm, deviceId);
        }

        public void PublishNewFingerEnrolled(DtoUserFinger finger, Guid deviceId)
        {
            NewFingerEnrolled?.Invoke(finger, deviceId);
        }

        public void PublishNewCardEnrolled(string card, Guid deviceId, long userId)
        {
            NewCardEnrolled?.Invoke(card, deviceId, userId);
        }

        public void PublishCommandSentToDevice(List<int> commandIds)
        {
            CommandSentToDevice?.Invoke(commandIds);
        }

        public void PublishUserProfileImageReceived(DtoUserImage employeeImage, Guid deviceId)
        {
            UserProfileImageReceived?.Invoke(employeeImage, deviceId);
        }

        public void PublishCommandResponseReceived(DtoDeviceCommandProcessingResult deviceResponse)
        {
            CommandResponseReceived?.Invoke(deviceResponse);
        }

        public void PublishCommandDescriptionReceived(DtoDeviceCommandProcessingDescription commandDescription)
        {
            CommandDescriptionReceived?.Invoke(commandDescription);
        }

        public void PublishUserCountReceived(Guid deviceId, int count)
        {
            UserCountReceived?.Invoke(deviceId, count);
        }

        public void PublishAccessLogCountReceived(Guid deviceId, int count)
        {
            AccessLogCountReceived?.Invoke(deviceId, count);
        }

        public void PublishUserChangedReceived(Guid deviceId, long employeeNumber)
        {
            UserChanged?.Invoke(deviceId, employeeNumber);
        }

        public void PublishFaceCountReceived(Guid deviceId, int count)
        {
            FaceCountReceived?.Invoke(deviceId, count);
        }

        public void PublishFingerCountReceived(Guid deviceId, int count)
        {
            FingerCountReceived?.Invoke(deviceId, count);
        }

    }
}
