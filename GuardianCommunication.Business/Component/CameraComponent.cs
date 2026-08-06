using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Camera.PouyaFanavaran;

namespace GuardianCommunication.Business.Component
{
    public class CameraComponent : BaseComponent
    {

        public CameraComponent(RepositoryFactory repositoryFactory) : base(repositoryFactory)
        { }

        #region Camera

        public void ResetCameraCacheAndSetConnectionModes()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogCameraCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Start of reset Camera gate cache");
            }
            var allActiveCameras = FetchAllActiveCameras();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogCameraCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Camera Gate list on reset cache", allActiveCameras);
            }
            CacheWrapper.Instance.SetCameraCache(allActiveCameras);
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogCameraCacheProcessFetch))
            {
                LoggingSystem.LogInfo("Set new Camera gates cache");
            }
            if (allActiveCameras.IsCollectionNotNullOrEmpty())
            {
                if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.PouyaFanavaran))
                {
                    try
                    {
                        PouyaFanavaranServer.Instance.SetKarabinCameraList(allActiveCameras);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on PouyaFanavaranServer.Instance.SetKarabinCameraList");
                    }
                }
            }
            //if (allActiveCameras.IsCollectionNotNullOrEmpty())
            //{
            //    PadisCameraServer.Instance.SetDeviceOnPushModeList(allActiveCameras);
            //    if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.GeneralLog))
            //    {
            //        LoggingSystem.LogInfo("Set new device list for PadisCameraServer.SetDeviceOnPushModeList");
            //    }
            //}
        }

        public List<DtoCamera> SearchCameraCache(Expression<Func<DtoCamera, bool>> expression)
        {
            return CacheWrapper.Instance.CameraCacheManager.Filter(expression.Compile()).ToList();
        }

        internal List<DtoCamera> FetchAllActiveCameras()
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var result = karnamaComponent.FetchAllCameras();
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogCameraFetch))
            {
                LoggingSystem.LogInfo("Camera list fetched", result);
            }

            return result;
        }

        #endregion



        #region Camera Communication Data

        internal void SaveCameraCommunicationDataInfo(DtoCameraCommunicationData entity)
        {
            RepositoryFactory.GetCameraCommunicationDataRepository().Upsert(entity);
        }

        internal List<DtoCameraCommunicationData> GetCameraCommunicationDataByCameraIds(List<int> cameraIds)
        {
            return RepositoryFactory.GetCameraCommunicationDataRepository().GetByCameraIds(cameraIds);
        }

        #endregion




    }
}
