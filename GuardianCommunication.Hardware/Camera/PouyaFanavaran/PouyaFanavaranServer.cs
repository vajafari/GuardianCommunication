using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using GuardianCommunication.Data.Logger;

namespace GuardianCommunication.Hardware.Camera.PouyaFanavaran
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    public class PouyaFanavaranServer : IDisposable
    {
        private KarabinAgentConfig _agentConfig;

        #region Singleton

        public static PouyaFanavaranServer Instance { get; }

        private PouyaFanavaranServer()
        {
        }

        static PouyaFanavaranServer()
        {
            Instance = new PouyaFanavaranServer();
        }

        #endregion


        public void StartPouyaFanavaranServerServer(
            KarabinAgentConfig agentConfig
            , List<DtoCamera> cameraInfos)
        {
            LoggingSystem.LogInfo("PouyaFanavaranServer started", new
            {
                OnlineMonitoringAgentConfig = agentConfig,
            });
            _agentConfig = agentConfig;
            var thread = new Thread(() => DoStartServerProcess(cameraInfos)) { IsBackground = true };
            thread.Start();
        }

        private void DoStartServerProcess(List<DtoCamera> deviceInfos)
        {
            SetKarabinCameraList(deviceInfos);
        }

        public void StopServer()
        {
            Dispose(true);
        }


        #region Karabin Camera

        private readonly List<DtoCamera> _karabinCameras = new List<DtoCamera>();
        private readonly List<KarabinAgent> _karabinCameraAgents = new List<KarabinAgent>();

        public void SetKarabinCameraList(List<DtoCamera> cameraInfos)
        {
            if (cameraInfos == null)
            {
                cameraInfos = new List<DtoCamera>();
            }
            var controllerBaseCameraInfos = cameraInfos.Where
                (row => row.CameraTask == CameraTaskEnumeration.PlateDetection
                        && row.CameraType == CameraTypeEnumeration.PouyaFanavaranKarabin).ToList();
            LogData("SetKarabinCamerasList on PouyaFanavaranServer", controllerBaseCameraInfos);

            lock (_karabinCameraAgents)
            {

                var agentsForRemove = new List<KarabinAgent>();
                foreach (var agent in _karabinCameraAgents)
                {
                    if (controllerBaseCameraInfos.All(row => row.Id != agent.Camera.Id))
                    {
                        agentsForRemove.Add(agent);
                    }
                }

                if (agentsForRemove.Any())
                {
                    foreach (var agent in agentsForRemove)
                    {
                        try
                        {
                            agent.Dispose();
                            _karabinCameraAgents.Remove(agent);

                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp
                                , "Error on removing camera from karabin agent"
                                , agent.Camera);
                        }
                    }
                }

                foreach (var camera in controllerBaseCameraInfos)
                {
                    try
                    {
                        if (_karabinCameraAgents.All(row => row.Camera.Id != camera.Id))
                        {
                            _karabinCameraAgents.Add(new KarabinAgent(_agentConfig, camera));
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp
                            , "Error on adding camera to karabin agent"
                            , camera);
                    }
                }

            }



            lock (_karabinCameras)
            {
                try
                {
                    _karabinCameras.Clear();
                    _karabinCameras.AddRange(controllerBaseCameraInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp
                        , "Error on SetControllerBasePlateDetectionCameraList");
                }
            }

        }

        public List<KarabinAgent> GetKarabinCameraAgentList()
        {
            lock (_karabinCameraAgents)
            {
                // Return a snapshot copy, never the live list (callers enumerate off-lock).
                return _karabinCameraAgents.ToList();
            }
        }

        #endregion


        #region Private methods

        private static void LogData(string title, dynamic dataToLog)
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinServer))
            {
                LoggingSystem.LogInfo(title, new
                {
                    DataToLog = dataToLog,
                });
            }
        }

        #endregion


        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Managed teardown only on explicit Dispose — never on the finalizer thread.
                lock (_karabinCameraAgents)
                {
                    foreach (var agent in _karabinCameraAgents)
                    {
                        agent.Dispose();
                    }
                }
            }

            _disposed = true;
        }

        #endregion


    }

    // ReSharper disable UnusedMember.Global
    internal enum BioType
    {
        Comm = 0,
        FingerPrint = 1,
        Face = 2,
        VocalPrint = 3,
        Iris = 4,
        Retina = 5,
        PalmPrint = 6,
        FingerVein = 7,
        Palm = 8,
        VisibleLightFace = 9
    }
    // ReSharper restore UnusedMember.Global


}
