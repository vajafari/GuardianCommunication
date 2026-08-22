using System;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Tasks
{
    public class AttendanceSendToGuardianTask : TimedBaseTask
    {
        private readonly AttendanceComponent _attendanceComponent;
        public AttendanceSendToGuardianTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _attendanceComponent = new AttendanceComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AttendanceSendToGuardianTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockAttendanceHook = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceSendToGuardianTask))
            {
                LoggingSystem.LogInfo("AttendanceSendToGuardianTask process is calling");
            }
            if (Monitor.TryEnter(_lockAttendanceHook))
            {
                try
                {
                    try
                    {
                        _attendanceComponent.ResendUnsentAttendancesToGuardian();
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
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.AttendanceSendToGuardianTask))
                {
                    LoggingSystem.LogInfo("AttendanceSendToGuardianTask skipped because of lock is taken");
                }
            }
        }
    }
}
