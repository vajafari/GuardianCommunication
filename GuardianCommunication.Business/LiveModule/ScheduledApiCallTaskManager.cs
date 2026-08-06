using System;
using CronNET;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.LiveModule
{
    public class ScheduledApiCallTaskManager
    {
        /*
            *    *    *    *    *  
            ┬    ┬    ┬    ┬    ┬
            │    │    │    │    │
            │    │    │    │    │
            │    │    │    │    └───── day of week (0 - 6) (Sunday=0 )
            │    │    │    └────────── month (1 - 12)
            │    │    └─────────────── day of month (1 - 31)
            │    └──────────────────── hour (0 - 23)
            └───────────────────────── min (0 - 59)
        */

        private readonly RepositoryFactory _repositoryFactory = new RepositoryFactory();
        private readonly CronDaemon _cronDaemon = new CronDaemon();

        #region Singleton

        public static ScheduledApiCallTaskManager Instance { get; }

        private ScheduledApiCallTaskManager()
        {
        }

        static ScheduledApiCallTaskManager()
        {
            Instance = new ScheduledApiCallTaskManager();
        }

        #endregion


        public void StartScheduledApiCallTask()
        {
            var component = new ScheduledApiCallTaskComponent(_repositoryFactory);
            var allScheduledApiCallTask = component.Search(null);
            if (allScheduledApiCallTask.IsCollectionNotNullOrEmpty())
            {
                foreach (var scheduledApiCallTask in allScheduledApiCallTask)
                {
                    var agent = new ScheduledApiCallAgent(scheduledApiCallTask);
                    _cronDaemon.AddJob(scheduledApiCallTask.Cron, agent.CallApi);
                }
                _cronDaemon.Start();
            }
        }

        private void StopScheduledApiCallTask()
        {
            _cronDaemon.Stop();
        }



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
                // Free managed resources
            }
            StopScheduledApiCallTask();
            _disposed = true;

        }

        ~ScheduledApiCallTaskManager()
        {
            Dispose(false);
        }

        #endregion

    }
}
