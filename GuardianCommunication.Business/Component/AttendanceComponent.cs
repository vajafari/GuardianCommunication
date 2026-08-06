using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Component
{
    public class AttendanceComponent : BaseComponent
    {
        //private static readonly object LockObject = new object();

        public AttendanceComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        #region Attendance


        public DtoServerMatchResult ProcessServerMatchEvent(DtoServerMatchData serverMatchData)
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            try
            {
                var deviceComponent = new DeviceComponent(RepositoryFactory);
                var deviceCache = deviceComponent.GetDeviceByDeviceNumber(serverMatchData.DeviceNumber);
                if (deviceCache != null)
                {
                    if (!deviceComponent.IsDeviceNumberValid(deviceCache.DeviceNumber, ValidSerialNumberCheckTypeEnumeration.Attendance))
                    {
                        if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
                        {
                            LoggingSystem.LogInfo("ProcessServerMatchEvent Invalid device", new
                            {
                                MatchData = serverMatchData,
                            });
                        }
                        return new DtoServerMatchResult
                        {
                            IsSuccessfullyProcessed = false
                        };
                    }
                    var resultServerMatch = karnamaComponent.SubmitServerMatching(serverMatchData);
                    if (resultServerMatch.IsSuccessfullyProcessed
                        && resultServerMatch.Attendance != null
                        && resultServerMatch.Attendance.UserId > 0)
                    {

                        var entity = new DtoAttendance
                        {
                            DeviceNumber = resultServerMatch.Attendance.DeviceNumber,
                            CameraId = null,
                            IoType = resultServerMatch.Attendance.IoType,
                            VerificationStyle = resultServerMatch.Attendance.VerificationStyle,
                            AttendanceDateTime = resultServerMatch.Attendance.AttendanceDateTime.FromNumericDateTime(),
                            ApplicationId = resultServerMatch.Attendance.ApplicationId,
                            EmployeeNumber = resultServerMatch.Attendance.UserId,
                            AttendanceSource = resultServerMatch.Attendance.AttendanceSource,
                            DeviceAttendanceIoRetrieveType = null,
                            IsInvalid = false,
                            IsSent = true,
                            RfCardNumber = resultServerMatch.Attendance.RfCardNumber,
                            StatusCode = resultServerMatch.Attendance.StatusCode,
                        };
                        entity.IsSent = true;
                        var resultOfSave = SaveAttendance
                            (new List<DtoAttendance> { entity }, false, false, false);
                        if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
                        {
                            var thread = new Thread(() => HookIoEvent(resultOfSave.Successful.First()));
                            thread.Start();

                            return new DtoServerMatchResult
                            {
                                IsSuccessfullyProcessed = true,
                                UserId = resultServerMatch.Attendance.UserId
                            };
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on ProcessIoEvent thread");
            }

            return new DtoServerMatchResult
            {
                IsSuccessfullyProcessed = false
            };
        }

        /// <summary>
        /// ثبت تردد های دستی
        /// دوربین تشخیص پلاک
        /// API
        /// در این تردد ها نوع ورود و خروج و اپلیکشین و چک کردن 
        /// </summary>
        public void HookOtherResourcesAttendance(DtoAttendance attendance, bool checkDuplicateInterval)
        {
            attendance.IsSent = true;
            var resultOfSave = SaveAttendance(new List<DtoAttendance> { attendance }
                , checkDuplicateInterval, false, false);
            if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
            {
                var newAttendance = resultOfSave.Successful.FirstOrDefault();
                try
                {
                    HookIoEvent(newAttendance);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on HookOtherResourceAttendances to hook system thread");
                }
            }
        }

        public DtoAttendanceSaveResult SaveAttendance
            (List<DtoAttendance> attendances
                , bool checkDuplicateInterval
                , bool checkDeviceSerialNumber
                , bool applyDeviceRelatedProperties)
        {

            var result = new DtoAttendanceSaveResult();
            if (attendances.IsCollectionNullOrEmpty())
            {
                return result;
            }
            if (ApplicationEmbeddedInfo.ExpireDate.HasValue && DateTime.Now > ApplicationEmbeddedInfo.ExpireDate.Value)
            {
                return new DtoAttendanceSaveResult
                {
                    UnknownErrorSave = attendances
                };
            }
            var hookComponent = new HookComponent(RepositoryFactory);
            var activeAttendanceHooks = hookComponent.SearchHookSystemCache(
                h => h.Details.IsCollectionNotNullOrEmpty()
                     && h.Details.Any(hd => hd.IsActive && hd.DetailType == HookDetailTypeEnumeration.Attendance));
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
            {
                LoggingSystem.LogInfo("SaveAttendance process log before lock", attendances);
            }
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);

            foreach (var attendance in attendances)
            {
                try
                {
                    result.AllRecords.Add(attendance);
                    if (!ValidateAttendance(attendance))
                    {
                        result.InvalidAttendances.Add(attendance);
                        continue;
                    }

                    if (attendance.DeviceNumber.HasValue
                        && checkDeviceSerialNumber
                        && !deviceComponent.IsDeviceNumberValid(attendance.DeviceNumber.Value, ValidSerialNumberCheckTypeEnumeration.Attendance))
                    {
                        result.InvalidDeviceSerialNumberRecords.Add(attendance);
                        continue;
                    }
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
                    {
                        LoggingSystem.LogInfo("SaveAttendance process attendance is valid", attendances);
                    }

                    DtoDevice deviceCache = null;
                    if (attendance.DeviceNumber.HasValue)
                    {
                        deviceCache = deviceComponent.GetDeviceByDeviceNumber(attendance.DeviceNumber.Value);
                        if (deviceCache == null)
                        {
                            LoggingSystem.LogWarning("DEVICE NOT FOUND IN CACHE", attendance.DeviceNumber);
                        }
                        if (applyDeviceRelatedProperties && deviceCache != null)
                        {
                            if (deviceCache.DeviceTypeSummary != null && deviceCache.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.Padis && attendance.DoorId.HasValue)
                            {
                                var doorInCache = deviceComponent.GetDeviceDoorByDoorId(attendance.DoorId.Value);
                                if (doorInCache != null)
                                {
                                    attendance.ApplicationId = doorInCache.ReaderApplicationId;
                                    attendance.IoType = doorInCache.ReaderIoType;
                                    attendance.ReaderDeviceNumber = doorInCache.ReaderDeviceNumber;
                                }
                                else
                                {
                                    attendance.ApplicationId = deviceCache.ApplicationId;
                                    attendance.IoType = deviceCache.IoType;
                                }
                            }
                            else
                            {
                                attendance.ApplicationId = deviceCache.ApplicationId;
                                attendance.IoType = deviceCache.IoType;
                            }
                        }
                    }
                    var attendanceRegisterIntervalSettings =
                        systemConfigComponent.GetAttendanceRegisterIntervalSetting(attendance.ApplicationId);

                    var hookSystemIds = new List<int>();
                    if (deviceCache != null
                        && deviceCache.IsHookActive
                        && activeAttendanceHooks.IsCollectionNotNullOrEmpty())
                    {
                        hookSystemIds = activeAttendanceHooks.Select(hs => hs.Id).ToList();
                    }

                    var resultOfInsert = RepositoryFactory.GetAttendanceRepository().Insert(attendance, attendanceRegisterIntervalSettings, hookSystemIds);
                    if (resultOfInsert == -1)
                    {
                        // Record is duplicate
                        result.ExistingRecords.Add(attendance);
                        continue;
                    }
                    result.Successful.Add(attendance);
                }
                catch (Exception exp)
                {
                    var resultOfTranslate = ExceptionHelper.TranslateSqlExceptionToOperationResultStatus(exp);
                    if (resultOfTranslate == OperationResultEnumeration.CommunicationObjectUniqueKeyConstraint)
                    {
                        result.ExistingRecords.Add(attendance);
                        if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
                        {
                            LoggingSystem.LogInfo("Existing Attendance", new
                            {
                                Attendance = attendance,
                            });
                        }
                    }
                    else
                    {
                        result.UnknownErrorSave.Add(attendance);
                        LoggingSystem.LogError(exp, "Error on save attendance",
                            ObjectHelper.SerializeAsJson(attendance));
                    }
                }
            }


            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
            {
                LoggingSystem.LogInfo("Save On AttendanceComponent.SaveAttendance", new
                {
                    result.Successful,
                    result.ExistingRecords,
                    result.UnknownErrorSave,
                    result.InvalidDeviceSerialNumberRecords,
                    result.InvalidAttendances,
                });
            }

            return result;
        }

        private static bool ValidateAttendance(DtoAttendance attendance)
        {
            if (attendance.EmployeeNumber <= 0)
            {
                return false;
            }

            return true;
        }

        public List<DtoAttendance> Search(PagingData<AttendanceFilter, AttendanceSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetAttendanceRepository().Search(searchInfo);
        }


        #region Internal methods


        internal bool IsAttendanceDuplicate(DtoAttendance attendance)
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var attendanceRegisterIntervalSettings =
                systemConfigComponent.GetAttendanceRegisterIntervalSetting(attendance.ApplicationId);
            if (attendanceRegisterIntervalSettings.Count > 0)
            {
                if (CheckExistence(attendance.EmployeeNumber, attendance.AttendanceDateTime, attendanceRegisterIntervalSettings))
                {
                    return true;
                }
            }

            return false;
        }


        #endregion


        #region Private methods


        private bool CheckExistence(long employeeNumber, DateTime attendanceDate, List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings)
        {
            return RepositoryFactory.GetAttendanceRepository().CheckExistence(employeeNumber, attendanceDate, registerIntervalSettings);
        }

        #endregion

        #endregion


        #region Karnama Attendance

        public void ResendUnsentAttendancesToKarnama()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var unsentAttendances = Search(new PagingData<AttendanceFilter, AttendanceSortEnumeration>
            {
                Filter = new AttendanceFilter
                {
                    IsSent = false,
                },
                CurrentPage = new CurrentPageInfo
                {
                    PageNumber = 1,
                    ItemPerPage = systemConfigComponent.GetSystemConfigCache().AttendanceSendToKarnamaTimerRecordCount
                },
                SortItems = new List<SortInfo<AttendanceSortEnumeration>>
                {
                    new SortInfo<AttendanceSortEnumeration>
                        { SortItemEnum = AttendanceSortEnumeration.AttendanceDate, SortType = SortTypeEnum.Asc }
                }
            });


            if (unsentAttendances.IsCollectionNotNullOrEmpty())
            {
                var markAsSent = new List<DtoAttendance>();
                foreach (var entity in unsentAttendances)
                {
                    try
                    {
                        var resultOfSave = SubmitIoEventToKarnama(entity);
                        if (resultOfSave.IsAttendanceSavedAtKarnama())
                        {
                            markAsSent.Add(entity);
                        }
                        else
                        {
                            LoggingSystem.LogWarning("Attendance not saved in karnama", new
                            {
                                Attendance = entity,
                                Result = resultOfSave
                            });
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on send attendance to karnama");
                    }
                }
                if (markAsSent.IsCollectionNotNullOrEmpty())
                {
                    MarkAttendanceAsSentToKarnama(markAsSent);
                }
            }

        }

        public void MarkAttendanceAsSentToKarnama(List<DtoAttendance> attendances)
        {
            if (!attendances.IsCollectionNotNullOrEmpty()) return;
            try
            {
                RepositoryFactory.GetAttendanceRepository().MarkAsSent(attendances);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }

        }

        public AttendanceProcessResultModel SubmitIoEventToKarnama(DtoAttendance entity)
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var result = karnamaComponent.SubmitIoEvent(entity);
            if (systemConfigComponent.GetSystemConfigCache().AttendanceSaveKarnamaSendResult)
            {
                RepositoryFactory.GetAttendanceSendToKarnamaResultRepository().Insert(new DtoAttendanceSendToKarnamaResult()
                {
                    StatusCode = result.ResultCode,
                    AttendanceId = entity.Id,
                    IsSuccessful = result.IsSuccessfullyProcessed,
                    ExceptionMessage = string.Empty,
                    SendTime = DateTime.Now
                });
            }
            return result;
        }

        public void SubmitInvalidIoEventToKarnama(DtoInvalidAttendance entity)
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            karnamaComponent.SubmitInvalidIoEvent(entity);
        }

        #endregion


        #region Attendance Hook

        public List<DtoAttendanceHookSystem> GetAttendancesHookSystem(long attendanceId)
        {

            return RepositoryFactory.GetAttendanceHookSystemRepository().Search(
                new PagingData<AttendanceHookSystemFilter, AttendanceHookSystemSortEnumeration>
                {
                    Filter = new AttendanceHookSystemFilter
                    {
                        AttendanceIds = new List<long>
                        {
                            attendanceId
                        }
                    }
                });
        }

        public List<DtoUnhookedAttendances> GetUnhookedAttendances(int count)
        {
            return RepositoryFactory.GetAttendanceRepository().GetUnhookedAttendances(count);
        }

        public void MarkAttendanceAsSendToHookSystem(long attendanceId, int hookSystemId)
        {
            RepositoryFactory.GetAttendanceHookSystemRepository().MarkAsSent(attendanceId, hookSystemId);
        }

        public void IncreaseAttendanceHookSystemRetryCount(long attendanceId, int hookSystemId)
        {
            RepositoryFactory.GetAttendanceHookSystemRepository().IncreaseRetryCount(attendanceId, hookSystemId);
        }

        public void HookUnsentAttendances()
        {
            var hookComponent = new HookComponent(RepositoryFactory);
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var unsentAttendances = attendanceComponent.GetUnhookedAttendances
                (systemConfigComponent.GetSystemConfigCache().AttendanceHookTimerRecordCount);

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
            {
                LoggingSystem.LogInfo(ObjectHelper.SerializeAsJson(unsentAttendances), "Unsent records list");
            }
            foreach (var item in unsentAttendances)
            {
                var deviceInCache = deviceComponent.SearchDeviceCache(row => row.DeviceNumber == item.DeviceNumber).FirstOrDefault();
                if (deviceInCache != null && !deviceInCache.IsHookActive)
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
                    {
                        LoggingSystem.LogInfo("Hooking IO skipped because of system setting", $"Data for hook is {ObjectHelper.SerializeAsJson(item)}");
                    }
                }
                var hookSystemCache = hookComponent.SearchHookSystemCache
                    (row => row.Id == item.HookSystemId).FirstOrDefault();
                var detail = hookSystemCache?.Details.FirstOrDefault
                    (row => row.DetailType == HookDetailTypeEnumeration.Attendance);
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
                {
                    LoggingSystem.LogInfo(
                        detail != null
                            ? $"Hook system detail is  {ObjectHelper.SerializeAsJson(detail)} data is {ObjectHelper.SerializeAsJson(item)} "
                            : $"Hook system detail is is null and data is {ObjectHelper.SerializeAsJson(item)}", "Sending Unhooked items");
                }

                if (detail != null)
                {
                    try
                    {
                        hookComponent.CallHookApi<DtoAttendance>(hookSystemCache, detail, item);
                        attendanceComponent.MarkAttendanceAsSendToHookSystem(item.Id, item.HookSystemId);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogHookError(exp, new { HookSystemDetail = detail, Attendance = item });
                        try
                        {
                            attendanceComponent.IncreaseAttendanceHookSystemRetryCount(item.Id, item.HookSystemId);
                        }
                        catch (Exception expSave)
                        {
                            LoggingSystem.LogError(expSave, "Error on IncreaseAttendanceHookSystemRetryCount", $"Hook system detail is {ObjectHelper.SerializeAsJson(detail)} data is {ObjectHelper.SerializeAsJson(item)} ");
                        }
                    }

                }

            }
        }

        public void HookIoEvent(DtoAttendance attendance)
        {
            var hookComponent = new HookComponent(RepositoryFactory);
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            var deviceInCache = deviceComponent.SearchDeviceCache(row => row.DeviceNumber == attendance.DeviceNumber).FirstOrDefault();
            if (deviceInCache != null && !deviceInCache.IsHookActive)
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
                {
                    LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)}", "Hooking IO skipped because of system setting");
                }

                return;
            }
            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var attendanceHookSystems = attendanceComponent.GetAttendancesHookSystem(attendance.Id);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
            {
                LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)} and attendance hook systems are {ObjectHelper.SerializeAsJson(attendanceHookSystems)}", "IO hook");
            }
            if (attendanceHookSystems.IsCollectionNullOrEmpty())
            {
                return;
            }

            foreach (var item in attendanceHookSystems)
            {
                var hookSystemCache = hookComponent.SearchHookSystemCache
                    (row => row.Id == item.HookSystemId).FirstOrDefault();
                var detail = hookSystemCache?.Details.FirstOrDefault
                    (row => row.DetailType == HookDetailTypeEnumeration.Attendance);
                if (detail != null)
                {

                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
                    {
                        LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)} and attendance hook systems is {ObjectHelper.SerializeAsJson(item)}", "IO send hook to hook system");
                    }
                    try
                    {
                        hookComponent.CallHookApi(hookSystemCache, detail, attendance);
                        attendanceComponent.MarkAttendanceAsSendToHookSystem(attendance.Id, item.HookSystemId);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogHookError(exp, new { HookSystemDetail = detail, Attendance = attendance });
                        LoggingSystem.LogHookError(exp, item);
                        try
                        {
                            attendanceComponent.IncreaseAttendanceHookSystemRetryCount(item.AttendanceId, item.HookSystemId);
                        }
                        catch (Exception expSave)
                        {
                            LoggingSystem.LogError(expSave, "Error on IncreaseAttendanceHookSystemRetryCount", $"Data for hook is {ObjectHelper.SerializeAsJson(attendance)} and attendance hook systems is {ObjectHelper.SerializeAsJson(item)}");
                        }
                    }
                }
            }

        }

        #endregion

    }
}
