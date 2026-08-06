using System;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Camera.PouyaFanavaran;

namespace GuardianCommunication.Business.Tasks
{
    public class KarabinAccessListTask : TimedBaseTask
    {


        private readonly KarnamaComponent _karnamaComponent;
        private readonly RepositoryFactory _repositoryFactory = new RepositoryFactory();

        public KarabinAccessListTask(TimeSpan interval) : base(interval)
        {
            LoggingSystem.LogInfo($"KarabinAccessListTask started with interval {(int)interval.TotalSeconds}");
            _karnamaComponent = new KarnamaComponent(_repositoryFactory);
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {

            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
            {
                LoggingSystem.LogInfo("KarabinValidListTask process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {
                    var agents = PouyaFanavaranServer.Instance.GetKarabinCameraAgentList();
                    foreach (var agent in agents)
                    {
                        try
                        {
                            if (!agent.Camera.CameraSettings.HasFlag(CameraSettingEnumeration.SendValidList))
                            {
                                continue;
                            }
                            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                            {
                                LoggingSystem.LogInfo("KarabinAccessListTask is calling for karabin device", agent.Camera);
                            }

                            var accessList = _karnamaComponent.GetCameraCarAccessData(agent.Camera.Id, DateTime.Now, DateTime.Now);
                            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                            {
                                LoggingSystem.LogInfo("KarabinAccessListTask valid list result", new
                                {
                                    agent.Camera,
                                    AccessList = accessList
                                });
                            }
                            agent.SendAccessFiles(accessList);
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "KarabinAccessListTask Error on auto collect karabin camera", agent.Camera);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lockCollect);
                }
            }
            else
            {
                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinSendValidList))
                {
                    LoggingSystem.LogInfo("KarabinAccessListTask skipped because of lock is taken");
                }
            }
        }

    }
}
