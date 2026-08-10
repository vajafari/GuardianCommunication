using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.CommunicationModels.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public static class CommunicationModelMapper
    {

        public static DtoAttendance MapOtherResourcesAttendanceModelToDtoAttendance(OtherResourcesAttendanceModel inputItems)
        {
            return MapOtherResourcesAttendanceModelToDtoAttendance(new List<OtherResourcesAttendanceModel>() { inputItems }).FirstOrDefault();
        }

        public static List<DtoAttendance> MapOtherResourcesAttendanceModelToDtoAttendance(List<OtherResourcesAttendanceModel> inputItems)
        {
            var result = new List<DtoAttendance>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new DtoAttendance
                {
                    DeviceId = attendance.DeviceId,
                    ReaderDeviceId = attendance.ReaderDeviceId,
                    UserIdOnDevice = attendance.UserIdOnDevice,
                    VerificationStyle = attendance.VerificationStyle,
                    AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
                    AttendanceSource = attendance.AttendanceSource,
                    IsSentToGuardian = attendance.IsSentToGuardian,
                    Id = attendance.Id,
                    StatusCode = attendance.StatusCode,
                    ModuleId = attendance.ModuleId,
                    IoType = attendance.IoType,
                    RfCardNumber = attendance.RfCardNumber,
                    CameraId = attendance.CameraId,
                    DeviceAttendanceIoRetrieveType = null,
                    DoorId = attendance.DoorId,
                });
            }

            return result;
        }

        public static List<DtoAttendance> MapDeviceAttendanceModelToDtoAttendance(List<DeviceAttendanceModel> inputItems)
        {
            var result = new List<DtoAttendance>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                //result.Add(new DtoAttendance
                //{
                //    DeviceId = attendance.DeviceId,
                //    ReaderDeviceId = attendance.ReaderDeviceNumber,
                //    UserIdOnDevice = attendance.UserIdOnDevice,
                //    VerificationStyle = attendance.VerificationStyle,
                //    AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
                //    AttendanceSource = attendance.AttendanceSource,
                //    IsSentToGuardian = attendance.IsSentToGuardian,
                //    Id = attendance.Id,
                //    StatusCode = attendance.StatusCode,
                //    ModuleId = attendance.ModuleId,
                //    IoType = attendance.IoType,
                //    RfCardNumber = attendance.RfCardNumber,
                //    CameraId = attendance.CameraId,
                //    DeviceAttendanceIoRetrieveType = null,
                //    DoorId = attendance.DoorId,
                //});
            }

            return result;
        }

        public static List<DeviceAttendanceModel> MapDtoAttendanceToDeviceAttendanceModel(List<DtoAttendance> inputItems)
        {
            var result = new List<DeviceAttendanceModel>();
            //if (inputItems.IsCollectionNullOrEmpty())
            //{
            //    return result;
            //}

            //foreach (var attendance in inputItems)
            //{
            //    result.Add(new DeviceAttendanceModel
            //    {
            //        DeviceId = attendance.DeviceId,
            //        UserIdOnDevice = attendance.UserIdOnDevice,
            //        VerificationStyle = attendance.VerificationStyle,
            //        AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
            //        AttendanceSource = attendance.AttendanceSource,
            //        IsSentToGuardian = attendance.IsSent,
            //        Id = attendance.Id,
            //        StatusCode = attendance.StatusCode,
            //        RfCardNumber = attendance.RfCardNumber,
            //        IoType = attendance.IoType,
            //        ModuleId = attendance.ModuleId,
            //    });
            //}

            return result;
        }

        public static DeviceAttendanceModel MapDtoAttendanceToDeviceAttendanceModel(DtoAttendance inputItem)
        {
            return MapDtoAttendanceToDeviceAttendanceModel(new List<DtoAttendance> { inputItem }).FirstOrDefault();
        }

        public static List<DeviceInvalidAttendanceModel> MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(List<DtoInvalidAttendance> inputItems)
        {
            var result = new List<DeviceInvalidAttendanceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var attendance in inputItems)
            {
                //result.Add(new DeviceInvalidAttendanceModel
                //{
                //    DeviceId = attendance.DeviceNumber ?? 0,
                //    UserIdOnDevice = attendance.UserIdOnDevice,
                //    VerificationStyle = attendance.VerificationStyle ?? 0,
                //    AttendanceDateTime = attendance.AttendanceDateTime.ToNumericDateTime(),
                //    AttendanceSource = attendance.AttendanceSource,
                //    Id = attendance.Id,
                //    StatusCode = attendance.StatusCode,
                //    RfCardNumber = attendance.RfCardNumber,
                //    Reason = attendance.Reason,
                //    Image = attendance.Image.IsCollectionNotNullOrEmpty() ? Convert.ToBase64String(attendance.Image) : null,
                //    DoorId = attendance.DoorId,
                //});
            }

            return result;
        }

        public static DeviceInvalidAttendanceModel MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(DtoInvalidAttendance inputItem)
        {
            return MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(new List<DtoInvalidAttendance> { inputItem }).FirstOrDefault();
        }

        public static ServerMatchDataModel MapServerMatchDataToServerMatchDataModel(DtoServerMatchData inputItem)
        {
            return new ServerMatchDataModel
            {
                Password = inputItem.Password,
                RfCardNumber = inputItem.RfCardNumber,
                DeviceId = inputItem.DeviceId,
                MatchType = inputItem.MatchType,
                EventDateTime = inputItem.EventDateTime.ToNumericDateTime(),
                TemplateData = inputItem.TemplateData != null ? Convert.ToBase64String(inputItem.TemplateData) : null,
                UserIdOnDevice = inputItem.UserIdOnDevice,
                TemplateType = inputItem.TemplateType,
                DoorId = inputItem.DoorId,
            };
        }

        public static List<ZkOperationLogModel> MapDtoZkOperationLogToZkOperationLogModel(List<DtoZkOperationLog> inputItems)
        {
            var result = new List<ZkOperationLogModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<ZkOperationLogModel>();
            }

            foreach (var attendance in inputItems)
            {
                result.Add(new ZkOperationLogModel
                {
                    DeviceId = attendance.DeviceId,
                    User = attendance.User,
                    Object1 = attendance.Object1,
                    Object2 = attendance.Object2,
                    Object3 = attendance.Object3,
                    Object4 = attendance.Object4,
                    OperationTime = attendance.OperationTime.ToNumericDateTime(),
                    Id = attendance.Id,
                    OperationType = attendance.OperationType,
                    Operator = attendance.Operator
                });
            }

            return result;
        }

        public static ZkOperationLogModel MapDtoZkOperationLogToZkOperationLogModel(DtoZkOperationLog inputItem)
        {
            return MapDtoZkOperationLogToZkOperationLogModel(new List<DtoZkOperationLog> { inputItem }).FirstOrDefault();
        }

        public static List<DeviceEventLogModel> MapDtoDeviceEventLogToDeviceEventLogModel(List<DtoDeviceEventLog> inputItems)
        {
            var result = new List<DeviceEventLogModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<DeviceEventLogModel>();
            }

            foreach (var item in inputItems)
            {
                result.Add(new DeviceEventLogModel
                {
                    DeviceId = item.DeviceId,
                    EventDateTime = item.EventDateTime.ToNumericDateTime(),
                    Id = item.Id.ToString(),
                    UserIdOnDevice = item.UserIdOnDevice,
                    EventCode = item.EventCode,
                    SdkVersion = item.SdkVersion,
                    Producer = item.Producer,
                });
            }

            return result;
        }

        public static DeviceEventLogModel MapDtoDeviceEventLogToDeviceEventLogModel(DtoDeviceEventLog inputItem)
        {
            return MapDtoDeviceEventLogToDeviceEventLogModel(new List<DtoDeviceEventLog> { inputItem }).FirstOrDefault();
        }

        public static List<UserFingerModel> MapDtoEmployeeFingerToEmployeeFingerModel(List<DtoUserFinger> inputItems)
        {
            var result = new List<UserFingerModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new UserFingerModel
                {
                    UserIdOnDevice = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    FingerIndex = model.FingerIndex,
                    TemplateData = Convert.ToBase64String(model.TemplateData)
                });
            }

            return result;
        }

        public static UserFingerModel MapDtoEmployeeFingerToEmployeeFingerModel(DtoUserFinger inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeFingerToEmployeeFingerModel(new List<DtoUserFinger> { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static List<DtoUserFinger> MapToEmployeeFingerModelDtoEmployeeFinger(List<UserFingerModel> inputItems)
        {
            var result = new List<DtoUserFinger>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserFinger
                {
                    UserIdOnDevice = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    FingerIndex = model.FingerIndex,
                    TemplateData = Convert.FromBase64String(model.TemplateData)
                });
            }

            return result;
        }

        public static List<UserFaceModel> MapDtoEmployeeFaceToEmployeeFaceModel(List<DtoUserFace> inputItems)
        {
            var result = new List<UserFaceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new UserFaceModel
                {
                    UserId = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    FaceIndex = model.FaceIndex,
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    Length = model.Length,
                    SupremaSdk2FaceFlag = model.SupremaSdk2FaceFlag,
                    SupremaSdk2FaceImageData = model.SupremaSdk2FaceImageData.IsCollectionNotNullOrEmpty()
                            ? Convert.ToBase64String(model.SupremaSdk2FaceImageData)
                            : null,
                    SupremaSdk2FaceImageLen = model.SupremaSdk2FaceImageLen,
                    SupremaSdk2FaceNumOfTemplate = model.SupremaSdk2FaceNumOfTemplate,
                });
            }

            return result;
        }

        public static List<DtoUserFace> MapToEmployeeFaceModelDtoEmployeeFace(List<UserFaceModel> inputItems)
        {
            var result = new List<DtoUserFace>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserFace
                {
                    UserIdOnDevice = model.UserId,
                    CheckSum = model.CheckSum,
                    FaceIndex = model.FaceIndex,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                    Length = model.Length,
                    SupremaSdk2FaceFlag = model.SupremaSdk2FaceFlag,
                    SupremaSdk2FaceImageData = model.SupremaSdk2FaceImageData.IsNotNullOrEmpty()
                        ? Convert.FromBase64String(model.SupremaSdk2FaceImageData)
                        : null,
                    SupremaSdk2FaceImageLen = model.SupremaSdk2FaceImageLen,
                    SupremaSdk2FaceNumOfTemplate = model.SupremaSdk2FaceNumOfTemplate,
                });
            }

            return result;
        }

        public static UserFaceModel MapDtoEmployeeFaceToEmployeeFaceModel(DtoUserFace inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeFaceToEmployeeFaceModel(new List<DtoUserFace> { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static List<DtoUserPalm> MapToEmployeePalmModelDtoEmployeePalm(List<UserPalmModel> inputItems)
        {
            var result = new List<DtoUserPalm>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserPalm
                {
                    UserIdOnDevice = model.UserIdOnDevice,
                    CheckSum = model.CheckSum,
                    Index = model.Index,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                    Length = model.Length
                });
            }

            return result;
        }

        public static List<DtoUserIris> MapToEmployeeIrisModelDtoEmployeeIris(List<UserIrisModel> inputItems)
        {
            var result = new List<DtoUserIris>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserIris
                {
                    UserIdOnDevice = model.UserIdOnDevice,
                    TemplateData = Convert.FromBase64String(model.TemplateData),
                });
            }

            return result;
        }

        public static List<UserPalmModel> MapDtoEmployeePalmToEmployeePalmModel(List<DtoUserPalm> inputItems)
        {
            var result = new List<UserPalmModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new UserPalmModel
                {
                    CheckSum = model.CheckSum,
                    Index = model.Index,
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    Length = model.Length,
                    UserIdOnDevice = model.UserIdOnDevice,
                });
            }

            return result;
        }

        public static List<UserIrisModel> MapDtoEmployeeIrisToEmployeeIrisModel(List<DtoUserIris> inputItems)
        {
            var result = new List<UserIrisModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new UserIrisModel
                {
                    TemplateData = Convert.ToBase64String(model.TemplateData),
                    UserIdOnDevice = model.UserIdOnDevice,
                });
            }

            return result;
        }

        public static UserIrisModel MapDtoEmployeeIrisToEmployeeIrisModel(DtoUserIris inputItem)
        {
            if (inputItem != null)
            {
                return MapDtoEmployeeIrisToEmployeeIrisModel(new List<DtoUserIris>() { inputItem }).FirstOrDefault();
            }
            return null;
        }

        public static UserPalmModel MapDtoEmployeePalmToEmployeePalmModel(DtoUserPalm inputItem)
        {
            return MapDtoEmployeePalmToEmployeePalmModel(new List<DtoUserPalm> { inputItem }).FirstOrDefault();
        }

        public static UserImageModel MapDtoEmployeeImageToEmployeeImageModel(DtoUserImage inputItem)
        {
            return new UserImageModel
            {
                UserIdOnDevice = inputItem.UserIdOnDevice,
                EmployeeImage = inputItem.PhotoData != null
                    ? Convert.ToBase64String(inputItem.PhotoData)
                    : null
            };
        }

        public static UserModel MapDtoEmployeeToEmployeeModel(DtoUserDeviceRelatedData inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new UserModel
            {
                UserIdOnDevice = inputItem.UserIdOnDevice,
                Privilege = inputItem.Privilege,
                ElevatorInfoInJsonFormat = inputItem.ElevatorInfoInJsonFormat,
                CabinetInfoInJsonFormat = inputItem.CabinetInfoInJsonFormat,
                VerificationStyle = inputItem.VerificationStyle,
                FaceDataList = MapDtoEmployeeFaceToEmployeeFaceModel(inputItem.FaceDataList),
                HardwareProfileImage = inputItem.HardwareProfileImage != null
                        ? Convert.ToBase64String(inputItem.HardwareProfileImage)
                        : null,
                Password = inputItem.Password,
                EndDate = inputItem.EndTime.ToNumericDateTime(),
                FingerDataList = MapDtoEmployeeFingerToEmployeeFingerModel(inputItem.FingerDataList),
                IsEnable = inputItem.IsEnable,
                PalmDataList = MapDtoEmployeePalmToEmployeePalmModel(inputItem.PalmDataList),
                IrisDataList = MapDtoEmployeeIrisToEmployeeIrisModel(inputItem.IrisDataList),
                RfCardNumbers = inputItem.RfCardNumbers,
                VisibleLightImage = inputItem.VisibleLightImage != null
                    ? Convert.ToBase64String(inputItem.VisibleLightImage)
                    : null,
                StartDate = inputItem.StartTime.ToNumericDateTime(),
                UserName = inputItem.UserName,
            };
            return result;
        }

        public static UserEnrolledSettingModel MapDtoEmployeeEnrolledSettingToEmployeeEnrolledSettingModel(DtoUserEnrolledSetting inputItem)
        {
            if (inputItem == null)
            {
                return null;
            }
            var result = new UserEnrolledSettingModel
            {
                OverwriteDevicePassword = inputItem.OverwriteDevicePassword,
                OverwriteIsEnabled = inputItem.OverwriteIsEnabled,
                OverwritePrivilege = inputItem.OverwritePrivilege,
                OverwriteRfCardNumber = inputItem.OverwriteRfCardNumber,
                OverwriteVerificationStyle = inputItem.OverwriteVerificationStyle,
            };
            return result;
        }

        public static List<DtoUserDeviceRelatedData> MapEmployeeModelToDtoEmployee(List<UserModel> inputItems)
        {
            var result = new List<DtoUserDeviceRelatedData>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                result.Add(new DtoUserDeviceRelatedData
                {
                    UserIdOnDevice = model.UserIdOnDevice,
                    Privilege = model.Privilege,
                    ElevatorInfoInJsonFormat = model.ElevatorInfoInJsonFormat,
                    CabinetInfoInJsonFormat = model.CabinetInfoInJsonFormat,
                    VerificationStyle = model.VerificationStyle,
                    FaceDataList = MapToEmployeeFaceModelDtoEmployeeFace(model.FaceDataList),
                    HardwareProfileImage = model.HardwareProfileImage != null
                        ? Convert.FromBase64String(model.HardwareProfileImage)
                        : null,
                    Password = model.Password,
                    EndTime = model.EndDate.FromNumericDateTime(),
                    FingerDataList = MapToEmployeeFingerModelDtoEmployeeFinger(model.FingerDataList),
                    IsEnable = model.IsEnable,
                    PalmDataList = MapToEmployeePalmModelDtoEmployeePalm(model.PalmDataList),
                    IrisDataList = MapToEmployeeIrisModelDtoEmployeeIris(model.IrisDataList),
                    RfCardNumbers = model.RfCardNumbers,
                    VisibleLightImage = model.VisibleLightImage != null
                        ? Convert.FromBase64String(model.VisibleLightImage)
                        : null,
                    StartTime = model.StartDate.FromNumericDateTime(),
                    UserName = model.UserName,
                    UserType = model.UserType,
                });
            }

            return result;
        }

        public static DtoUserDeviceRelatedData MapEmployeeModelToDtoEmployee(UserModel inputItem)
        {
            return MapEmployeeModelToDtoEmployee(new List<UserModel> { inputItem }).FirstOrDefault();
        }

        public static SystemConfigModel MapDtoSystemConfigToSystemConfigModel(DtoSystemConfig inputItem)
        {
            return new SystemConfigModel
            {
                AttendanceRegisterInterval = inputItem.AttendanceRegisterInterval,
                AttendanceRegisterIntervalForParking = inputItem.AttendanceRegisterIntervalForParking,
                AttendanceRegisterIntervalForTimeAttendance = inputItem.AttendanceRegisterIntervalForTimeAttendance,
                AttendanceRegisterIntervalForAccessControl = inputItem.AttendanceRegisterIntervalForAccessControl,
                OnlineDeviceTimerInterval = inputItem.OnlineDeviceTimerInterval,
                ResetRetryCountDayBefore = inputItem.ResetRetryCountDayBefore,
                KeepCommandAfterResponse = inputItem.KeepCommandAfterResponse,
                OnlineMonitoringDevicesIntervalFromLastDataToReset = inputItem.OnlineMonitoringDevicesIntervalFromLastDataToReset,
                OnlineMonitoringDevicesSleepAfterPingInSecond = inputItem.OnlineMonitoringDevicesSleepAfterPingInSecond,
                OnlineMonitoringDevicesPingTimeoutInMillisecond = inputItem.OnlineMonitoringDevicesPingTimeoutInMillisecond,
                OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = inputItem.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond,
                IsNetworkPingActive = inputItem.IsNetworkPingActive,
                AttendanceHookTimerInterval = inputItem.AttendanceHookTimerInterval,
                AttendanceSendToKarnamaTimerInterval = inputItem.AttendanceSendToKarnamaTimerInterval,
                AttendanceSendToKarnamaTimerRecordCount = inputItem.AttendanceSendToKarnamaTimerRecordCount,
                AttendanceHookTimerRecordCount = inputItem.AttendanceHookTimerRecordCount,
                AutomaticCollectAttendanceTimerInterval = inputItem.AutomaticCollectAttendanceTimerInterval,
                KarnamaAuthorizationToken = inputItem.KarnamaAuthorizationToken,
                KarnamaServiceUrl = inputItem.KarnamaServiceUrl,




                MaxZkCommandCount = inputItem.MaxZkCommandCount,
                ZkDeadlineInMinutes = inputItem.ZkDeadlineInMinutes,
                ZkPushServerPort = inputItem.ZkPushServerPort,
                ZkPushServerIp = inputItem.ZkPushServerIp,
                MaxRetryForZkOtherCommand = inputItem.MaxRetryForZkOtherCommand,
                MaxRetryForZkUserCommands = inputItem.MaxRetryForZkUserCommands,

                MaxRetryForPwOtherCommands = inputItem.MaxRetryForPwOtherCommands,
                MaxRetryForPwUserCommands = inputItem.MaxRetryForPwUserCommands,
                PwDeadlineInMinutes = inputItem.PwDeadlineInMinutes,

                SupremaSdk1ServerPort = inputItem.SupremaSdk1ServerPort,
                SupremaSdk1ServerMaxConnection = inputItem.SupremaSdk1ServerMaxConnection,
                MaxRetryForSupremaSdk1OtherCommands = inputItem.MaxRetryForSupremaSdk1OtherCommands,
                MaxRetryForSupremaSdk1UserCommands = inputItem.MaxRetryForSupremaSdk1UserCommands,
                SupremaSdk1DeadlineInMinutes = inputItem.SupremaSdk1DeadlineInMinutes,

                SupremaSdk2ServerPort = inputItem.SupremaSdk2ServerPort,
                SupremaSdk2ServerReconnectTimerInterval = inputItem.SupremaSdk2ServerReconnectTimerInterval,
                MaxRetryForSupremaSdk2OtherCommands = inputItem.MaxRetryForSupremaSdk2OtherCommands,
                MaxRetryForSupremaSdk2UserCommands = inputItem.MaxRetryForSupremaSdk2UserCommands,
                SupremaSdk2DeadlineInMinutes = inputItem.SupremaSdk2DeadlineInMinutes,

                MaxRetryForVirdiOtherCommands = inputItem.MaxRetryForVirdiOtherCommands,
                MaxRetryForVirdiUserCommands = inputItem.MaxRetryForVirdiUserCommands,
                VirdiDeadlineInMinutes = inputItem.VirdiDeadlineInMinutes,
                VirdiServerPort = inputItem.VirdiServerPort,


            };

        }

        public static DtoSystemConfig MapSystemConfigModelToDtoSystemConfig(SystemConfigModel inputItem)
        {
            return new DtoSystemConfig
            {
                AttendanceRegisterInterval = inputItem.AttendanceRegisterInterval,
                AttendanceRegisterIntervalForParking = inputItem.AttendanceRegisterIntervalForParking,
                AttendanceRegisterIntervalForTimeAttendance = inputItem.AttendanceRegisterIntervalForTimeAttendance,
                AttendanceRegisterIntervalForAccessControl = inputItem.AttendanceRegisterIntervalForAccessControl,
                OnlineDeviceTimerInterval = inputItem.OnlineDeviceTimerInterval,
                ResetRetryCountDayBefore = inputItem.ResetRetryCountDayBefore,
                KeepCommandAfterResponse = inputItem.KeepCommandAfterResponse,
                SupremaSdk1ServerMaxConnection = inputItem.SupremaSdk1ServerMaxConnection,
                AttendanceSendToKarnamaTimerInterval = inputItem.AttendanceSendToKarnamaTimerInterval,
                AttendanceHookTimerInterval = inputItem.AttendanceHookTimerInterval,
                AttendanceSendToKarnamaTimerRecordCount = inputItem.AttendanceSendToKarnamaTimerRecordCount,
                AttendanceHookTimerRecordCount = inputItem.AttendanceHookTimerRecordCount,
                AutomaticCollectAttendanceTimerInterval = inputItem.AutomaticCollectAttendanceTimerInterval,
                KarnamaServiceUrl = inputItem.KarnamaServiceUrl,
                OnlineMonitoringDevicesIntervalFromLastDataToReset = inputItem.OnlineMonitoringDevicesIntervalFromLastDataToReset,
                OnlineMonitoringDevicesSleepAfterPingInSecond = inputItem.OnlineMonitoringDevicesSleepAfterPingInSecond,
                OnlineMonitoringDevicesPingTimeoutInMillisecond = inputItem.OnlineMonitoringDevicesPingTimeoutInMillisecond,
                OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = inputItem.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond,

                ZkPushServerIp = inputItem.ZkPushServerIp,
                MaxZkCommandCount = inputItem.MaxZkCommandCount,
                ZkPushServerPort = inputItem.ZkPushServerPort,
                MaxRetryForZkOtherCommand = inputItem.MaxRetryForZkOtherCommand,
                MaxRetryForZkUserCommands = inputItem.MaxRetryForZkUserCommands,
                ZkDeadlineInMinutes = inputItem.ZkDeadlineInMinutes,

                MaxRetryForPwOtherCommands = inputItem.MaxRetryForPwOtherCommands,
                MaxRetryForPwUserCommands = inputItem.MaxRetryForPwUserCommands,
                PwDeadlineInMinutes = inputItem.PwDeadlineInMinutes,

                SupremaSdk1ServerPort = inputItem.SupremaSdk1ServerPort,
                MaxRetryForSupremaSdk1OtherCommands = inputItem.MaxRetryForSupremaSdk1OtherCommands,
                MaxRetryForSupremaSdk1UserCommands = inputItem.MaxRetryForSupremaSdk1UserCommands,
                SupremaSdk1DeadlineInMinutes = inputItem.SupremaSdk1DeadlineInMinutes,

                SupremaSdk2ServerReconnectTimerInterval = inputItem.SupremaSdk2ServerReconnectTimerInterval,
                SupremaSdk2ServerPort = inputItem.SupremaSdk2ServerPort,
                MaxRetryForSupremaSdk2OtherCommands = inputItem.MaxRetryForSupremaSdk2OtherCommands,
                MaxRetryForSupremaSdk2UserCommands = inputItem.MaxRetryForSupremaSdk2UserCommands,
                SupremaSdk2DeadlineInMinutes = inputItem.SupremaSdk2DeadlineInMinutes,

                MaxRetryForVirdiOtherCommands = inputItem.MaxRetryForVirdiOtherCommands,
                MaxRetryForVirdiUserCommands = inputItem.MaxRetryForVirdiUserCommands,
                VirdiDeadlineInMinutes = inputItem.VirdiDeadlineInMinutes,
                VirdiServerPort = inputItem.VirdiServerPort,
            };

        }

        public static DtoUserAndDeviceParam MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(UserAndDeviceModel inputItem)
        {
            return new DtoUserAndDeviceParam
            {
                DeviceId = inputItem.DeviceId,
                UserInfo = MapEmployeeModelToDtoEmployee(inputItem.UserData),
                CommandPriority = inputItem.CommandPriority,
                CommandIdentifier = inputItem.CommandIdentifier,
            };
        }

        public static List<DtoUserAndDeviceParam> MapEmployeeAndDeviceModelToDtoEmployeeAndDeviceParam(List<UserAndDeviceModel> inputItems)
        {
            var result = new List<DtoUserAndDeviceParam>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(new DtoUserAndDeviceParam
                {
                    DeviceId = model.DeviceId,
                    UserInfo = MapEmployeeModelToDtoEmployee(model.UserData),
                    CommandPriority = model.CommandPriority,
                    CommandIdentifier = model.CommandIdentifier,
                });
            }
            return result;
        }

        public static List<UserAndDeviceResultModel> MapDtoEmployeeAndDeviceResultToEmployeeAndDeviceResultModel(List<DtoUserAndDeviceResult> inputItems)
        {
            var result = new List<UserAndDeviceResultModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(new UserAndDeviceResultModel
                {
                    DeviceId = model.DeviceId,
                    UserIdOnDevice = model.UserIdOnDevice,
                    Result = model.Result
                });
            }
            return result;
        }


        public static DeviceAttendanceImageModel MapDtoDeviceAttendanceImageToDeviceAttendanceImageModel(DtoDeviceAttendanceImage inputItem)
        {
            return new DeviceAttendanceImageModel
            {
                DeviceId = inputItem.DeviceId,
                AttendanceDateTime = inputItem.AttendanceDateTime.ToNumericDateTime(),
                Image = Convert.ToBase64String(inputItem.Image),
                UserIdOnDevice = inputItem.UserIdOnDevice,
            };
        }

        public static DeviceUnauthorizedAttendanceImageModel MapDtoDeviceUnauthorizedAttendanceImageToDeviceUnauthorizedAttendanceImageModel(DtoDeviceUnauthorizedAttendanceImage inputItem)
        {
            return new DeviceUnauthorizedAttendanceImageModel
            {
                DeviceId = inputItem.DeviceId,
                AttendanceDateTime = inputItem.AttendanceDateTime.ToNumericDateTime(),
                Image = Convert.ToBase64String(inputItem.Image),
                UserIdOnDevice = inputItem.UserIdInDevice
            };
        }


        public static List<UserInfoDefinedOnDeviceModel> MapDtoUserInfoOnDeviceToUserInfoOnDeviceModel(List<DtoUserInfoDefinedOnDevice> inputItems)
        {
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return new List<UserInfoDefinedOnDeviceModel>();
            }
            return inputItems.Select(ii => new UserInfoDefinedOnDeviceModel
            {
                EmployeeNumber = ii.EmployeeNumber,
                Name = ii.Name,
                Privilege = ii.Privilege,
            }).ToList();
        }



    }
}
