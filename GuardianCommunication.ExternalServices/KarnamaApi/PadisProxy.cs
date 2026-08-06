using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;

namespace GuardianCommunication.ExternalServices.KarnamaApi
{
    public class PadisProxy
    {
        private PadisApiConfig Config { get; }

        public PadisProxy(PadisApiConfig config)
        {
            Config = config;
        }

        #region Basic Info

        public void SubmitUserCount(int deviceNumber, int count)
        {
            var model = new DataCountModel
            {
                DeviceNumber = deviceNumber,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitUserCount Called", new { DeviceNumber = deviceNumber, Count = count });
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

        public void SubmitAccessLogCount(int deviceNumber, int count)
        {
            var model = new DataCountModel
            {
                DeviceNumber = deviceNumber,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitAccessLogCount Called", new { DeviceNumber = deviceNumber, Count = count });
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

        public void SubmitFaceCount(int deviceNumber, int count)
        {
            var model = new DataCountModel
            {
                DeviceNumber = deviceNumber,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitFaceCount Called", new { DeviceNumber = deviceNumber, Count = count });
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

        public void SubmitFingerCount(int deviceNumber, int count)
        {
            var model = new DataCountModel
            {
                DeviceNumber = deviceNumber,
                Count = count,
            };
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.DeviceStatistics))
            {
                LoggingSystem.LogInfo("SubmitFingerCount Called", new { DeviceNumber = deviceNumber, Count = count });
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.EventLog))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.EventLog))
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

        public void SendMetalDetectorPersonPassed(DtoMetalDetectorPersonPassedData passData)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.MetalDetectorLog))
            {
                LoggingSystem.LogInfo("SendMetalDetectorPersonPassed Called", passData);
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitMetalDetectorPersonPassed"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new MetalDetectorPersonPassedModel
                {
                    DeviceId = passData.DeviceId,
                    PassingDateTime = passData.Date.ToNumericDateTime(),
                    ZoneData = Convert.ToBase64String(passData.ZoneData),
                    TotalAlarm = passData.TotalAlarm,
                    TotalPassed = passData.TotalPassed,
                }
            });
        }

        public void SendXRayDeviceScanData(string deviceId, DateTime date, byte[] zoneData)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.XRayLog))
            {
                LoggingSystem.LogInfo("SendXRayDeviceScanData Called", new { DeviceId = deviceId, Date = date, ZoneData = zoneData });
            }

            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitXRayDeviceScanData"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new XRayDeviceScanDataModel
                {
                    DeviceId = deviceId,
                    ScanDateTime = date.ToNumericDateTime(),
                    ScanData = Convert.ToBase64String(zoneData),
                }
            });
        }

        public void SendAttendanceImage(DtoDeviceAttendanceImage image)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SendAttendanceImage))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SendAttendanceImage))
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

        public List<DtoDevice> GetAllDevices(DeviceSearchParams param)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllDevices Called", param);
            }

            var result = RestSharpClient.GetInstance().PostAsJson<List<DtoDevice>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/Device/SearchBasic"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = param
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllDevices result", result);
            }
            return result;

        }

        public List<DtoDeviceDoor> GetAllDeviceDoors()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllControllerDeviceDoors Called");
            }

            var result = RestSharpClient.GetInstance().GetAsJson<List<DtoDeviceDoor>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/ac/DeviceDoorBase/GetAllBasic"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllControllerDeviceDoors result", result);
            }
            return result;

        }


        public List<DtoMetalDetectorGate> GetAllActiveMetalDetectorGates()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllActiveMetalDetectorGates Called");
            }
            var result = RestSharpClient.GetInstance().GetAsJson<List<MetalDetectorGateModel>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/ac/MetalDetectorGate/GetAllActiveGatesBasic"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllActiveMetalDetectorGates result", result);
            }
            return CommunicationModelMapper.MapMetalDetectorGateModelToDtoMetalDetectorGate(result);
        }

        public List<DtoXRayDevice> GetAllActiveXRayDevices()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllActiveXRayDevices Called");
            }
            var result = RestSharpClient.GetInstance().GetAsJson<List<XRayDeviceModel>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/ac/XRayDevice/GetAllActiveXRayDeviceBasic"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllActiveXRayDevices result", result);
            }
            return CommunicationModelMapper.MapXRayDeviceModelToDtoXRayDevice(result);
        }

        public List<DtoCamera> GetAllCameras()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllCameras Called");
            }
            var result = RestSharpClient.GetInstance().GetAsJson<List<CameraModel>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/ac/Camera/GetAllCamerasWithDeviceAdAreaInfoBasic"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.FetchBasicResourceData))
            {
                LoggingSystem.LogInfo("GetAllCameras result", result);
            }
            return CommunicationModelMapper.MapCameraModelToDtoCamera(result);
        }

        public void SubmitInvalidIoEvent(DtoInvalidAttendance entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SubmitInvalidIoEvent))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SubmitIoEvent))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SubmitIoEvent))
            {
                LoggingSystem.LogInfo("SubmitIoEvent Result", result);
            }
            return result;
        }

        public ServerMatchingResultModel SubmitServerMatching(DtoServerMatchData entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SubmitSeverMatching))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SubmitSeverMatching))
            {
                LoggingSystem.LogInfo("SubmitServerMatching result", result);
            }
            return result;
        }

        public void SubmitConnectionStatusChanged(List<DtoDeviceConnectionStatus> deviceConnectionStatuses)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.ChangeConnectionStatus))
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
                    DeviceNumber = cs.DeviceNumber,
                    IsConnected = cs.IsConnected
                }).ToArray()
            });
        }

        public void SubmitFingerEnrolled(DtoEmployeeFinger fingerInfo, int deviceNumber)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitFingerEnrolled Called", new { FingerInfo = fingerInfo, DeviceNumber = deviceNumber });
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
                Body = new NewFingerEnrolledModel
                {
                    DeviceNumber = deviceNumber,
                    FingerInfo = CommunicationModelMapper.MapDtoEmployeeFingerToEmployeeFingerModel(fingerInfo)
                }
            });
        }

        public void SubmitUserProfileImage(DtoEmployeeImage employeeImage, int deviceNumber)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitUserProfileImage Called", new { EmployeeImage = employeeImage, DeviceNumber = deviceNumber });
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
                Body = new NewEmployeeImageEnrolledModel
                {
                    DeviceNumber = deviceNumber,
                    EmployeeImage = CommunicationModelMapper.MapDtoEmployeeImageToEmployeeImageModel(employeeImage)
                }
            });
        }

        public void SubmitFaceEnrolled(DtoEmployeeFace faceInfo, int deviceNumber)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitFaceEnrolled Called", new { FaceInfo = faceInfo, DeviceNumber = deviceNumber });
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
                Body = new NewFaceEnrolledModel
                {
                    DeviceNumber = deviceNumber,
                    FaceInfo = CommunicationModelMapper.MapDtoEmployeeFaceToEmployeeFaceModel(faceInfo)
                }
            });
        }

        public void SubmitCardEnrolled(string cardNumber, int deviceNumber, long userId)
        {

            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitCardEnrolled Called", new { CardNumber = cardNumber, DeviceNumber = deviceNumber, UserId = userId });
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
                Body = new NewCardEnrolledModel
                {
                    DeviceNumber = deviceNumber,
                    Card = cardNumber,
                    EmployeeNumber = userId,
                }
            });
        }

        public void SubmitPalmEnrolled(DtoEmployeePalm palmInfo, int deviceNumber)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitPalmEnrolled Called", new { PalmInfo = palmInfo, DeviceNumber = deviceNumber });
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
                Body = new NewPalmModelEnrolled
                {
                    DeviceNumber = deviceNumber,
                    PalmInfo = CommunicationModelMapper.MapDtoEmployeePalmToEmployeePalmModel(palmInfo)
                }
            });
        }

        public void SubmitIrisEnrolled(DtoEmployeeIris irisInfo, int deviceNumber)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitIrisEnrolled Called", new { IrisInfo = irisInfo, DeviceNumber = deviceNumber });
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
                Body = new NewIrisModelEnrolled
                {
                    DeviceNumber = deviceNumber,
                    IrisInfo = CommunicationModelMapper.MapDtoEmployeeIrisToEmployeeIrisModel(irisInfo)
                }
            });
        }

        public void SubmitUserEnrolled(DtoEmployeeDeviceRelatedData userInfo, int deviceNumber, DtoEmployeeEnrolledSetting setting)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.UserEnrollment))
            {
                LoggingSystem.LogInfo("SubmitUserEnrolled Called", new { UserInfo = userInfo, DeviceNumber = deviceNumber, Setting = setting });
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/EmployeeEnrolledOnDevice"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new NewUserEnrolledModel
                {
                    DeviceNumber = deviceNumber,
                    EmployeeDeviceInfo = CommunicationModelMapper.MapDtoEmployeeToEmployeeModel(userInfo),
                    EmployeeEnrolledSetting = CommunicationModelMapper.MapDtoEmployeeEnrolledSettingToEmployeeEnrolledSettingModel(setting),
                }
            });
        }

        public void SubmitNotLiveValidPlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.PlateDetection))
            {
                LoggingSystem.LogInfo("SubmitNotLiveValidPlateDetectionCameraAttendance Called", attendance);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitNotLiveValidParkingCameraEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new SubmitValidCameraEventModel
                {
                    PlateString = attendance.PlateString,
                    AttendanceDateTimeNumeric = attendance.AttendanceDateTime.ToNumericDateTime(),
                    CameraId = attendance.CameraId,
                    CarImage = attendance.CarImage.IsCollectionNotNullOrEmpty() ? Convert.ToBase64String(attendance.CarImage) : null,
                    AcceptType = attendance.AcceptType,
                }
            });
        }

        public void SubmitNotLiveInvalidPlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.PlateDetection))
            {
                LoggingSystem.LogInfo("SubmitNotLiveInvalidPlateDetectionCameraAttendance Called", attendance);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitNotLiveInvalidParkingCameraEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new SubmitInvalidCameraEventModel
                {
                    PlateString = attendance.PlateString,
                    AttendanceDateTimeNumeric = attendance.AttendanceDateTime.ToNumericDateTime(),
                    CameraId = attendance.CameraId,
                    CarImage = attendance.CarImage.IsCollectionNotNullOrEmpty() ? Convert.ToBase64String(attendance.CarImage) : null,
                    AcceptType = attendance.AcceptType,
                }
            });
        }

        public void SubmitLivePlateDetectionCameraAttendance(DtoPlateDetectionCameraCarAttendance attendance)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.PlateDetection))
            {
                LoggingSystem.LogInfo("SubmitLivePlateDetectionCameraAttendance Called", attendance);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/general/HardwareEvent/SubmitParkingCameraEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new SubmitValidCameraEventModel
                {
                    PlateString = attendance.PlateString,
                    AttendanceDateTimeNumeric = attendance.AttendanceDateTime.ToNumericDateTime(),
                    CameraId = attendance.CameraId,
                    CarImage = Convert.ToBase64String(attendance.CarImage),
                }
            });
        }

        #endregion


        #region Self

        public List<DtoDeviceMealOrderData> GetMealOrderMealsInfo(int offsetTime)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.GetMealOrder))
            {
                LoggingSystem.LogInfo("GetMealOrderMealsInfo Called", offsetTime);
            }
            var result = RestSharpClient.GetInstance().GetAsJson<List<DtoDeviceMealOrderData>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/sf/SelfHardwareEvent/GetActiveDeviceDataToMonitor"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.GetMealOrder))
            {
                LoggingSystem.LogInfo("GetMealOrderMealsInfo result", result);
            }
            return result;
        }

        public void SubmitSelfEvent(DtoAttendance entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SelfAttendance))
            {
                LoggingSystem.LogInfo("SubmitSelfEvent Called", entity);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/sf/SelfHardwareEvent/SubmitSelfEvent"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new SubmitSelfEventModel
                {
                    DtoDeviceAttendance = CommunicationModelMapper.MapDtoAttendanceToDeviceAttendanceModel(entity)
                }
            });
        }

        public void SubmitSelfPrintResult(DtoSelfPrintResult entity)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.SelfPrintResult))
            {
                LoggingSystem.LogInfo("SubmitSelfPrintResult Called", entity);
            }
            RestSharpClient.GetInstance().PostAsJsonVoid(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/sf/SelfHardwareEvent/SubmitSelfPrintResult"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = CommunicationModelMapper.MapDtoSelfPrintResultToSelfPrintResultModel(entity)
            });
        }

        #endregion


        #region General info

        public DtoApplicationEncodedConfig GetSoftwareEncodedConfig()
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.GeneralInfo))
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
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.GeneralInfo))
            {
                LoggingSystem.LogInfo("GetSoftwareEncodedConfig result", result);
            }
            return result;
        }


        #endregion


        #region Access control


        public List<DtoCarAccessData> GetCameraCarAccessData(int cameraId, DateTime startDateTime, DateTime endDateTime)
        {
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.CameraCarAccessData))
            {
                LoggingSystem.LogInfo("GetCameraCarAccessData Called", new { CameraId = cameraId, StartDateTime = startDateTime, EndDateTime = endDateTime });
            }
            var result = RestSharpClient.GetInstance().PostAsJson<List<DtoCarAccessData>>(new RestApiRequestData
            {
                Uri = ApiCallHelpers.CombineUri(Config.BaseUri, "/api/ac/Camera/GetCarsThatAreValidToPass"),
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = Config.Password,
                    Username = Config.Username
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new CameraAccessDataParamsModel
                {
                    CameraId = cameraId,
                    StartDate = startDateTime.ToNumericDateTime(),
                    EndDate = endDateTime.ToNumericDateTime()
                }
            });
            if (AppConfigs.LogLevelKarnamaCall.HasFlag(KarnamaCallLogLevelEnumeration.CameraCarAccessData))
            {
                LoggingSystem.LogInfo("GetCameraCarAccessData result", result);
            }
            return result;
        }


        #endregion

    }
}
