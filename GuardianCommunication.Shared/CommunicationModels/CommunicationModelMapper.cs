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

        public static DtoAttendance MapOtherResourcesAttendanceModelToDtoAttendance(OtherResourcesAttendanceModel attendance)
        {
            return new DtoAttendance
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
                LocationId = attendance.LocationId,
            };
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
                result.Add(MapOtherResourcesAttendanceModelToDtoAttendance(attendance));
            }
            return result;
        }

        //public static List<DtoAttendance> MapDeviceAttendanceModelToDtoAttendance(List<DeviceAttendanceModel> inputItems)
        //{
        //    var result = new List<DtoAttendance>();
        //    if (inputItems.IsCollectionNullOrEmpty())
        //    {
        //        return result;
        //    }

        //    foreach (var attendance in inputItems)
        //    {
        //        //result.Add(new DtoAttendance
        //        //{
        //        //    DeviceId = attendance.DeviceId,
        //        //    ReaderDeviceId = attendance.ReaderDeviceNumber,
        //        //    UserIdOnDevice = attendance.UserIdOnDevice,
        //        //    VerificationStyle = attendance.VerificationStyle,
        //        //    AttendanceDateTime = attendance.AttendanceDateTime.FromNumericDateTime(),
        //        //    AttendanceSource = attendance.AttendanceSource,
        //        //    IsSentToGuardian = attendance.IsSentToGuardian,
        //        //    Id = attendance.Id,
        //        //    StatusCode = attendance.StatusCode,
        //        //    ModuleId = attendance.ModuleId,
        //        //    IoType = attendance.IoType,
        //        //    RfCardNumber = attendance.RfCardNumber,
        //        //    CameraId = attendance.CameraId,
        //        //    DeviceAttendanceIoRetrieveType = null,
        //        //    DoorId = attendance.DoorId,
        //        //});
        //    }

        //    return result;
        //}

        public static List<DeviceAttendanceModel> MapDtoAttendanceToDeviceAttendanceModel(List<DtoAttendance> inputItems)
        {
            var result = new List<DeviceAttendanceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var attendance in inputItems)
            {
                result.Add(MapDtoAttendanceToDeviceAttendanceModel(attendance));
            }
            return result;
        }

        public static DeviceAttendanceModel MapDtoAttendanceToDeviceAttendanceModel(DtoAttendance attendance)
        {
            return new DeviceAttendanceModel
            {
                DeviceId = attendance.DeviceId ?? Guid.Empty,
                UserIdOnDevice = attendance.UserIdOnDevice,
                VerificationStyle = attendance.VerificationStyle ?? 0,
                AttendanceDateTime = attendance.AttendanceDateTime,
                AttendanceSource = attendance.AttendanceSource,
                Id = attendance.Id,
                StatusCode = attendance.StatusCode ?? 0,
                RfCardNumber = attendance.RfCardNumber,
                IoType = attendance.IoType,
                ModuleId = attendance.ModuleId,
                DoorId = attendance.DoorId,
            };
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
                result.Add(MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(attendance));
            }
            return result;
        }

        public static DeviceInvalidAttendanceModel MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(DtoInvalidAttendance attendance)
        {
            return new DeviceInvalidAttendanceModel
            {
                DeviceId = attendance.DeviceId ?? Guid.Empty,
                UserIdOnDevice = attendance.UserIdOnDevice,
                VerificationStyle = attendance.VerificationStyle ?? 0,
                AttendanceDateTime = attendance.AttendanceDateTime,
                AttendanceSource = attendance.AttendanceSource,
                Id = attendance.Id,
                StatusCode = attendance.StatusCode,
                RfCardNumber = attendance.RfCardNumber,
                Reason = attendance.Reason,
                Image = attendance.Image.IsCollectionNotNullOrEmpty() ? Convert.ToBase64String(attendance.Image) : null,
                DoorId = attendance.DoorId,
            };
        }

        public static ServerMatchDataModel MapServerMatchDataToServerMatchDataModel(DtoServerMatchData inputItem)
        {
            return new ServerMatchDataModel
            {
                Password = inputItem.Password,
                RfCardNumber = inputItem.RfCardNumber,
                DeviceId = inputItem.DeviceId,
                MatchType = inputItem.MatchType,
                EventDateTime = inputItem.EventDateTime,
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
                return [];
            }
            foreach (var opLog in inputItems)
            {
                result.Add(MapDtoZkOperationLogToZkOperationLogModel(opLog));
            }

            return result;
        }

        public static ZkOperationLogModel MapDtoZkOperationLogToZkOperationLogModel(DtoZkOperationLog opLog)
        {
            return new ZkOperationLogModel
            {
                DeviceId = opLog.DeviceId,
                User = opLog.User,
                Object1 = opLog.Object1,
                Object2 = opLog.Object2,
                Object3 = opLog.Object3,
                Object4 = opLog.Object4,
                OperationTime = opLog.OperationTime.ToNumericDateTime(),
                Id = opLog.Id,
                OperationType = opLog.OperationType,
                Operator = opLog.Operator
            };
        }

        public static List<DeviceEventLogModel> MapDtoDeviceEventLogToDeviceEventLogModel(List<DtoDeviceEventLog> inputItems)
        {
            var result = new List<DeviceEventLogModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return [];
            }
            foreach (var opLog in inputItems)
            {
                result.Add(MapDtoDeviceEventLogToDeviceEventLogModel(opLog));
            }
            return result;
        }

        public static DeviceEventLogModel MapDtoDeviceEventLogToDeviceEventLogModel(DtoDeviceEventLog opLog)
        {
            return new DeviceEventLogModel
            {
                DeviceId = opLog.DeviceId,
                EventDateTime = opLog.EventDateTime.ToNumericDateTime(),
                Id = opLog.Id.ToString(),
                UserIdOnDevice = opLog.UserIdOnDevice,
                EventCode = opLog.EventCode,
                SdkVersion = opLog.SdkVersion,
                Producer = opLog.Producer,
                DoorId = opLog.DoorId
            };
        }

        public static List<UserFingerModel> MapDtoUserFingerToUserFingerModel(List<DtoUserFinger> inputItems)
        {
            var result = new List<UserFingerModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(MapDtoUserFingerToUserFingerModel(model));
            }
            return result;
        }

        public static UserFingerModel MapDtoUserFingerToUserFingerModel(DtoUserFinger model)
        {
            return new UserFingerModel
            {
                UserIdOnDevice = model.UserIdOnDevice,
                CheckSum = model.CheckSum,
                FingerIndex = model.FingerIndex,
                TemplateData = Convert.ToBase64String(model.TemplateData),
            };
        }

        public static List<DtoUserFinger> MapToUserFingerModelDtoUserFinger(List<UserFingerModel> inputItems)
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
                    TemplateData = Convert.FromBase64String(model.TemplateData),

                });
            }

            return result;
        }

        public static List<DtoUserFace> MapToUserFaceModelDtoUserFace(List<UserFaceModel> inputItems)
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
                    AdditionalDataInJson = model.AdditionalDataInJson,
                });
            }

            return result;
        }

        public static UserFaceModel MapDtoUserFaceToUserFaceModel(DtoUserFace model)
        {
            return new UserFaceModel
            {
                UserId = model.UserIdOnDevice,
                CheckSum = model.CheckSum,
                FaceIndex = model.FaceIndex,
                TemplateData = Convert.ToBase64String(model.TemplateData),
                Length = model.Length,
                AdditionalDataInJson = model.AdditionalDataInJson,
            };
        }

        public static List<UserFaceModel> MapDtoUserFaceToUserFaceModel(List<DtoUserFace> inputItems)
        {
            var result = new List<UserFaceModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                if (model != null)
                {
                    model.UpdateDeviceSettingsJson();
                    result.Add(MapDtoUserFaceToUserFaceModel(model));
                }
            }

            return result;
        }

        public static List<DtoUserPalm> MapToUserPalmModelDtoUserPalm(List<UserPalmModel> inputItems)
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

        public static List<DtoUserIris> MapToUserIrisModelDtoUserIris(List<UserIrisModel> inputItems)
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

        public static List<UserPalmModel> MapDtoUserPalmToUserPalmModel(List<DtoUserPalm> inputItems)
        {
            var result = new List<UserPalmModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                if (model != null)
                {
                    result.Add(MapDtoUserPalmToUserPalmModel(model));
                }
            }
            return result;
        }

        public static UserPalmModel MapDtoUserPalmToUserPalmModel(DtoUserPalm model)
        {
            return new UserPalmModel
            {
                CheckSum = model.CheckSum,
                Index = model.Index,
                TemplateData = Convert.ToBase64String(model.TemplateData),
                Length = model.Length,
                UserIdOnDevice = model.UserIdOnDevice,
            };
        }

        public static List<UserIrisModel> MapDtoUserIrisToUserIrisModel(List<DtoUserIris> inputItems)
        {
            var result = new List<UserIrisModel>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }

            foreach (var model in inputItems)
            {
                if (model != null)
                {
                    result.Add(MapDtoUserIrisToUserIrisModel(model));
                }
            }

            return result;
        }

        public static UserIrisModel MapDtoUserIrisToUserIrisModel(DtoUserIris model)
        {
            return new UserIrisModel
            {
                TemplateData = Convert.ToBase64String(model.TemplateData),
                UserIdOnDevice = model.UserIdOnDevice,
            };
        }


        public static UserImageModel MapDtoUserImageToUserImageModel(DtoUserImage inputItem)
        {
            return new UserImageModel
            {
                UserIdOnDevice = inputItem.UserIdOnDevice,
                UserImage = inputItem.PhotoData != null
                    ? Convert.ToBase64String(inputItem.PhotoData)
                    : null
            };
        }

        public static UserModel MapDtoUserToUserModel(DtoUserDeviceRelatedData inputItem)
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
                FaceDataList = MapDtoUserFaceToUserFaceModel(inputItem.FaceDataList),
                HardwareProfileImage = inputItem.HardwareProfileImage != null
                        ? Convert.ToBase64String(inputItem.HardwareProfileImage)
                        : null,
                Password = inputItem.Password,
                EndDateTime = inputItem.EndDateTime,
                FingerDataList = MapDtoUserFingerToUserFingerModel(inputItem.FingerDataList),
                IsEnable = inputItem.IsEnable,
                PalmDataList = MapDtoUserPalmToUserPalmModel(inputItem.PalmDataList),
                IrisDataList = MapDtoUserIrisToUserIrisModel(inputItem.IrisDataList),
                RfCardNumbers = inputItem.RfCardNumbers,
                VisibleLightImage = inputItem.VisibleLightImage != null
                    ? Convert.ToBase64String(inputItem.VisibleLightImage)
                    : null,
                StartDateTime = inputItem.StartDateTime,
                UserName = inputItem.UserName,
                UserType = inputItem.UserType,
            };
            return result;
        }

        public static UserEnrolledSettingModel MapDtoUserEnrolledSettingToUserEnrolledSettingModel(DtoUserEnrolledSetting inputItem)
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

        public static List<DtoUserDeviceRelatedData> MapUserModelToDtoUser(List<UserModel> inputItems)
        {
            var result = new List<DtoUserDeviceRelatedData>();
            if (inputItems.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var model in inputItems)
            {
                result.Add(MapUserModelToDtoUser(model));
            }
            return result;
        }

        public static DtoUserDeviceRelatedData MapUserModelToDtoUser(UserModel model)
        {
            return new DtoUserDeviceRelatedData
            {
                UserIdOnDevice = model.UserIdOnDevice,
                Privilege = model.Privilege,
                ElevatorInfoInJsonFormat = model.ElevatorInfoInJsonFormat,
                CabinetInfoInJsonFormat = model.CabinetInfoInJsonFormat,
                VerificationStyle = model.VerificationStyle,
                FaceDataList = MapToUserFaceModelDtoUserFace(model.FaceDataList),
                HardwareProfileImage = model.HardwareProfileImage != null
                    ? Convert.FromBase64String(model.HardwareProfileImage)
                    : null,
                Password = model.Password,
                EndDateTime = model.EndDateTime,
                FingerDataList = MapToUserFingerModelDtoUserFinger(model.FingerDataList),
                IsEnable = model.IsEnable,
                PalmDataList = MapToUserPalmModelDtoUserPalm(model.PalmDataList),
                IrisDataList = MapToUserIrisModelDtoUserIris(model.IrisDataList),
                RfCardNumbers = model.RfCardNumbers,
                VisibleLightImage = model.VisibleLightImage != null
                    ? Convert.FromBase64String(model.VisibleLightImage)
                    : null,
                StartDateTime = model.StartDateTime,
                UserName = model.UserName,
                UserType = model.UserType,
                StartAndEndHasTime = model.StartAndEndHasTime,
            };
        }

        public static DtoUserAndDeviceParam MapUserAndDeviceModelToDtoUserAndDeviceParam(UserAndDeviceModel inputItem)
        {
            return new DtoUserAndDeviceParam
            {
                DeviceId = inputItem.DeviceId,
                UserInfo = MapUserModelToDtoUser(inputItem.UserData),
                CommandPriority = inputItem.CommandPriority,
                CommandIdentifier = inputItem.CommandIdentifier,
            };
        }

        public static List<DtoUserAndDeviceParam> MapUserAndDeviceModelToDtoUserAndDeviceParam(List<UserAndDeviceModel> inputItems)
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
                    UserInfo = MapUserModelToDtoUser(model.UserData),
                    CommandPriority = model.CommandPriority,
                    CommandIdentifier = model.CommandIdentifier,
                });
            }
            return result;
        }

        public static List<UserAndDeviceResultModel> MapDtoUserAndDeviceResultToUserAndDeviceResultModel(List<DtoUserAndDeviceResult> inputItems)
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
                return [];
            }
            return inputItems.Select(ii => new UserInfoDefinedOnDeviceModel
            {
                UserIdOnDevice = ii.UserIdOnDevice,
                Name = ii.Name,
                Privilege = ii.Privilege,
            }).ToList();
        }



    }
}
