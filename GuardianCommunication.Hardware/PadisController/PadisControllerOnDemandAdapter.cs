using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;
using GuardianCommunication.Hardware.PadisController.Definition;
using GuardianCommunication.Hardware.PadisController.Model;
using GuardianCommunication.Hardware.Shared.Helpers;
using Newtonsoft.Json;
using RestSharp;

namespace GuardianCommunication.Hardware.PadisController
{
    public class PadisControllerOnDemandAdapter : IDisposable
    {

        private readonly string _baseAddress;
        private readonly string _communicationUsername;
        private readonly string _communicationPassword;
        public DtoCommunicationDeviceData DeviceInfo { get; set; }

        public PadisControllerOnDemandAdapter(DtoCommunicationDeviceData deviceInfo)
        {
            DeviceInfo = deviceInfo;
            if (string.IsNullOrEmpty(DeviceInfo.Ip))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorIpIsNotValid);
            if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorTcpPortIsNotValid);
            if (deviceInfo.CommunicationPassword.IsNullOrEmpty())
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorCommunicationSecurityDataIsNotValid);
            var result = deviceInfo.CommunicationPassword.Split(':');
            if (result.IsCollectionNullOrEmpty() || result.Length != 2 || result.Any(r => r.IsNullOrEmpty()))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorCommunicationSecurityDataIsNotValid);
            _communicationUsername = result[0];
            _communicationPassword = result[1];
            _baseAddress = $"{(deviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.PadisControllerUseHttps) ? "https" : "http")}://{deviceInfo.Ip}:{(deviceInfo.TcpPort)}/";

        }

        #region Public Methods

        #region Other

        public void SetDateTime(DateTime date)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetDateTime", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Body = new PadisControllerDateAndTimeCommunicationModel
                {
                    DateTimeEpoch = date.ToEpochMillisecondsTime()
                },
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/datetime"),
            });
            CheckResponseAndReturnError(response);
        }

        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetDateTime", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Get,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/datetime"),
            });
            CheckResponseAndReturnError(response);
            return DeserializeResult<PadisControllerDateAndTimeCommunicationModel>(response, true).DateTimeEpoch.ToDateTimeFromEpochMillisecondTime(true);
        }

        //public void EnableDevice()
        //{
        //    if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
        //    {
        //        LoggingSystem.LogInfo("Padis Controller OnDemand EnableDevice", DeviceInfo);
        //    }
        //    var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
        //    {
        //        Method = Method.Post,
        //        AuthorizationData = new RestApiBasicAuthorizationData
        //        {
        //            Password = _communicationPassword,
        //            Username = _communicationUsername
        //        },
        //        AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
        //        Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/system-enable"),
        //    });
        //    CheckResponseAndReturnError(response);
        //}

        //public void DisableDevice(int timeout)
        //{

        //    if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
        //    {
        //        LoggingSystem.LogInfo("Padis Controller OnDemand DisableDevice", DeviceInfo);
        //    }
        //    var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
        //    {
        //        Method = Method.Post,
        //        AuthorizationData = new RestApiBasicAuthorizationData
        //        {
        //            Password = _communicationPassword,
        //            Username = _communicationUsername
        //        },
        //        AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
        //        Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/system-disable"),
        //    });
        //    CheckResponseAndReturnError(response);
        //}

        public void RebootDevice()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand RebootDevice", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/system-reboot"),
            });
            CheckResponseAndReturnError(response);
        }


        private PadisControllerDeviceInfoCommunicationModel GetDeviceInfo()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetFirmwareVersion", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Get,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/get-device-info"),
            });
            CheckResponseAndReturnError(response);
            return DeserializeResult<PadisControllerDeviceInfoCommunicationModel>(response, true);
        }


        public string GetFirmwareVersion()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetFirmwareVersion", DeviceInfo);
            }
            return GetDeviceInfo()?.FirmwareInfo;
        }

        public bool TestConnection()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand TestConnection", DeviceInfo);
            }

            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Get,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/ping"),
            });
            CheckResponseAndReturnError(response);
            var result = DeserializeResult<PadisControllerTestConnectionCommunicationModel>(response, true);
            return result?.Message == "pong";
        }

        public string GetSerialNumber()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetSerialNumber", DeviceInfo);
            }
            var result = GetDeviceInfo();
            return result?.SerialNumber;
        }

        #endregion

        #region Attendance

        public bool ClearData()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand ClearData", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/hardware_events/clear-all"),
            });
            CheckResponseAndReturnError(response);
            return true;
        }

        public List<DtoAttendance> GetAttendances(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ReadLog))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetAttendances", DeviceInfo);
            }
            var result = new List<DtoAttendance>();
            var offset = 0;
            while (true)
            {

                var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
                {
                    Method = Method.Post,
                    AuthorizationData = new RestApiBasicAuthorizationData
                    {
                        Password = _communicationPassword,
                        Username = _communicationUsername
                    },
                    Body = new PadisControllerDateIntervalCommunicationModel
                    {
                        StartTime = startDate.ToEpochMillisecondsTime(),
                        EndTime = endDate.ToEpochMillisecondsTime(),
                        Limit = PadisControllerConstants.AttendanceLimit,
                        Offset = offset
                    },
                    AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                    Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/hardware_events/get-attendances"),
                });
                CheckResponseAndReturnError(response);
                var resultFromDevice = DeserializeResult<PadisControllerAttendanceCommunicationModel[]>(response);
                if (resultFromDevice.IsCollectionNotNullOrEmpty())
                {
                    foreach (var item in resultFromDevice)
                    {
                        if (item.IsAccessGranted)
                        {
                            result.Add(new DtoAttendance
                            {
                                EmployeeNumber = item.EmployeeNumber,
                                ApplicationId = DeviceInfo.ApplicationId,
                                AttendanceDateTime = item.AttendanceDateTime.ToDateTimeFromEpochMillisecondTime(true),
                                AttendanceSource = AttendanceSourceEnumeration.Device,
                                CameraId = null,
                                DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                                DeviceNumber = DeviceInfo.DeviceNumber,
                                DoorId = item.DoorId,
                                InsertDateTime = DateTime.Now,
                                IsSent = false,
                                IsInvalid = item.VerificationStatus != PadisControllerAttendanceStatusEnumeration.FailedForPassAccuracy,
                                RfCardNumber = item.RfCardNumber,
                                StatusCode = 0,
                                VerificationStyle = null,
                            });
                        }
                    }
                }

                if (resultFromDevice.IsCollectionNullOrEmpty() || result.Count < PadisControllerConstants.AttendanceLimit)
                {
                    break;
                }
                else
                {
                    offset += PadisControllerConstants.AttendanceLimit;
                }

            }

            return result;

        }

        public int GetRecordCount()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ReadLog))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetRecordCount", DeviceInfo);
            }
            return GetDeviceInfo()?.SuccessAttendanceCount ?? 0;
        }

        #endregion

        #region Usering

        public void DeleteUserById(long userId)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand DeleteUserById", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, $"/api/v1/user/delete-by-user-id/{userId}"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand DeleteAllUsers", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/user/delete-all-users"),
            });
            CheckResponseAndReturnError(response);

        }

        public DtoEmployeeDeviceRelatedData GetUserInfoByUserId(long userId)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetUserInfoByUserId", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Get,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, $"api/v1/user/get/{userId}"),
            });

            CheckResponseAndReturnError(response);
            var resultFromDevice = DeserializeResult<PadisControllerUserCommunicationModel>(response);
            if (resultFromDevice != null)
            {
                return new DtoEmployeeDeviceRelatedData
                {
                    PadisControllerUserAccessData = new DtoPadisControllerUserAccessData
                    {
                        EasyPermissions = resultFromDevice.user_permissions.IsCollectionNotNullOrEmpty()
                            ? resultFromDevice.user_permissions.Select(up => new DtoPadisControllerUserEasyPermission
                            {
                                CalendarNumber = up.calendar_number,
                                DoorId = up.door_id,
                                StartDateTime = up.start_date.ToDateTimeFromEpochMillisecondTime(true),
                                EndDateTime = up.end_date.ToDateTimeFromEpochMillisecondTime(true),
                            }).ToList() : null,
                        AttendanceLimitations = resultFromDevice.attendance_limitations.IsCollectionNotNullOrEmpty()
                            ? resultFromDevice.attendance_limitations.Select(up => new DtoPadisControllerUserLimitationAttendance
                            {
                                DoorId = up.door_id,

                                StartDateTime = up.start_date.ToDateTimeFromEpochMillisecondTime(true),
                                EndDateTime = up.end_date.ToDateTimeFromEpochMillisecondTime(true),
                                AttendanceCount = up.attendance_count,
                                IoType = (DeviceIoTypeEnumeration?)up.io_type,
                            }).ToList() : null,
                        TimeLimitations = resultFromDevice.time_limitations.IsCollectionNotNullOrEmpty()
                            ? resultFromDevice.time_limitations.Select(up => new DtoPadisControllerUserLimitationTime
                            {
                                DoorId = up.door_id,
                                StartDateTime = up.start_date.ToDateTimeFromEpochMillisecondTime(true),
                                EndDateTime = up.end_date?.ToDateTimeFromEpochMillisecondTime(true),
                            }).ToList() : null,
                    },

                    RfCardNumbers = resultFromDevice.rf_card_numbers,
                    Password = resultFromDevice.password,
                    EmployeeNumber = resultFromDevice.user_id,
                    IsEnable = resultFromDevice.is_enable,
                    UserType = resultFromDevice.user_type,
                };
            }
            return null;
        }

        public void SetUserInfo(DtoEmployeeDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("PadisController OnDemand SetUserInfo", new { DeviceInfo, User = userInfo });
            }

            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerUserCommunicationModel
                {
                    user_group_numbers = userInfo.PadisControllerUserAccessData.UserGroupNumbers,
                    user_permissions = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.EasyPermissions.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.EasyPermissions.Select(up => new PadisControllerEasyPermissionCommunicationModel
                    {
                        calendar_number = up.CalendarNumber,
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime.ToEpochMillisecondsTime(),
                    }).ToList() : null,
                    attendance_limitations = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.AttendanceLimitations.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.AttendanceLimitations.Select(up => new PadisControllerUserLimitationAttendanceCommunicationModel
                    {
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime.ToEpochMillisecondsTime(),
                        attendance_count = up.AttendanceCount,
                        io_type = (int?)up.IoType,
                    }).ToList() : null,
                    time_limitations = userInfo.PadisControllerUserAccessData != null && userInfo.PadisControllerUserAccessData.TimeLimitations.IsCollectionNotNullOrEmpty()
                    ? userInfo.PadisControllerUserAccessData.TimeLimitations.Select(up => new PadisControllerUserLimitationTimeCommunicationModel
                    {
                        door_id = up.DoorId,
                        start_date = up.StartDateTime.ToEpochMillisecondsTime(),
                        end_date = up.EndDateTime?.ToEpochMillisecondsTime(),
                    }).ToList() : null,
                    rf_card_numbers = userInfo.RfCardNumbers,
                    password = userInfo.Password,
                    user_id = userInfo.EmployeeNumber,
                    is_enable = userInfo.IsEnable,
                    user_type = userInfo.UserType,
                    start_date = userInfo.StartTime.ToEpochMillisecondsTime(),
                    end_date = DeviceSharedHelperMethods.GetEndDate(userInfo.EndTime, ProducerEnumeration.Padis, SdkVersionEnumeration.SdkVersion1).ToEpochMillisecondsTime(),
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, $"api/v1/user/set-user-info"),
            });
            CheckResponseAndReturnError(response);
        }

        public List<DtoUserInfoDefinedOnDevice> GetAllUsers()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetUserInfoByUserId", DeviceInfo);
            }
            var result = new List<DtoUserInfoDefinedOnDevice>();
            var offset = 0;
            while (true)
            {

                var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
                {
                    Method = Method.Get,
                    AuthorizationData = new RestApiBasicAuthorizationData
                    {
                        Password = _communicationPassword,
                        Username = _communicationUsername
                    },
                    AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                    Uri = ApiCallHelpers.CombineUri(_baseAddress, $"/api/v1/user/get-all"),
                    QueryStringParameters = new Dictionary<string, string>
                    {
                        {"offset" , offset.ToString()},
                        {"limit" , PadisControllerConstants.UserLimit.ToString()},
                        {"include_cards" , ""},
                        {"include_fast_permissions" , "false"},
                        {"include_time_limitations" , "false"},
                        {"include_attendance_limitations" , "false"}
                    }
                });

                CheckResponseAndReturnError(response);

                var resultFromDevice = DeserializeResult<List<PadisControllerUserCommunicationModel>>(response);
                if (resultFromDevice.IsCollectionNotNullOrEmpty())
                {
                    foreach (var item in resultFromDevice)
                    {
                        result.Add(new DtoUserInfoDefinedOnDevice
                        {
                            EmployeeNumber = item.user_id,
                        });
                    }
                }
                if (resultFromDevice.IsCollectionNullOrEmpty() || result.Count < PadisControllerConstants.UserLimit)
                {
                    break;
                }
                else
                {
                    offset += PadisControllerConstants.UserLimit;
                }

            }
            return result;
        }

        public int GetUserCount()
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand GetUserCount", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Get,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1info"),
            });
            CheckResponseAndReturnError(response);
            return DeserializeResult<PadisControllerDeviceInfoCommunicationModel>(response, true).UserCount;
        }

        #endregion

        #region AccessControl

        /// <summary>
        /// Open/Close door
        /// </summary>
        /// <param name="doorId">Id of door</param>
        /// <param name="lockStatus">   0 = Close -----1 = Open   </param>
        /// <param name="delay"> null: Permanent ----- Number = Delay</param>
        public void ChangeDoorStatus(int doorId, int lockStatus, int? delay)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.ChangeDoorStatus))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand ChangeDoorStatus", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerChangeLockStatusCommunicationModel
                {
                    duration = delay,
                    door_id = doorId,
                    lock_status = (short)lockStatus,
                    force_override_emergency = false,
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/access/door/control/lock-status"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetRelay(DtoPadisControllerRelay relay)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetRelay", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerRelayCommunicationModel
                {
                    RelayNumber = relay.RelayNumber,
                    IsActive = relay.IsActive,
                    RelayType = relay.RelayType,
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1hardware/relay"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetIoPort(DtoPadisControllerIoPort ioPort)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetIoPort", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerIoPortCommunicationModel
                {
                    IoNumber = ioPort.IoNumber,
                    IsActive = ioPort.IsActive,
                    Id = ioPort.Id
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1hardware/ioport"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetWiegand(DtoPadisControllerWiegand wiegand)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetWiegand", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerWiegandCommunicationModel
                {
                    WiegandNumber = wiegand.WiegandNumber,
                    IsActive = wiegand.IsActive,
                    Id = wiegand.Id,
                    WiegandDataType = (int)wiegand.WiegandDataType,
                    WiegandFormat = (int)wiegand.WiegandFormat,
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1hardware/Wiegand"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetDoor(DtoPadisControllerDeviceDoor door)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetDoor", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerDeviceDoorCommunicationModel
                {
                    CombinationAccessGroupNumbersInJson = door.CombinationAccessGroupNumbersInJson,
                    Id = door.Id,
                    DoorNumberOnDevice = door.DoorNumberOnDevice,
                    IsActive = door.IsActive,
                    OpenTimeCalendarNumber = door.OpenTimeCalendarNumber,
                    PassVerificationIoPortId = door.PassVerificationIoPortId,
                    ReaderDeviceNumber = door.ReaderDeviceNumber,
                    WiegandId = door.WiegandId,
                    ReaderIoType = (int?)door.ReaderIoType
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1hardware/door"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetCalendar(DtoPadisControllerCalendar calendar)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetCalendar", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerCalendarCommunicationModel
                {
                    title = calendar.Title,
                    calendar_number = calendar.CalendarNumber,
                    is_active = calendar.IsActive,
                    details = calendar.Details?.Select(d => new PadisControllerCalendarDetailsCommunicationModel
                    {
                        date = d.Date.Date.ToEpochMillisecondsTime(),
                        end_time = d.EndTime,
                        start_time = d.StartTime,
                    }).ToList(),
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/access/calendar/save"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetAccessLevel(DtoPadisControllerAccessLevel accessLevel)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetAccessLevel", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerAccessLevelCommunicationModel
                {
                    title = accessLevel.Title,
                    access_level_number = accessLevel.AccessLevelNumber,
                    is_active = accessLevel.IsActive,
                    door_ids = accessLevel.Doors?.Select(ald => ald.DoorId).ToList(),
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/access/level/set-access-level"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetAccessGroup(DtoPadisControllerAccessGroup accessGroup)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Padis Controller OnDemand SetAccessGroup", DeviceInfo);
            }
            var response = RestSharpClient.GetInstance().ExecuteAndReturnResponse(new RestApiRequestData
            {
                Method = Method.Post,
                AuthorizationData = new RestApiBasicAuthorizationData
                {
                    Password = _communicationPassword,
                    Username = _communicationUsername
                },
                Body = new PadisControllerAccessGroupCommunicationModel
                {
                    title = accessGroup.Title,
                    access_group_number = accessGroup.AccessGroupNumber,
                    is_active = accessGroup.IsActive,
                    access_data = accessGroup.AccessData?.Select(ald => new PadisControllerAccessGroupAccessDataCommunicationModel
                    {
                        access_level_number = ald.AccessLevelNumber,
                        calendar_number = ald.CalendarNumber,
                    }).ToList(),
                },
                AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "/api/v1/access/access-group/save"),
            });
            CheckResponseAndReturnError(response);
        }

        #endregion

        #endregion

        #region Utility

        private void CheckResponseAndReturnError(RestResponse restResponse)
        {
            if (restResponse == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerResponseIsNotValid);
            }
            if (restResponse.IsSuccessful)
            {
                return;
            }

            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorCommunicationSecurityDataIsNotValid);
                case HttpStatusCode.Forbidden:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorCommunicationAccessDenied);
                case HttpStatusCode.NotFound:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusPadisControllerErrorCommunicationResourceNotFound);
                default:
                    {
                        var model = JsonConvert.DeserializeObject<PadisControllerErrorCommunicationModel>(restResponse.Content);
                        if (model != null)
                        {
                            throw new OperationCannotBeDoneException(DeviceSharedHelperMethods
                                .MapToOperationResult((int)model.ErrorCode, DeviceInfo));
                        }
                    }
                    break;
            }
        }

        private static T DeserializeResult<T>(RestResponse restResponse, bool throwErrorInNull = false,
            OperationResultEnumeration nullError = OperationResultEnumeration.CommunicationStatusPadisControllerResponseIsNotValid)
        {
            if (restResponse == null || !restResponse.IsSuccessful || restResponse.Content == null)
            {
                return default(T);
            }
            var result = JsonConvert.DeserializeObject<T>(restResponse.Content);
            if (result == null && throwErrorInNull)
            {
                throw new OperationCannotBeDoneException(nullError);
            }
            return result;
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Free any other managed objects here.
            }
            // Free Unmanaged resource
            _disposed = true;
        }

        #endregion

    }

}
