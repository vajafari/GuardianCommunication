using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Camera.PouyaFanavaran;

namespace GuardianCommunication.Business.Tasks
{
    public class KarabinAutoCollectTask : TimedBaseTask
    {

        private readonly CameraComponent _cameraComponent;
        private readonly KarnamaComponent _karnamaComponent;
        private readonly RepositoryFactory _repositoryFactory = new RepositoryFactory();

        public KarabinAutoCollectTask(TimeSpan interval) : base(interval)
        {
            _cameraComponent = new CameraComponent(_repositoryFactory);
            _karnamaComponent = new KarnamaComponent(_repositoryFactory);
            LoggingSystem.LogInfo($"KarabinAutoCollectTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAutoCollect))
            {
                LoggingSystem.LogInfo("KarabinAutoCollectTask process is calling");
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
                            if (!agent.Camera.CameraSettings.HasFlag(CameraSettingEnumeration.AutoCollectActive))
                            {
                                continue;
                            }
                            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAutoCollect))
                            {
                                LoggingSystem.LogInfo("KarabinAutoCollectTask is calling for karabin device", agent.Camera);
                            }
                            var communicationData = _cameraComponent.GetCameraCommunicationDataByCameraIds(new List<int> { agent.Camera.Id }).FirstOrDefault();
                            var lastReadDateTime = DateTime.Now.Date;
                            if (communicationData?.LastAttendanceLogDateTime != null)
                            {
                                lastReadDateTime = communicationData.LastAttendanceLogDateTime.Value;
                            }
                            var allAttendances = agent.ReadCameraAttendance(lastReadDateTime, DateTime.Now);
                            if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAutoCollect))
                            {
                                LoggingSystem.LogInfo("KarabinAutoCollectTask Attendance result for  karabin device",
                                    allAttendances.Select(a => new { a.PlateString, a.AttendanceDateTime, a.PlateType, a.IdOnCamera, a.AcceptType }));
                            }

                            if (allAttendances.IsCollectionNotNullOrEmpty())
                            {
                                foreach (var attendance in allAttendances)
                                {
                                    try
                                    {
                                        switch (attendance.AcceptType)
                                        {
                                            case PlateRecognitionAcceptType.AcceptedVip:
                                            case PlateRecognitionAcceptType.Accepted:
                                                _karnamaComponent.SubmitNotLiveValidPlateDetectionCameraAttendance(attendance);
                                                break;
                                            case PlateRecognitionAcceptType.None:
                                            case PlateRecognitionAcceptType.Rejected:
                                            case PlateRecognitionAcceptType.Blacklist:
                                            default:
                                                _karnamaComponent.SubmitNotLiveInvalidPlateDetectionCameraAttendance(attendance);
                                                break;
                                        }
                                    }
                                    catch (Exception exp)
                                    {
                                        LoggingSystem.LogError(exp, "KarabinAutoCollectTask Error on save attendance karabin camera", new { Camera = agent.Camera, Attendance = attendance });
                                    }
                                }
                                _cameraComponent.SaveCameraCommunicationDataInfo(new DtoCameraCommunicationData()
                                {
                                    CameraId = agent.Camera.Id,
                                    LastAttendanceLogDateTime = allAttendances.Max(att => att.AttendanceDateTime)
                                });
                                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAutoCollect))
                                {
                                    LoggingSystem.LogInfo("KarabinAutoCollectTask Attendance communication data saved for karabin", new
                                    {
                                        CameraId = agent.Camera.Id,
                                        LastAttendanceLogDateTime = allAttendances.Max(att => att.AttendanceDateTime)
                                    });
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "KarabinAutoCollectTask Error on auto collect karabin camera", agent.Camera);
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
                if (AppConfigs.LogLevelCamera.HasFlag(LogLevelCameraEnumeration.KarabinAutoCollect))
                {
                    LoggingSystem.LogInfo("KarabinAutoCollectTask skipped because of lock is taken");
                }
            }
        }

    }
}
