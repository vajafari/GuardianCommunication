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
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;

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
                    var deviceIds = new List<Guid>();
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
                    {
                        if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion1))
                        {
                            deviceIds.AddRange(SupremaSdk1Server.Instance.GetConnectedDeviceNumbers());
                        }

                        if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion2))
                        {
                            deviceIds.AddRange(SupremaSdk2Server.Instance.GetConnectedDeviceNumbers());
                        }
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
                    {
                        deviceIds.AddRange(ZkServer.Instance.GetConnectedDeviceNumbers());
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
                    {
                        deviceIds.AddRange(TimyServer.Instance.GetConnectedDeviceNumbers());
                    }
                    if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
                    {
                        deviceIds.AddRange(VirdiServer.Instance.GetConnectedDeviceIds());
                    }

                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.SendOnlineStatusTimer))
                    {
                        LoggingSystem.LogInfo("OnlineDeviceTask Online device result", deviceIds);
                    }
                    if (deviceIds.IsCollectionNotNullOrEmpty())
                    {
                        HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(deviceIds.Select(dn => new DtoDeviceConnectionStatus
                        {
                            DeviceId = dn,
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
