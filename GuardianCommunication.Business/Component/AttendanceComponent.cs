using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.CommunicationModels;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;

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
            var karnamaComponent = new GuardianComponent(RepositoryFactory);
            try
            {
                var deviceComponent = new DeviceComponent(RepositoryFactory);
                var deviceCache = deviceComponent.GetDeviceCache(serverMatchData.DeviceId);
                if (deviceCache != null)
                {
                    var resultServerMatch = karnamaComponent.SubmitServerMatching(serverMatchData);
                    if (resultServerMatch.IsSuccessfullyProcessed
                        && resultServerMatch.Attendance != null
                        && resultServerMatch.Attendance.UserIdOnDevice > 0)
                    {

                        var entity = new DtoAttendance
                        {
                            DeviceId = resultServerMatch.Attendance.DeviceId,
                            CameraId = null,
                            IoType = resultServerMatch.Attendance.IoType,
                            VerificationStyle = resultServerMatch.Attendance.VerificationStyle,
                            AttendanceDateTime = resultServerMatch.Attendance.AttendanceDateTime.FromNumericDateTime(),
                            ModuleId = resultServerMatch.Attendance.ModuleId,
                            UserIdOnDevice = resultServerMatch.Attendance.UserIdOnDevice,
                            AttendanceSource = resultServerMatch.Attendance.AttendanceSource,
                            DeviceAttendanceIoRetrieveType = null,
                            IsSentToGuardian = true,
                            RfCardNumber = resultServerMatch.Attendance.RfCardNumber,
                            StatusCode = resultServerMatch.Attendance.StatusCode,
                        };
                        var resultOfSave = SaveAttendance
                            (new List<DtoAttendance> { entity }, false, false);
                        if (resultOfSave.Successful.IsCollectionNotNullOrEmpty())
                        {
                            var thread = new Thread(() => HookIoEvent(resultOfSave.Successful.First()));
                            thread.Start();

                            return new DtoServerMatchResult
                            {
                                IsSuccessfullyProcessed = true,
                                UserIdOnDevice = resultServerMatch.Attendance.UserIdOnDevice
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
            attendance.IsSentToGuardian = true;
            var resultOfSave = SaveAttendance(new List<DtoAttendance> { attendance }
                , checkDuplicateInterval, false);
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
            var allHooks = hookComponent.GetHookDefinitionCache();
            var activeAttendanceHooks = allHooks?.Where(
                h => h.IsActive && h.HookType == HookTypeEnumeration.Attendance).ToList();
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
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceSaveProcess))
                    {
                        LoggingSystem.LogInfo("SaveAttendance process attendance is valid", attendances);
                    }

                    DtoDevice deviceCache = null;
                    if (attendance.DeviceId.HasValue)
                    {
                        deviceCache = deviceComponent.GetDeviceCache(attendance.DeviceId.Value);
                        if (deviceCache == null)
                        {
                            LoggingSystem.LogWarning("DEVICE NOT FOUND IN CACHE", attendance.DeviceId);
                        }
                        if (applyDeviceRelatedProperties && deviceCache != null)
                        {
                            if (deviceCache.ProducerNumber == ProducerEnumeration.AsaGuard && attendance.DoorId.HasValue)
                            {
                                var doorInCache = deviceComponent.GetDeviceDoorCache(attendance.DoorId.Value);
                                if (doorInCache?.ReaderDeviceId != null)
                                {
                                    attendance.ModuleId = doorInCache.ReaderModuleId;
                                    attendance.IoType = doorInCache.ReaderIoType;
                                    attendance.ReaderDeviceId = doorInCache.ReaderDeviceId;
                                    attendance.LocationId = doorInCache.ReaderLocationId;
                                    attendance.CameraId = null;
                                }
                                else if (doorInCache?.ReaderCameraId != null)
                                {
                                    attendance.ModuleId = doorInCache.ReaderModuleId;
                                    attendance.IoType = doorInCache.ReaderIoType;
                                    attendance.CameraId = doorInCache.ReaderCameraId;
                                    attendance.CameraId = doorInCache.ReaderLocationId;
                                    attendance.ReaderDeviceId = null;
                                }
                                else
                                {
                                    attendance.ModuleId = deviceCache.ModuleId;
                                    attendance.IoType = deviceCache.IoType;
                                    attendance.LocationId = deviceCache.LocationId;
                                    attendance.ReaderDeviceId = null;
                                    attendance.CameraId = null;
                                }
                            }
                            else
                            {
                                attendance.ModuleId = deviceCache.ModuleId;
                                attendance.IoType = deviceCache.IoType;
                                attendance.LocationId = deviceCache.LocationId;
                                attendance.ReaderDeviceId = null;
                                attendance.CameraId = null;
                            }
                        }
                    }
                    var attendanceRegisterIntervalSettings =
                        systemConfigComponent.GetAttendanceRegisterIntervalSetting(attendance.ModuleId);

                    var hookSystemIds = new List<Guid>();
                    if (deviceCache?.DeviceSettings != null
                        && deviceCache.DeviceSettings.IsHookActive
                        && activeAttendanceHooks.IsCollectionNotNullOrEmpty())
                    {
                        hookSystemIds = activeAttendanceHooks.Select(hs => hs.Id).ToList();
                    }

                    var resultOfInsert = RepositoryFactory.GetAttendanceRepository()
                        .Insert(attendance, attendanceRegisterIntervalSettings, hookSystemIds);
                    if (resultOfInsert != null)
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
            if (attendance.UserIdOnDevice <= 0)
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
                systemConfigComponent.GetAttendanceRegisterIntervalSetting(attendance.ModuleId);
            if (attendanceRegisterIntervalSettings.Count > 0)
            {
                if (CheckExistence(attendance.UserIdOnDevice, attendance.AttendanceDateTime, attendanceRegisterIntervalSettings))
                {
                    return true;
                }
            }

            return false;
        }


        #endregion


        #region Private methods


        private bool CheckExistence(long userIdOnDevice, DateTime attendanceDate, List<DtoAttendanceRegisterIntervalSetting> registerIntervalSettings)
        {
            return RepositoryFactory.GetAttendanceRepository().CheckExistence(userIdOnDevice, attendanceDate, registerIntervalSettings);
        }

        #endregion

        #endregion


        #region Guardian Attendance

        public void ResendUnsentAttendancesToGuardian()
        {
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var configCache = systemConfigComponent.GetSystemConfigCache();
            var unsentAttendances = RepositoryFactory.GetAttendanceRepository()
                .GetUnsentToGuardianAttendances(configCache.AttendanceSendToGuardianTimerRecordCount,
                    configCache.AttendanceSendToGuardianRetryCount);
            if (unsentAttendances.IsCollectionNotNullOrEmpty())
            {
                var markAsSent = new List<DtoAttendance>();
                var increaseRetryCount = new List<DtoAttendance>();
                foreach (var entity in unsentAttendances)
                {
                    try
                    {
                        var resultOfSave = SubmitIoEventToGuardian(entity);
                        if (resultOfSave.IsAttendanceSavedAtGuardian())
                        {
                            markAsSent.Add(entity);
                        }
                        else
                        {
                            increaseRetryCount.Add(entity);
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
                    MarkAttendanceAsSentToGuardian(markAsSent);
                }

                if (increaseRetryCount.IsCollectionNotNullOrEmpty())
                {
                    MarkAttendanceAsSentToGuardian(increaseRetryCount);
                }
            }

        }

        public void MarkAttendanceAsSentToGuardian(List<DtoAttendance> attendances)
        {
            if (!attendances.IsCollectionNotNullOrEmpty()) return;
            try
            {
                RepositoryFactory.GetAttendanceRepository().MarkAsSentToGuardian(attendances);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }

        }

        public void IncreaseSentToGuardianRetryCount(List<DtoAttendance> attendances)
        {
            if (!attendances.IsCollectionNotNullOrEmpty()) return;
            try
            {
                RepositoryFactory.GetAttendanceRepository().IncreaseSentToGuardianRetryCount(attendances);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }

        }

        public AttendanceProcessResultModel SubmitIoEventToGuardian(DtoAttendance entity)
        {
            var karnamaComponent = new GuardianComponent(RepositoryFactory);
            var result = karnamaComponent.SubmitIoEvent(entity);
            return result;
        }

        public void SubmitInvalidIoEventToGuardian(DtoInvalidAttendance entity)
        {
            var karnamaComponent = new GuardianComponent(RepositoryFactory);
            karnamaComponent.SubmitInvalidIoEvent(entity);
        }

        #endregion


        #region Attendance Hook

        public List<DtoAttendanceHookDefinition> GetAttendancesHookDefinition(Guid attendanceId)
        {

            return RepositoryFactory.GetAttendanceHookDefinitionRepository().Search(
                new PagingData<AttendanceHookDefinitionFilter, AttendanceHookDefinitionSortEnumeration>()
                {
                    Filter = new AttendanceHookDefinitionFilter
                    {
                        AttendanceIds = new List<Guid>
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

        public void MarkAttendanceAsSendToHook(Guid attendanceId, Guid hookDefinitionId)
        {
            RepositoryFactory.GetAttendanceHookDefinitionRepository().MarkAsSent(attendanceId, hookDefinitionId);
        }

        public void IncreaseAttendanceHookRetryCount(Guid attendanceId, Guid hookDefinitionId)
        {
            RepositoryFactory.GetAttendanceHookDefinitionRepository()
                .IncreaseRetryCount(attendanceId, hookDefinitionId);
        }

        public void HookUnsentAttendances()
        {
            var hookComponent = new HookComponent(RepositoryFactory);
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            var systemConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var allHookDefinitionsInCache = hookComponent.GetHookDefinitionCache();
            if (allHookDefinitionsInCache.IsCollectionNullOrEmpty())
            {
                return;
            }

            var unsentAttendances = attendanceComponent.GetUnhookedAttendances
                (systemConfigComponent.GetSystemConfigCache().AttendanceHookTimerRecordCount);

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
            {
                LoggingSystem.LogInfo(ObjectHelper.SerializeAsJson(unsentAttendances), "Unsent records list");
            }
            foreach (var item in unsentAttendances)
            {
                if (!item.DeviceId.HasValue)
                {
                    continue;
                }
                var deviceInCache = deviceComponent.GetDeviceCache(item.DeviceId.Value);
                if (deviceInCache?.DeviceSettings != null && deviceInCache.DeviceSettings.IsHookActive)
                {
                    var currentItemHookSystemDefinition = allHookDefinitionsInCache.FirstOrDefault
                    (hsd => hsd.IsActive = hsd.HookType == HookTypeEnumeration.Attendance
                                           && hsd.Id == item.HookDefinitionId);

                    if (currentItemHookSystemDefinition == null)
                    {
                        continue;
                    }

                    try
                    {
                        hookComponent.CallHookApi<DtoAttendance>(currentItemHookSystemDefinition, item);
                        attendanceComponent.MarkAttendanceAsSendToHook(item.Id, item.HookDefinitionId);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogHookError(exp, new
                        {
                            HookDefinition = currentItemHookSystemDefinition, Attendance = item
                        });
                        try
                        {
                            attendanceComponent.IncreaseAttendanceHookRetryCount(item.Id, item.HookDefinitionId);
                        }
                        catch (Exception expSave)
                        {
                            LoggingSystem.LogError(expSave, "Error on IncreaseAttendanceHookSystemRetryCount",
                                $"Hook system detail is {ObjectHelper.SerializeAsJson(currentItemHookSystemDefinition)} data is {ObjectHelper.SerializeAsJson(item)} ");
                        }
                    }

                }

            }
        }

        public void HookIoEvent(DtoAttendance attendance)
        {
            var hookComponent = new HookComponent(RepositoryFactory);
            var deviceComponent = new DeviceComponent(RepositoryFactory);

            var allHookDefinitions = hookComponent.GetHookDefinitionCache();
            if (allHookDefinitions.IsCollectionNullOrEmpty())
            {
                return;
            }

            var allHookDefinitionActiveForAttendance = allHookDefinitions
                .Where(hd => hd.IsActive && hd.HookType == HookTypeEnumeration.Attendance)
                .ToList();
            if (allHookDefinitionActiveForAttendance.IsCollectionNullOrEmpty())
            {
                return;
            }
            if (attendance.DeviceId.HasValue)
            {
                var deviceInCache = deviceComponent.GetDeviceCache(attendance.DeviceId.Value);
                if (deviceInCache?.DeviceSettings != null && !deviceInCache.DeviceSettings.IsHookActive)
                {
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
                    {
                        LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)}",
                            "Hooking IO skipped because of system setting");
                    }
                    return;
                }
            }


            var allAttendanceHookDefinitions = GetAttendancesHookDefinition(attendance.Id);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
            {
                LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)} and attendance hook systems are {ObjectHelper.SerializeAsJson(allAttendanceHookDefinitions)}", "IO hook");
            }
            if (allAttendanceHookDefinitions.IsCollectionNullOrEmpty())
            {
                return;
            }

            foreach (var item in allAttendanceHookDefinitions)
            {
                var hookDefinitionInCache = allHookDefinitionActiveForAttendance
                    .FirstOrDefault(hd => hd.Id == item.HookDefinitionId );
                if (hookDefinitionInCache != null)
                {

                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogHookProcess))
                    {
                        LoggingSystem.LogInfo($"Data for hook is {ObjectHelper.SerializeAsJson(attendance)} and attendance hook systems is {ObjectHelper.SerializeAsJson(item)}", "IO send hook to hook system");
                    }
                    try
                    {
                        hookComponent.CallHookApi(hookDefinitionInCache, attendance);
                        MarkAttendanceAsSendToHook(attendance.Id, item.HookDefinitionId);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogHookError(exp, new
                        {
                            HookDefintion = hookDefinitionInCache, Attendance = attendance
                        });
                        LoggingSystem.LogHookError(exp, item);
                        try
                        {
                            IncreaseAttendanceHookRetryCount(item.AttendanceId, item.HookDefinitionId);
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
