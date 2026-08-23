using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.LiveModule
{
    public class HardwareEventManager : IDisposable
    {
        private readonly RepositoryFactory _repositoryFactory;

        #region Singleton

        public static HardwareEventManager Instance { get; }

        public void ManageHardwareEvents() { }

        private HardwareEventManager()
        {
            _repositoryFactory = new RepositoryFactory();
            HardwareEventPublisher.Instance.AttendanceReceived += AttendanceReceived;
            HardwareEventPublisher.Instance.InvalidAttendanceReceived += InvalidAttendanceReceived;
            HardwareEventPublisher.Instance.AttendanceImageReceived += AttendanceImageReceived;
            HardwareEventPublisher.Instance.UnauthorizedAttendanceImageReceived += UnauthorizedAttendanceImageReceived;
            HardwareEventPublisher.Instance.DeviceConnectionStatusChanged += DeviceConnectionStatusChanged;
            HardwareEventPublisher.Instance.NewUserEnrolled += NewUserEnrolled;
            HardwareEventPublisher.Instance.NewFaceEnrolled += NewFaceEnrolled;
            HardwareEventPublisher.Instance.NewCardEnrolled += NewCardEnrolled;
            HardwareEventPublisher.Instance.NewPalmEnrolled += NewPalmEnrolled;
            HardwareEventPublisher.Instance.NewIrisEnrolled += NewIrisEnrolled;
            HardwareEventPublisher.Instance.NewFingerEnrolled += NewFingerEnrolled;
            HardwareEventPublisher.Instance.CommandResponseReceived += CommandResponseReceived;
            HardwareEventPublisher.Instance.CommandDescriptionReceived += CommandDescriptionReceived;
            HardwareEventPublisher.Instance.CommandSentToDevice += CommandSentToDevice;
            HardwareEventPublisher.Instance.ZkOperationLogDataReceived += OperationZkLogDataReceived;
            HardwareEventPublisher.Instance.DeviceEventLogDataReceived += DeviceEventLogDataReceived;
            HardwareEventPublisher.Instance.UserProfileImageReceived += UserProfileImageReceived;
            HardwareEventPublisher.Instance.UserCountReceived += UserCountReceived;
            HardwareEventPublisher.Instance.AccessLogCountReceived += AccessLogCountReceived;
            HardwareEventPublisher.Instance.FingerCountReceived += FingerCountReceived;
            HardwareEventPublisher.Instance.FaceCountReceived += FaceCountReceived;
            HardwareEventPublisher.Instance.UserChanged += UserChangedReceived;
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.StartupLog))
            {
                LoggingSystem.LogInfo("Initialize HardwareEventManager called");
            }
        }

        static HardwareEventManager()
        {
            Instance = new HardwareEventManager();
        }

        #endregion

        private void CommandSentToDevice(List<long> commandIds)
        {
            SetCommandsAsSent(commandIds);
        }

        private void CommandResponseReceived(DtoDeviceCommandProcessingResult response)
        {
            SetCommandResponse(response);
        }

        private void CommandDescriptionReceived(DtoDeviceCommandProcessingDescription commandDescription)
        {
            SetCommandDescription(commandDescription);
        }

        private void UserProfileImageReceived(DtoUserImage userImage, Guid deviceId)
        {
            var thread = new Thread(() => SubmitUserProfileImageReceived(userImage, deviceId));
            thread.Start();
        }

        private void NewFingerEnrolled(DtoUserFinger fingerInfo, Guid deviceId)
        {
            var thread = new Thread(() => SubmitFingerEnrolled(fingerInfo, deviceId));
            thread.Start();
        }

        private void OperationZkLogDataReceived(DtoZkOperationLog operationLog)
        {
            var thread = new Thread(() => SubmitZkOperationLog(operationLog));
            thread.Start();
        }

        private void DeviceEventLogDataReceived(DtoDeviceEventLog operationLog)
        {
            var thread = new Thread(() => SubmitDeviceEventLog(operationLog));
            thread.Start();
        }

        private void NewCardEnrolled(string finger, Guid deviceId, long userId)
        {
            var thread = new Thread(() => SubmitCardEnrolled(finger, deviceId, userId));
            thread.Start();
        }

        private void NewFaceEnrolled(DtoUserFace faceInfo, Guid deviceId)
        {
            var thread = new Thread(() => SubmitFaceEnrolled(faceInfo, deviceId));
            thread.Start();
        }

        private void NewPalmEnrolled(DtoUserPalm palm, Guid deviceId)
        {
            var thread = new Thread(() => SubmitPalmEnrolled(palm, deviceId));
            thread.Start();
        }

        private void NewIrisEnrolled(DtoUserIris iris, Guid deviceId)
        {
            var thread = new Thread(() => SubmitIrisEnrolled(iris, deviceId));
            thread.Start();
        }

        private void NewUserEnrolled(DtoUserDeviceRelatedData userInfo, Guid deviceId, DtoUserEnrolledSetting setting)
        {
            var thread = new Thread(() => SubmitUserEnrolled(userInfo, deviceId, setting));
            thread.Start();
        }

        private void DeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            var thread = new Thread(() => SubmitDeviceConnectionStatusChanged(deviceConnectionStatuses));
            thread.Start();

        }

        private void InvalidAttendanceReceived(DtoInvalidAttendance attendance)
        {
            try
            {
                var thread = new Thread(() => ProcessInvalidAttendance(attendance));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void AttendanceReceived(DtoAttendance attendance)
        {
            try
            {
                var thread = new Thread(() => ProcessAttendance(attendance));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void UserCountReceived(Guid deviceId, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitUserCount(deviceId, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void AccessLogCountReceived(Guid deviceId, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitAccessLogRecordCount(deviceId, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void UserChangedReceived(Guid deviceId, long userNumber)
        {
            try
            {
                var thread = new Thread(() => SubmitUserChanged(deviceId, userNumber));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void FaceCountReceived(Guid deviceId, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitFaceCount(deviceId, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void FingerCountReceived(Guid deviceId, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitFingerCount(deviceId, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void AttendanceImageReceived(DtoDeviceAttendanceImage image)
        {
            var thread = new Thread(() => SendAttendanceImage(image));
            thread.Start();
        }

        private void UnauthorizedAttendanceImageReceived(DtoDeviceUnauthorizedAttendanceImage image)
        {
            var thread = new Thread(() => SendUnauthorizedAttendanceImage(image));
            thread.Start();
        }

        #region Thread Handlers



        private void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {
            try
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherSendAttendanceImage))
                {
                    LoggingSystem.LogInfo("EventPublisher.SendAttendanceImage", image);
                }
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendAttendanceImage(image);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendAttendanceImage thread", new
                {
                    image.DeviceId,
                    image.AttendanceDateTime,
                    image.UserIdOnDevice,
                    ImageSize = image.Image?.Length ?? 0
                });
            }
        }

        private void SendUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {
            try
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherSendAttendanceImage))
                {
                    LoggingSystem.LogInfo("EventPublisher.SendUnauthorizedAttendanceImage", image);
                }
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendUnauthorizedAttendanceImage(image);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendUnauthorizedAttendanceImage thread", new
                {
                    image.DeviceId,
                    image.AttendanceDateTime,
                    ImageSize = image.Image?.Length ?? 0
                });
            }
        }

        private void SetCommandsAsSent(List<long> commandIds)
        {
            try
            {
                var commandComponent = new DeviceCommandComponent(_repositoryFactory);
                commandComponent.UpdateSendDataByNumericIds(commandIds);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SetCommandsAsSent thread");
            }

        }

        private void SetCommandResponse(DtoDeviceCommandProcessingResult response)
        {
            try
            {
                var commandComponent = new DeviceCommandComponent(_repositoryFactory);
                commandComponent.SetResponse(response);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SetCommandResponse thread");
            }

        }

        private void SetCommandDescription(DtoDeviceCommandProcessingDescription commandDescription)
        {
            try
            {
                var commandComponent = new DeviceCommandComponent(_repositoryFactory);
                commandComponent.SetDescription(commandDescription);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SetCommandDescription thread");
            }

        }

        private void ProcessInvalidAttendance(DtoInvalidAttendance entity)
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherInvalidAttendanceProcess))
            {
                LoggingSystem.LogInfo("EventPublisher.ProcessInvalidAttendance", entity);
            }
            try
            {
                if (!entity.DeviceId.HasValue)
                {
                    LoggingSystem.LogError("Invalid device", $"EventPublisher.ProcessInvalidAttendance Invalid hardware attendance {ObjectHelper.SerializeAsJsonFormatted(entity)}");
                    return;
                }
                if (ApplicationEmbeddedInfo.ExpireDate.HasValue && DateTime.Now > ApplicationEmbeddedInfo.ExpireDate.Value)
                {
                    return;
                }

                var attendanceComponent = new AttendanceComponent(_repositoryFactory);
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                {
                    LoggingSystem.LogInfo("EventPublisher.ProcessInvalidAttendance is in normal attendance", entity);
                }

                try
                {
                    attendanceComponent.SubmitInvalidIoEventToGuardian(entity);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "EventPublisher.ProcessInvalidAttendance Error on SendIoEvent To karnama thread", ObjectHelper.SerializeAsJson(entity));
                }


            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "EventPublisher.ProcessInvalidAttendance  Error on ProcessIoEvent thread");
            }
        }

        private void ProcessAttendance(DtoAttendance entity)
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
            {
                LoggingSystem.LogInfo("EventPublisher.ProcessAttendance", entity);
            }
            try
            {
                if (!entity.DeviceId.HasValue)
                {
                    LoggingSystem.LogError("Invalid device", $"EventPublisher.ProcessAttendance Invalid hardware attendance {ObjectHelper.SerializeAsJsonFormatted(entity)}");
                    return;
                }
                if (ApplicationEmbeddedInfo.ExpireDate.HasValue && DateTime.Now > ApplicationEmbeddedInfo.ExpireDate.Value)
                {
                    return;
                }
                var attendanceComponent = new AttendanceComponent(_repositoryFactory);
                var deviceComponent = new DeviceComponent(_repositoryFactory);
                var deviceCache = deviceComponent.GetDeviceCache(entity.DeviceId.Value);

                // تردد های سلف
                // تردد های افلاین دستگاه های پارکینگ
                // تردد هایی که به این نقطه می رسند یعنی در مود آفلاین دریافت شده اند
                // چون تردد های آنلاین از مسیر دیگری در سیستم ثبت می شوند
                if (deviceCache != null
                         && ApplicationEmbeddedInfo.Modules.HasFlag(ModuleEnumeration.Parking)
                         && deviceCache.ModuleId.HasFlag(ModuleEnumeration.Parking)
                         && deviceCache.DeviceSettings != null
                         && deviceCache.DeviceSettings.IsServerMatch
                         && entity.RfCardNumber.IsNotNullOrEmpty())
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance is online access control attendance", entity);
                    }
                    try
                    {
                        
                        if (attendanceComponent.IsAttendanceDuplicate(entity))
                        {
                            return;
                        }
                        var resultLiveCheck = attendanceComponent.SubmitIoEventToGuardian(entity);
                        if (resultLiveCheck.IsAttendanceSavedAtGuardian())
                        {
                            entity.IsSentToGuardian = true;
                            var resultOfSave = attendanceComponent.SaveAttendance
                                (new List<DtoAttendance> { entity }, false, true);
                            if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
                            {
                                try
                                {
                                    attendanceComponent.HookIoEvent(resultOfSave.Successful.First());
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp, "EventPublisher.ProcessAttendance Error on HookIoEvent to hook system thread");
                                }
                            }

                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "EventPublisher.ProcessAttendance Error on SendSelfEvent To karnama thread", ObjectHelper.SerializeAsJson(entity));
                    }
                }
                // سایر سایر تردد ها
                else
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance is in normal attendance", entity);
                    }
                    entity.IsSentToGuardian = false;
                    var resultOfSave = attendanceComponent.SaveAttendance(new List<DtoAttendance> { entity }, true, true);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance attendance save result", entity);
                    }
                    if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
                    {
                        var newEntity = resultOfSave.Successful.First();
                        try
                        {
                            var resultOfSubmit = attendanceComponent.SubmitIoEventToGuardian(newEntity);
                            if (resultOfSubmit.IsAttendanceSavedAtGuardian())
                            {
                                attendanceComponent.MarkAttendanceAsSentToGuardian(new List<DtoAttendance> { entity });
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "EventPublisher.ProcessAttendance Error on SendIoEvent To karnama thread", ObjectHelper.SerializeAsJson(newEntity));
                        }

                        try
                        {
                            attendanceComponent.HookIoEvent(newEntity);
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "EventPublisher.ProcessAttendance Error on HookIoEvent to hook system thread");
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "EventPublisher.ProcessAttendance  Error on ProcessIoEvent thread");
            }
        }

        private void SubmitDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendDeviceConnectionStatusChanged(deviceConnectionStatuses);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitDeviceConnectionStatusChanged thread", ObjectHelper.SerializeAsJson(deviceConnectionStatuses));
            }

        }

        private void SubmitUserProfileImageReceived(DtoUserImage userImage, Guid deviceId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendUserProfileImage(userImage, deviceId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserProfileImageReceived thread", ObjectHelper.SerializeAsJson(userImage));
            }
        }

        private void SubmitFingerEnrolled(DtoUserFinger fingerInfo, Guid deviceId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendFingerEnrolled(fingerInfo, deviceId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFingerEnrolled thread", ObjectHelper.SerializeAsJson(fingerInfo));
            }

        }

        private void SubmitZkOperationLog(DtoZkOperationLog operationLog)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendZkOperationLog(operationLog);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitZkOperationLog thread", ObjectHelper.SerializeAsJson(operationLog));
            }

        }

        private void SubmitDeviceEventLog(DtoDeviceEventLog operationLog)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendDeviceEventLog(operationLog);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitDeviceEventLog thread", ObjectHelper.SerializeAsJson(operationLog));
            }

        }

        private void SubmitCardEnrolled(string cardNumber, Guid deviceId, long userId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendCardEnrolled(cardNumber, deviceId, userId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitCardEnrolled thread", ObjectHelper.SerializeAsJson(cardNumber));
            }
        }

        private void SubmitFaceEnrolled(DtoUserFace faceInfo, Guid deviceId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendFaceEnrolled(faceInfo, deviceId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFaceEnrolled thread", ObjectHelper.SerializeAsJson(faceInfo));
            }
        }

        private void SubmitPalmEnrolled(DtoUserPalm palmInfo, Guid deviceId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendPalmEnrolled(palmInfo, deviceId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitPalmEnrolled thread", ObjectHelper.SerializeAsJson(palmInfo));
            }
        }

        private void SubmitIrisEnrolled(DtoUserIris irisInfo, Guid deviceId)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendIrisEnrolled(irisInfo, deviceId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitIrisEnrolled thread", ObjectHelper.SerializeAsJson(irisInfo));
            }
        }

        private void SubmitUserEnrolled(DtoUserDeviceRelatedData userInfo, Guid deviceId, DtoUserEnrolledSetting setting)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendUserEnrolled(userInfo, deviceId, setting);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserEnrolled thread", ObjectHelper.SerializeAsJson(userInfo));
            }
        }

        private void SubmitUserCount(Guid deviceId, int count)

        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendUserCount(deviceId, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceId }));
            }
        }

        private void SubmitAccessLogRecordCount(Guid deviceId, int count)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendAccessLogCount(deviceId, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitAccessLogRecordCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceId }));
            }
        }

        private void SubmitFaceCount(Guid deviceId, int count)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendFaceCount(deviceId, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFaceCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceId }));
            }
        }

        private void SubmitFingerCount(Guid deviceId, int count)
        {
            try
            {
                var karnamaComponent = new GuardianComponent(_repositoryFactory);
                karnamaComponent.SendFingerCount(deviceId, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFingerCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceId }));
            }
        }

        private void SubmitUserChanged(Guid deviceId, long userNumber)
        {
            try
            {
                var deviceComponent = new DeviceComponent(_repositoryFactory);
                var deviceInCache = deviceComponent.GetDeviceCache(deviceId);
                if (deviceInCache != null && deviceInCache.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                {
                    var communicationComponent = new CommunicationComponent(_repositoryFactory);
                    communicationComponent.CommunicationGetUserById
                        (deviceInCache.Id, userNumber, TemplateTypeEnumeration.All);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserChanged thread",
                    ObjectHelper.SerializeAsJson(new { deviceId, userNumber }));
            }
        }

        #endregion


        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Free managed resources
            }
            HardwareEventPublisher.Instance.AttendanceReceived -= AttendanceReceived;
            HardwareEventPublisher.Instance.InvalidAttendanceReceived -= InvalidAttendanceReceived;
            HardwareEventPublisher.Instance.AttendanceImageReceived -= AttendanceImageReceived;
            HardwareEventPublisher.Instance.DeviceConnectionStatusChanged -= DeviceConnectionStatusChanged;
            HardwareEventPublisher.Instance.NewUserEnrolled -= NewUserEnrolled;
            HardwareEventPublisher.Instance.NewFaceEnrolled -= NewFaceEnrolled;
            HardwareEventPublisher.Instance.NewPalmEnrolled -= NewPalmEnrolled;
            HardwareEventPublisher.Instance.NewIrisEnrolled -= NewIrisEnrolled;
            HardwareEventPublisher.Instance.NewFingerEnrolled -= NewFingerEnrolled;
            HardwareEventPublisher.Instance.NewCardEnrolled -= NewCardEnrolled;
            HardwareEventPublisher.Instance.CommandResponseReceived -= CommandResponseReceived;
            HardwareEventPublisher.Instance.CommandDescriptionReceived -= CommandDescriptionReceived;
            HardwareEventPublisher.Instance.CommandSentToDevice -= CommandSentToDevice;
            HardwareEventPublisher.Instance.ZkOperationLogDataReceived -= OperationZkLogDataReceived;
            HardwareEventPublisher.Instance.DeviceEventLogDataReceived -= DeviceEventLogDataReceived;
            HardwareEventPublisher.Instance.UserProfileImageReceived -= UserProfileImageReceived;
            HardwareEventPublisher.Instance.UserCountReceived -= UserCountReceived;
            HardwareEventPublisher.Instance.AccessLogCountReceived -= AccessLogCountReceived;
            HardwareEventPublisher.Instance.FaceCountReceived -= FaceCountReceived;
            HardwareEventPublisher.Instance.UserChanged -= UserChangedReceived;
            HardwareEventPublisher.Instance.FingerCountReceived -= FingerCountReceived;
            HardwareEventPublisher.Instance.UnauthorizedAttendanceImageReceived -= UnauthorizedAttendanceImageReceived;
            _disposed = true;
        }

        ~HardwareEventManager()
        {
            Dispose(false);
        }

        #endregion

    }
}
