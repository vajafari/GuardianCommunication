using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.ElmOSanat;
using GuardianCommunication.Hardware.PadisController;
using GuardianCommunication.Hardware.Pw;
using GuardianCommunication.Hardware.Shared.Helpers;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Virdi.VirdiConcepts;
using GuardianCommunication.Hardware.Zk;

namespace GuardianCommunication.Business.Component
{
    public class CommunicationComponent : BaseComponent
    {
        private const int DeadlineScan = 10;
        public CommunicationComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }


        #region Bulk Operations

        public List<DtoEmployeeAndDeviceResult> CommunicationBulkEnrollUser(List<DtoEmployeeAndDeviceParam> allEmployeeAndDeviceInfos)
        {
            var commandConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var result = new List<DtoEmployeeAndDeviceResult>();
            if (allEmployeeAndDeviceInfos.IsCollectionNullOrEmpty())
            {
                return result;
            }
            foreach (var employeeAndDeviceInfo in allEmployeeAndDeviceInfos)
            {
                try
                {
                    if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode != DeviceConnectionModeEnumeration.Push)
                    {
                        result.Add(new DtoEmployeeAndDeviceResult
                        {
                            DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                            EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                            Result = OperationResultEnumeration.CommunicationStatusNotSupport
                        });
                        continue;
                    }

                    if (employeeAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !employeeAndDeviceInfo.UserInfo.EndTime.HasValue)
                    {
                        result.Add(new DtoEmployeeAndDeviceResult
                        {
                            DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                            EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                            Result = OperationResultEnumeration.CommunicationStatusTempUserMustHaveEndDate
                        });
                        continue;
                    }

                    ProcessStartAndEndTimeOfUser(employeeAndDeviceInfo.UserInfo);
                    var commandConfig = commandConfigComponent.GetCommandSettingFromCache(employeeAndDeviceInfo.DeviceInfo.ProducerEnum, employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum);


                    // نکات مهم در ارسال کاربران
                    // 1- در ارسال کاربران موقت به دلیل اینکه ممکن است ورزن های مختلفی از دستورات در زمان های مختلف نیاز باشد به سیستم ارسال شود
                    //    می بایست کاربر دقیقا سر تاریخ و ساعت ذکر شده به دستگاه ارسال شود و سر تاریخ و ساعت ذکر شده از روی دستگاه حذف شود
                    //    همچنین در مورد حذف دستورات پیشین از دیتابیس، نمی توان تمامی دستورات را از دیتابیس حذف کرد و می بایست قبل از ارسال دستوراتی را که
                    //    مرتبط با ارسال جاری هستند حذف نمود
                    // 2- در کاربران دائم، تنها کاربر یکبار به دستگاه ارسال می گردد و در صورت ارسال مجدد می بایست تمامی دستورات موجود قبلی 
                    //    بازنویسی شوند، همچنین در کاربران دائم دیگر ساعت شروع و پایان از اهمیت برخوردار نیست و تنها تاریخ شروع و پایان مهم است
                    //    بنابراین در هنگام ارسال در صورت پشتیبانی از دستگاه از تاریخ شروع و پایان، می توان در همان لحظه کاربر را به دستگاه ارسال نمود. 
                    switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
                    {
                        case ProducerEnumeration.Virdi:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(VirdiCommands.GetEnrollUserCommand(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        // دستگاه های ویردی ساعت شروع و پایان را پشتیبانی نمی کنند
                                        // بنابراین می بایست در همان لحظه به دستگاه ها ارسال شوند
                                        var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                        commands.Add(VirdiCommands.GetEnrollUserCommand(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        commands.Add(
                                            VirdiCommands.GetDeleteUserCommand(
                                                employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier)
                                        );
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Zk:
                            {
                                var commands = GetZkEnrollUserCommands(employeeAndDeviceInfo, commandConfig);
                                RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Timy:
                            {
                                var commands = new List<DtoDeviceCommand>();

                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands
                                        (employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Suprema:
                            {
                                // به دلیل اینکه دستگاه های ساپریما به صورت کلی
                                // از تاریخ و ساعت شروع و پایان پشتیبانی می کنند، 
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
                                {
                                    case SdkVersionEnumeration.SdkVersion1:
                                        {
                                            switch (employeeAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                        employeeAndDeviceInfo.DeviceInfo,
                                                        employeeAndDeviceInfo.UserInfo,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        employeeAndDeviceInfo.CommandPriority,
                                                        employeeAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                                    commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                        employeeAndDeviceInfo.DeviceInfo,
                                                        employeeAndDeviceInfo.UserInfo,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        visibilityTime,
                                                        employeeAndDeviceInfo.CommandPriority,
                                                        employeeAndDeviceInfo.CommandIdentifier));
                                                    commands.Add(SupremaSdk1Commands.GetDeleteUserCommand
                                                    (employeeAndDeviceInfo.DeviceInfo,
                                                        employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                                        employeeAndDeviceInfo.CommandPriority,
                                                        employeeAndDeviceInfo.CommandIdentifier));
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoEmployeeAndDeviceResult
                                            {
                                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                    case SdkVersionEnumeration.SdkVersion2:
                                        {
                                            switch (employeeAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetEnrollUserCommand(
                                                            employeeAndDeviceInfo.DeviceInfo,
                                                            employeeAndDeviceInfo.UserInfo,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            employeeAndDeviceInfo.CommandPriority,
                                                            employeeAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetEnrollUserCommand(
                                                            employeeAndDeviceInfo.DeviceInfo,
                                                            employeeAndDeviceInfo.UserInfo,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            visibilityTime,
                                                            employeeAndDeviceInfo.CommandPriority,
                                                            employeeAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    commands.Add(
                                                        SupremaSdk2Commands.GetDeleteUserCommand(
                                                            employeeAndDeviceInfo.DeviceInfo,
                                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                                            employeeAndDeviceInfo.CommandPriority,
                                                            employeeAndDeviceInfo.CommandIdentifier)
                                                    );
                                                    break;
                                            }

                                            RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoEmployeeAndDeviceResult
                                            {
                                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            result.Add(new DtoEmployeeAndDeviceResult
                            {
                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                Result = OperationResultEnumeration.CommunicationStatusNotSupport
                            });
                            break;
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on insert command");
                    result.Add(new DtoEmployeeAndDeviceResult
                    {
                        DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                        EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                        Result = OperationResultEnumeration.CommunicationStatusUnknownError
                    });
                }

            }
            return result;

        }

        public List<DtoEmployeeAndDeviceResult> CommunicationBulkDeleteUser(List<DtoEmployeeAndDeviceParam> allEmployeeAndDeviceInfos)
        {
            var commandConfigComponent = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var result = new List<DtoEmployeeAndDeviceResult>();
            if (allEmployeeAndDeviceInfos.IsCollectionNullOrEmpty())
            {
                return result;
            }

            // نکات مهم در حذف کاربران
            //  ارسال کاربران به صورت موقت می تواند بر روی یک دستگاه مربوط به روزهای متوالی و متفاوتی بر روی دیتابیس وجود داشته باشد
            //  در صورتی که دستورات مربوط به اینده است و هنوز به دستگاه ارسال نشده است و نیاز است حذف شود
            //  ولی اگر دستور مربوط به همین الان بود، می بایست بلافاصله سایر درستورات مرنبط زا پاک کنیم و دستور حذفی ثبت نماییم

            foreach (var employeeAndDeviceInfo in allEmployeeAndDeviceInfos)
            {
                try
                {
                    if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode != DeviceConnectionModeEnumeration.Push)
                    {
                        result.Add(new DtoEmployeeAndDeviceResult
                        {
                            DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                            EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                            Result = OperationResultEnumeration.CommunicationStatusNotSupport
                        });
                        continue;
                    }
                    if (employeeAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !employeeAndDeviceInfo.UserInfo.EndTime.HasValue)
                    {
                        result.Add(new DtoEmployeeAndDeviceResult
                        {
                            DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                            EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                            Result = OperationResultEnumeration.CommunicationStatusTempUserMustHaveEndDate
                        });
                        continue;
                    }
                    var commandConfig = commandConfigComponent.GetCommandSettingFromCache(employeeAndDeviceInfo.DeviceInfo.ProducerEnum, employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum);
                    switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
                    {
                        case ProducerEnumeration.Virdi:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(VirdiCommands.GetDeleteUserCommand(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (employeeAndDeviceInfo.UserInfo.StartTime < DateTime.Now)
                                        {
                                            commands.Add(
                                                VirdiCommands.GetDeleteUserCommand(
                                                    employeeAndDeviceInfo.DeviceInfo,
                                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    employeeAndDeviceInfo.CommandPriority,
                                                    employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                        }
                                        break;
                                }
                                RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });

                            }
                            break;
                        case ProducerEnumeration.Zk:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.Add(ZkPushCommands.GetDeleteUserCommands
                                        (employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (employeeAndDeviceInfo.UserInfo.StartTime < DateTime.Now)
                                        {
                                            commands.Add(
                                                ZkPushCommands.GetDeleteUserCommands(
                                                    employeeAndDeviceInfo.DeviceInfo,
                                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    employeeAndDeviceInfo.CommandPriority,
                                                    employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                        }
                                        break;
                                }

                                RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;
                        case ProducerEnumeration.Timy:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        if (employeeAndDeviceInfo.UserInfo.StartTime < DateTime.Now)
                                        {
                                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                                employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                        }
                                        break;
                                }
                                RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);

                                commandComponent.Insert(commands);
                                result.Add(new DtoEmployeeAndDeviceResult
                                {
                                    DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                    EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                });
                            }
                            break;

                        case ProducerEnumeration.Suprema:
                            {
                                switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
                                {
                                    case SdkVersionEnumeration.SdkVersion1:
                                        {
                                            var commands = new List<DtoDeviceCommand>();
                                            switch (employeeAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk1Commands.GetDeleteUserCommand(
                                                        employeeAndDeviceInfo.DeviceInfo,
                                                        employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        employeeAndDeviceInfo.CommandPriority,
                                                        employeeAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    if (employeeAndDeviceInfo.UserInfo.StartTime < DateTime.Now)
                                                    {
                                                        commands.Add(SupremaSdk1Commands.GetDeleteUserCommand(
                                                            employeeAndDeviceInfo.DeviceInfo,
                                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            employeeAndDeviceInfo.CommandPriority,
                                                            employeeAndDeviceInfo.CommandIdentifier));
                                                    }
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);

                                            commandComponent.Insert(commands);
                                            result.Add(new DtoEmployeeAndDeviceResult
                                            {
                                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                    case SdkVersionEnumeration.SdkVersion2:
                                        {
                                            var commands = new List<DtoDeviceCommand>();
                                            switch (employeeAndDeviceInfo.UserInfo.UserType)
                                            {
                                                case DeviceUserTypeEnumeration.PermanentUser:
                                                    commands.Add(SupremaSdk2Commands.GetDeleteUserCommand(
                                                        employeeAndDeviceInfo.DeviceInfo,
                                                        employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                        commandConfig.MaxRetryForUserCommand,
                                                        null,
                                                        null,
                                                        employeeAndDeviceInfo.CommandPriority,
                                                        employeeAndDeviceInfo.CommandIdentifier));
                                                    break;
                                                case DeviceUserTypeEnumeration.TempUser:
                                                    if (employeeAndDeviceInfo.UserInfo.StartTime < DateTime.Now)
                                                    {
                                                        commands.Add(SupremaSdk2Commands.GetDeleteUserCommand(
                                                            employeeAndDeviceInfo.DeviceInfo,
                                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                            commandConfig.MaxRetryForUserCommand,
                                                            null,
                                                            null,
                                                            employeeAndDeviceInfo.CommandPriority,
                                                            employeeAndDeviceInfo.CommandIdentifier));
                                                    }
                                                    break;
                                            }
                                            RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                            commandComponent.Insert(commands);
                                            result.Add(new DtoEmployeeAndDeviceResult
                                            {
                                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                Result = OperationResultEnumeration.CommunicationStatusSuccessful
                                            });
                                        }
                                        break;
                                }
                            }
                            break;
                        default:
                            result.Add(new DtoEmployeeAndDeviceResult
                            {
                                DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                Result = OperationResultEnumeration.CommunicationStatusNotSupport
                            });
                            break;
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on insert command");
                    result.Add(new DtoEmployeeAndDeviceResult
                    {
                        DeviceNumber = employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                        EmployeeNumber = employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                        Result = OperationResultEnumeration.CommunicationStatusUnknownError
                    });
                }


            }
            return result;

        }

        #endregion


        #region Normal Communication

        public void RebootDevice(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.RebootDevice();
                            }
                        }
                        else
                        {
                            var command = PadisControllerPushCommands.GetRebootCommand(deviceInfo, 1, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

        }

        public List<DtoAttendance> CommunicationGetUnreadAttendanceForClientFromSdk(DtoCommunicationDeviceData deviceInfo, bool deleteAttendance)
        {
            CheckActiveProducer(deviceInfo);
            var allAttendances = CommunicationGetUnreadAttendancesFromSdk(deviceInfo);
            if (allAttendances.IsCollectionNullOrEmpty())
            {
                return new List<DtoAttendance>();
            }
            foreach (var item in allAttendances)
            {
                item.IsSent = true;
            }

            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var saveResult = attendanceComponent.SaveAttendance(allAttendances, true, true, true);
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
                CommunicationClearData(deviceInfo);
            }

            return result;
        }

        public void CommunicationEnrollUserWithTemplate(DtoEmployeeAndDeviceParam employeeAndDeviceInfo)
        {
            CheckActiveProducer(employeeAndDeviceInfo.DeviceInfo);
            ProcessStartAndEndTimeOfUser(employeeAndDeviceInfo.UserInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(employeeAndDeviceInfo.DeviceInfo.ProducerEnum, employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum);

            if (employeeAndDeviceInfo.UserInfo.UserType == DeviceUserTypeEnumeration.TempUser && !employeeAndDeviceInfo.UserInfo.EndTime.HasValue)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusTempUserMustHaveEndDate);
            }

            switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>();
                        switch (employeeAndDeviceInfo.UserInfo.UserType)
                        {
                            case DeviceUserTypeEnumeration.PermanentUser:
                                commands.Add(VirdiCommands.GetEnrollUserCommand(
                                    employeeAndDeviceInfo.DeviceInfo,
                                    employeeAndDeviceInfo.UserInfo,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    employeeAndDeviceInfo.CommandPriority,
                                    employeeAndDeviceInfo.CommandIdentifier));
                                break;
                            case DeviceUserTypeEnumeration.TempUser:
                                var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                commands.Add(VirdiCommands.GetEnrollUserCommand(
                                    employeeAndDeviceInfo.DeviceInfo,
                                    employeeAndDeviceInfo.UserInfo,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    visibilityTime,
                                    employeeAndDeviceInfo.CommandPriority,
                                    employeeAndDeviceInfo.CommandIdentifier));
                                commands.Add(
                                    VirdiCommands.GetDeleteUserCommand(
                                        employeeAndDeviceInfo.DeviceInfo,
                                        employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        // ReSharper disable PossibleInvalidOperationException
                                        employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                        // ReSharper restore PossibleInvalidOperationException
                                        employeeAndDeviceInfo.CommandPriority,
                                        employeeAndDeviceInfo.CommandIdentifier)
                                );
                                break;
                        }
                        RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                        commandComponent.Insert(commands);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (employeeAndDeviceInfo.DeviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var commands = GetZkEnrollUserCommands(
                                new DtoEmployeeAndDeviceParam { UserInfo = employeeAndDeviceInfo.UserInfo, DeviceInfo = employeeAndDeviceInfo.DeviceInfo }
                                , commandConfig);
                            RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                            commandComponent.Insert(commands);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetUserInfoWithTemplate(employeeAndDeviceInfo.UserInfo);
                                if (employeeAndDeviceInfo.UserInfo.TimeZones.IsCollectionNotNullOrEmpty())
                                {
                                    deviceDriver.SendUserTimeZone(employeeAndDeviceInfo.UserInfo.EmployeeNumber, employeeAndDeviceInfo.UserInfo.TimeZones);
                                }
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Timy:
                    switch (employeeAndDeviceInfo.DeviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var commands = new List<DtoDeviceCommand>();
                                switch (employeeAndDeviceInfo.UserInfo.UserType)
                                {
                                    case DeviceUserTypeEnumeration.PermanentUser:
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands
                                        (employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            null,
                                            null,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                    case DeviceUserTypeEnumeration.TempUser:
                                        var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                        commands.AddRange(TimyPushCommands.GetEnrollUserCommands(
                                            employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            visibilityTime,
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        commands.AddRange(TimyPushCommands.GetDeleteUserCommands
                                        (employeeAndDeviceInfo.DeviceInfo,
                                            employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                            commandConfig.MaxRetryForUserCommand,
                                            null,
                                            // ReSharper disable PossibleInvalidOperationException
                                            employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                            // ReSharper restore PossibleInvalidOperationException
                                            employeeAndDeviceInfo.CommandPriority,
                                            employeeAndDeviceInfo.CommandIdentifier));
                                        break;
                                }
                                RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                commandComponent.Insert(commands);
                            }
                            break;
                        case DeviceConnectionModeEnumeration.Standalone:
                            using (var deviceDriver = new TimyOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetUserInfoWithTemplate(employeeAndDeviceInfo.UserInfo);
                            }
                            break;

                    }

                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetUserInfoWithTemplate(employeeAndDeviceInfo.UserInfo);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    switch (employeeAndDeviceInfo.UserInfo.UserType)
                                    {
                                        case DeviceUserTypeEnumeration.PermanentUser:
                                            commands.Add(SupremaSdk1Commands.GetEnrollUserCommand
                                            (employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                            break;
                                        case DeviceUserTypeEnumeration.TempUser:
                                            var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                            commands.Add(SupremaSdk1Commands.GetEnrollUserCommand(
                                                employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                visibilityTime,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                            commands.Add(SupremaSdk1Commands.GetDeleteUserCommand
                                            (employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                // ReSharper disable PossibleInvalidOperationException
                                                employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                                // ReSharper restore PossibleInvalidOperationException
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                            break;
                                    }
                                    RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetUserInfoWithTemplate(employeeAndDeviceInfo.UserInfo);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    switch (employeeAndDeviceInfo.UserInfo.UserType)
                                    {
                                        case DeviceUserTypeEnumeration.PermanentUser:
                                            commands.Add(
                                                SupremaSdk2Commands.GetEnrollUserCommand(
                                                    employeeAndDeviceInfo.DeviceInfo,
                                                    employeeAndDeviceInfo.UserInfo,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    null,
                                                    employeeAndDeviceInfo.CommandPriority,
                                                    employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                            break;
                                        case DeviceUserTypeEnumeration.TempUser:
                                            var visibilityTime = ProcessVisibilityTime(employeeAndDeviceInfo);
                                            commands.Add(
                                                SupremaSdk2Commands.GetEnrollUserCommand(
                                                    employeeAndDeviceInfo.DeviceInfo,
                                                    employeeAndDeviceInfo.UserInfo,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    visibilityTime,
                                                    employeeAndDeviceInfo.CommandPriority,
                                                    employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                            commands.Add(
                                                SupremaSdk2Commands.GetDeleteUserCommand(
                                                    employeeAndDeviceInfo.DeviceInfo,
                                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                    commandConfig.MaxRetryForUserCommand,
                                                    null,
                                                    // ReSharper disable PossibleInvalidOperationException
                                                    employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                                    // ReSharper restore PossibleInvalidOperationException
                                                    employeeAndDeviceInfo.CommandPriority,
                                                    employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                            break;
                                    }

                                    RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                        }
                    }
                    break;

                case ProducerEnumeration.Padis:
                    {
                        if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                            {
                                deviceDriver.SetUserInfo(employeeAndDeviceInfo.UserInfo);
                            }
                        }
                        else
                        {
                            var commands = PadisControllerPushCommands.GetUserInfoCommand
                                  (employeeAndDeviceInfo.DeviceInfo,
                                      employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                      commandConfig.MaxRetryForUserCommand,
                                      null,
                                      null,
                                      null,
                                      employeeAndDeviceInfo.CommandIdentifier);
                            RemoveNecessaryCommandsOnEnrollUser(employeeAndDeviceInfo, commandComponent);
                            commandComponent.Insert(commands);
                        }
                    }
                    break;

                case ProducerEnumeration.ProcessingWorld:
                    {
                        switch (employeeAndDeviceInfo.DeviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                            default:
                                {
                                    using (var deviceDriver = new PwOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SetUserInfoWithTemplate(new List<DtoEmployeeDeviceRelatedData> { employeeAndDeviceInfo.UserInfo });
                                    }
                                }
                                break;
                        }

                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSendUser(DtoEmployeeAndDeviceParam employeeAndDeviceParam)
        {
            employeeAndDeviceParam.UserInfo.ClearTemplateData();
            CommunicationEnrollUserWithTemplate(employeeAndDeviceParam);
        }

        public DtoEmployeeDeviceRelatedData CommunicationGetUserById(DtoCommunicationDeviceData deviceInfo, long employeeNumber, TemplateTypeEnumeration templateType)
        {
            CheckActiveProducer(deviceInfo);
            DtoEmployeeDeviceRelatedData result = null;
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = VirdiCommands.GetUserInfoCommand
                            (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
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
                                    (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                            }
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserInfoByUserId(employeeNumber, templateType);
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
                                    (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                            }
                            break;
                        default:
                            using (var deviceDriver = new TimyOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserInfoByUserId(employeeNumber, templateType);
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetUserInfoByUserId(employeeNumber, templateType);
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk1Commands.GetUserInfoCommand
                                        (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
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
                                        result = deviceDriver.GetUserById(employeeNumber, templateType);
                                        if (result == null)
                                        {
                                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusSupremaSdk2UserIdIsNotValid);
                                        }
                                    }
                                }
                                else
                                {
                                    var commands = SupremaSdk2Commands.GetUserInfoCommand
                                        (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }

                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                result = deviceDriver.GetUserInfoByUserId(employeeNumber);
                            }
                        }
                        else
                        {
                            var commands = PadisControllerPushCommands.GetUserInfoCommand
                                (deviceInfo, employeeNumber, commandConfig.MaxRetryForUserCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(commands);
                        }
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                    {
                        using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result = deviceDriver.GetUserInfoByUserId(employeeNumber, templateType);
                        }
                    }
                    else
                    {
                        var commands = PwCommands.GetUserInfoCommand
                            (deviceInfo, employeeNumber, templateType, commandConfig.MaxRetryForUserCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(commands);
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public List<DtoUserInfoDefinedOnDevice> CommunicationGetUsersInfoDefinedOnDevice(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var result = new List<DtoUserInfoDefinedOnDevice>();
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber).GetAllUsersInfo();
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
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).GetAllUsersInfo();
                                }
                                break;
                        }

                    }
                    break;
                case ProducerEnumeration.Padis:
                    using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                    {
                        result = deviceDriver.GetAllUsers();
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result = deviceDriver.GetAllUsersInfo();
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public void CommunicationCancelOperation(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);

            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.CancelOperation();
                        }

                        //switch (deviceInfo.ConnectionMode)
                        //{
                        //    case DeviceConnectionModeEnumeration.Push:
                        //    {
                        //        var command = ZkPushCommands.GetClearDataCommand
                        //            (deviceInfo, DeviceLogTypeEnumeration.Attendance, commandConfig.MaxRetryForOtherCommand, null, null, null);
                        //        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        //        commandComponent.Insert(command);
                        //    }
                        //        break;
                        //    default:
                        //        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        //        {
                        //            deviceDriver.Connect();
                        //            deviceDriver.CancelOperation();
                        //        }
                        //        break;
                        //}
                        break;
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSetDateAndTime(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Standalone:
                                using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.SetDateTime(DateTime.Now);
                                }
                                break;
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetDateTime(DateTime.Now);
                            }
                        }
                        else
                        {
                            var commands = PwCommands.SetDateAndTimeCommand
                                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(commands);
                        }
                    }
                    break;
                case ProducerEnumeration.ElmOSanat:
                    {
                        using (var deviceDriver = new ElmoSanatOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.SetDateTime();
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public DateTime CommunicationGetDateAndTime(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                        var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber);
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
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
                                    return deviceDriver.GetDateTime();
                                }
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            return deviceDriver.GetDateTime();
                        }
                    }
                case ProducerEnumeration.ProcessingWorld:
                    {
                        using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                        {
                            var result = deviceDriver.Connect(true);
                            return result.DeviceDateTime;
                        }
                    }
                case ProducerEnumeration.ElmOSanat:
                    {
                        using (var deviceDriver = new ElmoSanatOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            return deviceDriver.GetDateTime();
                        }
                    }
            }
            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
        }

        public string CommunicationGetFirmwareVersion(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        return deviceDriver.GetFirmwareVersion();
                    }
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
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
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
                                    return deviceDriver.GetFirmwareVersion();
                                }
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                case ProducerEnumeration.Padis:
                    using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                    {
                        return deviceDriver.GetFirmwareVersion();
                    }
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationUpgradeFirmware(DtoCommunicationDeviceData deviceInfo, string fileName, byte[] firmwareFile)
        {
            CheckActiveProducer(deviceInfo);

            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void CommunicationClearData(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Attendance, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.ClearData();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                var commands = PwCommands.GetClearDataCommand
                                         (deviceInfo,
                                         DeviceLogTypeEnumeration.Attendance,
                                         commandConfig.MaxRetryForOtherCommand,
                                         null,
                                         null,
                                         null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                                break;
                            default:
                                using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.ClearData();
                                }
                                break;
                        }

                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public int CommunicationRecordCount(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            var result = 0;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                var deviceComponent = new DeviceComponent(RepositoryFactory);
                                var deviceCommunicationData = deviceComponent
                                    .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                    .FirstOrDefault();
                                var startDate = DateTime.Now.AddDays(-3);
                                if (deviceCommunicationData?.LastAttendanceLogDateTime != null)
                                {
                                    startDate = deviceCommunicationData.LastAttendanceLogDateTime.Value;
                                }
                                var endDate = DateTime.Now;

                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.GetRecordCount(startDate, endDate);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.GetAttendanceLogCount
                                    (deviceInfo, startDate, endDate, commandConfig.MaxRetryForOtherCommand, null, null, null);
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                result = deviceDriver.GetRecordCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                var resultOfPwConnect = deviceDriver.Connect();
                                return resultOfPwConnect.RecordCount;
                            }
                        }
                        else
                        {
                            var command = PwCommands.GetAttendanceLogCount
                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                case ProducerEnumeration.ElmOSanat:
                    {
                        using (var deviceDriver = new ElmoSanatOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result = deviceDriver.GetRecordCount();
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public int CommunicationFaceCount(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            var result = 0;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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

        public int CommunicationFingerCount(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            var result = 0;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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

        public int CommunicationUserCount(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            var result = 0;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                result = deviceDriver.GetUserCount();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.GetUserCount();
                            }
                        }
                        else
                        {
                            var command = PwCommands.GetUserCount
                            (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public void CommunicationDeleteUserByInfo(DtoEmployeeAndDeviceParam employeeAndDeviceInfo)
        {
            // در اینجا نیاز است که کاربر در همین لحطه و همین الان از روی دیتابیس حذف شود
            // بنابراین دستورات برای همین الان بر روی دیتابیس ثبت می شود. همچنین می بایست
            // در صورتی که دستورات مرتبطی با ارسال و یا حذف وجود دارد، از دیتابیس حذف شوند. 

            CheckActiveProducer(employeeAndDeviceInfo.DeviceInfo);
            var configCache = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = configCache.GetCommandSettingFromCache(employeeAndDeviceInfo.DeviceInfo.ProducerEnum, employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum);

            switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>();
                        switch (employeeAndDeviceInfo.UserInfo.UserType)
                        {
                            case DeviceUserTypeEnumeration.PermanentUser:
                                commands.Add(VirdiCommands.GetDeleteUserCommand(
                                    employeeAndDeviceInfo.DeviceInfo,
                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    employeeAndDeviceInfo.CommandPriority,
                                    employeeAndDeviceInfo.CommandIdentifier));
                                break;
                            case DeviceUserTypeEnumeration.TempUser:
                                commands.Add(
                                    VirdiCommands.GetDeleteUserCommand(
                                        employeeAndDeviceInfo.DeviceInfo,
                                        employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        employeeAndDeviceInfo.CommandPriority,
                                        employeeAndDeviceInfo.CommandIdentifier)
                                );
                                break;
                        }
                        RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                        commandComponent.Insert(commands);
                    }

                    break;
                case ProducerEnumeration.Zk:
                    {
                        switch (employeeAndDeviceInfo.DeviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    commands.Add(ZkPushCommands.GetDeleteUserCommands
                                            (employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                    RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            default:
                                using (var deviceDriver = new ZkOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Timy:
                    {
                        if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new TimyOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                            }
                        }
                        else
                        {
                            var commands = new List<DtoDeviceCommand>();
                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                employeeAndDeviceInfo.DeviceInfo,
                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                employeeAndDeviceInfo.CommandPriority,
                                employeeAndDeviceInfo.CommandIdentifier));
                            RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                            commandComponent.Insert(commands);
                        }

                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    commands.Add(SupremaSdk1Commands.GetDeleteUserCommand(
                                                employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                    RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    commands.Add(SupremaSdk2Commands.GetDeleteUserCommand(
                                                employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier));
                                    RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);

                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        switch (employeeAndDeviceInfo.DeviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                {
                                    var commands = new List<DtoDeviceCommand>();
                                    commands.Add(PadisControllerPushCommands.GetDeleteUserCommands
                                            (employeeAndDeviceInfo.DeviceInfo,
                                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                                commandConfig.MaxRetryForUserCommand,
                                                null,
                                                null,
                                                employeeAndDeviceInfo.CommandPriority,
                                                employeeAndDeviceInfo.CommandIdentifier)
                                            );
                                    RemoveNecessaryCommandsOnRemoveUser(employeeAndDeviceInfo, commandComponent);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            default:
                                using (var deviceDriver = new PadisControllerOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                                {
                                    deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (employeeAndDeviceInfo.DeviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(employeeAndDeviceInfo.DeviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteUserById(employeeAndDeviceInfo.UserInfo.EmployeeNumber);
                            }
                        }
                        else
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationDeleteUserByUserId(DtoCommunicationDeviceData deviceInfo, long employeeNumber)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var commands = new List<DtoDeviceCommand>
                        {
                            VirdiCommands.GetDeleteUserCommand(
                                deviceInfo,
                                employeeNumber,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                null)
                        };
                        commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                            (employeeNumber, deviceInfo.DeviceNumber, VirdiCommands.GetDefineAndDeleteUserCommandTypes());
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
                                        employeeNumber,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        null)
                                };
                                commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                                    (employeeNumber, deviceInfo.DeviceNumber, ZkPushCommands.GetDefineAndDeleteUserCommandTypes());
                                commandComponent.Insert(commands);
                                break;
                            default:
                                using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.DeleteUserById(employeeNumber);
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
                                deviceDriver.DeleteUserById(employeeNumber);
                            }
                        }
                        else
                        {
                            var commands = new List<DtoDeviceCommand>();
                            commands.AddRange(TimyPushCommands.GetDeleteUserCommands(
                                deviceInfo
                                , employeeNumber
                                , commandConfig.MaxRetryForUserCommand
                                , null
                                , null
                                , null));
                            commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                                (employeeNumber, deviceInfo.DeviceNumber, TimyPushCommands.GetDefineAndDeleteUserCommandTypes());
                            commandComponent.Insert(commands);
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(employeeNumber);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk1Commands.GetDeleteUserCommand(
                                            deviceInfo
                                            , employeeNumber
                                            , commandConfig.MaxRetryForUserCommand
                                            , null
                                            , null
                                            , null)
                                    };
                                    commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                                        (employeeNumber, deviceInfo.DeviceNumber, SupremaSdk1Commands.GetDefineAndDeleteUserCommandTypes());
                                    commandComponent.Insert(commands);
                                }
                                break;
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DeleteUserById(employeeNumber);
                                    }
                                }
                                else
                                {
                                    var commands = new List<DtoDeviceCommand>
                                    {
                                        SupremaSdk2Commands.GetDeleteUserCommand(
                                            deviceInfo
                                            , employeeNumber
                                            , commandConfig.MaxRetryForUserCommand
                                            , null
                                            , null
                                            , null)
                                    };
                                    commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                                        (employeeNumber, deviceInfo.DeviceNumber, SupremaSdk2Commands.GetDefineAndDeleteUserCommandTypes());
                                    commandComponent.Insert(commands);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                var commands = new List<DtoDeviceCommand>
                                {
                                    PadisControllerPushCommands.GetDeleteUserCommands(
                                        deviceInfo,
                                        employeeNumber,
                                        commandConfig.MaxRetryForUserCommand,
                                        null,
                                        null,
                                        null)
                                };
                                commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                                    (employeeNumber, deviceInfo.DeviceNumber, ZkPushCommands.GetDefineAndDeleteUserCommandTypes());
                                commandComponent.Insert(commands);
                                break;
                            default:
                                using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.DeleteUserById(employeeNumber);
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.DeleteUserById(employeeNumber);
                            }
                        }
                        else
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSendWithoutFinger(DtoCommunicationDeviceData deviceInfo, List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SendWithoutFinger(userInfos);
                            }
                        }
                        else
                        {
                            var command = PwCommands.GetSendWithoutFingerCommand
                           (deviceInfo, userInfos, commandConfig.MaxRetryForUserCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationSendValidInvalid(DtoCommunicationDeviceData deviceInfo, List<DtoEmployeeDeviceRelatedData> userInfos)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SetValidInvalidList(userInfos);
                            }
                        }
                        else
                        {
                            var command = PwCommands.GetSendValidInvalidCommand
                           (deviceInfo, userInfos, commandConfig.MaxRetryForUserCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void CommunicationDeleteAllUsers(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetClearDataCommand
                                    (deviceInfo, DeviceLogTypeEnumeration.Users, commandConfig.MaxRetryForUserCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.DeleteAllUsers();
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                var commands = PwCommands.GetClearDataCommand
                                         (deviceInfo,
                                         DeviceLogTypeEnumeration.Users,
                                         commandConfig.MaxRetryForUserCommand,
                                         null,
                                         null,
                                         null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(commands);
                                break;
                            default:
                                using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    deviceDriver.DeleteAllUsers();
                                }
                                break;
                        }

                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public string CommunicationGetSerialNumber(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            string result;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber).DeviceId.ToString();
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
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).DeviceId.ToString();
                                }
                                break;
                            default:
                                result = string.Empty;
                                return result;
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            result = deviceDriver.GetSerialNumber();
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public OperationResultEnumeration CommunicationTestConnection(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var result = OperationResultEnumeration.CommunicationStatusCannotConnect;
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    bool isConnected = VirdiServer.Instance.GetConnectedDeviceNumbers().Contains(deviceInfo.DeviceNumber);
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                    result = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber).IsDeviceConnected
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
                                    result = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).IsDeviceConnected
                                        ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                        : OperationResultEnumeration.CommunicationStatusSuccessful;
                                }

                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                    {
                        throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                    }
                    using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
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
                case ProducerEnumeration.ProcessingWorld:
                    {
                        using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                        {
                            var resultOfPwConnect = deviceDriver.Connect();
                            result = !resultOfPwConnect.IsConnected
                                ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                : OperationResultEnumeration.CommunicationStatusSuccessful;
                        }
                    }
                    break;
                case ProducerEnumeration.ElmOSanat:
                    {
                        using (var deviceDriver = new ElmoSanatOnDemandAdapter(deviceInfo))
                        {
                            result = !deviceDriver.TestConnection()
                                ? OperationResultEnumeration.CommunicationStatusCannotConnect
                                : OperationResultEnumeration.CommunicationStatusSuccessful;
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public DtoDeviceStatistics CommunicationGetDeviceStatistics(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var config = new SystemConfigComponent(RepositoryFactory);
            var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

            var result = new DtoDeviceStatistics
            {
                IsConnected = true,
            };
            switch (deviceInfo.ProducerEnum)
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
                    switch (deviceInfo.SdkVersionEnum)
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
                                            var deviceComponent = new DeviceComponent(RepositoryFactory);
                                            var deviceCommunicationData = deviceComponent
                                                .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                                .FirstOrDefault();
                                            result.CountOfUnreadAttendance = deviceCommunicationData?.LastAttendanceLogDateTime != null
                                                ? deviceDriver.GetRecordCount(deviceCommunicationData.LastAttendanceLogDateTime.Value, DateTime.Now)
                                                : deviceDriver.GetRecordCountWithDefaultDates();
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetDeviceStatisticsCommand
                                    (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                            {
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
                            }
                            break;
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                        {
                            using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                            {
                                var resultOfPwConnect = deviceDriver.Connect();
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
                                    result.CountOfUnreadAttendance = resultOfPwConnect.RecordCount;
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogError(exp);
                                }

                                try
                                {
                                    result.CountOfFaces = 0;
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

                            var command = SupremaSdk1Commands.GetFaceCount
                                (deviceInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
            return result;
        }

        public DtoEmployeeFace ScanFace(DtoCommunicationDeviceData deviceInfo, DtoEmployeeDeviceRelatedData userInfo)
        {
            CheckActiveProducer(deviceInfo);
            DtoEmployeeFace result = null;
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanFace(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = ZkPushCommands.GetScanFaceCommand(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
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
                    switch (deviceInfo.SdkVersionEnum)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                            {
                                using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.Connect();
                                    result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
                                }
                            }
                            else
                            {
                                var command = SupremaSdk1Commands.ScanFace(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
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
                                    result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
                                }
                            }
                            else
                            {
                                var command = SupremaSdk2Commands.ScanFace(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
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

        public DtoEmployeeFace ScanFaceStandalone(DtoCommunicationDeviceData deviceInfo, DtoEmployeeDeviceRelatedData userInfo)
        {
            CheckActiveProducer(deviceInfo);
            DtoEmployeeFace result = null;
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanFace(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
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
                    switch (deviceInfo.SdkVersionEnum)
                    {

                        case SdkVersionEnumeration.SdkVersion1:
                            using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
                            }
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFace(userInfo.EmployeeNumber);
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

        public DtoEmployeeIris ScanIris(DtoCommunicationDeviceData deviceInfo, DtoEmployeeDeviceRelatedData userInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.ScanIris(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return null;
        }

        public string ScanCard(DtoCommunicationDeviceData deviceInfo, DtoEmployeeDeviceRelatedData userInfo)
        {
            CheckActiveProducer(deviceInfo);
            string result = null;
            switch (deviceInfo.ProducerEnum)
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
                        switch (deviceInfo.SdkVersionEnum)
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
                                    var command = SupremaSdk1Commands.ScanCard(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
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
                                    var command = SupremaSdk2Commands.ScanCard(deviceInfo, userInfo.EmployeeNumber, 1, DeadlineScan, null, null);
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

        public DtoEmployeeFinger ScanFinger(DtoCommunicationDeviceData deviceInfo, DtoEmployeeDeviceRelatedData userInfo, int fingerIndex)
        {
            CheckActiveProducer(deviceInfo);
            DtoEmployeeFinger result = null;
            switch (deviceInfo.ProducerEnum)
            {
                //case ProducerEnumeration.Virdi:
                //    {
                //        var command = VirdiCommands.ScanFinger(deviceInfo, employeeNumber, fingerIndex, 1, DeadlineScan, null, null);
                //        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                //        commandComponent.Insert(command);
                //    }
                //    break;
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var command = ZkPushCommands.GetScanFingerCommand(deviceInfo, userInfo.EmployeeNumber, fingerIndex, 1, DeadlineScan, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result = deviceDriver.ScanFinger(userInfo.EmployeeNumber, fingerIndex);
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
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                {
                                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        result = deviceDriver.ScanFinger(userInfo.EmployeeNumber, fingerIndex);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk1Commands.ScanFinger(deviceInfo, userInfo.EmployeeNumber, fingerIndex, 1, DeadlineScan, null, null);
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
                                        result = deviceDriver.ScanFinger(userInfo.EmployeeNumber, fingerIndex);
                                    }
                                }
                                else
                                {
                                    var command = SupremaSdk2Commands.ScanFinger(deviceInfo, userInfo.EmployeeNumber, fingerIndex, 1, DeadlineScan, null, null);
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

        public void CommunicationCheck(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
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

        public List<DtoAttendance> CommunicationReadoutFromDevice(DtoCommunicationDeviceData deviceInfo, DateTime startDate, DateTime endDate)
        {
            CheckActiveProducer(deviceInfo);
            var resultOfReadout = new List<DtoAttendance>();
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        var command = VirdiCommands.GetReadoutFromDeviceCommand
                            (deviceInfo, startDate, endDate, 1, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(command);

                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Push:
                                {
                                    var commands = PwCommands.GetReadoutFromDeviceCommand
                                             (deviceInfo,
                                             startDate,
                                             endDate,
                                             1,
                                             null,
                                             null,
                                             null);
                                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                    commandComponent.Insert(commands);
                                }
                                break;
                            default:
                                {
                                    using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        resultOfReadout = deviceDriver.Readout(startDate, endDate);
                                    }
                                }
                                break;
                        }
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
                        switch (deviceInfo.SdkVersionEnum)
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
                case ProducerEnumeration.Padis:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            {
                                var command = PadisControllerPushCommands.GetReadoutFromDeviceCommand
                                    (deviceInfo, startDate, endDate, 1, null, null, null);
                                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                                commandComponent.Insert(command);
                            }
                            break;
                        default:
                            {
                                using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                                {
                                    resultOfReadout = deviceDriver.GetAttendances(startDate, endDate);
                                }
                            }
                            break;
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }


            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            var saveResult = attendanceComponent.SaveAttendance(resultOfReadout, true, true, true);
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

        public byte[] CommunicationGetAttendanceImage(DtoCommunicationDeviceData deviceInfo, long employeeNumber, DateTime attendanceDateTime)
        {
            CheckActiveProducer(deviceInfo);
            return AttendanceImageHelpers.GetImageContent(employeeNumber, deviceInfo.DeviceNumber, attendanceDateTime, AttendanceImageHelpers.GetAttendanceImageExtention(deviceInfo.ProducerEnum));
        }

        public List<DtoDeviceEventLog> CommunicationGetUnreadLogs(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var result = new List<DtoDeviceEventLog>();
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                        .FirstOrDefault();
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(deviceCommunicationData?.LastLogDateTime != null
                                                ? deviceDriver.GetLog(deviceCommunicationData.LastLogDateTime, DateTime.Now)
                                                : deviceDriver.GetLogWithDefaultDates());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(deviceCommunicationData?.LastLogDateTime != null
                                            ? GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetLog(deviceCommunicationData.LastLogDateTime, DateTime.Now)
                                            : GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetLogWithDefaultDates());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastReadTime = result.Max(row => row.EventDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            deviceCommunicationData.LastLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceNumber = deviceInfo.DeviceNumber,
                                                LastLogDateTime = lastReadTime,
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
                                        .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                        .FirstOrDefault();
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(deviceCommunicationData?.LastLogId != null
                                                ? deviceDriver.GetLog((uint)deviceCommunicationData.LastLogId.Value)
                                                : deviceDriver.GetLogWithDefault());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(deviceCommunicationData?.LastLogId != null
                                            ? GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetLog((uint)deviceCommunicationData.LastLogId.Value)
                                            : GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).GetLogWithDefault());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastEventId = result.Max(row => row.Id);
                                        var lastReadTime = result.Max(row => row.EventDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            deviceCommunicationData.LastLogId = lastEventId;
                                            deviceCommunicationData.LastLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceNumber = deviceInfo.DeviceNumber,
                                                LastLogId = lastEventId,
                                                LastLogDateTime = lastReadTime,
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

        public void CommunicationSendFunctionTitles(DtoCommunicationDeviceData deviceInfo, List<string> titles)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.SendDeviceFunctionTitles(titles);
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                                {
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
                                    if (deviceDriver != null)
                                    {
                                        deviceDriver.SendDeviceFunctionTitles(titles);
                                    }
                                }
                                else
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.SendDeviceFunctionTitles(titles);
                                    }
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

        public void CommunicationDisableFunctionTitles(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.DisableDeviceFunctionTitles();
                        }
                    }
                    break;
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                                {
                                    var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
                                    if (deviceDriver != null)
                                    {
                                        deviceDriver.DisableDeviceFunctionTitles();
                                    }
                                }
                                else
                                {
                                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                    {
                                        deviceDriver.Connect();
                                        deviceDriver.DisableDeviceFunctionTitles();
                                    }
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

        public void CommunicationReconnectOnlineMonitoringDevice(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Zk && deviceInfo.OnlineMonitoringMode)
            {
                ZkServer.Instance.ReconnectOnlineMonitoringDevice(deviceInfo.DeviceNumber);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }


        #endregion


        #region Access Control


        public void OpenDoor(DtoCommunicationDeviceData deviceInfo, DtoDeviceDoorBase doorBaseInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:

                    if (deviceInfo.OnlineMonitoringMode)
                    {
                        var agent = ZkServer.Instance.GetOnlineMonitoringAgent(deviceInfo.DeviceNumber);
                        if (agent != null && agent.IsDeviceConnected)
                        {
                            agent.OpenDoor(doorBaseInfo.OpenDoorDelay);
                        }
                        else
                        {
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.OpenDoor(doorBaseInfo.OpenDoorDelay);
                            }
                            agent?.ResetOnlineMonitoring("Disconnect on open door");
                        }
                    }
                    else
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.OpenDoor(doorBaseInfo.OpenDoorDelay);
                        }
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
                                    deviceDriver.OpenDoor(doorBaseInfo.OpenDoorDelay);
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

        public void OpenDoorWithDelay(DtoCommunicationDeviceData deviceInfo, int delayInSecond)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    if (deviceInfo.OnlineMonitoringMode)
                    {
                        var agent = ZkServer.Instance.GetOnlineMonitoringAgent(deviceInfo.DeviceNumber);
                        if (agent != null && agent.IsDeviceConnected)
                        {
                            agent.OpenDoor(delayInSecond);
                        }
                        else
                        {
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.OpenDoor(delayInSecond);
                            }
                            agent?.ResetOnlineMonitoring("disconnect on open door with delay");
                        }
                    }
                    else
                    {
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.OpenDoor(delayInSecond);
                        }
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

        public void OpenCabinetDoor(DtoCommunicationDeviceData deviceInfo, int cabinetNumber)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
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


        #region ZK

        public void SetZkDeviceDoorInfo(DtoCommunicationDeviceData deviceInfo, DtoZkDeviceDoor deviceDoorInfo)
        {
            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
        }

        public void SendTimezone(DtoCommunicationDeviceData deviceInfo, DtoTimezone timezone)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    switch (deviceInfo.ConnectionMode)
                    {
                        case DeviceConnectionModeEnumeration.Push:
                            var config = new SystemConfigComponent(RepositoryFactory);
                            var configCache = config.GetSystemConfigCache();
                            var command = ZkPushCommands.GetAcTimezoneCommands(deviceInfo, timezone, configCache.MaxRetryForZkOtherCommand, null, null, null);
                            var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                            commandComponent.Insert(command);
                            break;
                        default:
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                deviceDriver.SendTimeZone(timezone);
                            }
                            break;
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

        }

        public void SendHolidays(DtoCommunicationDeviceData deviceInfo, List<DtoDeviceHoliday> holidays)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Zk)
            {
                switch (deviceInfo.ConnectionMode)
                {
                    case DeviceConnectionModeEnumeration.Push:
                        var config = new SystemConfigComponent(RepositoryFactory);
                        var configCache = config.GetSystemConfigCache();
                        var commands = ZkPushCommands.GetAcHolidayCommands(deviceInfo, holidays, configCache.MaxRetryForZkOtherCommand, null, null, null);
                        var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                        commandComponent.Insert(commands);
                        break;
                    default:
                        using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            deviceDriver.SendHolidays(holidays);
                        }
                        break;
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #region Suprema 1

        public void SendSupremaSdk1Holidays(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk1DeviceHolidayGroup> holidayGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1)
            {

                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SendHolidays(holidayGroups);
                    }
                }
                else
                {
                    var configComponent = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = configComponent.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk1Commands.SendHolidays(deviceInfo
                        , holidayGroups
                        , commandConfig.MaxRetryForOtherCommand
                        , null
                        , null
                        , CommandPriorityEnumeration.Medium);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk1Timezones(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk1Timezone> timezones)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1)
            {

                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetTimezones(timezones);
                    }
                }
                else
                {
                    var configComponent = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = configComponent.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

                    var command = SupremaSdk1Commands.SendTimezones(deviceInfo
                        , timezones
                        , commandConfig.MaxRetryForOtherCommand
                        , null
                        , null
                        , CommandPriorityEnumeration.Medium);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk1AccessGroups(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk1AccessGroup> accessGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetAccessGroups(accessGroups);
                    }
                }
                else
                {
                    var configComponent = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = configComponent.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);

                    var command = SupremaSdk1Commands.SendAccessGroups(deviceInfo
                        , accessGroups
                        , commandConfig.MaxRetryForOtherCommand
                        , null
                        , null
                        , CommandPriorityEnumeration.Medium);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk1DoorInfo(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk1DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion1)
            {

                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetDoorInfo(doorInfo);
                    }
                }
                else
                {
                    var configComponent = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = configComponent.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk1Commands.SetDoorInfo(deviceInfo
                        , doorInfo
                        , commandConfig.MaxRetryForOtherCommand
                        , null
                        , null
                        , CommandPriorityEnumeration.Medium);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenSupremaSdk1DoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk1DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void CloseSupremaSdk1DoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk1DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void OpenSupremaSdk1Door(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk1DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void OpenSupremaSdk1DoorWithDelay(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk1DeviceDoor doorInfo, int delayInSecond)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void SendSupremaSdk2Holidays(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk2DeviceHolidayGroup> holidayGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SendHolidays(holidayGroups);
                    }
                }
                else
                {
                    var config = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk2Commands.GetSendHolidayGroupCommands
                        (deviceInfo, holidayGroups, commandConfig.MaxRetryForOtherCommand, null, null, null);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk2AccessSchedules(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk2AccessSchedule> accessSchedules)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetAccessSchedules(accessSchedules);
                    }
                }
                else
                {
                    var config = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk2Commands.GetSendAccessScheduleCommands
                        (deviceInfo, accessSchedules, commandConfig.MaxRetryForOtherCommand, null, null, null);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk2AccessLevels(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk2AccessLevel> accessLevels)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetAccessLevels(accessLevels);
                    }
                }
                else
                {
                    var config = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk2Commands.GetSendAccessLevelCommands
                        (deviceInfo, accessLevels, commandConfig.MaxRetryForOtherCommand, null, null, null);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk2AccessGroups(DtoCommunicationDeviceData deviceInfo, List<DtoSupremaSdk2AccessGroup> accessGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetAccessGroups(accessGroups);
                    }
                }
                else
                {
                    var config = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk2Commands.GetSendAccessGroupCommands
                        (deviceInfo, accessGroups, commandConfig.MaxRetryForOtherCommand, null, null, null);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void SendSupremaSdk2DoorInfo(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk2DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema
                && deviceInfo.SdkVersionEnum == SdkVersionEnumeration.SdkVersion2)
            {
                if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                {
                    using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SetDoorInfo(new List<DtoSupremaSdk2DeviceDoor> { doorInfo });
                    }
                }
                else
                {
                    var config = new SystemConfigComponent(RepositoryFactory);
                    var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                    var command = SupremaSdk2Commands.GetSendDoorCommands
                        (deviceInfo, doorInfo, commandConfig.MaxRetryForOtherCommand, null, null, null);
                    var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                    commandComponent.Insert(command);


                    GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).SetDoorInfo(new List<DtoSupremaSdk2DeviceDoor>() { doorInfo });
                }
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenSupremaSdk2DoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk2DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void CloseSupremaSdk2DoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk2DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
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

        public void OpenSupremaSdk2Door(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk2DeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
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
                            default:
                                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenSupremaSdk2DoorWithDelay(DtoCommunicationDeviceData deviceInfo, DtoSupremaSdk2DeviceDoor doorInfo, int delayInSecond)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Suprema:
                    {
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion2:
                                switch (deviceInfo.ConnectionMode)
                                {
                                    case DeviceConnectionModeEnumeration.Push:
                                        {
                                            var deviceDriver = GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber);
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


        #region Timy

        public void TimySetDayTimezone(DtoCommunicationDeviceData deviceInfo, List<DtoTimyDayTimezoneGroup> dayTimezoneGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Timy)
            {
                var config = new SystemConfigComponent(RepositoryFactory);
                var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                var command = TimyPushCommands.GetDayTimezoneControlCommand
                    (deviceInfo, dayTimezoneGroups, commandConfig.MaxRetryForOtherCommand, null, null, null);
                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                commandComponent.Insert(command);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void TimySetWeekTimezone(DtoCommunicationDeviceData deviceInfo, List<DtoTimyWeekTimezoneGroup> weekTimezoneGroups)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Timy)
            {
                var config = new SystemConfigComponent(RepositoryFactory);
                var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                var command = TimyPushCommands.GetWeekTimezoneControlCommand
                    (deviceInfo, weekTimezoneGroups, commandConfig.MaxRetryForOtherCommand, null, null, null);
                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                commandComponent.Insert(command);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void TimySetHolidays(DtoCommunicationDeviceData deviceInfo, List<DtoTimyHoliday> holidays)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Timy)
            {
                var config = new SystemConfigComponent(RepositoryFactory);
                var commandConfig = config.GetCommandSettingFromCache(deviceInfo.ProducerEnum, deviceInfo.SdkVersionEnum);
                var command = TimyPushCommands.GetHolidayCommand
                    (deviceInfo, holidays, commandConfig.MaxRetryForOtherCommand, null, null, null);
                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                commandComponent.Insert(command);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #region Padis Controller

        public void OpenPadisControllerDoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerDeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        switch (deviceInfo.ConnectionMode)
                        {
                            case DeviceConnectionModeEnumeration.Standalone:
                                using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                                {
                                    deviceDriver.ChangeDoorStatus(doorInfo.Id, 1, null);
                                }
                                break;
                            case DeviceConnectionModeEnumeration.Push:
                                break;
                        }

                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void ClosePadisControllerDoorPermanent(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerDeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.ChangeDoorStatus(doorInfo.Id, 0, null);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenPadisControllerDoor(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerDeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.ChangeDoorStatus(doorInfo.Id, 1, doorInfo.OpenDoorDelay);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void OpenPadisControllerDoorWithDelay(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerDeviceDoor doorInfo, int delayInSecond)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.ChangeDoorStatus(doorInfo.Id, 1, delayInSecond);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetDoor(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerDeviceDoor doorInfo)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetDoor(doorInfo);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetRelay(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerRelay relay)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetRelay(relay);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetIoPort(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerIoPort ioPort)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetIoPort(ioPort);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetWiegand(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerWiegand wiegand)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetWiegand(wiegand);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetCalendar(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerCalendar calendar)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetCalendar(calendar);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetAccessLevel(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerAccessLevel calendar)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetAccessLevel(calendar);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        public void PadisControllerSetAccessGroup(DtoCommunicationDeviceData deviceInfo, DtoPadisControllerAccessGroup calendar)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Padis:
                    {
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.SetAccessGroup(calendar);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }


        #endregion


        #region Virdi


        public void VirdiSendAccessControlData(DtoCommunicationDeviceData deviceInfo, DtoVirdiAccessControlData accessControlData)
        {
            CheckActiveProducer(deviceInfo);
            if (deviceInfo.ProducerEnum == ProducerEnumeration.Virdi)
            {
                var config = new SystemConfigComponent(RepositoryFactory);
                var configCache = config.GetSystemConfigCache();
                var commands = VirdiCommands.GetSendAccessControlDataCommand(
                    deviceInfo
                    , accessControlData
                    , configCache.MaxRetryForZkOtherCommand
                    , null
                    , null
                    , null);
                var commandComponent = new DeviceCommandComponent(RepositoryFactory);
                commandComponent.Insert(commands);
            }
            else
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }


        #endregion


        public void SendUserTimeZones(DtoCommunicationDeviceData deviceInfo, long employeeNumber, List<int> timeZoneNumbers)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                    {
                        deviceDriver.Connect();
                        deviceDriver.SendUserTimeZone(employeeNumber, timeZoneNumbers);
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }
        }

        #endregion


        #region Internal Method

        internal DtoAttendanceSaveResult DownloadAndSaveUnreadAttendancesFromSdk(DtoCommunicationDeviceData deviceInfo)
        {
            CheckActiveProducer(deviceInfo);
            var allAttendances = CommunicationGetUnreadAttendancesFromSdk(deviceInfo);
            foreach (var item in allAttendances)
            {
                item.IsSent = false;
            }

            var attendanceComponent = new AttendanceComponent(RepositoryFactory);
            return attendanceComponent.SaveAttendance(allAttendances, true, true, true);
        }

        internal DtoPwAutoCollectClearDataResult CommunicationPwClearDataWithRecordCount(DtoCommunicationDeviceData deviceInfo, int previousRecordCount)
        {
            CheckActiveProducer(deviceInfo);
            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.ProcessingWorld:
                    using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                    {
                        var resultOfConnect = deviceDriver.Connect();
                        if (resultOfConnect.RecordCount == previousRecordCount)
                        {
                            deviceDriver.ClearData();
                            return new DtoPwAutoCollectClearDataResult
                            {
                                IsSuccessful = true,
                                NewRecordCount = resultOfConnect.RecordCount,
                                OldRecordCount = previousRecordCount
                            };
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return new DtoPwAutoCollectClearDataResult
            {
                IsSuccessful = false,
                NewRecordCount = 0,
                OldRecordCount = previousRecordCount
            };
        }

        #endregion


        #region Private Method

        private static void CheckActiveProducer(DtoCommunicationDeviceData deviceInfo)
        {
            if (!ApplicationEmbeddedInfo.ActiveProducers.HasFlag(deviceInfo.ProducerEnum))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusProducerNotSupport);
            }

            if (deviceInfo.ProducerEnum == ProducerEnumeration.Suprema)
            {
                if (!ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(deviceInfo.SdkVersionEnum))
                {
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusProducerNotSupport);
                }
            }
        }

        private static SupremaSdk1OnDemandAdapter GetSupremaSdk1DeviceAdapter(int deviceNumber)
        {
            var deviceAdapter = SupremaSdk1Server.Instance.GetDeviceAdapter(deviceNumber);
            if (deviceAdapter == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }

            return deviceAdapter;
        }

        private static SupremaSdk2OnDemandAdapter GetSupremaSdk2DeviceAdapter(int deviceNumber)
        {
            var deviceAdapter = SupremaSdk2Server.Instance.GetDeviceAdapter(deviceNumber);
            if (deviceAdapter == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
            }

            return deviceAdapter;
        }

        private List<DtoAttendance> CommunicationGetUnreadAttendancesFromSdk(DtoCommunicationDeviceData deviceInfo)
        {
            var result = new List<DtoAttendance>();

            switch (deviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Virdi:
                    {
                        VirdiServer.Instance.GetLogAsync(1, deviceInfo.DeviceNumber, VirdiDeviceLogTypeEnum.New);
                    }
                    break;
                case ProducerEnumeration.Zk:
                    {
                        if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Push)
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration
                                .CommunicationStatusNotSupport);
                        }

                        if (deviceInfo.IsOldVersion)
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
                                .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                .FirstOrDefault();
                            using (var deviceDriver = new ZkOnDemandAdapter(deviceInfo))
                            {
                                deviceDriver.Connect();
                                result.AddRange(deviceCommunicationData?.LastAttendanceLogDateTime != null
                                    ? deviceDriver.GetData(deviceCommunicationData.LastAttendanceLogDateTime.Value, DateTime.Now)
                                    : deviceDriver.GetDataWithDefaultDates());
                            }
                            if (result.IsCollectionNotNullOrEmpty())
                            {
                                var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                if (deviceCommunicationData != null)
                                {
                                    deviceCommunicationData.LastAttendanceLogDateTime = lastReadTime;
                                }
                                else
                                {
                                    deviceCommunicationData = new DtoDeviceCommunicationData
                                    {
                                        DeviceNumber = deviceInfo.DeviceNumber,
                                        LastAttendanceLogDateTime = lastReadTime,
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
                        switch (deviceInfo.SdkVersionEnum)
                        {
                            case SdkVersionEnumeration.SdkVersion1:
                                {
                                    var deviceComponent = new DeviceComponent(RepositoryFactory);
                                    var deviceCommunicationData = deviceComponent
                                        .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                        .FirstOrDefault();
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk1OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(deviceCommunicationData?.LastAttendanceLogDateTime != null
                                                ? deviceDriver.GetData(deviceCommunicationData.LastAttendanceLogDateTime, DateTime.Now)
                                                : deviceDriver.GetDataWithDefaultDates());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(deviceCommunicationData?.LastAttendanceLogDateTime != null
                                            ? GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetData(deviceCommunicationData.LastAttendanceLogDateTime, DateTime.Now)
                                            : GetSupremaSdk1DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetDataWithDefaultDates());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            deviceCommunicationData.LastAttendanceLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceNumber = deviceInfo.DeviceNumber,
                                                LastAttendanceLogDateTime = lastReadTime,
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
                                        .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                                        .FirstOrDefault();
                                    if (deviceInfo.ConnectionMode == DeviceConnectionModeEnumeration.Standalone)
                                    {
                                        using (var deviceDriver = new SupremaSdk2OnDemandAdapter(deviceInfo))
                                        {
                                            deviceDriver.Connect();
                                            result.AddRange(deviceCommunicationData?.LastAttendanceLogId != null
                                                ? deviceDriver.GetData((uint)deviceCommunicationData.LastAttendanceLogId.Value)
                                                : deviceDriver.GetDataWithDefault());
                                        }
                                    }
                                    else
                                    {
                                        result.AddRange(deviceCommunicationData?.LastAttendanceLogId != null
                                            ? GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber)
                                                .GetData((uint)deviceCommunicationData.LastAttendanceLogId.Value)
                                            : GetSupremaSdk2DeviceAdapter(deviceInfo.DeviceNumber).GetDataWithDefault());
                                    }

                                    if (result.IsCollectionNotNullOrEmpty())
                                    {
                                        var lastEventId = result.Max(row => row.Id);
                                        var lastReadTime = result.Max(row => row.AttendanceDateTime);
                                        if (deviceCommunicationData != null)
                                        {
                                            deviceCommunicationData.LastAttendanceLogId = lastEventId;
                                            deviceCommunicationData.LastAttendanceLogDateTime = lastReadTime;
                                        }
                                        else
                                        {
                                            deviceCommunicationData = new DtoDeviceCommunicationData
                                            {
                                                DeviceNumber = deviceInfo.DeviceNumber,
                                                LastAttendanceLogId = lastEventId,
                                                LastAttendanceLogDateTime = lastReadTime,
                                            };
                                        }

                                        deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case ProducerEnumeration.ProcessingWorld:
                    {
                        using (var deviceDriver = new PwOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result.AddRange(deviceDriver.GetData());
                        }
                    }
                    break;
                case ProducerEnumeration.ElmOSanat:
                    {
                        using (var deviceDriver = new ElmoSanatOnDemandAdapter(deviceInfo))
                        {
                            deviceDriver.Connect();
                            result.AddRange(deviceDriver.GetData());
                        }
                    }
                    break;
                case ProducerEnumeration.Padis:
                    {
                        var deviceComponent = new DeviceComponent(RepositoryFactory);
                        var deviceCommunicationData = deviceComponent
                            .GetDeviceCommunicationDataByDeviceNumbers(new List<int> { deviceInfo.DeviceNumber })
                            .FirstOrDefault();
                        using (var deviceDriver = new PadisControllerOnDemandAdapter(deviceInfo))
                        {
                            result.AddRange(deviceCommunicationData?.LastAttendanceLogDateTime != null
                                                ? deviceDriver.GetAttendances(deviceCommunicationData.LastAttendanceLogDateTime.Value, DateTime.Now)
                                                : deviceDriver.GetAttendances(DateTime.Now.AddDays(-30), DateTime.Now));
                        }
                        if (result.IsCollectionNotNullOrEmpty())
                        {
                            var lastReadTime = result.Max(row => row.AttendanceDateTime);
                            if (deviceCommunicationData != null)
                            {
                                deviceCommunicationData.LastAttendanceLogDateTime = lastReadTime;
                            }
                            else
                            {
                                deviceCommunicationData = new DtoDeviceCommunicationData
                                {
                                    DeviceNumber = deviceInfo.DeviceNumber,
                                    LastAttendanceLogDateTime = lastReadTime,
                                };
                            }
                            deviceComponent.SaveDeviceCommunicationDataInfo(deviceCommunicationData);
                        }
                    }
                    break;
                default:
                    throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusNotSupport);
            }

            return result;

        }

        private static List<DtoDeviceCommand> GetZkEnrollUserCommands(
            DtoEmployeeAndDeviceParam employeeAndDeviceInfo
            , DtoSystemConfigDeviceCommandSetting commandConfig)
        {
            // در مورد دستگاه های زد-کا، به دلیل عدم پشتیبانی از تاریخ شروع و پایان، 
            // کاربران عادی و کاربران موقت می بایست در تاریه شروع به دستگاه ارسال شوند

            var commands = new List<DtoDeviceCommand>();
            switch (employeeAndDeviceInfo.UserInfo.UserType)
            {
                case DeviceUserTypeEnumeration.PermanentUser:
                    {

                        if (employeeAndDeviceInfo.UserInfo.EndTime.HasValue &&
                            employeeAndDeviceInfo.UserInfo.EndTime < DateTime.Now)
                        {
                            // در صورتی که کابر دائم بود و تاریخ پایان مربوط به گذشته بود، می بایست
                            // کاربر بلافاصله از روی دیتگاه حذف شود
                            commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                employeeAndDeviceInfo.DeviceInfo,
                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                null,
                                employeeAndDeviceInfo.CommandPriority,
                                employeeAndDeviceInfo.CommandIdentifier));
                        }
                        else
                        {
                            // این تاریخ پایان کاربر دائمی در اینده است. حال می بایست تاریخ شروع را بررسی کنیم و بر اساس آن 
                            // تصمیم بگیریم. در صورتی که تاریخ شروع نیز در اینده بود، بلافاصله می بایست کاربر را از روی سخت افزار حذف نماییم
                            // و سپس در زمان اینده مجددا آن را ارسال نماییم. 
                            DateTime? defineUserVisibilityDateTime = employeeAndDeviceInfo.UserInfo.StartTime;
                            if (defineUserVisibilityDateTime < DateTime.Now)
                            {
                                defineUserVisibilityDateTime = null;
                            }
                            else
                            {
                                // یعنی زمان شروع در اینده است و می بایست کاربر الان از روی ساعت حذف شود
                                commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                    employeeAndDeviceInfo.DeviceInfo,
                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    null,
                                    employeeAndDeviceInfo.CommandPriority,
                                    employeeAndDeviceInfo.CommandIdentifier));
                            }

                            commands.AddRange(ZkPushCommands.GetEnrollUserCommands(
                                employeeAndDeviceInfo.DeviceInfo,
                                employeeAndDeviceInfo.UserInfo,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                defineUserVisibilityDateTime,
                                employeeAndDeviceInfo.CommandPriority,
                                employeeAndDeviceInfo.CommandIdentifier));

                            if (employeeAndDeviceInfo.UserInfo.EndTime.HasValue)
                            {
                                commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                    employeeAndDeviceInfo.DeviceInfo,
                                    employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                    commandConfig.MaxRetryForUserCommand,
                                    null,
                                    employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                    employeeAndDeviceInfo.CommandPriority,
                                    employeeAndDeviceInfo.CommandIdentifier));
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

                        DateTime? defineUserVisibilityDateTime = employeeAndDeviceInfo.UserInfo.StartTime;
                        if (defineUserVisibilityDateTime < DateTime.Now)
                        {
                            defineUserVisibilityDateTime = null;
                        }

                        commands.AddRange(ZkPushCommands.GetEnrollUserCommands(
                            employeeAndDeviceInfo.DeviceInfo,
                            employeeAndDeviceInfo.UserInfo,
                            commandConfig.MaxRetryForUserCommand,
                            null,
                            defineUserVisibilityDateTime,
                            employeeAndDeviceInfo.CommandPriority,
                            employeeAndDeviceInfo.CommandIdentifier));

                        if (employeeAndDeviceInfo.UserInfo.EndTime.HasValue)
                        {
                            commands.Add(ZkPushCommands.GetDeleteUserCommands(
                                employeeAndDeviceInfo.DeviceInfo,
                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                commandConfig.MaxRetryForUserCommand,
                                null,
                                employeeAndDeviceInfo.UserInfo.EndTime.Value,
                                employeeAndDeviceInfo.CommandPriority,
                                employeeAndDeviceInfo.CommandIdentifier));
                        }

                    }
                    break;
            }
            return commands;

        }

        private static void ProcessStartAndEndTimeOfUser(DtoEmployeeDeviceRelatedData userInfo)
        {
            if (userInfo.UserType == DeviceUserTypeEnumeration.PermanentUser)
            {
                userInfo.StartTime = userInfo.StartTime.Date;
                if (userInfo.EndTime.HasValue)
                {
                    userInfo.EndTime = DateTimeHelper.GetEndOf(userInfo.EndTime.Value, DateTimeHelper.DateInterval.Day);
                }
            }
        }

        private static void RemoveNecessaryCommandsOnEnrollUser(DtoEmployeeAndDeviceParam employeeAndDeviceInfo
                , DeviceCommandComponent commandComponent)
        {
            var commandTypes = new List<DeviceCommandTypeEnumeration>();
            switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    commandTypes = ZkPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Timy:
                    commandTypes = TimyPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Suprema:
                    switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
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
                switch (employeeAndDeviceInfo.UserInfo.UserType)
                {
                    case DeviceUserTypeEnumeration.PermanentUser:
                        // در صورتی که کاربر دائمی بود، می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                        // روی دستگاه جاری وجود دارد، را حذف نماییم
                        commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                        (employeeAndDeviceInfo.UserInfo.EmployeeNumber
                            , employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                            , commandTypes);
                        break;
                    case DeviceUserTypeEnumeration.TempUser:
                        if (employeeAndDeviceInfo.CommandIdentifier.HasValue)
                        {
                            // در صورتی که کاربر موقت است و دستوراتی مرتبط با دستور جاری وجود دارد، فقط همان دستورات را حذف می کنیم
                            // و به سایر دستور مربوط به کاربر جاری بر روی دستگاه جاری کاری نداریم.
                            // برای مثال فرض کنیم که کاربر مراجعه کننده است و در دو تاریخ مجزا قرار است که به سازمان مراجعه نماید
                            // تاریخ یکی از مراجعه ها تغییر کرده است، فقط می بایست دستورات مربوط به آن مراجعه حذف شوند
                            // و با دستورات سایر مراجعه ها کاری نداشته باشیم
                            commandComponent.DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier
                            (employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                                , commandTypes
                                , new List<Guid> { employeeAndDeviceInfo.CommandIdentifier.Value });

                            // حال در صورتی که کاربر دستورت حذفی در بازه کاربر جاری دارد برای جلوگیری از تداخل می بایست 
                            // آن دستور قبل از ثبت دستورات جدید از سیستم حذف شود. 
                            // چون در حالت های دیگر تمامی دستورات کاربر بر روی آن دستگاه حذف می شوند و فقط در این حالت است که
                            // می بایست در صورتی که دستور حذفی در بین بازه فعال بودن کاربر وجود داشت
                            commandComponent.DeleteNotSentByEmployeeDeviceCommandTypesAndCommandDateInterval(
                                employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                employeeAndDeviceInfo.DeviceInfo.DeviceNumber,
                                new List<DeviceCommandTypeEnumeration>
                                {
                                    DeviceCommandTypeEnumeration.DeleteUser
                                }
                                // ReSharper disable PossibleInvalidOperationException
                                , employeeAndDeviceInfo.UserInfo.StartTime, employeeAndDeviceInfo.UserInfo.EndTime.Value
                            // ReSharper restore PossibleInvalidOperationException
                            );
                        }
                        else
                        {
                            // در صورتی که کاربر موقت باشد ولی دستورات مرتبط پیشین نداشته باشد، 
                            // می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                            // روی دستگاه جاری وجود دارد، را حذف نماییم
                            commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                            (employeeAndDeviceInfo.UserInfo.EmployeeNumber
                                , employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                                , commandTypes);
                        }

                        break;
                }
            }

        }

        private static void RemoveNecessaryCommandsOnRemoveUser(DtoEmployeeAndDeviceParam employeeAndDeviceInfo
                , DeviceCommandComponent commandComponent)
        {
            var commandTypes = new List<DeviceCommandTypeEnumeration>();
            switch (employeeAndDeviceInfo.DeviceInfo.ProducerEnum)
            {
                case ProducerEnumeration.Zk:
                    commandTypes = ZkPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Timy:
                    commandTypes = TimyPushCommands.GetDefineAndDeleteUserCommandTypes();
                    break;
                case ProducerEnumeration.Suprema:
                    switch (employeeAndDeviceInfo.DeviceInfo.SdkVersionEnum)
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
                switch (employeeAndDeviceInfo.UserInfo.UserType)
                {
                    case DeviceUserTypeEnumeration.PermanentUser:
                        // در صورتی که کاربر دائمی بود، می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                        // روی دستگاه جاری وجود دارد، را حذف نماییم
                        commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                        (employeeAndDeviceInfo.UserInfo.EmployeeNumber
                            , employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                            , commandTypes);
                        break;
                    case DeviceUserTypeEnumeration.TempUser:
                        if (employeeAndDeviceInfo.CommandIdentifier.HasValue)
                        {
                            // در صورتی که کاربر موقت است و دستوراتی مرتبط با دستور جاری وجود دارد، فقط همان دستورات را حذف می کنیم
                            // و به سایر دستور مربوط به کاربر جاری بر روی دستگاه جاری کاری نداریم.
                            // برای مثال فرض کنیم که کاربر مراجعه کننده است و در دو تاریخ مجزا قرار است که به سازمان مراجعه نماید
                            // تاریخ یکی از مراجعه ها تغییر کرده است، فقط می بایست دستورات مربوط به آن مراجعه حذف شوند
                            // و با دستورات سایر مراجعه ها کاری نداشته باشیم
                            commandComponent.DeleteNotSentByEmployeeDeviceCommandTypesAndCommandIdentifier
                            (employeeAndDeviceInfo.UserInfo.EmployeeNumber,
                                employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                                , commandTypes
                                , new List<Guid> { employeeAndDeviceInfo.CommandIdentifier.Value });
                        }
                        else
                        {
                            // در صورتی که کاربر موقت باشد ولی دستورات مرتبط پیشین نداشته باشد، 
                            // می بایست هر دستور معرفی و یا حذفی که برای کاربر جاری و بر
                            // روی دستگاه جاری وجود دارد، را حذف نماییم
                            commandComponent.DeleteNotSentByEmployeeDeviceAndCommandTypes
                            (employeeAndDeviceInfo.UserInfo.EmployeeNumber
                                , employeeAndDeviceInfo.DeviceInfo.DeviceNumber
                                , commandTypes);
                        }

                        break;
                }
            }

        }

        private static DateTime? ProcessVisibilityTime(DtoEmployeeAndDeviceParam dataToProcess)
        {
            DateTime? visibilityTime = dataToProcess.UserInfo.StartTime;
            if (visibilityTime < DateTime.Now)
            {
                visibilityTime = null;
            }
            return visibilityTime;
        }

        #endregion


    }
}
