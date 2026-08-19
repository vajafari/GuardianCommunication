using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;
using GuardianCommunication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.ExternalServices.KarnamaApi
{
    public class GuardianProxy
    {
        private GuardianApiConfig Config { get; }

        public GuardianProxy(GuardianApiConfig config)
        {
            Config = config;
        }

        #region Basic Info

        public void SubmitUserCount(Guid deviceId, int count)
        {
            var model = new DataCountModel
            {
                DeviceId = deviceId,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitUserCount Called", new { DeviceId = deviceId, Count = count });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitUserCount"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = model
            });
        }

        public void SubmitAccessLogCount(Guid deviceId, int count)
        {
            var model = new DataCountModel
            {
                DeviceId = deviceId,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitAccessLogCount Called", new { DeviceId = deviceId, Count = count });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitAccessLogCount"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = model
            });
        }

        public void SubmitFaceCount(Guid deviceId, int count)
        {
            var model = new DataCountModel
            {
                DeviceId = deviceId,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitFaceCount Called", new { DeviceId = deviceId, Count = count });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitFaceCount"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = model
            });
        }

        public void SubmitFingerCount(Guid deviceId, int count)
        {
            var model = new DataCountModel
            {
                DeviceId = deviceId,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitFingerCount Called", new { DeviceId = deviceId, Count = count });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitFingerCount"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = model
            });

        }

        public void SubmitDeviceEventLog(DtoDeviceEventLog eventLog)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.EventLog))
            {
                LoggingSystem.LogInfo("SubmitDeviceEventLog Called", eventLog);
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitDeviceEventLog"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoDeviceEventLogToDeviceEventLogModel(eventLog)
            });
        }

        public void SubmitZkOperationLog(DtoZkOperationLog operationLog)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.EventLog))
            {
                LoggingSystem.LogInfo("SubmitZkOperationLog Called", operationLog);
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/ZkOperationLog"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoZkOperationLogToZkOperationLogModel(operationLog)
            });
        }

        public void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SendAttendanceImage))
            {
                LoggingSystem.LogInfo("SendAttendanceImage Called", image);
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitDeviceValidAttendanceImage"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoDeviceAttendanceImageToDeviceAttendanceImageModel(image)
            });
        }

        public void SendUnauthorizedAttendanceImage(DtoDeviceUnauthorizedAttendanceImage image)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SendAttendanceImage))
            {
                LoggingSystem.LogInfo("SendUnauthorizedAttendanceImage Called", image);
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitDeviceUnauthorizedAttendanceImage"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoDeviceUnauthorizedAttendanceImageToDeviceUnauthorizedAttendanceImageModel(image)
            });
        }
        public void SubmitInvalidIoEvent(DtoInvalidAttendance entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SubmitInvalidIoEvent))
            {
                LoggingSystem.LogInfo("SubmitInvalidIoEvent Called", entity);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitInvalidIoEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoInvalidAttendanceToDeviceInvalidAttendanceModel(entity)
            });
        }

        public AttendanceProcessResultModel SubmitIoEvent(DtoAttendance entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SubmitIoEvent))
            {
                LoggingSystem.LogInfo("SubmitIoEvent Called", entity);
            }
            var result = RestSharpClient.GetInstance().PostAsJson<AttendanceProcessResultModel>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitIoEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(entity)
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SubmitIoEvent))
            {
                LoggingSystem.LogInfo("SubmitIoEvent Result", result);
            }
            return result;
        }

        public ServerMatchingResultModel SubmitServerMatching(DtoServerMatchData entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SubmitSeverMatching))
            {
                LoggingSystem.LogInfo("SubmitServerMatching Called", entity);
            }
            var result = RestSharpClient.GetInstance().PostAsJson<ServerMatchingResultModel>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitServerMatching"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapServerMatchDataToServerMatchDataModel(entity)
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.SubmitSeverMatching))
            {
                LoggingSystem.LogInfo("SubmitServerMatching result", result);
            }
            return result;
        }

        public void SubmitConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.ChangeConnectionStatus))
            {
                LoggingSystem.LogInfo("SubmitConnectionStatusChanged Called", deviceConnectionStatuses);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/DeviceConnectionStatusChanged"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = deviceConnectionStatuses.Select(cs => new DeviceConnectionChangedModel
                {
                    DeviceId = cs.DeviceId,
                    IsConnected = cs.IsConnected
                }).ToArray()
            });
        }

        public void SubmitFingerEnrolled(DtoUserFinger fingerInfo, Guid deviceId)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitFingerEnrolled Called", new { FingerInfo = fingerInfo, DeviceId = deviceId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/FingerEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserFingerEnrolledModel
                {
                    DeviceId = deviceId,
                    FingerInfo = CommunicationModelMapper.MapDtoUserFingerToUserFingerModel(fingerInfo)
                }
            });
        }

        public void SubmitUserProfileImage(DtoUserImage employeeImage, Guid deviceId)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitUserProfileImage Called", new { UserImage = employeeImage, DeviceId = deviceId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/UserProfileImageReceived"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserImageEnrolledModel
                {
                    DeviceId = deviceId,
                    UserImage = CommunicationModelMapper.MapDtoUserImageToUserImageModel(employeeImage)
                }
            });
        }

        public void SubmitFaceEnrolled(DtoUserFace faceInfo, Guid deviceId)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitFaceEnrolled Called", new { FaceInfo = faceInfo, DeviceId = deviceId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/FaceEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserFaceEnrolledModel
                {
                    DeviceId = deviceId,
                    FaceInfo = CommunicationModelMapper.MapDtoUserFaceToUserFaceModel(faceInfo)
                }
            });
        }

        public void SubmitCardEnrolled(string cardNumber, Guid deviceId, long userId)
        {

            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitCardEnrolled Called", new { CardNumber = cardNumber, DeviceId = deviceId, UserId = userId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/CardEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserCardEnrolledModel
                {
                    DeviceId = deviceId,
                    Card = cardNumber,
                    UserIdOnDevice = userId,
                }
            });
        }

        public void SubmitPalmEnrolled(DtoUserPalm palmInfo, Guid deviceId)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitPalmEnrolled Called", new { PalmInfo = palmInfo, DeviceId = deviceId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/PalmEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserPalmModelEnrolled
                {
                    DeviceId = deviceId,
                    PalmInfo = CommunicationModelMapper.MapDtoUserPalmToUserPalmModel(palmInfo)
                }
            });
        }

        public void SubmitIrisEnrolled(DtoUserIris irisInfo, Guid deviceId)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitIrisEnrolled Called", new { IrisInfo = irisInfo, DeviceId = deviceId });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/IrisEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserIrisModelEnrolled
                {
                    DeviceId = deviceId,
                    IrisInfo = CommunicationModelMapper.MapDtoUserIrisToUserIrisModel(irisInfo)
                }
            });
        }

        public void SubmitUserEnrolled(DtoUserDeviceRelatedData userInfo, Guid deviceId, DtoUserEnrolledSetting setting)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitUserEnrolled Called", new { UserInfo = userInfo, DeviceId = deviceId, Setting = setting });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/UserEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new UserEnrolledModel
                {
                    DeviceId = deviceId,
                    UserDeviceInfo = CommunicationModelMapper.MapDtoUserToUserModel(userInfo),
                    UserEnrolledSetting = CommunicationModelMapper.MapDtoUserEnrolledSettingToUserEnrolledSettingModel(setting),
                }
            });
        }

        #endregion


        #region General info

        public DtoApplicationEncodedConfig GetSoftwareEncodedConfig()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.GeneralInfo))
            {
                LoggingSystem.LogInfo("GetSoftwareEncodedConfig Called");
            }
            var result = RestSharpClient.GetInstance().GetAsJson<DtoApplicationEncodedConfig>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/GeneralData/GetSoftwareEncodedConfig"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = null
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(GuardianCallLogLevelEnumeration.GeneralInfo))
            {
                LoggingSystem.LogInfo("GetSoftwareEncodedConfig result", result);
            }
            return result;
        }


        #endregion


    }
}
