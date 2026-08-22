using System;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Tasks
{
    public class AttendanceHookTask : TimedBaseTask
    {
        private readonly AttendanceComponent _attendanceComponent;
        public AttendanceHookTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _attendanceComponent = new AttendanceComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AttendanceHookTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockAttendanceHook = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
            {
                LoggingSystem.LogInfo("AttendanceHookTask Process is calling");
            }
            if (Monitor.TryEnter(_lockAttendanceHook))
            {
                try
                {
                    try
                    {
                        _attendanceComponent.HookUnsentAttendances();
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
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceHookTask))
                {
                    LoggingSystem.LogInfo("AttendanceHookTask skipped because of lock is taken");
                }
            }
        }
    }
}
