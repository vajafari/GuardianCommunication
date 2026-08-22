using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Virdi.VirdiConcepts;
using GuardianCommunication.Hardware.Zk;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Component
{
    public class CommunicationComponent : BaseComponent
    {
        private const int DeadlineScan = 10;
        public CommunicationComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        #region Bulk Operations

        public List<DtoUserAndDeviceResult> CommunicationBulkEnrollUser(List<DtoUserAndDeviceParam> allUserAndDeviceInfos)
        {
            var commandConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var result = new List<DtoUserAndDeviceResult>();
            if (allUserAndDeviceInfos.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var userAndDeviceInfo in allUserAndDeviceInfos)
            {
                var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId, false);
                try
                {
                    if (deviceInfo == null)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = userAndDeviceInfo.DeviceId,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.DeviceNotFoundInCache
                        });
                        continue;
                    }
                    if (deviceInfo.ConnectionMode != DeviceConnectionModeEnumeration.Push)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = deviceInfo.Id,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.CommunicationStatusNotSupport
                        });
                        continue;
                    }

                    if (userAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !userAndDeviceInfo.UserInfo.EndDateTime.HasValue)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = deviceInfo.Id,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.CommunicationStatusTempUserMustHaveEndDate
                        });
                        continue;
                    }

                    ProcessStartAndEndTimeOfUser(userAndDeviceInfo.UserInfo);
                    var commandConfig = commandConfigComponent.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);


                    // نکات مهم در ارسال کاربران
                    // 1- در ارسال کاربران موقت به دلیل اینکه ممکن است ورزن های مختلفی از دستورات در زمان های مختلف نیاز باشد به سیستم ارسال شود
                    //    می بایست کاربر دقیقا سر تاریخ و ساعت ذکر شده به دستگاه ارسال شود و سر تاریخ و ساعت ذکر شده از روی دستگاه حذف شود
                    //    همچنین در مورد حذف دستورات پیشین از دیتابیس، نمی توان تمامی دستورات را از دیتابیس حذف کرد و می بایست قبل از ارسال دستوراتی را که
                    //    مرتبط با ارسال جاری هستند حذف نمود
                    // 2- در کاربران دائم، تنها کاربر یکبار به دستگاه ارسال می گردد و در صورت ارسال مجدد می بایست تمامی دستورات موجود قبلی 
                    //    بازنویسی شوند، همچنین در کاربران دائم دیگر ساعت شروع و پایان از اهمیت برخوردار نیست و تنها تاریخ شروع و پایان مهم است
                    //    بنابراین در هنگام ارسال در صورت پشتیبانی از دستگاه از تاریخ شروع و پایان، می توان در همان لحظه کاربر را به دستگاه ارسال نمود. 
                    switch (deviceInfo.ProducerNumber)
                    {
                        case ProducerEnumeration.Virdi:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(VirdiCommands.GetEnrollUserCommand(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        // دستگاه های ویردی ساعت شروع و پایان را پشتیبانی نمی کنند
                                        // بنابراین می بایست در همان لحظه به دستگاه ها ارسال شوند
                                        var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                        commands.Add(VirdiCommands.GetEnrollUserCommand(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        commands.Add(
                                            VirdiCommands.GetDeleteUserCommand(
                                                deviceInfo,
                                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                                userAndDeviceInfo.CommandPriority,
                                                userAndDeviceInfo.CommandIdentifier)
                                        );
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInfo.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Zk:
                            {
                                var commands = GetZkEnrollUserCommands(userAndDeviceInfo, commandConfig);
                                RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInfo.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Timy:
                            {
                                var commands = new List<DtoDeviceCommand>();

                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands
                                        (deviceInfo,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInfo.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Suprema:
                            {
                                // به دلیل اینکه دستگاه های ساپریما به صورت کلی
                                // از تاریخ و ساعت شروع و پایان پشتیبانی می کنند، 
                                var commands = new List<DtoDeviceCommand>();
                                switch (deviceInfo.SdkVersion)
                                {
                                    case SdkVersionEnumeration.SdkVersion1:
                                        {
                                            switch (userAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                        deviceInfo,
                                                        userAndDeviceInfo.UserInfo,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        userAndDeviceInfo.CommandPriority,
                                                        userAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                                    commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                        deviceInfo,
                                                        userAndDeviceInfo.UserInfo,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        visibilityTime,
                                                        userAndDeviceInfo.CommandPriority,
                                                        userAndDeviceInfo.CommandIdentifier));
                                                    commands.Add(SupremaSdk1Commands.GetDeleteUserCommand
                                                    (deviceInfo,
                                                        userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                                        userAndDeviceInfo.CommandPriority,
                                                        userAndDeviceInfo.CommandIdentifier));
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoUserAndDeviceResult
                                            {
                                                DeviceId = deviceInfo.Id,
                                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                    case SdkVersionEnumeration.SdkVersion2:
                                        {
                                            switch (userAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetEnrollUserCommand(
                                                            deviceInfo,
                                                            userAndDeviceInfo.UserInfo,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            userAndDeviceInfo.CommandPriority,
                                                            userAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetEnrollUserCommand(
                                                            deviceInfo,
                                                            userAndDeviceInfo.UserInfo,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            visibilityTime,
                                                            userAndDeviceInfo.CommandPriority,
                                                            userAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetDeleteUserCommand(
                                                            deviceInfo,
                                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                                            userAndDeviceInfo.CommandPriority,
                                                            userAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    break;
                                            }

                                            RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoUserAndDeviceResult
                                            {
                                                DeviceId = deviceInfo.Id,
                                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            result.Add(new DtoUserAndDeviceResult
                            {
                                DeviceId = deviceInfo.Id,
                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                Result = OperationResultEnumeration.CommunicationStatusNotSupport
                            });
                            break;
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on insert command");
                    result.Add(new DtoUserAndDeviceResult
                    {
                        DeviceId = userAndDeviceInfo.DeviceId,
                        UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                        Result = OperationResultEnumeration.CommunicationStatusUnknownError
                    });
                }

            }
            return result;

        }

        public List<DtoUserAndDeviceResult> CommunicationBulkDeleteUser(List<DtoUserAndDeviceParam> allUserAndDeviceInfos)
        {
            var commandConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var result = new List<DtoUserAndDeviceResult>();
            if (allUserAndDeviceInfos.IsCollectionNullOrEmpty())
            {
                return result;
            }

            // نکات مهم در حذف کاربران
            //  ارسال کاربران به صورت موقت می تواند بر روی یک دستگاه مربوط به روزهای متوالی و متفاوتی بر روی دیتابیس وجود داشته باشد
            //  در صورتی که دستورات مربوط به اینده است و هنوز به دستگاه ارسال نشده است و نیاز است حذف شود
            //  ولی اگر دستور مربوط به همین الان بود، می بایست بلافاصله سایر درستورات مرنبط زا پاک کنیم و دستور حذفی ثبت نماییم

            foreach (var userAndDeviceInfo in allUserAndDeviceInfos)
            {
                var deviceInCache = GetDeviceFromCache(userAndDeviceInfo.DeviceId, false);

                try
                {
                    if (deviceInCache == null)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = userAndDeviceInfo.DeviceId,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.DeviceNotFoundInCache
                        });
                        continue;
                    }
                    if (deviceInCache.ConnectionMode != DeviceConnectionModeEnumeration.Push)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = deviceInCache.Id,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.CommunicationStatusNotSupport
                        });
                        continue;
                    }
                    if (userAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !userAndDeviceInfo.UserInfo.EndDateTime.HasValue)
                    {
                        result.Add(new DtoUserAndDeviceResult
                        {
                            DeviceId = deviceInCache.Id,
                            UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                            Result = OperationResultEnumeration.CommunicationStatusTempUserMustHaveEndDate
                        });
                        continue;
                    }
                    var commandConfig = commandConfigComponent.GetCommandSettingFromCache(deviceInCache.ProducerNumber, deviceInCache.SdkVersion);
                    switch (deviceInCache.ProducerNumber)
                    {
                        case ProducerEnumeration.Virdi:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(VirdiCommands.GetDeleteUserCommand(
                                            deviceInCache,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (userAndDeviceInfo.UserInfo.StartDateTime < DateTime.Now)
                                        {
                                            commands.Add(
                                                VirdiCommands.GetDeleteUserCommand(
                                                    deviceInCache,
                                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    userAndDeviceInfo.CommandPriority,
                                                    userAndDeviceInfo.CommandIdentifier)
                                            );
                                        }
                                        break;
                                }
                                RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInCache.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });

                            }
                            break;
                        case ProducerEnumeration.Zk:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(ZkPushCommands.GetDeleteUserCommands
                                        (deviceInCache,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (userAndDeviceInfo.UserInfo.StartDateTime < DateTime.Now)
                                        {
                                            commands.Add(
                                                ZkPushCommands.GetDeleteUserCommands(
                                                    deviceInCache,
                                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    userAndDeviceInfo.CommandPriority,
                                                    userAndDeviceInfo.CommandIdentifier)
                                            );
                                        }
                                        break;
                                }

                                RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInCache.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Timy:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                            deviceInCache,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (userAndDeviceInfo.UserInfo.StartDateTime < DateTime.Now)
                                        {
                                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                                deviceInCache,
                                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                userAndDeviceInfo.CommandPriority,
                                                userAndDeviceInfo.CommandIdentifier));
                                        }
                                        break;
                                }
                                RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);

                                commandComponent.Insert(commands);
                                result.Add(new DtoUserAndDeviceResult
                                {
                                    DeviceId = deviceInCache.Id,
                                    UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;

                        case ProducerEnumeration.Suprema:
                            {
                                switch (deviceInCache.SdkVersion)
                                {
                                    case SdkVersionEnumeration.SdkVersion1:
                                        {
                                            var commands = new List<DtoDeviceCommand>();
                                            switch (userAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk1Commands.GetDeleteUserCommand(
                                                        deviceInCache,
                                                        userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        userAndDeviceInfo.CommandPriority,
                                                        userAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    if (userAndDeviceInfo.UserInfo.StartDateTime < DateTime.Now)
                                                    {
                                                        commands.Add(SupremaSdk1Commands.GetDeleteUserCommand(
                                                            deviceInCache,
                                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            userAndDeviceInfo.CommandPriority,
                                                            userAndDeviceInfo.CommandIdentifier));
                                                    }
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);

                                            commandComponent.Insert(commands);
                                            result.Add(new DtoUserAndDeviceResult
                                            {
                                                DeviceId = deviceInCache.Id,
                                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                    case SdkVersionEnumeration.SdkVersion2:
                                        {
                                            var commands = new List<DtoDeviceCommand>();
                                            switch (userAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk2Commands.GetDeleteUserCommand(
                                                        deviceInCache,
                                                        userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        userAndDeviceInfo.CommandPriority,
                                                        userAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    if (userAndDeviceInfo.UserInfo.StartDateTime < DateTime.Now)
                                                    {
                                                        commands.Add(SupremaSdk2Commands.GetDeleteUserCommand(
                                                            deviceInCache,
                                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            userAndDeviceInfo.CommandPriority,
                                                            userAndDeviceInfo.CommandIdentifier));
                                                    }
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoUserAndDeviceResult
                                            {
                                                DeviceId = deviceInCache.Id,
                                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            result.Add(new DtoUserAndDeviceResult
                            {
                                DeviceId = deviceInCache.Id,
                                UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                Result = OperationResultEnumeration.CommunicationStatusNotSupport
                            });
                            break;
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on insert command");
                    result.Add(new DtoUserAndDeviceResult
                    {
                        DeviceId = userAndDeviceInfo.DeviceId,
                        UserIdOnDevice = userAndDeviceInfo.UserInfo.UserIdOnDevice,
                        Result = OperationResultEnumeration.CommunicationStatusUnknownError
                    });
                }


            }
            return result;

        }

        #endregion


        #region Normal Communication

        public void RebootDevice(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetRebootCommand(deviceInfo, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.RebootDevice();
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetRebootCommand(deviceInfo, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.RebootDevice();
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.RebootDevice();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetRebootCommand(deviceInfo, 1, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.RebootDevice();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetRebootCommand(deviceInfo, 1, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

        }

        public List<DtoAttendance> CommunicationGetUnreadAttendanceForClientFromSdk(Guid deviceId, bool deleteAttendance)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var allAttendances = CommunicationGetUnreadAttendancesFromSdk(deviceInfo);
            if (allAttendances.IsCollectionNullOrEmpty())
            {
                return new List<DtoAttendance>();
            }
            foreach (var item in allAttendances)
            {
                item.IsSentToGuardian = true;
            }

            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var saveResult = attendanceComponent.SaveAttendance(allAttendances, true, true);
            var result = new List<DtoAttendance>();
            if (saveResult.ExistingRecords.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.ExistingRecords);
            }
            if (saveResult.Successful.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.Successful);
            }
            if (saveResult.UnknownErrorSave.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.UnknownErrorSave);
            }
            if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone
                && deleteAttendance
                && saveResult.UnknownErrorSave.IsCollectionNullOrEmpty()
                && saveResult.InvalidDeviceSerialNumberRecords.IsCollectionNullOrEmpty())
            {
                CommunicationClearData(deviceInfo.Id);
            }

            return result;
        }

        public void CommunicationEnrollUserWithTemplate(DtoUserAndDeviceParam userAndDeviceInfo)
        {
            var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId);
            CheckActiveProducer(deviceInfo);
            ProcessStartAndEndTimeOfUser(userAndDeviceInfo.UserInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            if (userAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !userAndDeviceInfo.UserInfo.EndDateTime.HasValue)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusTempUserMustHaveEndDate);
            }

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>();
                        switch (userAndDeviceInfo.UserInfo.UserType)
                        {
                            case DeviceUserTypeEnumeration.PermanentUser:
                                commands.Add(VirdiCommands.GetEnrollUserCommand(
                                    deviceInfo,
                                    userAndDeviceInfo.UserInfo,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    userAndDeviceInfo.CommandPriority,
                                    userAndDeviceInfo.CommandIdentifier));
                                break;
                            case DeviceUserTypeEnumeration.TempUser:
                                var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                commands.Add(VirdiCommands.GetEnrollUserCommand(
                                    deviceInfo,
                                    userAndDeviceInfo.UserInfo,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    visibilityTime,
                                    userAndDeviceInfo.CommandPriority,
                                    userAndDeviceInfo.CommandIdentifier));
                                commands.Add(
                                    VirdiCommands.GetDeleteUserCommand(
                                        deviceInfo,
                                        userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        // ReSharper disable PossibleInvalidOperationException
                                        userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                        // ReSharper restore PossibleInvalidOperationException
                                        userAndDeviceInfo.CommandPriority,
                                        userAndDeviceInfo.CommandIdentifier)
                                );
                                break;
                        }
                        RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                        commandComponent.Insert(commands);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var commands = GetZkEnrollUserCommands(userAndDeviceInfo
                                , commandConfig);
                            RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                            commandComponent.Insert(commands);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetUserInfoWithTemplate(userAndDeviceInfo.UserInfo);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (userAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands
                                        (deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands
                                        (deviceInfo,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            // ReSharper disable PossibleInvalidOperationException
                                            userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                            // ReSharper restore PossibleInvalidOperationException
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier));
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                            }
                            break;
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetUserInfoWithTemplate(userAndDeviceInfo.UserInfo);
                            }
                            break;

                    }

                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetUserInfoWithTemplate(userAndDeviceInfo.UserInfo);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    switch (userAndDeviceInfo.UserInfo.UserType)
                                    {
                                        case DeviceUserTypeEnumeration.PermanentUser:
                                            commands.Add(SupremaSdk1Commands.GetEnrollUserCommand
                                            (deviceInfo,
                                                userAndDeviceInfo.UserInfo,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                null,
                                                userAndDeviceInfo.CommandIdentifier));
                                            break;
                                        case DeviceUserTypeEnumeration.TempUser:
                                            var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                            commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                deviceInfo,
                                                userAndDeviceInfo.UserInfo,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                visibilityTime,
                                                userAndDeviceInfo.CommandPriority,
                                                userAndDeviceInfo.CommandIdentifier));
                                            commands.Add(SupremaSdk1Commands.GetDeleteUserCommand
                                            (deviceInfo,
                                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                // ReSharper disable PossibleInvalidOperationException
                                                userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                                // ReSharper restore PossibleInvalidOperationException
                                                userAndDeviceInfo.CommandPriority,
                                                userAndDeviceInfo.CommandIdentifier));
                                            break;
                                    }
                                    RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetUserInfoWithTemplate(userAndDeviceInfo.UserInfo);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    switch (userAndDeviceInfo.UserInfo.UserType)
                                    {
                                        case DeviceUserTypeEnumeration.PermanentUser:
                                            commands.Add(
                                                SupremaSdk2Commands.GetEnrollUserCommand(
                                                    deviceInfo,
                                                    userAndDeviceInfo.UserInfo,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    userAndDeviceInfo.CommandPriority,
                                                    userAndDeviceInfo.CommandIdentifier)
                                            );
                                            break;
                                        case DeviceUserTypeEnumeration.TempUser:
                                            var visibilityTime = ProcessVisibilityTime(userAndDeviceInfo);
                                            commands.Add(
                                                SupremaSdk2Commands.GetEnrollUserCommand(
                                                    deviceInfo,
                                                    userAndDeviceInfo.UserInfo,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    visibilityTime,
                                                    userAndDeviceInfo.CommandPriority,
                                                    userAndDeviceInfo.CommandIdentifier)
                                            );
                                            commands.Add(
                                                SupremaSdk2Commands.GetDeleteUserCommand(
                                                    deviceInfo,
                                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    // ReSharper disable PossibleInvalidOperationException
                                                    userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                                    // ReSharper restore PossibleInvalidOperationException
                                                    userAndDeviceInfo.CommandPriority,
                                                    userAndDeviceInfo.CommandIdentifier)
                                            );
                                            break;
                                    }

                                    RemoveNecessaryCommandsOnEnrollUser(userAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSendUser(DtoUserAndDeviceParam userAndDeviceParam)
        {
            userAndDeviceParam.UserInfo.ClearTemplateData();
            CommunicationEnrollUserWithTemplate(userAndDeviceParam);
        }

        public DtoUserDeviceRelatedData CommunicationGetUserById(Guid deviceId, long userIdOnDevice, TemplateTypeEnumeration templateType)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);

            CheckActiveProducer(deviceInfo);
            DtoUserDeviceRelatedData result = null;
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = VirdiCommands.GetUserInfoCommand
                            (deviceInfo, userIdOnDevice, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(commands);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var commands = ZkPushCommands.GetUserInfoCommand
                                    (deviceInfo, userIdOnDevice, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserInfoByUserId(userIdOnDevice, templateType);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var commands = TimyPushCommands.GetUserInfoCommand
                                    (deviceInfo, userIdOnDevice, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                            }
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserInfoByUserId(userIdOnDevice, templateType);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetUserInfoByUserId(userIdOnDevice, templateType);
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk1Commands.GetUserInfoCommand
                                        (deviceInfo, userIdOnDevice, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetUserById(userIdOnDevice, templateType);
                                        if (result == null)
                                        {
                                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk2UserIdIsNotValid);
                                        }
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk2Commands.GetUserInfoCommand
                                        (deviceInfo, userIdOnDevice, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }

                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public List<DtoUserInfoDefinedOnDevice> CommunicationGetUsersInfoDefinedOnDevice(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);

            CheckActiveProducer(deviceInfo);
            var result = new List<DtoUserInfoDefinedOnDevice>();
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        result = deviceDriver.GetAllUsers();
                    }
                    break;
                case ProducerEnumeration.Timy:
                    using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        result = deviceDriver.GetAllUsersInfo();
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetAllUsersInfo();
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.Id).GetAllUsersInfo();
                                }

                                break;
                            case SdkVersionEnumeration.SdkVersion2:

                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetAllUsersInfo();
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.Id).GetAllUsersInfo();
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public void CommunicationCancelOperation(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.CancelOperation();
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSetDateAndTime(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Standalone:
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.SetDateTime(DateTime.Now);
                                }
                                break;
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.SetDateTime();
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetDateTime();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk1Commands.SetDateAndTimeCommand
                                        (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetDateTime();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk2Commands.SetDateAndTimeCommand
                                        (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }

                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public DateTime CommunicationGetDateAndTime(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            return deviceDriver.GetDateTime();
                        }
                    }
                case ProducerEnumeration.Timy:
                    {
                        using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            return deviceDriver.GetDateTime();
                        }
                    }
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            return deviceDriver.GetDateTime();
                                        }
                                    }
                                    else
                                    {
                                        var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.Id);
                                        return deviceDriver.GetDateTime();
                                    }
                                }
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetDateTime();
                                    }
                                }
                                else
                                {
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                    return deviceDriver.GetDateTime();
                                }
                        }
                    }
                    break;
            }
            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
        }

        public string CommunicationGetFirmwareVersion(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        return deviceDriver.GetFirmwareVersion();
                    }
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetFirmwareVersion();
                                    }
                                }
                                else
                                {
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                    return deviceDriver.GetFirmwareVersion();
                                }
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationUpgradeFirmware(Guid deviceId, string fileName, byte[] firmwareFile)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.UpgradeFirmware(fileName, firmwareFile);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.UpgradeFirmware(fileName, firmwareFile);
                                    }
                                }
                                else
                                {
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                    deviceDriver.UpgradeFirmware(fileName, firmwareFile);
                                }
                                break;
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationClearData(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Attendance, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.ClearData();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Attendance, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.ClearData();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.ClearData();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk1Commands.GetClearDataCommand
                                        (deviceInfo,
                                        DeviceLogTypeEnumeration.Attendance,
                                        commandConfig.MaxRetryForOtherCommand,
                                        null,
                                        null,
                                        null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.ClearData();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk2Commands.GetClearDataCommand
                                        (deviceInfo,
                                        DeviceLogTypeEnumeration.Attendance,
                                        commandConfig.MaxRetryForOtherCommand,
                                        null,
                                        null,
                                        null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }

                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public int CommunicationRecordCount(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            var result = 0;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetAttendanceLogCount
                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetRecordCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetRecordCount();
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetRecordCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetAttendanceLogCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public int CommunicationFaceCount(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            var result = 0;
            switch (deviceInfo.ProducerNumber)
            {

                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetFaceCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetFaceCount();
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetFaceCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetFaceCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetFaceCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetFaceCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public int CommunicationFingerCount(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            var result = 0;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetFingerCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetFingerCount();
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetFingerCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetFingerCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetFingerCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetFingerCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public int CommunicationUserCount(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            var result = 0;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetUserCount
                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserCount();
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetUserCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetUserCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetUserCount();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetUserCount
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public void CommunicationDeleteUserByInfo(DtoUserAndDeviceParam userAndDeviceInfo)
        {
            // در اینجا نیاز است که کاربر در همین لحطه و همین الان از روی دیتابیس حذف شود
            // بنابراین دستورات برای همین الان بر روی دیتابیس ثبت می شود. همچنین می بایست
            // در صورتی که دستورات مرتبطی با ارسال و یا حذف وجود دارد، از دیتابیس حذف شوند. 

            var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId);
            CheckActiveProducer(deviceInfo);
            var configCache = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = configCache.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>();
                        switch (userAndDeviceInfo.UserInfo.UserType)
                        {
                            case DeviceUserTypeEnumeration.PermanentUser:
                                commands.Add(VirdiCommands.GetDeleteUserCommand(
                                    deviceInfo,
                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    userAndDeviceInfo.CommandPriority,
                                    userAndDeviceInfo.CommandIdentifier));
                                break;
                            case DeviceUserTypeEnumeration.TempUser:
                                commands.Add(
                                    VirdiCommands.GetDeleteUserCommand(
                                        deviceInfo,
                                        userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        userAndDeviceInfo.CommandPriority,
                                        userAndDeviceInfo.CommandIdentifier)
                                );
                                break;
                        }
                        RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                        commandComponent.Insert(commands);
                    }

                    break;
                case ProducerEnumeration.Zk:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        ZkPushCommands.GetDeleteUserCommands
                                        (deviceInfo,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier)
                                    };
                                    RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            default:
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.DeleteUserById(userAndDeviceInfo.UserInfo.UserIdOnDevice);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteUserById(userAndDeviceInfo.UserInfo.UserIdOnDevice);
                            }
                        }
                        else
                        {
                            var commands = new List<DtoDeviceCommand>();
                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                deviceInfo,
                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                userAndDeviceInfo.CommandPriority,
                                userAndDeviceInfo.CommandIdentifier));
                            RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                            commandComponent.Insert(commands);
                        }

                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(userAndDeviceInfo.UserInfo.UserIdOnDevice);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk1Commands.GetDeleteUserCommand(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier)
                                    };
                                    RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(userAndDeviceInfo.UserInfo.UserIdOnDevice);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk2Commands.GetDeleteUserCommand(
                                            deviceInfo,
                                            userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            userAndDeviceInfo.CommandPriority,
                                            userAndDeviceInfo.CommandIdentifier)
                                    };
                                    RemoveNecessaryCommandsOnRemoveUser(userAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);

                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationDeleteUserByUserId(Guid deviceId, long userIdOnDevice)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>
                        {
                            VirdiCommands.GetDeleteUserCommand(
                                deviceInfo,
                                userIdOnDevice,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                null)
                        };
                        commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                            (userIdOnDevice, deviceInfo.Id, VirdiCommands.GetDefineAndDeleteUserCommandTypes());
                        commandComponent.Insert(commands);
                    }

                    break;
                case ProducerEnumeration.Zk:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                var commands = new List<DtoDeviceCommand>
                                {
                                    ZkPushCommands.GetDeleteUserCommands(
                                        deviceInfo,
                                        userIdOnDevice,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        null)
                                };
                                commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                                    (userIdOnDevice, deviceInfo.Id, ZkPushCommands.GetDefineAndDeleteUserCommandTypes());
                                commandComponent.Insert(commands);
                                break;
                            default:
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.DeleteUserById(userIdOnDevice);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteUserById(userIdOnDevice);
                            }
                        }
                        else
                        {
                            var commands = new List<DtoDeviceCommand>();
                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                deviceInfo
                                , userIdOnDevice
                                , commandConfig.MaxRetryForUserCommand
                                , null
                                , null
                                , null));
                            commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                                (userIdOnDevice, deviceInfo.Id, TimyPushCommands.GetDefineAndDeleteUserCommandTypes());
                            commandComponent.Insert(commands);
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(userIdOnDevice);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk1Commands.GetDeleteUserCommand(
                                            deviceInfo
                                            , userIdOnDevice
                                            , commandConfig.MaxRetryForUserCommand
                                            , null
                                            , null
                                            , null)
                                    };
                                    commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                                        (userIdOnDevice, deviceInfo.Id, SupremaSdk1Commands.GetDefineAndDeleteUserCommandTypes());
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(userIdOnDevice);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk2Commands.GetDeleteUserCommand(
                                            deviceInfo
                                            , userIdOnDevice
                                            , commandConfig.MaxRetryForUserCommand
                                            , null
                                            , null
                                            , null)
                                    };
                                    commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                                        (userIdOnDevice, deviceInfo.Id, SupremaSdk2Commands.GetDefineAndDeleteUserCommandTypes());
                                    commandComponent.Insert(commands);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationDeleteAllUsers(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetClearAllDataCommand
                            (deviceInfo, DeviceLogTypeEnumeration.Users, commandConfig.MaxRetryForUserCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;

                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Users, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteAllUsers();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Users, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteAllUsers();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteAllUsers();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk1Commands.GetClearDataCommand
                                        (deviceInfo,
                                        DeviceLogTypeEnumeration.Users,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteAllUsers();
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk2Commands.GetClearDataCommand
                                        (deviceInfo,
                                        DeviceLogTypeEnumeration.Users,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }

                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public string CommunicationGetSerialNumber(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            string result;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result = deviceDriver.GetSerialNumber();
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result = deviceDriver.GetSerialNumber();
                        }
                    }
                    break;

                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetSerialNumber();
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.Id).DeviceId.ToString();
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:

                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetSerialNumber();
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.Id).DeviceId.ToString();
                                }
                                break;
                            default:
                                result = string.Empty;
                                return result;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public OperationResultEnumeration CommunicationTestConnection(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var result = OperationResultEnumeration.CommunicationStatusCannotConnect;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    bool isConnected = VirdiServer.Instance.GetConnectedDeviceIds().Contains(deviceInfo.Id);
                    return isConnected
                        ? OperationResultEnumeration.CommunicationStatusSuccessful
                        : OperationResultEnumeration.CommunicationStatusSupremaSdk2SocketIsNotConnected;
                case ProducerEnumeration.Zk:
                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                    {
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        try
                        {
                            result = !deviceDriver.TestConnection()
                                ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                : OperationResultEnumeration.CommunicationStatusSuccessful;
                        }
                        catch (OperationCannotBeDoneException exp)
                        {
                            return exp.OperationResult.Errors.IsCollectionNotNullOrEmpty()
                                ? exp.OperationResult.Errors.First()
                                : OperationResultEnumeration.CommunicationStatusCannotConnect;
                        }
                        catch (Exception)
                        {
                            return OperationResultEnumeration.CommunicationStatusCannotConnect;
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                    {
                        try
                        {
                            result = !deviceDriver.TestConnection()
                                ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                : OperationResultEnumeration.CommunicationStatusSuccessful;
                        }
                        catch (OperationCannotBeDoneException exp)
                        {
                            return exp.OperationResult.Errors.IsCollectionNotNullOrEmpty()
                                ? exp.OperationResult.Errors.First()
                                : OperationResultEnumeration.CommunicationStatusCannotConnect;
                        }
                        catch (Exception)
                        {
                            return OperationResultEnumeration.CommunicationStatusCannotConnect;
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        result = !deviceDriver.TestConnection()
                                            ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                            : OperationResultEnumeration.CommunicationStatusSuccessful;
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.Id).IsDeviceConnected
                                        ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                        : OperationResultEnumeration.CommunicationStatusSuccessful;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        result = !deviceDriver.TestConnection()
                                            ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                            : OperationResultEnumeration.CommunicationStatusSuccessful;
                                    }
                                }
                                else
                                {
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.Id).IsDeviceConnected
                                        ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                        : OperationResultEnumeration.CommunicationStatusSuccessful;
                                }

                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public DtoDeviceStatistics CommunicationGetDeviceStatistics(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerNumber, deviceInfo.SdkVersion);

            var result = new DtoDeviceStatistics
            {
                IsConnected = true,
            };
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetDeviceStatistics
                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                try
                                {
                                    result.CountOfUsers = deviceDriver.GetUserCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfUnreadAttendance = deviceDriver.GetRecordCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfFaces = deviceDriver.GetFaceCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfFingers = deviceDriver.GetFingerCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                try
                                {
                                    result.CountOfUsers = deviceDriver.GetUserCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfUnreadAttendance = deviceDriver.GetRecordCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfFaces = deviceDriver.GetFaceCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfFingers = deviceDriver.GetFingerCount();
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    switch (deviceInfo.SdkVersion)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            {
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        try
                                        {
                                            result.CountOfUsers = deviceDriver.GetUserCount();
                                        }
                                        catch (Exception exp)
                                        {
                                            LoggingSystem.LogError(exp);
                                        }

                                        try
                                        {
                                            result.CountOfFaces = deviceDriver.GetFaceCount();
                                        }
                                        catch (Exception exp)
                                        {
                                            LoggingSystem.LogError(exp);
                                        }

                                        try
                                        {
                                            result.CountOfFingers = deviceDriver.GetFingerCount();
                                        }
                                        catch (Exception exp)
                                        {
                                            LoggingSystem.LogError(exp);
                                        }
                                    }
                                }
                                else
                                {

                                    var command = SupremaSdk1Commands.GetDeviceStatistics
                                        (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                            }
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            {
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        return deviceDriver.GetDeviceStatistics();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetDeviceStatistics
                                        (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                            }
                            break;
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public DtoUserFace ScanFace(Guid deviceId, DtoUserDeviceRelatedData userInfo)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            DtoUserFace result = null;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanFace(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = ZkPushCommands.GetScanFaceCommand(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = TimyPushCommands.GetScanFaceCommand(deviceInfo, userInfo, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.ScanFace(userInfo);
                            }
                            break;
                    }
                    break;

                case ProducerEnumeration.Suprema:
                    switch (deviceInfo.SdkVersion)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                            {
                                using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                                }
                            }
                            else
                            {
                                var command = SupremaSdk1Commands.ScanFace(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                            {
                                using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                                }
                            }
                            else
                            {
                                var command = SupremaSdk2Commands.ScanFace(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;

                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;
        }

        public DtoUserFace ScanFaceStandalone(Guid deviceId, DtoUserDeviceRelatedData userInfo)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            DtoUserFace result = null;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanFace(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                    }
                    break;
                case ProducerEnumeration.Timy:
                    using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.ScanFace(userInfo);
                    }
                    break;

                case ProducerEnumeration.Suprema:
                    switch (deviceInfo.SdkVersion)
                    {

                        case SdkVersionEnumeration.SdkVersion1:
                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                            }
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.UserIdOnDevice);
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;

                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;
        }

        public DtoUserIris ScanIris(Guid deviceId, DtoUserDeviceRelatedData userInfo)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanIris(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return null;
        }

        public string ScanCard(Guid deviceId, DtoUserDeviceRelatedData userInfo)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            string result = null;
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = TimyPushCommands.GetScanCardCommand(deviceInfo, userInfo, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.ScanCard(userInfo);
                            }
                            break;
                    }
                    break;

                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.ScanCard();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.ScanCard(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.ScanCard();
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.ScanCard(deviceInfo, userInfo.UserIdOnDevice, 1, DeadlineScan, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;
        }

        public DtoUserFinger ScanFinger(Guid deviceId, DtoUserDeviceRelatedData userInfo, int fingerIndex)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            DtoUserFinger result = null;
            switch (deviceInfo.ProducerNumber)
            {
                //case ProducerEnumeration.Virdi:
                //    {
                //        var command = VirdiCommands.ScanFinger(deviceInfo, userIdOnDevice, fingerIndex, 1, DeadlineScan, null, null);
                //        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                //        commandComponent.Insert(command);
                //    }
                //    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = ZkPushCommands.GetScanFingerCommand(deviceInfo, userInfo.UserIdOnDevice, fingerIndex, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFinger(userInfo.UserIdOnDevice, fingerIndex);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = TimyPushCommands.GetScanFingerCommand(deviceInfo, userInfo, fingerIndex, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.ScanFinger(userInfo, fingerIndex);
                            }
                            break;
                    }
                    break;

                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.ScanFinger(userInfo.UserIdOnDevice, fingerIndex);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.ScanFinger(deviceInfo, userInfo.UserIdOnDevice, fingerIndex, 1, DeadlineScan, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.ScanFinger(userInfo.UserIdOnDevice, fingerIndex);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.ScanFinger(deviceInfo, userInfo.UserIdOnDevice, fingerIndex, 1, DeadlineScan, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;
        }

        public void CommunicationCheck(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = ZkPushCommands.GetCheckCommand(deviceInfo, 1, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;

                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public List<DtoAttendance> CommunicationReadoutFromDevice(Guid deviceId, DateTime startDate, DateTime endDate)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var resultOfReadout = new List<DtoAttendance>();
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetReadoutFromDeviceCommand
                            (deviceInfo, startDate, endDate, 1, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);

                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = ZkPushCommands.GetReadoutFromDeviceCommand
                                    (deviceInfo, startDate, endDate, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    resultOfReadout = deviceDriver.Readout(startDate, endDate);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetReadoutFromDeviceCommand
                                    (deviceInfo, startDate, endDate, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        resultOfReadout = deviceDriver.Readout(startDate, endDate);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetReadoutFromDeviceCommand
                                        (deviceInfo, startDate, endDate, 1, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        resultOfReadout = deviceDriver.Readout(startDate, endDate);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.GetReadoutFromDeviceCommand
                                        (deviceInfo, startDate, endDate, 1, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(command);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }


            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var saveResult = attendanceComponent.SaveAttendance(resultOfReadout, true, true);
            var result = new List<DtoAttendance>();

            if (saveResult.ExistingRecords.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.ExistingRecords);
            }
            if (saveResult.Successful.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.Successful);
            }
            if (saveResult.UnknownErrorSave.IsCollectionNotNullOrEmpty())
            {
                result.AddRange(saveResult.UnknownErrorSave);
            }

            //if (saveResult.UnknownErrorSave.IsCollectionNotNullOrEmpty())
            //{
            //    result.AddRange(saveResult.UnknownErrorSave);
            //}

            return result;
        }

        public byte[] CommunicationGetAttendanceImage(Guid deviceId, long userIdOnDevice, DateTime attendanceDateTime)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            return AttendanceImageHelpers.GetImageContent(userIdOnDevice, deviceInfo.Id, attendanceDateTime, AttendanceImageHelpers.GetAttendanceImageExtension(deviceInfo.ProducerNumber));
        }

        public List<DtoDeviceEventLog> CommunicationGetUnreadLogs(Guid deviceId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            var result = new List<DtoDeviceEventLog>();
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceIds(new List<Guid> { deviceInfo.Id })
                                        .FirstOrDefault();
                                    DateTime? lastLogDateTime = null;
                                    if (deviceCommunicationData?.DeviceCommunicationData?.Suprema1?.LastAttendanceLogDateTime != null)
                                    {
                                        lastLogDateTime = deviceCommunicationData.DeviceCommunicationData.Suprema1
                                            .LastLogDateTime;
                                    }


                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(lastLogDateTime.HasValue
                                                ? deviceDriver.GetLog(lastLogDateTime.Value, DateTime.Now)
                                                : deviceDriver.GetLogWithDefaultDates());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(lastLogDateTime.HasValue
                                            ? GetSupremaSdk1DeviceAdapter(deviceInfo.Id)
                                                .GetLog(lastLogDateTime.Value, DateTime.Now)
                                            : GetSupremaSdk1DeviceAdapter(deviceInfo.Id)
                                                .GetLogWithDefaultDates());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastReadTime = result.Max(row => row.EventDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            if (deviceCommunicationData.DeviceCommunicationData == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData =
                                                    new DtoDeviceCommunicationDataJsonEntity();
                                            }

                                            if (deviceCommunicationData.DeviceCommunicationData.Suprema1 == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData.Suprema1 =
                                                    new DtoDeviceCommunicationDataSuprema1();
                                            }
                                            deviceCommunicationData.DeviceCommunicationData.Suprema1.LastLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceId = deviceInfo.Id,
                                                DeviceCommunicationData = new DtoDeviceCommunicationDataJsonEntity
                                                {
                                                    Suprema1 = new DtoDeviceCommunicationDataSuprema1
                                                    {
                                                        LastLogDateTime = lastReadTime
                                                    }
                                                }
                                            };
                                        }
                                        deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                                    }
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceIds(new List<Guid> { deviceInfo.Id })
                                        .FirstOrDefault();
                                    long? lastLogId = null;
                                    if (deviceCommunicationData?.DeviceCommunicationData?.Suprema2?.LastAttendanceLogId != null)
                                    {
                                        lastLogId = deviceCommunicationData.DeviceCommunicationData.Suprema2
                                            .LastAttendanceLogId;
                                    }
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(lastLogId.HasValue
                                                ? deviceDriver.GetLog((uint)lastLogId.Value)
                                                : deviceDriver.GetLogWithDefault());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(lastLogId.HasValue
                                            ? GetSupremaSdk2DeviceAdapter(deviceInfo.Id)
                                                .GetLog((uint)lastLogId.Value)
                                            : GetSupremaSdk2DeviceAdapter(deviceInfo.Id).GetLogWithDefault());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastEventId = result.Max(row => row.Id);
                                        var lastReadTime = result.Max(row => row.EventDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            if (deviceCommunicationData.DeviceCommunicationData == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData =
                                                    new DtoDeviceCommunicationDataJsonEntity();
                                            }

                                            if (deviceCommunicationData.DeviceCommunicationData.Suprema2 == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData.Suprema2 =
                                                    new DtoDeviceCommunicationDataSuprema2();
                                            }

                                            deviceCommunicationData.DeviceCommunicationData.Suprema2.LastLogId = lastEventId;
                                            deviceCommunicationData.DeviceCommunicationData.Suprema2.LastLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceId = deviceInfo.Id,
                                                DeviceCommunicationData = new DtoDeviceCommunicationDataJsonEntity
                                                {
                                                    Suprema2 = new DtoDeviceCommunicationDataSuprema2
                                                    {
                                                        LastLogId = lastEventId,
                                                        LastLogDateTime = lastReadTime,
                                                    }
                                                }
                                            };
                                        }

                                        deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            return result;
        }

        #endregion


        #region Access Control

        public void OpenDoor(Guid deviceId, Guid doorId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);

            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:

                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.OpenDoor(doorInfo.OpenDoorDelay);
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetUnlockDoorCommand(deviceInfo, 1, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.OpenDoor(doorInfo.OpenDoorDelay);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Virdi:
                    VirdiServer.Instance.OpenDoor(1, deviceInfo.DeviceNumber);
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoor(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoor(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoor(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoor(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenDoorWithDelay(Guid deviceId, int delayInSecond)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:

                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.OpenDoor(delayInSecond);
                    }

                    break;
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetUnlockDoorCommand(deviceInfo, 1, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.OpenDoor(delayInSecond);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Virdi:
                    VirdiServer.Instance.OpenDoor(1, deviceInfo.DeviceNumber);
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenCabinetDoor(Guid deviceId, int cabinetNumber)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Timy:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = TimyPushCommands.GetUnlockLockerDoorCommand(deviceInfo, cabinetNumber, cabinetNumber, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.OpenDoor(cabinetNumber);
                                }
                            }
                            break;
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }


        #region Suprema 1

        public void OpenSupremaSdk1DoorPermanent(Guid deviceId, Guid doorId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoorPermanent(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoorPermanent(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CloseSupremaSdk1DoorPermanent(Guid deviceId, Guid doorId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.CloseDoorPermanent(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.CloseDoorPermanent(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenSupremaSdk1DoorWithDelay(Guid deviceId, Guid doorId, int delayInSecond)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoorWithDelay(doorInfo, delayInSecond);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoorWithDelay(doorInfo, delayInSecond);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #region Suprema 2

        public void OpenSupremaSdk2DoorPermanent(Guid deviceId, Guid doorId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoorPermanent(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoorPermanent(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion1:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CloseSupremaSdk2DoorPermanent(Guid deviceId, Guid doorId)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.CloseDoorPermanent(doorInfo);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.CloseDoorPermanent(doorInfo);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion1:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenSupremaSdk2DoorWithDelay(Guid deviceId, Guid doorId, int delayInSecond)
        {
            var deviceInfo = GetDeviceFromCache(deviceId);
            var doorInfo = GetDoorFromCache(doorId);
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.Id);
                                            deviceDriver.OpenDoorWithDelay(doorInfo, delayInSecond);
                                        }
                                        break;
                                    default:
                                        {
                                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                            {
                                                deviceDriver.Connect();
                                                deviceDriver.OpenDoorWithDelay(doorInfo, delayInSecond);
                                            }
                                        }
                                        break;
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion1:
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #endregion


        #region Internal Method

        internal DtoAttendanceSaveResult DownloadAndSaveUnreadAttendancesFromSdk(DtoDevice deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var allAttendances = CommunicationGetUnreadAttendancesFromSdk(deviceInfo);
            foreach (var item in allAttendances)
            {
                item.IsSentToGuardian = false;
            }

            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            return attendanceComponent.SaveAttendance(allAttendances, true, true);
        }

        #endregion


        #region Private Method

        private static void CheckActiveProducer(DtoDevice deviceInfo)
        {
            if (!ApplicationEmbeddedInfo.ActiveProducers.HasFlag(deviceInfo.ProducerNumber))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusProducerNotSupport);
            }

            if (deviceInfo.ProducerNumber == ProducerEnumeration.Suprema)
            {
                if (!ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(deviceInfo.SdkVersion))
                {
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusProducerNotSupport);
                }
            }
        }

        private DtoDevice GetDeviceFromCache(Guid deviceId, bool throwError = true)
        {
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            var deviceInfo = deviceComponent.GetDeviceCache(deviceId);
            if (deviceInfo == null && throwError)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.DeviceNotFoundInCache);
            }
            return deviceInfo;
        }

        private DtoDeviceDoorFullInfo GetDoorFromCache(Guid deviceDoorId, bool throwError = true)
        {
            var deviceComponent = new DeviceComponent(RepositoryFactory);
            var deviceDoorInfo = deviceComponent.GetDeviceDoorCache(deviceDoorId);
            if (deviceDoorInfo == null && throwError)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.DeviceDoorNotFoundInCache);
            }
            return deviceDoorInfo;
        }

        private static SupremaSdk1OnDemandAdapter GetSupremaSdk1DeviceAdapter(Guid deviceId)
        {
            var deviceAdapter = SupremaSdk1Server.Instance.GetDeviceAdapter(deviceId);
            if (deviceAdapter == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }

            return deviceAdapter;
        }

        private static SupremaSdk2OnDemandAdapter GetSupremaSdk2DeviceAdapter(Guid deviceId)
        {
            var deviceAdapter = SupremaSdk2Server.Instance.GetDeviceAdapter(deviceId);
            if (deviceAdapter == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }

            return deviceAdapter;
        }

        private List<DtoAttendance> CommunicationGetUnreadAttendancesFromSdk(DtoDevice deviceInfo)
        {
            var result = new List<DtoAttendance>();
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Virdi:
                    {
                        VirdiServer.Instance.GetLogAsync
                            (1, deviceInfo.DeviceNumber, VirdiDeviceLogTypeEnum.New);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration
                                .CommunicationStatusNotSupport);
                        }

                        if (deviceInfo.DeviceSettings?.ZkDeviceSettings != null
                            && deviceInfo.DeviceSettings.ZkDeviceSettings.IsOldVersion)
                        {
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result.AddRange(deviceDriver.GetDataOldVersion());
                            }
                        }
                        else
                        {
                            var deviceComponent = new DeviceComponent(RepositoryFactory);
                            var deviceCommunicationData = deviceComponent
                                .GetDeviceCommunicationDataByDeviceIds(new List<Guid> { deviceInfo.Id })
                                .FirstOrDefault();
                            DateTime? lastLogDateTime = null;
                            if (deviceCommunicationData?.DeviceCommunicationData?.Zk?.LastAttendanceLogDateTime != null)
                            {
                                lastLogDateTime = deviceCommunicationData.DeviceCommunicationData.Zk
                                    .LastAttendanceLogDateTime;
                            }
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result.AddRange(lastLogDateTime.HasValue
                                    ? deviceDriver.GetData(lastLogDateTime.Value, DateTime.Now)
                                    : deviceDriver.GetDataWithDefaultDates());
                            }
                            if (result.IsCollectionNotNullOrEmpty())
                            {
                                var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                if (deviceCommunicationData != null)
                                {
                                    if (deviceCommunicationData.DeviceCommunicationData == null)
                                    {
                                        deviceCommunicationData.DeviceCommunicationData =
                                            new DtoDeviceCommunicationDataJsonEntity();
                                    }

                                    if (deviceCommunicationData.DeviceCommunicationData.Zk == null)
                                    {
                                        deviceCommunicationData.DeviceCommunicationData.Zk =
                                            new DtoDeviceCommunicationDataZk();
                                    }
                                    deviceCommunicationData.DeviceCommunicationData.Zk.LastAttendanceLogDateTime = lastReadTime;
                                }
                                else
                                {
                                    deviceCommunicationData = new DtoDeviceCommunicationData
                                    {
                                        DeviceId = deviceInfo.Id,
                                        DeviceCommunicationData = new DtoDeviceCommunicationDataJsonEntity
                                        {
                                            Zk = new DtoDeviceCommunicationDataZk
                                            {
                                                LastAttendanceLogDateTime = lastReadTime
                                            }
                                        }
                                    };
                                }
                                deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                            }
                        }

                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result.AddRange(deviceDriver.GetData());
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersion)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceIds(new List<Guid> { deviceInfo.Id })
                                        .FirstOrDefault();
                                    DateTime? lastLogDateTime = null;
                                    if (deviceCommunicationData?.DeviceCommunicationData?.Suprema1?.LastAttendanceLogDateTime != null)
                                    {
                                        lastLogDateTime = deviceCommunicationData.DeviceCommunicationData.Suprema1
                                            .LastAttendanceLogDateTime;
                                    }
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(lastLogDateTime.HasValue
                                                ? deviceDriver.GetData(lastLogDateTime.Value, DateTime.Now)
                                                : deviceDriver.GetDataWithDefaultDates());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(lastLogDateTime.HasValue
                                            ? GetSupremaSdk1DeviceAdapter(deviceInfo.Id)
                                                .GetData(lastLogDateTime.Value, DateTime.Now)
                                            : GetSupremaSdk1DeviceAdapter(deviceInfo.Id)
                                                .GetDataWithDefaultDates());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            if (deviceCommunicationData.DeviceCommunicationData == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData =
                                                    new DtoDeviceCommunicationDataJsonEntity();
                                            }

                                            if (deviceCommunicationData.DeviceCommunicationData.Suprema1 == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData.Suprema1 =
                                                    new DtoDeviceCommunicationDataSuprema1();
                                            }
                                            deviceCommunicationData.DeviceCommunicationData.Suprema1.LastAttendanceLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceId = deviceInfo.Id,
                                                DeviceCommunicationData = new DtoDeviceCommunicationDataJsonEntity
                                                {
                                                    Suprema1 = new DtoDeviceCommunicationDataSuprema1
                                                    {
                                                        LastAttendanceLogDateTime = lastReadTime
                                                    }
                                                }
                                            };
                                        }
                                        deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                                    }
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceIds(new List<Guid> { deviceInfo.Id })
                                        .FirstOrDefault();
                                    long? lastLogId = null;
                                    if (deviceCommunicationData?.DeviceCommunicationData?.Suprema2?.LastAttendanceLogId != null)
                                    {
                                        lastLogId = deviceCommunicationData.DeviceCommunicationData.Suprema2
                                            .LastAttendanceLogId;
                                    }
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(lastLogId.HasValue
                                                ? deviceDriver.GetData((uint)lastLogId.Value)
                                                : deviceDriver.GetDataWithDefault());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(lastLogId.HasValue
                                            ? GetSupremaSdk2DeviceAdapter(deviceInfo.Id)
                                                .GetData((uint)lastLogId.Value)
                                            : GetSupremaSdk2DeviceAdapter(deviceInfo.Id).GetDataWithDefault());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastEventId = result.Max(row => row.LogIdOnDevice) ?? 0;
                                        var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            if (deviceCommunicationData.DeviceCommunicationData == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData =
                                                    new DtoDeviceCommunicationDataJsonEntity();
                                            }

                                            if (deviceCommunicationData.DeviceCommunicationData.Suprema2 == null)
                                            {
                                                deviceCommunicationData.DeviceCommunicationData.Suprema2 =
                                                    new DtoDeviceCommunicationDataSuprema2();
                                            }

                                            deviceCommunicationData.DeviceCommunicationData.Suprema2.LastAttendanceLogId = lastEventId;
                                            deviceCommunicationData.DeviceCommunicationData.Suprema2.LastAttendanceLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceId = deviceInfo.Id,
                                                DeviceCommunicationData = new DtoDeviceCommunicationDataJsonEntity
                                                {
                                                    Suprema2 = new DtoDeviceCommunicationDataSuprema2
                                                    {
                                                        LastAttendanceLogId = lastEventId,
                                                        LastAttendanceLogDateTime = lastReadTime,
                                                    }
                                                }
                                            };
                                        }

                                        deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;

        }

        private List<DtoDeviceCommand> GetZkEnrollUserCommands(
            DtoUserAndDeviceParam userAndDeviceInfo
            , DtoSystemConfigDeviceCommandSetting commandConfig)
        {
            // در مورد دستگاه های زد-کا، به دلیل عدم پشتیبانی از تاریخ شروع و پایان، 
            // کاربران عادی و کاربران موقت می بایست در تاریه شروع به دستگاه ارسال شوند
            var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId);
            var commands = new List<DtoDeviceCommand>();
            switch (userAndDeviceInfo.UserInfo.UserType)
            {
                case DeviceUserTypeEnumeration.PermanentUser:
                    {

                        if (userAndDeviceInfo.UserInfo.EndDateTime.HasValue &&
                            userAndDeviceInfo.UserInfo.EndDateTime < DateTime.Now)
                        {
                            // در صورتی که کابر دائم بود و تاریخ پایان مربوط به گذشته بود، می بایست
                            // کاربر بلافاصله از روی دیتگاه حذف شود
                            commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                deviceInfo,
                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                userAndDeviceInfo.CommandPriority,
                                userAndDeviceInfo.CommandIdentifier));
                        }
                        else
                        {
                            // این تاریخ پایان کاربر دائمی در اینده است. حال می بایست تاریخ شروع را بررسی کنیم و بر اساس آن 
                            // تصمیم بگیریم. در صورتی که تاریخ شروع نیز در اینده بود، بلافاصله می بایست کاربر را از روی سخت افزار حذف نماییم
                            // و سپس در زمان اینده مجددا آن را ارسال نماییم. 
                            DateTime? defineUserVisibilityDateTime = userAndDeviceInfo.UserInfo.StartDateTime;
                            if (defineUserVisibilityDateTime < DateTime.Now)
                            {
                                defineUserVisibilityDateTime = null;
                            }
                            else
                            {
                                // یعنی زمان شروع در اینده است و می بایست کاربر الان از روی ساعت حذف شود
                                commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                    deviceInfo,
                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    userAndDeviceInfo.CommandPriority,
                                    userAndDeviceInfo.CommandIdentifier));
                            }

                            commands.AddRange(ZkPushCommands.GetEnrollUserCommands(
                                deviceInfo,
                                userAndDeviceInfo.UserInfo,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                defineUserVisibilityDateTime,
                                userAndDeviceInfo.CommandPriority,
                                userAndDeviceInfo.CommandIdentifier));

                            if (userAndDeviceInfo.UserInfo.EndDateTime.HasValue)
                            {
                                commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                    deviceInfo,
                                    userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                    userAndDeviceInfo.CommandPriority,
                                    userAndDeviceInfo.CommandIdentifier));
                            }
                        }
                    }
                    break;
                case DeviceUserTypeEnumeration.TempUser:
                    {

                        // در مورد کاربران موقت، تنها کاربر را در تاریخ شروع به دستگاه ارسال می کنیم و
                        // در تاریخ پایان از دستگاه حذف می کنیم. به دستورات فعلی کاری نداریم و کاربر را 
                        // نیز از دستگاه فقط در تاریخ پایان دستور فعلی حذف می کنیم 
                        // چون ممکن کاربر با دستورات و مجوز های موقت دیگری اکنون بر روی دستگاه وجود داشته باشد. 

                        DateTime? defineUserVisibilityDateTime = userAndDeviceInfo.UserInfo.StartDateTime;
                        if (defineUserVisibilityDateTime < DateTime.Now)
                        {
                            defineUserVisibilityDateTime = null;
                        }

                        commands.AddRange(ZkPushCommands.GetEnrollUserCommands(
                            deviceInfo,
                            userAndDeviceInfo.UserInfo,
                            commandConfig.MaxRetryForUserCommand,
                            null,
                            defineUserVisibilityDateTime,
                            userAndDeviceInfo.CommandPriority,
                            userAndDeviceInfo.CommandIdentifier));

                        if (userAndDeviceInfo.UserInfo.EndDateTime.HasValue)
                        {
                            commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                deviceInfo,
                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                userAndDeviceInfo.UserInfo.EndDateTime.Value,
                                userAndDeviceInfo.CommandPriority,
                                userAndDeviceInfo.CommandIdentifier));
                        }

                    }
                    break;
            }
            return commands;

        }

        private static void ProcessStartAndEndTimeOfUser(DtoUserDeviceRelatedData userInfo)
        {
            if (userInfo.UserType == DeviceUserTypeEnumeration.PermanentUser)
            {
                userInfo.StartDateTime = userInfo.StartDateTime.Date;
                if (userInfo.EndDateTime.HasValue)
                {
                    userInfo.EndDateTime = DateTimeHelper.GetEndOf(userInfo.EndDateTime.Value, DateTimeHelper.DateInterval.Day);
                }
            }
        }

        private void RemoveNecessaryCommandsOnEnrollUser(DtoUserAndDeviceParam userAndDeviceInfo
                , DeviceCommandComponent commandComponent)
        {
            var commandTypes = new List<DeviceCommandTypeEnumeration>();
            var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    commandTypes = ZkPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Timy:
                    commandTypes = TimyPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Suprema:
                    switch (deviceInfo.SdkVersion)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            commandTypes = SupremaSdk1Commands.GetDefineAndDeleteUserCommandTypes();
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            commandTypes = SupremaSdk2Commands.GetDefineAndDeleteUserCommandTypes();
                            break;
                    }
                    break;
                case ProducerEnumeration.Virdi:
                    commandTypes = VirdiCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
            }

            if (commandTypes.IsCollectionNotNullOrEmpty())
            {
                switch (userAndDeviceInfo.UserInfo.UserType)
                {
                    case DeviceUserTypeEnumeration.PermanentUser:
                        // در صورتی که کاربر دائمی بود، می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                        // روی دستگاه جاری وجود دارد، را حذف نماییم
                        commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                        (userAndDeviceInfo.UserInfo.UserIdOnDevice
                            , deviceInfo.Id
                            , commandTypes);
                        break;
                    case DeviceUserTypeEnumeration.TempUser:
                        if (userAndDeviceInfo.CommandIdentifier.HasValue)
                        {
                            // در صورتی که کاربر موقت است و دستوراتی مرتبط با دستور جاری وجود دارد، فقط همان دستورات را حذف می کنیم
                            // و به سایر دستور مربوط به کاربر جاری بر روی دستگاه جاری کاری نداریم.
                            // برای مثال فرض کنیم که کاربر مراجعه کننده است و در دو تاریخ مجزا قرار است که به سازمان مراجعه نماید
                            // تاریخ یکی از مراجعه ها تغییر کرده است، فقط می بایست دستورات مربوط به آن مراجعه حذف شوند
                            // و با دستورات سایر مراجعه ها کاری نداشته باشیم
                            commandComponent.DeleteNotSentByUserIdOnDeviceDeviceCommandTypesAndCommandIdentifier
                            (userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                deviceInfo.Id
                                , commandTypes
                                , new List<Guid> { userAndDeviceInfo.CommandIdentifier.Value });

                            // حال در صورتی که کاربر دستورت حذفی در بازه کاربر جاری دارد برای جلوگیری از تداخل می بایست 
                            // آن دستور قبل از ثبت دستورات جدید از سیستم حذف شود. 
                            // چون در حالت های دیگر تمامی دستورات کاربر بر روی آن دستگاه حذف می شوند و فقط در این حالت است که
                            // می بایست در صورتی که دستور حذفی در بین بازه فعال بودن کاربر وجود داشت
                            commandComponent.DeleteNotSentByUserIdOnDeviceCommandTypesAndCommandDateInterval(
                                userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                deviceInfo.Id,
                                new List<DeviceCommandTypeEnumeration>
                                {
                                    DeviceCommandTypeEnumeration.DeleteUser
                                }
                                // ReSharper disable PossibleInvalidOperationException
                                , userAndDeviceInfo.UserInfo.StartDateTime, userAndDeviceInfo.UserInfo.EndDateTime.Value
                            // ReSharper restore PossibleInvalidOperationException
                            );
                        }
                        else
                        {
                            // در صورتی که کاربر موقت باشد ولی دستورات مرتبط پیشین نداشته باشد، 
                            // می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                            // روی دستگاه جاری وجود دارد، را حذف نماییم
                            commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                            (userAndDeviceInfo.UserInfo.UserIdOnDevice
                                , deviceInfo.Id
                                , commandTypes);
                        }

                        break;
                }
            }

        }

        private void RemoveNecessaryCommandsOnRemoveUser(DtoUserAndDeviceParam userAndDeviceInfo
                , DeviceCommandComponent commandComponent)
        {
            var commandTypes = new List<DeviceCommandTypeEnumeration>();
            var deviceInfo = GetDeviceFromCache(userAndDeviceInfo.DeviceId);
            switch (deviceInfo.ProducerNumber)
            {
                case ProducerEnumeration.Zk:
                    commandTypes = ZkPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Timy:
                    commandTypes = TimyPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Suprema:
                    switch (deviceInfo.SdkVersion)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            commandTypes = SupremaSdk1Commands.GetDefineAndDeleteUserCommandTypes();
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            commandTypes = SupremaSdk2Commands.GetDefineAndDeleteUserCommandTypes();
                            break;
                    }
                    break;
                case ProducerEnumeration.Virdi:
                    commandTypes = VirdiCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
            }

            if (commandTypes.IsCollectionNotNullOrEmpty())
            {
                switch (userAndDeviceInfo.UserInfo.UserType)
                {
                    case DeviceUserTypeEnumeration.PermanentUser:
                        // در صورتی که کاربر دائمی بود، می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                        // روی دستگاه جاری وجود دارد، را حذف نماییم
                        commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                        (userAndDeviceInfo.UserInfo.UserIdOnDevice
                            , deviceInfo.Id
                            , commandTypes);
                        break;
                    case DeviceUserTypeEnumeration.TempUser:
                        if (userAndDeviceInfo.CommandIdentifier.HasValue)
                        {
                            // در صورتی که کاربر موقت است و دستوراتی مرتبط با دستور جاری وجود دارد، فقط همان دستورات را حذف می کنیم
                            // و به سایر دستور مربوط به کاربر جاری بر روی دستگاه جاری کاری نداریم.
                            // برای مثال فرض کنیم که کاربر مراجعه کننده است و در دو تاریخ مجزا قرار است که به سازمان مراجعه نماید
                            // تاریخ یکی از مراجعه ها تغییر کرده است، فقط می بایست دستورات مربوط به آن مراجعه حذف شوند
                            // و با دستورات سایر مراجعه ها کاری نداشته باشیم
                            commandComponent.DeleteNotSentByUserIdOnDeviceDeviceCommandTypesAndCommandIdentifier
                            (userAndDeviceInfo.UserInfo.UserIdOnDevice,
                                deviceInfo.Id
                                , commandTypes
                                , new List<Guid> { userAndDeviceInfo.CommandIdentifier.Value });
                        }
                        else
                        {
                            // در صورتی که کاربر موقت باشد ولی دستورات مرتبط پیشین نداشته باشد، 
                            // می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                            // روی دستگاه جاری وجود دارد، را حذف نماییم
                            commandComponent.DeleteNotSentByUserIdOnDeviceAndCommandTypes
                            (userAndDeviceInfo.UserInfo.UserIdOnDevice
                                , deviceInfo.Id
                                , commandTypes);
                        }

                        break;
                }
            }

        }

        private static DateTime? ProcessVisibilityTime(DtoUserAndDeviceParam dataToProcess)
        {
            DateTime? visibilityTime = dataToProcess.UserInfo.StartDateTime;
            if (visibilityTime < DateTime.Now)
            {
                visibilityTime = null;
            }
            return visibilityTime;
        }

        #endregion


    }
}
