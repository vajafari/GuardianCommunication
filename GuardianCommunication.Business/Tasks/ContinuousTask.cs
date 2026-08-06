using System;
using System.Threading;

namespace GuardianCommunication.Business.Tasks
{
    public abstract class ContinuousTask
	{
		private readonly Thread _thread;

		protected ContinuousTask(TimeSpan interval)
		{

		}

        protected ContinuousTask()
		{
			_thread = new Thread(Process);
		}

		public abstract void Process();

		public void Start()
		{
			_thread.Start();

		}



		#region Implementation of IDisposable

		private bool _disposed;

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				// Free managed resources
			}
			if (_thread != null && _thread.IsAlive)
			{
				_thread.Abort();
			}
			_disposed = true;
		}

		~ContinuousTask()
		{
			Dispose(false);
		}

		#endregion
	}
}
