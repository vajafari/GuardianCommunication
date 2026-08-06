using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class SelfSendMealsTask : TimedBaseTask
    {
        private readonly KarnamaComponent _karnamaComponent;
        private readonly CommunicationComponent _communicationComponent;
        private readonly DeviceComponent _deviceComponent;
        private readonly int _selfOffsetForFutureMealsInMinute;
        private readonly bool _sendSingleFood;
        private readonly List<DtoDeviceMealOrderData> _successfulSend = new List<DtoDeviceMealOrderData>();

        public SelfSendMealsTask(TimeSpan interval, int selfOffsetForFutureMealsInMinute, bool sendSingleFood) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _karnamaComponent = new KarnamaComponent(repositoryFactory);
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            _selfOffsetForFutureMealsInMinute = selfOffsetForFutureMealsInMinute;
            _sendSingleFood = sendSingleFood;
            LoggingSystem.LogInfo($"SelfSendMealsTask started with interval {(int)interval.TotalMinutes}");
        }


        private readonly object _lock = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
            {
                LoggingSystem.LogInfo("SelfSendMealsTask process is calling");
            }
            if (Monitor.TryEnter(_lock))
            {
                try
                {

                    _successfulSend.RemoveAll(i => i.StartDateTime.Date <= DateTime.Now.Date.AddDays(-2));

                    var mealsOrder = _karnamaComponent.GetMealOrderMealsInfo(_selfOffsetForFutureMealsInMinute);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
                    {
                        LoggingSystem.LogInfo("SelfSendMealsTask GetMealOrderMealsInfo result", mealsOrder);
                    }
                    if (mealsOrder.IsCollectionNotNullOrEmpty())
                    {
                        var allSelfDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                            .Where(row => row.ApplicationId.HasFlag(ApplicationTypeEnumeration.Self))
                            .ToList();

                        foreach (var item in mealsOrder.OrderBy(mo => mo.DeviceNumber))
                        {
                            try
                            {
                                var device = allSelfDeviceInCache.FirstOrDefault(d => d.DeviceNumber == item.DeviceNumber);
                                if (device != null)
                                {
                                    if (_successfulSend.All(ss => ss != item))
                                    {
                                        if (item.FoodTitles.IsCollectionNotNullOrEmpty())
                                        {
                                            if (item.FoodTitles.Count == 1)
                                            {
                                                // تنها یک غذا وجود دارد
                                                if (_sendSingleFood)
                                                {
                                                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
                                                    {
                                                        LoggingSystem.LogInfo("SelfSendMealsTask CommunicationSendFunctionTitles is called", item.FoodTitles);
                                                    }
                                                    _communicationComponent.CommunicationSendFunctionTitles
                                                        (_deviceComponent.ConvertDeviceToDeviceInfo(device), item.FoodTitles);
                                                }
                                                else
                                                {
                                                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
                                                    {
                                                        LoggingSystem.LogInfo("SelfSendMealsTask CommunicationDisableFunctionTitles is called because of single food");
                                                    }
                                                    _communicationComponent.CommunicationDisableFunctionTitles
                                                        (_deviceComponent.ConvertDeviceToDeviceInfo(device));
                                                }
                                            }
                                            else
                                            {
                                                // بیش از یک غذا وجود دارد
                                                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration
                                                        .SelfSendMealsTask))
                                                {
                                                    LoggingSystem.LogInfo("SelfSendMealsTask CommunicationSendFunctionTitles is called", item.FoodTitles);
                                                }
                                                _communicationComponent.CommunicationSendFunctionTitles
                                                    (_deviceComponent.ConvertDeviceToDeviceInfo(device), item.FoodTitles);
                                            }
                                        }
                                        else
                                        {
                                            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration
                                                    .SelfSendMealsTask))
                                            {
                                                LoggingSystem.LogInfo("SelfSendMealsTask CommunicationDisableFunctionTitles is called");
                                            }
                                            _communicationComponent.CommunicationDisableFunctionTitles
                                                (_deviceComponent.ConvertDeviceToDeviceInfo(device));
                                        }
                                        _successfulSend.Add(item);
                                    }
                                    else
                                    {

                                        if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
                                        {
                                            LoggingSystem.LogInfo(ObjectHelper.SerializeAsJsonFormatted(item), "SelfSendMealsTask Self item sent before");
                                        }
                                    }
                                }
                            }
                            catch (Exception exp)
                            {
                                LoggingSystem.LogError(exp, "SelfSendMealsTask Error on CommunicationSendFunctionTitles");
                            }
                        }

                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "SelfSendMealsTask Exception at SelfSendMealsTask");
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            else
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SelfSendMealsTask))
                {
                    LoggingSystem.LogInfo("SelfSendMealsTask skipped because of lock is taken");
                }
            }
        }

    }
}
