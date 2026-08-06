using System;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class AttendanceSendToKarnamaTask : TimedBaseTask
    {
        private readonly AttendanceComponent _attendanceComponent;
        public AttendanceSendToKarnamaTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _attendanceComponent = new AttendanceComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AttendanceSendToKarnamaTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockAttendanceHook = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceSendToKarnamaTask))
            {
                LoggingSystem.LogInfo("AttendanceSendToKarnamaTask process is calling");
            }
            if (Monitor.TryEnter(_lockAttendanceHook))
            {
                try
                {
                    try
                    {
                        _attendanceComponent.ResendUnsentAttendancesToKarnama();
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp);
                    }
                }
                finally
                {
                    Monitor.Exit(_lockAttendanceHook);
                }

            }
            else
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceSendToKarnamaTask))
                {
                    LoggingSystem.LogInfo("AttendanceSendToKarnamaTask skipped because of lock is taken");
                }
            }
        }
    }
}
