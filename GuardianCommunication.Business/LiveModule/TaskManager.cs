using System;
using System.Collections.Generic;
using GuardianCommunication.Business.Tasks;

namespace GuardianCommunication.Business.LiveModule
{
	public class TaskManager
	{
        private readonly List<TimedBaseTask> _timedBasedTasks = new List<TimedBaseTask>();
        private readonly List<ContinuousTask> _continuousTasks = new List<ContinuousTask>();

		#region Singleton

		public static TaskManager Instance { get; }

		private TaskManager()
		{
		}

		static TaskManager()
		{
			Instance = new TaskManager();
		}

		#endregion


		public void SetTimedBaseTaskList(params TimedBaseTask[] tasks)
		{
			if (tasks.IsCollectionNotNullOrEmpty())
			{
				_timedBasedTasks.AddRange(tasks);
				foreach (var item in tasks)
				{
					item.Start();
				}
			}
		}

		public void SetContinuousTasksList(params ContinuousTask[] tasks)
		{
			if (tasks.IsCollectionNotNullOrEmpty())
			{
				_continuousTasks.AddRange(tasks);
				foreach (var item in tasks)
				{
					item.Start();
				}
			}
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
            foreach (var task in _continuousTasks)
            {
				task.Dispose();
            }
			foreach (var task in _timedBasedTasks)
			{
				task.Dispose();
			}
			_timedBasedTasks.Clear();
			_continuousTasks.Clear();
			_disposed = true;
		}

		~TaskManager()
		{
			Dispose(false);
		}

		#endregion


	}
}
