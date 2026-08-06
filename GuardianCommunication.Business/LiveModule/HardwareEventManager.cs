using System;
using System.Collections.Generic;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared;

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
            HardwareEventPublisher.Instance.NotLivePlateDetectionCameraCarAttendanceReceived += NotLivePlateDetectionCameraCarAttendanceReceived;
            HardwareEventPublisher.Instance.LivePlateDetectionCameraCarAttendanceReceived += LivePlateDetectionCameraCarAttendanceReceived;
            HardwareEventPublisher.Instance.AttendanceImageReceived += AttendanceImageReceived;
            HardwareEventPublisher.Instance.UnauthorizedAttendanceImageReceived += UnauthorizedAttendanceImageReceived;
            HardwareEventPublisher.Instance.MetalDetectorPersonPassed += MetalDetectorPersonPassed;
            HardwareEventPublisher.Instance.XRayDeviceDataReceived += XRayDeviceDataReceived;
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
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandSent += CameraCommandSent;
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandResponseReceived += CameraCommandResponseReceived;
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandSetDescription += CameraCommandSetDescription;
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

        private void CameraCommandSent(List<int> param)
        {
            var thread = new Thread(() => ProcessFaceDetectionCameraCommandSent(param));
            thread.Start();
        }

        private void CameraCommandSetDescription(DtoOtherHardwareCommandProcessingDescription param)
        {
            var thread = new Thread(() => ProcessCameraCommandSetDescription(param));
            thread.Start();
        }

        private void CameraCommandResponseReceived(DtoOtherHardwareCommandProcessingResult param)
        {
            var thread = new Thread(() => ProcessCameraCommandResponseReceived(param));
            thread.Start();
        }

        private void MetalDetectorPersonPassed(DtoMetalDetectorPersonPassedData passData)
        {
            var thread = new Thread(() => SendMetalDetectorPersonPassed(passData));
            thread.Start();
        }

        private void XRayDeviceDataReceived(string deviceId, DateTime date, byte[] scanData)
        {
            var thread = new Thread(() => SendXRayDeviceScanData(deviceId, date, scanData));
            thread.Start();
        }

        private void CommandSentToDevice(List<int> commandIds)
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

        private void UserProfileImageReceived(DtoEmployeeImage employeeImage, int deviceNumber)
        {
            var thread = new Thread(() => SubmitUserProfileImageReceived(employeeImage, deviceNumber));
            thread.Start();
        }

        private void NewFingerEnrolled(DtoEmployeeFinger fingerInfo, int deviceNumber)
        {
            var thread = new Thread(() => SubmitFingerEnrolled(fingerInfo, deviceNumber));
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

        private void NewCardEnrolled(string finger, int deviceNumber, long userId)
        {
            var thread = new Thread(() => SubmitCardEnrolled(finger, deviceNumber, userId));
            thread.Start();
        }

        private void NewFaceEnrolled(DtoEmployeeFace faceInfo, int deviceNumber)
        {
            var thread = new Thread(() => SubmitFaceEnrolled(faceInfo, deviceNumber));
            thread.Start();
        }

        private void NewPalmEnrolled(DtoEmployeePalm palm, int deviceNumber)
        {
            var thread = new Thread(() => SubmitPalmEnrolled(palm, deviceNumber));
            thread.Start();
        }

        private void NewIrisEnrolled(DtoEmployeeIris iris, int deviceNumber)
        {
            var thread = new Thread(() => SubmitIrisEnrolled(iris, deviceNumber));
            thread.Start();
        }

        private void NewUserEnrolled(DtoEmployeeDeviceRelatedData userInfo, int deviceNumber, DtoEmployeeEnrolledSetting setting)
        {
            var thread = new Thread(() => SubmitUserEnrolled(userInfo, deviceNumber, setting));
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

        private void NotLivePlateDetectionCameraCarAttendanceReceived(DtoPlateDetectionCameraCarAttendance attendance)
        {
            try
            {
                var thread = new Thread(() => ProcessNotLiveValidPlateDetectionCameraCarAttendance(attendance));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void LivePlateDetectionCameraCarAttendanceReceived(DtoPlateDetectionCameraCarAttendance attendance)
        {
            try
            {
                var thread = new Thread(() => ProcessLivePlateDetectionCameraCarAttendance(attendance));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void UserCountReceived(int deviceNumber, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitUserCount(deviceNumber, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void AccessLogCountReceived(int deviceNumber, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitAccessLogRecordCount(deviceNumber, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void UserChangedReceived(int deviceNumber, long employeeNumber)
        {
            try
            {
                var thread = new Thread(() => SubmitUserChanged(deviceNumber, employeeNumber));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void FaceCountReceived(int deviceNumber, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitFaceCount(deviceNumber, count));
                thread.Start();

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private void FingerCountReceived(int deviceNumber, int count)
        {
            try
            {
                var thread = new Thread(() => SubmitFingerCount(deviceNumber, count));
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



        private void ProcessCameraCommandSetDescription(DtoOtherHardwareCommandProcessingDescription param)
        {
            try
            {
                var command = new OtherHardwareCommandComponent(_repositoryFactory);
                command.SetDescription(param);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessFaceDetectionCameraCommandSetDescription thread", param);
            }
        }

        private void ProcessCameraCommandResponseReceived(DtoOtherHardwareCommandProcessingResult param)
        {
            try
            {
                var command = new OtherHardwareCommandComponent(_repositoryFactory);
                command.SetResponse(param);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessFaceDetectionCameraCommandResponseReceived thread", param);
            }
        }

        private void ProcessFaceDetectionCameraCommandSent(List<int> param)
        {
            try
            {
                var command = new OtherHardwareCommandComponent(_repositoryFactory);
                command.UpdateSendData(param);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessFaceDetectionCameraCommandSent thread", param);
            }
        }


        private void SendMetalDetectorPersonPassed(DtoMetalDetectorPersonPassedData passData)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendMetalDetectorPersonPassed(passData);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendMetalDetectorPersonPassed thread", passData);
            }
        }

        private void SendXRayDeviceScanData(string deviceId, DateTime date, byte[] scanData)
        {
            try
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherXRayPublish))
                {
                    LoggingSystem.LogInfo("EventPublisher.SendXRayDeviceScanData", new
                    {
                        DeviceId = deviceId,
                        Date = date,
                        ScanDataLenght = scanData?.Length ?? 0
                    });
                }

                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendXRayDeviceScanData(deviceId, date, scanData);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendXRayDeviceScanData thread",
                    ObjectHelper.SerializeAsJson(new { DeviceID = deviceId, Date = date, ScanData = scanData }));
            }
        }

        private void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {
            try
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherSendAttendanceImage))
                {
                    LoggingSystem.LogInfo("EventPublisher.SendAttendanceImage", image);
                }
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendAttendanceImage(image);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendAttendanceImage thread", new
                {
                    image.DeviceNumber,
                    image.AttendanceDateTime,
                    image.EmployeeNumber,
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
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendUnauthorizedAttendanceImage(image);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SendUnauthorizedAttendanceImage thread", new
                {
                    image.DeviceNumber,
                    image.AttendanceDateTime,
                    ImageSize = image.Image?.Length ?? 0
                });
            }
        }

        private void SetCommandsAsSent(List<int> commandIds)
        {
            try
            {
                var commandComponent = new DeviceCommandComponent(_repositoryFactory);
                commandComponent.UpdateSendData(commandIds);
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
                if (!entity.DeviceNumber.HasValue)
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
                    attendanceComponent.SubmitInvalidIoEventToKarnama(entity);
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
            var karnamaComponent = new KarnamaComponent(_repositoryFactory);
            try
            {
                if (!entity.DeviceNumber.HasValue)
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
                var deviceCache = deviceComponent.GetDeviceByDeviceNumber(entity.DeviceNumber.Value);

                // تردد های سلف
                if (deviceCache != null && deviceCache.ApplicationId.HasFlag(ApplicationTypeEnumeration.Self))
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance is self attendance", entity);
                    }
                    try
                    {

                        if (!deviceComponent.IsDeviceNumberValid(entity.DeviceNumber.Value, ValidSerialNumberCheckTypeEnumeration.Attendance))
                        {
                            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                            {
                                LoggingSystem.LogInfo("EventPublisher.ProcessAttendance Invalid device attendance", new
                                {
                                    Atendance = entity,
                                });
                            }
                            return;
                        }

                        if (ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Self))
                        {
                            karnamaComponent.SendSelfEvent(entity);
                        }
                        else
                        {
                            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                            {
                                LoggingSystem.LogInfo("EventPublisher.ProcessAttendance Invalid Self Attendance", new
                                {
                                    Atendance = entity,
                                });
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on SendSelfEvent To karnama thread", ObjectHelper.SerializeAsJson(entity));
                    }
                }
                // تردد های افلاین دستگاه های پارکینگ
                // تردد هایی که به این نقطه می رسند یعنی در مود آفلاین دریافت شده اند
                // چون تردد های آنلاین از مسیر دیگری در سیستم ثبت می شوند
                else if (deviceCache != null
                         && ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Parking)
                         && deviceCache.ApplicationId.HasFlag(ApplicationTypeEnumeration.Parking)
                         && deviceCache.DeviceSettings.HasFlag(DeviceSettingsEnumeration.ServerMatch)
                         && entity.RfCardNumber.IsNotNullOrEmpty())
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance is online access control attendance", entity);
                    }
                    try
                    {
                        if (!deviceComponent.IsDeviceNumberValid(entity.DeviceNumber.Value, ValidSerialNumberCheckTypeEnumeration.Attendance))
                        {
                            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                            {
                                LoggingSystem.LogInfo("EventPublisher.ProcessAttendance Invalid device attendance", new
                                {
                                    Atendance = entity,
                                });
                            }
                            return;
                        }
                        if (attendanceComponent.IsAttendanceDuplicate(entity))
                        {
                            return;
                        }
                        var resultLiveCheck = attendanceComponent.SubmitIoEventToKarnama(entity);
                        if (resultLiveCheck.IsAttendanceSavedAtKarnama())
                        {
                            entity.IsSent = true;
                            var resultOfSave = attendanceComponent.SaveAttendance
                                (new List<DtoAttendance> { entity }, false, true, true);
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
                // تردد دستگاه های کنترل دسترسی آنلاین
                else if (deviceCache != null && deviceCache.HasSoftwareAccessControl)
                {

                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance is online access control attendance", entity);
                    }
                    try
                    {
                        if (!deviceComponent.IsDeviceNumberValid(entity.DeviceNumber.Value, ValidSerialNumberCheckTypeEnumeration.Attendance))
                        {
                            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                            {
                                LoggingSystem.LogInfo("EventPublisher.ProcessAttendance Invalid device attendance", new
                                {
                                    Atendance = entity,
                                });
                            }
                            return;
                        }
                        if (attendanceComponent.IsAttendanceDuplicate(entity))
                        {
                            return;
                        }
                        var resultLiveCheck = attendanceComponent.SubmitIoEventToKarnama(entity);
                        if (resultLiveCheck.IsAttendanceSavedAtKarnama())
                        {
                            entity.IsSent = true;
                            var resultOfSave = attendanceComponent.SaveAttendance
                                (new List<DtoAttendance> { entity }, false, true, true);
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
                    entity.IsSent = false;
                    var resultOfSave = attendanceComponent.SaveAttendance(new List<DtoAttendance> { entity }, true, true, true);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherAttendanceProcess))
                    {
                        LoggingSystem.LogInfo("EventPublisher.ProcessAttendance attendance save result", entity);
                    }
                    if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
                    {
                        var newEntity = resultOfSave.Successful.First();
                        try
                        {
                            var resultOfSubmit = attendanceComponent.SubmitIoEventToKarnama(newEntity);
                            if (resultOfSubmit.IsAttendanceSavedAtKarnama())
                            {
                                attendanceComponent.MarkAttendanceAsSentToKarnama(new List<DtoAttendance> { entity });
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

        private void ProcessNotLiveValidPlateDetectionCameraCarAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherCameraEvent))
            {
                LoggingSystem.LogInfo("EventPublisher.ProcessNotLiveValidPlateDetectionCameraCarAttendance", attendance);
            }
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SubmitNotLiveValidPlateDetectionCameraAttendance(attendance);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessNotLiveValidPlateDetectionCameraCarAttendance thread", attendance);
            }
        }

        private void ProcessLivePlateDetectionCameraCarAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.HardwarePublisherCameraEvent))
            {
                LoggingSystem.LogInfo("EventPublisher.ProcessLivePlateDetectionCameraCarAttendance", attendance);
            }
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SubmitLivePlateDetectionCameraAttendance(attendance);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessLivePlateDetectionCameraCarAttendance thread", attendance);
            }
        }

        private void SubmitDeviceConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendDeviceConnectionStatusChanged(deviceConnectionStatuses);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitDeviceConnectionStatusChanged thread", ObjectHelper.SerializeAsJson(deviceConnectionStatuses));
            }

        }

        private void SubmitUserProfileImageReceived(DtoEmployeeImage employeeImage, int deviceNumber)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendUserProfileImage(employeeImage, deviceNumber);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserProfileImageReceived thread", ObjectHelper.SerializeAsJson(employeeImage));
            }
        }

        private void SubmitFingerEnrolled(DtoEmployeeFinger fingerInfo, int deviceNumber)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendFingerEnrolled(fingerInfo, deviceNumber);
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
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
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
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendDeviceEventLog(operationLog);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitDeviceEventLog thread", ObjectHelper.SerializeAsJson(operationLog));
            }

        }

        private void SubmitCardEnrolled(string cardNumber, int deviceNumber, long userId)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendCardEnrolled(cardNumber, deviceNumber, userId);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitCardEnrolled thread", ObjectHelper.SerializeAsJson(cardNumber));
            }
        }

        private void SubmitFaceEnrolled(DtoEmployeeFace faceInfo, int deviceNumber)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendFaceEnrolled(faceInfo, deviceNumber);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFaceEnrolled thread", ObjectHelper.SerializeAsJson(faceInfo));
            }
        }

        private void SubmitPalmEnrolled(DtoEmployeePalm palmInfo, int deviceNumber)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendPalmEnrolled(palmInfo, deviceNumber);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitPalmEnrolled thread", ObjectHelper.SerializeAsJson(palmInfo));
            }
        }

        private void SubmitIrisEnrolled(DtoEmployeeIris irisInfo, int deviceNumber)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendIrisEnrolled(irisInfo, deviceNumber);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitIrisEnrolled thread", ObjectHelper.SerializeAsJson(irisInfo));
            }
        }

        private void SubmitUserEnrolled(DtoEmployeeDeviceRelatedData userInfo, int deviceNumber, DtoEmployeeEnrolledSetting setting)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendUserEnrolled(userInfo, deviceNumber, setting);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserEnrolled thread", ObjectHelper.SerializeAsJson(userInfo));
            }
        }

        private void SubmitUserCount(int deviceNumber, int count)

        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendUserCount(deviceNumber, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceNumber }));
            }
        }

        private void SubmitAccessLogRecordCount(int deviceNumber, int count)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendAccessLogCount(deviceNumber, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitAccessLogRecordCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceNumber }));
            }
        }

        private void SubmitFaceCount(int deviceNumber, int count)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendFaceCount(deviceNumber, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFaceCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceNumber }));
            }
        }

        private void SubmitFingerCount(int deviceNumber, int count)
        {
            try
            {
                var karnamaComponent = new KarnamaComponent(_repositoryFactory);
                karnamaComponent.SendFingerCount(deviceNumber, count);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitFingerCount thread",
                    ObjectHelper.SerializeAsJson(new { count, deviceNumber }));
            }
        }

        private void SubmitUserChanged(int deviceNumber, long employeeNumber)
        {
            try
            {
                var deviceComponent = new DeviceComponent(_repositoryFactory);
                var deviceInCache = deviceComponent.GetCommunicationDeviceDataByDeviceNumber(deviceNumber);
                if (deviceInCache != null && deviceInCache.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                {
                    var communicationComponent = new CommunicationComponent(_repositoryFactory);
                    communicationComponent.CommunicationGetUserById
                        (deviceInCache, employeeNumber, TemplateTypeEnumeration.All);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on SubmitUserChanged thread",
                    ObjectHelper.SerializeAsJson(new { deviceNumber, employeeNumber }));
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
            HardwareEventPublisher.Instance.NotLivePlateDetectionCameraCarAttendanceReceived -= NotLivePlateDetectionCameraCarAttendanceReceived;
            HardwareEventPublisher.Instance.LivePlateDetectionCameraCarAttendanceReceived -= LivePlateDetectionCameraCarAttendanceReceived;
            HardwareEventPublisher.Instance.MetalDetectorPersonPassed -= MetalDetectorPersonPassed;
            HardwareEventPublisher.Instance.AttendanceImageReceived -= AttendanceImageReceived;
            HardwareEventPublisher.Instance.XRayDeviceDataReceived -= XRayDeviceDataReceived;
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
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandSent -= CameraCommandSent;
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandResponseReceived -= CameraCommandResponseReceived;
            HardwareEventPublisher.Instance.FaceDetectionCameraCommandSetDescription -= CameraCommandSetDescription;
            _disposed = true;
        }

        ~HardwareEventManager()
        {
            Dispose(false);
        }

        #endregion

    }
}
