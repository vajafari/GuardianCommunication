using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Zk;

namespace GuardianCommunication.Business.Tasks
{
    public class OnlineDeviceTask : TimedBaseTask
    {
        public OnlineDeviceTask(TimeSpan interval) : base(interval)
        {
            LoggingSystem.LogInfo($"OnlineDeviceTask started with interval {(int)interval.TotalSeconds}");
        }

        private readonly object _onlineDeviceDeviceList = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SendOnlineStatusTimer))
            {
                LoggingSystem.LogInfo("OnlineDeviceTask process is calling");
            }
            if (Monitor.TryEnter(_onlineDeviceDeviceList))
            {
                try
                {
                    var deviceNumbers = new List<int>();
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
                    {
                        if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
                        {
                            deviceNumbers.AddRange(SupremaSdk1Server.Instance.GetConnectedDeviceNumbers());
                        }

                        if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
                        {
                            deviceNumbers.AddRange(SupremaSdk2Server.Instance.GetConnectedDeviceNumbers());
                        }
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
                    {
                        deviceNumbers.AddRange(ZkServer.Instance.GetConnectedDeviceNumbers());
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
                    {
                        deviceNumbers.AddRange(TimyServer.Instance.GetConnectedDeviceNumbers());
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
                    {
                        deviceNumbers.AddRange(VirdiServer.Instance.GetConnectedDeviceNumbers());
                    }

                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SendOnlineStatusTimer))
                    {
                        LoggingSystem.LogInfo("OnlineDeviceTask Online device result", deviceNumbers);
                    }
                    if (deviceNumbers.IsCollectionNotNullOrEmpty())
                    {
                        HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(deviceNumbers.Select(dn => new DtoDeviceConnectionStatus
                        {
                            DeviceNumber = dn,
                            IsConnected = true
                        }).ToList());
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                }
                finally
                {
                    Monitor.Exit(_onlineDeviceDeviceList);
                }
            }
            else
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SendOnlineStatusTimer))
                {
                    LoggingSystem.LogInfo("OnlineDeviceTask skipped because of lock is taken");
                }
            }
        }
    }
}
