using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.MetalDetectorGate.Padis;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Zk;

namespace GuardianCommunication.Business.Component
{
    public class DeviceComponent : BaseComponent
    {

        public DeviceComponent(RepositoryFactory repositoryFactory) : base(repositoryFactory)
        { }

        #region X-Ray

        public void ResetXRayDeviceCacheAndSetConnectionModes()
        {
            if (!AppConfigs.IsXRayActive)
            {
                return;
            }
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogXRayCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Start of reset X-Ray gate cache");
            }
            var allActiveXRayDevices = FetchAllActiveXRayDevices();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogXRayCacheProcessFetch))
            {
                LoggingSystem.LogInfo("X-Ray Gate list on reset cache", allActiveXRayDevices);
            }
            CacheWrapper.Instance.SetXRayDeviceCache(allActiveXRayDevices);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogXRayCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Set new X-Ray gates cache");
            }

            //if (allActiveXRayDevices.IsCollectionNotNullOrEmpty())
            //{
            //    PadisXRayDeviceServer.Instance.SetDeviceOnPushModeList(allActiveXRayDevices);
            //    if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.GeneralLog))
            //    {
            //        LoggingSystem.LogInfo("Set new device list for PadisXRayDeviceServer.SetDeviceOnPushModeList");
            //    }
            //}
        }

        public List<DtoXRayDevice> SearchXRayDeviceCache(Expression<Func<DtoXRayDevice, bool>> expression)
        {
            if (!AppConfigs.IsXRayActive)
            {
                return new List<DtoXRayDevice>();
            }

            return CacheWrapper.Instance.XRayDeviceCacheManager.Filter(expression.Compile()).ToList();
        }

        internal List<DtoXRayDevice> FetchAllActiveXRayDevices()
        {
            if (!AppConfigs.IsXRayActive)
            {
                return new List<DtoXRayDevice>();
            }
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var result = karnamaComponent.FetchAllXRayDevices();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogXRayFetch))
            {
                LoggingSystem.LogInfo("Device list fetched", result);
            }

            return result;
        }

        #endregion


        #region Metal detector

        public void ResetMetalDetectorGateCacheAndSetConnectionModes()
        {
            if (!AppConfigs.IsMetalDetectorGateActive)
            {
                return;
            }
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogMetalDetectionGateCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Start of reset metal detector gate cache");
            }
            var allActiveMetalDetectorGates = FetchAllActiveMetalDetectorGates();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogMetalDetectionGateCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Metal detector Gate list on reset cache", allActiveMetalDetectorGates);
            }
            CacheWrapper.Instance.SetMetalDetectorGateCache(allActiveMetalDetectorGates);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogMetalDetectionGateCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Set new metal detector gates cache");
            }

            if (allActiveMetalDetectorGates.IsCollectionNotNullOrEmpty())
            {
                PadisMetalDetectorGateServer.Instance.SetDeviceOnPushModeList(allActiveMetalDetectorGates);
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogMetalDetectionGateCacheProcessFetch))
                {
                    LoggingSystem.LogInfo("Set new device list for PadisMetalDetectorGateServer.SetDeviceOnPushModeList");
                }
            }
        }

        public List<DtoMetalDetectorGate> SearchMetalDetectorGateCache(Expression<Func<DtoMetalDetectorGate, bool>> expression)
        {
            if (!AppConfigs.IsMetalDetectorGateActive)
            {
                return new List<DtoMetalDetectorGate>();
            }
            return CacheWrapper.Instance.MetalDetectorGateCacheManager.Filter(expression.Compile()).ToList();
        }


        internal List<DtoMetalDetectorGate> FetchAllActiveMetalDetectorGates()
        {
            if (!AppConfigs.IsMetalDetectorGateActive)
            {
                return new List<DtoMetalDetectorGate>();
            }
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var result = karnamaComponent.FetchAllMetalDetectorGates();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogMetalDetectionGateFetch))
            {
                LoggingSystem.LogInfo("Metal detector list fetched", result);
            }
            return result;
        }

        #endregion


        #region Device

        public void ResetDeviceCacheAndSetDeviceConnectionModes()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Start of reset device cache");
            }
            var allDevices = GetDeviceList();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Device list on reset cache", allDevices);
            }
            CacheWrapper.Instance.SetDeviceCache(allDevices);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Set new device cache");
            }

            if (allDevices.IsCollectionNotNullOrEmpty())
            {
                var deviceInfos = ConvertDeviceToDeviceInfo(allDevices);
                if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
                {
                    ZkServer.Instance.SetDeviceOnPushModeList(deviceInfos);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                    {
                        LoggingSystem.LogInfo("Set new device list for ZkServer SetDeviceOnPushModeList");
                    }


                    ZkServer.Instance.SetOnlineMonitoringDeviceModeList(deviceInfos);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                    {
                        LoggingSystem.LogInfo("Set new device list for ZkServer SetOnlineMonitoringDeviceModeList");
                    }
                }
                if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
                {
                    TimyServer.Instance.SetDeviceList(deviceInfos);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                    {
                        LoggingSystem.LogInfo("Set new device list for TimyServer SetDeviceList");
                    }
                }

                if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
                {
                    if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
                    {
                        SupremaSdk1Server.Instance.SetDeviceList(deviceInfos);
                        if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                        {
                            LoggingSystem.LogInfo("Set new device list for SupremaSdk1Server");
                        }
                    }

                    if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
                    {
                        SupremaSdk2Server.Instance.SetDeviceList(deviceInfos);
                        if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                        {
                            LoggingSystem.LogInfo("Set new device list for SupremaSdk2Server");
                        }
                    }
                }

                if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
                {
                    VirdiServer.Instance.SetDeviceList(deviceInfos);
                    if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceCacheProcessFetch))
                    {
                        LoggingSystem.LogInfo("Set new device list for Virdi SetDeviceList");
                    }
                }
            }
        }

        public List<DtoDevice> SearchDeviceCache(Expression<Func<DtoDevice, bool>> expression)
        {
            return CacheWrapper.Instance.DeviceCacheManager.Filter(expression.Compile()).ToList();
        }

        public List<DtoCommunicationDeviceData> ConvertDeviceToDeviceInfo(List<DtoDevice> deviceList)
        {
            return deviceList.IsCollectionNotNullOrEmpty() ? deviceList.Select(ConvertDeviceToDeviceInfo).ToList() : new List<DtoCommunicationDeviceData>();
        }


        #region Internal Method

        internal List<DtoDevice> GetDeviceList()
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var result = karnamaComponent.FetchAllDevices();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeviceFetch))
            {
                LoggingSystem.LogInfo("Device list fetched", result);
            }

            return result;
        }

        internal DtoDevice GetDeviceByDeviceNumber(int deviceNumber)
        {
            return CacheWrapper.Instance.DeviceCacheManager.GetCacheItem(deviceNumber.ToString());
        }

        internal DtoCommunicationDeviceData GetCommunicationDeviceDataByDeviceNumber(int deviceNumber)
        {
            return ConvertDeviceToDeviceInfo(CacheWrapper.Instance.DeviceCacheManager.GetCacheItem(deviceNumber.ToString()));
        }

        internal DtoCommunicationDeviceData ConvertDeviceToDeviceInfo(DtoDevice device)
        {
            const int StartDayLight = 322;
            const int EndDayLight = 922;
            const int DaylightChangeTimeInSeconds = 3600;

            if (device == null)
                return null;
            return new DtoCommunicationDeviceData
            {
                SdkVersionEnum = device.DeviceTypeSummary.SdkVersion,
                ConnectionMode = device.ConnectionMode,
                BuadRate = device.BaudRate,
                ComPort = device.ComPort,
                CommunicationPassword = device.DevicePassword,
                ConnectTimeout = device.ConnectTimeout,
                ConnectionTypeEnum = device.ConnectionType,
                DeviceNumber = device.DeviceNumber,
                DoorTypeEnum = device.DeviceTypeSummary.DoorType,
                EnrollStandardEnum = device.DeviceTypeSummary.EnrollStandard,
                HasFace = device.DeviceTypeSummary.HasFace,
                HasFinger = device.DeviceTypeSummary.HasFingerPrint,
                HasRfCard = device.DeviceTypeSummary.HasRfReader,
                HasVisibleLight = device.HasVisibleLight,
                HasPalm = device.HasPalm,
                HasIris = device.DeviceTypeSummary.HasIris,
                AutomaticDataCollect = device.AutomaticDataCollect,
                IsOldVersion = device.IsOldVersion,
                DeviceSettings = device.DeviceSettings,
                IoType = device.IoType,
                IsHookActive = device.IsHookActive,
                Ip = device.DeviceIp,
                ProducerEnum = device.DeviceTypeSummary.ProducerNumber,
                DeviceTypeCode = device.DeviceTypeSummary.DeviceTypeCode,
                DeviceTypeNumber = device.DeviceTypeSummary.DeviceTypeNumber,
                TcpPort = device.TcpPort,
                SerialNumber = device.SerialNumber,
                IsMasterDevice = device.IsMasterDevice,
                HasAttendanceValidationCheck = device.HasAttendanceValidationCheck,
                PwMaxRetry = device.PwMaxRetry,
                PwConnectionTimeout = device.PwConnectionTimeout,
                PwAcceptValidList = device.PwAcceptValidList,
                PwPcPort = device.PwPcPort,
                ApplicationId = device.ApplicationId,
                HasSoftwareAccessControl = device.HasSoftwareAccessControl,
                OnlineMonitoringMode = device.OnlineMonitoringMode,
                JustDoorControl = device.JustDoorControl,
                SendProfileImage = device.SendProfileImage,
                TimeSetting = new DtoCommunicationDeviceTimeSettings
                {
                    IsDaylightActive = false,
                    DaylightStart = StartDayLight,
                    DaylightEnd = EndDayLight,
                    DaylightChangeTimeInSeconds = DaylightChangeTimeInSeconds,
                    TimeZone = device.TimeZone,
                }
            };
        }


        private static readonly Random RandomGenerator = new Random();
        internal bool IsDeviceNumberValid(int deviceNumber, ValidSerialNumberCheckTypeEnumeration checkType)
        {
            var deviceInCache = GetDeviceByDeviceNumber(deviceNumber);
            if (deviceInCache != null
                    && ApplicationEmbeddedInfo.CheckDeviceSerialNumber
                    && deviceInCache.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.Zk
                    && deviceInCache.ConnectionMode == DeviceConnectionModeEnumeration.Push
                    && deviceInCache.HasVisibleLight
                    && ApplicationEmbeddedInfo.ValidSerialNumberCheckTypes.HasFlag(checkType)
                    && (!ApplicationEmbeddedInfo.EffectiveDateForValidDeviceSerialNumbers.HasValue || ApplicationEmbeddedInfo.EffectiveDateForValidDeviceSerialNumbers.Value <= DateTime.Now)
                    && !ApplicationEmbeddedInfo.ValidDeviceSerialNumbers.Contains(deviceInCache.SerialNumber.ToNotNullString())
                )
            {
                switch (checkType)
                {
                    case ValidSerialNumberCheckTypeEnumeration.User:
                        {
                            var divisionRandom = RandomGenerator.Next(3, 10);
                            var randomNumber = RandomGenerator.Next(1, 200);
                            if (randomNumber % divisionRandom == 0)
                            {
                                return false;
                            }
                        }
                        break;
                    case ValidSerialNumberCheckTypeEnumeration.Attendance:
                        {
                            var random = RandomGenerator.Next(0, 10);
                            if (random == 9)
                            {
                                LoggingSystem.LogWarning(ObjectHelper.SerializeAsJson(new
                                {
                                    ApplicationEmbeddedInfo.CheckDeviceSerialNumber,
                                    deviceInCache.SerialNumber,
                                    CheckType = checkType,
                                    ApplicationEmbeddedInfo.ValidSerialNumberCheckTypes,
                                    ApplicationEmbeddedInfo.EffectiveDateForValidDeviceSerialNumbers,
                                    ApplicationEmbeddedInfo.ValidDeviceSerialNumbers
                                }), "VDS");
                                return false;
                            }
                        }
                        break;
                }

            }

            return true;
        }

        #endregion


        #endregion


        #region Device Door

        public void ResetDeviceDoorCache()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceDoorCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Start of reset Door cache");
            }
            var allDoors = GetDeviceDoorList();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceDoorCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Door list on reset cache", allDoors);
            }
            CacheWrapper.Instance.SetDeviceDoorCache(allDoors);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogAttendanceDoorCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Set new Door cache");
            }
        }

        public List<DtoDeviceDoor> SearchDeviceDoorCache(Expression<Func<DtoDeviceDoor, bool>> expression)
        {
            return CacheWrapper.Instance.DeviceDoorCacheManager.Filter(expression.Compile()).ToList();
        }

        #region Internal Method

        internal List<DtoDeviceDoor> GetDeviceDoorList()
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var result = karnamaComponent.FetchAllDeviceDoors();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogControllerDeviceDoorFetch))
            {
                LoggingSystem.LogInfo("Controller device door list fetched", result);
            }
            return result;
        }

        internal DtoDeviceDoor GetDeviceDoorByDoorId(int doorId)
        {
            return CacheWrapper.Instance.DeviceDoorCacheManager.GetCacheItem(doorId.ToString());
        }

        #endregion


        #endregion

        
        #region Device Communication Data

        internal void SaveDeviceCommunicationDataInfo(DtoDeviceCommunicationData entity)
        {
            var deviceInDatabase = GetDeviceCommunicationDataByDeviceNumbers(new List<int> { entity.DeviceNumber }).FirstOrDefault();
            if (deviceInDatabase != null)
            {
                RepositoryFactory.GetDeviceCommunicationDataRepository().Update(entity);
            }
            else
            {
                RepositoryFactory.GetDeviceCommunicationDataRepository().Insert(entity);
            }
        }

        internal List<DtoDeviceCommunicationData> GetDeviceCommunicationDataByDeviceNumbers(List<int> deviceNumbers)
        {
            return RepositoryFactory.GetDeviceCommunicationDataRepository().GetByDeviceNumbers(deviceNumbers);
        }

        #endregion


        #region Printers

        public List<string> GetPrinterNames()
        {
            var result = new List<string>();
            for (var i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                result.Add(PrinterSettings.InstalledPrinters[i]);
            }
            return result;
        }

        #endregion

    }
}
