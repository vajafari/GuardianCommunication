using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.ExternalServices.Shared;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Hardware.GuardianController.Model;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;
using Newtonsoft.Json;
using RestSharp;

namespace GuardianCommunication.Hardware.GuardianController
{
    /// <summary>
    /// حالت Standalone برد کنترلر گاردین: هر متد یک API برد را همان لحظه صدا می‌زند.
    /// بدنه هر فرمان همان مدل ارتباطی است که در Push محتوای دستور است (GuardianControllerUtils)
    /// </summary>
    public class GuardianControllerOnDemandAdapter : IDisposable
    {
        private const string JsonContentType = "application/json";

        private readonly string _baseAddress;
        private readonly string _communicationUsername;
        private readonly string _communicationPassword;
        public DtoDevice DeviceInfo { get; set; }

        public GuardianControllerOnDemandAdapter(DtoDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;
            if (string.IsNullOrWhiteSpace(DeviceInfo.DeviceIp))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorIpIsNotValid);
            if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorTcpPortIsNotValid);
            // رمز ممکن است خودش «:» داشته باشد؛ فقط اولین «:» جداکننده است
            var result = DeviceInfo.DevicePassword.ToNotNullString().Split(new[] { ':' }, 2);
            if (result.Length != 2 || result.Any(string.IsNullOrEmpty))
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorCommunicationSecurityDataIsNotValid);
            _communicationUsername = result[0];
            _communicationPassword = result[1];
            var useHttps = DeviceInfo.DeviceSettings?.GuardianControllerDeviceSettings != null
                           && DeviceInfo.DeviceSettings.GuardianControllerDeviceSettings.UseHttps;
            _baseAddress = $"{(useHttps ? "https" : "http")}://{DeviceInfo.DeviceIp.Trim()}:{DeviceInfo.TcpPort.Value}/";
        }

        #region Public Methods

        #region Other

        /// <summary>
        /// تنظیم ساعت برد با منطقه زمانی دستگاه
        /// </summary>
        public void SetDateTime(DateTime date)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetDateTime", new { DeviceInfo.Id });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapDateAndTimeToCommunicationModel(DeviceInfo, date)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/time"),
            });
            CheckResponseAndReturnError(response);
        }

        /// <summary>
        /// ساعت فعلی برد (UTC)
        /// </summary>
        public DateTime GetDateTime()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetDateTime", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/time"),
            });
            CheckResponseAndReturnError(response);
            return DeserializeResult<GuardianControllerDateAndTimeCommunicationModel>(response, true).date_time_epoch.ToUtcDateTimeFromEpochMillisecondTime();
        }

        public void RebootDevice()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand RebootDevice", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/reboot"),
            });
            CheckResponseAndReturnError(response);
        }

        private GuardianControllerDeviceInfoCommunicationModel GetDeviceInfo()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetDeviceInfo", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/info"),
            });
            CheckResponseAndReturnError(response);
            return DeserializeResult<GuardianControllerDeviceInfoCommunicationModel>(response, true);
        }

        public string GetFirmwareVersion()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetFirmwareVersion", new { DeviceInfo.Id });
            }
            return GetDeviceInfo()?.firmware_version;
        }

        /// <summary>
        /// برد API آزمون اتصال جداگانه ندارد؛ پاسخ درست به اطلاعات برد یعنی برد در دسترس است و رمز ارتباطی درست است
        /// </summary>
        public bool TestConnection()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand TestConnection", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/system/info"),
            });
            CheckResponseAndReturnError(response);
            var result = DeserializeResult<GuardianControllerDeviceInfoCommunicationModel>(response);
            return result != null && result.serial_number.IsNotNullOrEmpty();
        }

        public string GetSerialNumber()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetSerialNumber", new { DeviceInfo.Id });
            }
            return GetDeviceInfo()?.serial_number;
        }

        #endregion

        #region Attendance

        /// <summary>
        /// پاک کردن همه لاگ‌های برد (ترددها و رویدادهای درب)
        /// </summary>
        public bool ClearData()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand ClearData", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/logs/clear"),
            });
            CheckResponseAndReturnError(response);
            return true;
        }

        /// <summary>
        /// ترددهای مجاز برد در این بازه (UTC)، صفحه به صفحه
        /// </summary>
        public List<DtoAttendance> GetAttendances(DateTime startDate, DateTime endDate)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetAttendances", new { DeviceInfo.Id, StartDate = startDate, EndDate = endDate });
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
                    BodyString = ObjectHelper.SerializeAsJson(new GuardianControllerDateIntervalCommunicationModel
                    {
                        start_time = startDate.ToEpochMillisecondsTime(),
                        end_time = endDate.ToEpochMillisecondsTime(),
                        limit = GuardianControllerConstants.AttendanceLimit,
                        offset = offset
                    }),
                    BodyStringContentType = JsonContentType,
                    AuthorizationType = AuthorizationTypeEnumeration.BasicAuth,
                    Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/logs/attendances"),
                });
                CheckResponseAndReturnError(response);
                var resultFromDevice = DeserializeResult<List<GuardianControllerAttendanceCommunicationModel>>(response);
                if (resultFromDevice.IsCollectionNotNullOrEmpty())
                {
                    foreach (var item in resultFromDevice)
                    {
                        if (item.IsAccessGranted)
                        {
                            result.Add(GuardianControllerUtils.MapAttendanceFromCommunicationModel(DeviceInfo, item, DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand));
                        }
                    }
                }

                if (resultFromDevice.IsCollectionNullOrEmpty() || resultFromDevice.Count < GuardianControllerConstants.AttendanceLimit)
                {
                    break;
                }
                offset += GuardianControllerConstants.AttendanceLimit;
            }

            return result;
        }

        public int GetRecordCount()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetRecordCount", new { DeviceInfo.Id });
            }
            return GetDeviceInfo()?.success_attendance_count ?? 0;
        }

        #endregion

        #region Usering

        /// <summary>
        /// حذف کاربر با گروه‌ها و دسترسی‌های آسانش از برد
        /// </summary>
        public void DeleteUserById(long userIdOnDevice)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteUserById", new { DeviceInfo.Id, UserIdOnDevice = userIdOnDevice });
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
                BodyString = ObjectHelper.SerializeAsJson(new GuardianControllerUserIdOnDeviceCommunicationModel { user_id_on_device = userIdOnDevice }),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/users/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteAllUsers()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ClearData))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteAllUsers", new { DeviceInfo.Id });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/users/delete-all"),
            });
            CheckResponseAndReturnError(response);
        }

        /// <summary>
        /// اطلاعات یک کاربر روی برد؛ برد همان مدل کاربری را برمی‌گرداند که SetUserInfo می‌فرستد
        /// </summary>
        public DtoUserDeviceRelatedData GetUserInfoByUserIdOnDevice(long userIdOnDevice)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetUserInfoByUserIdOnDevice", new { DeviceInfo.Id, UserIdOnDevice = userIdOnDevice });
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
                Uri = ApiCallHelpers.CombineUri(_baseAddress, $"api/v1/users/get/{userIdOnDevice}"),
            });
            CheckResponseAndReturnError(response);
            var resultFromDevice = DeserializeResult<GuardianControllerUserCommunicationModel>(response);
            return resultFromDevice != null
                ? GuardianControllerUtils.MapUserFromCommunicationModel(DeviceInfo, resultFromDevice)
                : null;
        }

        /// <summary>
        /// ثبت کاربر با گروه‌های دسترسی و دسترسی‌های آسانش روی همین برد؛ این فهرست‌ها جایگزین فهرست‌های قبلی کاربر روی برد می‌شوند
        /// </summary>
        public void SetUserInfo(DtoUserDeviceRelatedData userInfo)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetUserInfo", new { DeviceInfo.Id, userInfo.UserIdOnDevice });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapUserToCommunicationModel(DeviceInfo, userInfo)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/users/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        public List<DtoUserInfoDefinedOnDevice> GetAllUsers()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetAllUsers", new { DeviceInfo.Id });
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
                    Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/users/get-all"),
                    QueryStringParameters = new Dictionary<string, string>
                    {
                        { "offset", offset.ToString() },
                        { "limit", GuardianControllerConstants.UserLimit.ToString() },
                    }
                });
                CheckResponseAndReturnError(response);
                var resultFromDevice = DeserializeResult<List<GuardianControllerUserCommunicationModel>>(response);
                if (resultFromDevice.IsCollectionNotNullOrEmpty())
                {
                    foreach (var item in resultFromDevice)
                    {
                        result.Add(new DtoUserInfoDefinedOnDevice
                        {
                            UserIdOnDevice = item.user_id_on_device,
                            Name = item.name,
                        });
                    }
                }

                if (resultFromDevice.IsCollectionNullOrEmpty() || resultFromDevice.Count < GuardianControllerConstants.UserLimit)
                {
                    break;
                }
                offset += GuardianControllerConstants.UserLimit;
            }

            return result;
        }

        public int GetUserCount()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.LogGeneralMethodsCall))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand GetUserCount", new { DeviceInfo.Id });
            }
            return GetDeviceInfo()?.user_count ?? 0;
        }

        /// <summary>
        /// ثبت یا به‌روزرسانی جداگانه دسترسی‌های آسان روی برد (با شناسه)؛ شمارنده تردد هر کدام روی برد حفظ می‌شود
        /// </summary>
        public void SetEasyAccesses(List<DtoGuardianControllerEasyAccess> easyAccesses)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetEasyAccesses", new { DeviceInfo.Id, EasyAccesses = easyAccesses });
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
                BodyString = ObjectHelper.SerializeAsJson(easyAccesses
                    .Select(easyAccess => GuardianControllerUtils.MapEasyAccessToCommunicationModel(DeviceInfo, easyAccess))
                    .ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/easy-accesses/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        /// <summary>
        /// حذف جداگانه دسترسی‌های آسان از برد (با شناسه)
        /// </summary>
        public void DeleteEasyAccesses(List<Guid> easyAccessIds)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.UserInfo))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteEasyAccesses", new { DeviceInfo.Id, EasyAccessIds = easyAccessIds });
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
                BodyString = ObjectHelper.SerializeAsJson(easyAccessIds.Distinct().ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/easy-accesses/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        #endregion

        #region AccessControl

        /// <summary>
        /// فرمان درب با شناسه درب
        /// </summary>
        /// <param name="doorId">شناسه درب</param>
        /// <param name="action">باز کردن لحظه‌ای، باز یا بسته نگه داشتن دائم، یا برگشت به حالت عادی</param>
        /// <param name="duration">فقط برای باز کردن لحظه‌ای؛ خالی یعنی مدت باز ماندن قفل درب</param>
        public void ChangeDoorStatus(Guid doorId, GuardianControllerDoorActionEnumeration action, int? duration)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ChangeDoorStatus))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand ChangeDoorStatus", new { DeviceInfo.Id, DoorId = doorId, Action = action, Duration = duration });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapChangeDoorStatusToCommunicationModel(doorId, action, duration)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/doors/control"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetCalendarTemplate(DtoGuardianControllerCalendarTemplate calendarTemplate)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetCalendarTemplate", new { DeviceInfo.Id, CalendarTemplate = calendarTemplate });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapCalendarTemplateToCommunicationModel(calendarTemplate)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/calendar-templates/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteCalendarTemplates(List<Guid> calendarTemplateIds)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteCalendarTemplates", new { DeviceInfo.Id, CalendarTemplateIds = calendarTemplateIds });
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
                BodyString = ObjectHelper.SerializeAsJson(calendarTemplateIds.Distinct().ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/calendar-templates/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetCalendar(DtoGuardianControllerCalendar calendar)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetCalendar", new { DeviceInfo.Id, Calendar = calendar });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapCalendarToCommunicationModel(calendar)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/calendars/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteCalendars(List<Guid> calendarIds)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteCalendars", new { DeviceInfo.Id, CalendarIds = calendarIds });
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
                BodyString = ObjectHelper.SerializeAsJson(calendarIds.Distinct().ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/calendars/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetAccessLevel(DtoGuardianControllerAccessLevel accessLevel)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetAccessLevel", new { DeviceInfo.Id, AccessLevel = accessLevel });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapAccessLevelToCommunicationModel(accessLevel)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/access-levels/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteAccessLevels(List<Guid> accessLevelIds)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteAccessLevels", new { DeviceInfo.Id, AccessLevelIds = accessLevelIds });
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
                BodyString = ObjectHelper.SerializeAsJson(accessLevelIds.Distinct().ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/access-levels/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        public void SetAccessGroup(DtoGuardianControllerAccessGroup accessGroup)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetAccessGroup", new { DeviceInfo.Id, AccessGroup = accessGroup });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapAccessGroupToCommunicationModel(accessGroup)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/access-groups/upsert"),
            });
            CheckResponseAndReturnError(response);
        }

        public void DeleteAccessGroups(List<Guid> accessGroupIds)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand DeleteAccessGroups", new { DeviceInfo.Id, AccessGroupIds = accessGroupIds });
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
                BodyString = ObjectHelper.SerializeAsJson(accessGroupIds.Distinct().ToList()),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/access-groups/delete"),
            });
            CheckResponseAndReturnError(response);
        }

        #endregion

        #region Configuration

        /// <summary>
        /// پیکربندی کامل سخت‌افزار برد (درب‌ها با رله‌ها، ویگند و تنظیمات درب، و ورودی/خروجی‌های برد)؛ جایگزین پیکربندی قبلی می‌شود
        /// </summary>
        public void SetHardwareConfig(List<DtoGuardianControllerHardwareConfigDoor> doors, List<DtoGuardianControllerHardwareConfigIoPort> ioPorts)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand SetHardwareConfig", new { DeviceInfo.Id, Doors = doors, IoPorts = ioPorts });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapHardwareConfigToCommunicationModel(doors, ioPorts)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/config/hardware"),
            });
            CheckResponseAndReturnError(response);
        }

        /// <summary>
        /// پایان همگام‌سازی کامل برد: برد هر چیزی را که در این فهرست‌ها نیست حذف می‌کند
        /// </summary>
        public void KeepOnly(DtoGuardianControllerKeepOnly keepOnly)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.AccessControl))
            {
                LoggingSystem.LogInfo("Guardian Controller OnDemand KeepOnly", new { DeviceInfo.Id, KeepOnly = keepOnly });
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
                BodyString = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapKeepOnlyToCommunicationModel(keepOnly)),
                BodyStringContentType = JsonContentType,
                Uri = ApiCallHelpers.CombineUri(_baseAddress, "api/v1/sync/keep-only"),
            });
            CheckResponseAndReturnError(response);
        }

        #endregion

        #endregion

        #region Utility

        /// <summary>
        /// خطاهای ارتباط: برد در دسترس نیست، رمز ارتباطی نادرست است، دسترسی ندارد، این API را نمی‌شناسد،
        /// یا فرمان را رد کرد (شرح خطای برد در خطا می‌آید)
        /// </summary>
        private void CheckResponseAndReturnError(RestResponse restResponse)
        {
            if (restResponse == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerResponseIsNotValid);
            }
            if (restResponse.IsSuccessful)
            {
                return;
            }

            switch (restResponse.StatusCode)
            {
                case 0:
                    if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ErrorCodes))
                    {
                        LoggingSystem.LogInfo("Guardian Controller OnDemand device is not reachable", new { DeviceInfo.Id, restResponse.ErrorMessage });
                    }
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect, restResponse.ErrorMessage);
                case HttpStatusCode.Unauthorized:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorCommunicationSecurityDataIsNotValid);
                case HttpStatusCode.Forbidden:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorCommunicationAccessDenied);
                case HttpStatusCode.NotFound:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerErrorCommunicationResourceNotFound);
                default:
                    {
                        var model = TryDeserializeError(restResponse);
                        if (model != null)
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerCommandRejected,
                                GuardianControllerUtils.DescribeError(model));
                        }
                    }
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerResponseIsNotValid,
                        $"{(int)restResponse.StatusCode} {restResponse.Content}");
            }
        }

        private static GuardianControllerErrorCommunicationModel TryDeserializeError(RestResponse restResponse)
        {
            try
            {
                return restResponse.Content.IsNullOrEmpty()
                    ? null
                    : JsonConvert.DeserializeObject<GuardianControllerErrorCommunicationModel>(restResponse.Content);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static T DeserializeResult<T>(RestResponse restResponse, bool throwErrorInNull = false,
            OperationResultEnumeration nullError = OperationResultEnumeration.CommunicationStatusGuardianControllerResponseIsNotValid)
        {
            if (restResponse == null || !restResponse.IsSuccessful || restResponse.Content.IsNullOrEmpty())
            {
                if (throwErrorInNull)
                {
                    throw new OperationCannotBeDoneException(nullError);
                }
                return default(T);
            }
            T result;
            try
            {
                result = JsonConvert.DeserializeObject<T>(restResponse.Content);
            }
            catch (JsonException exp)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusGuardianControllerResponseIsNotValid, exp.Message);
            }
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
