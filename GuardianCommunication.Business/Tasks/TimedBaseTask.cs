using System;
using System.Timers;

namespace GuardianCommunication.Business.Tasks
{
    public abstract class TimedBaseTask: IDisposable
    {
        private Timer _timer;

        protected TimedBaseTask(TimeSpan interval)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Process();
        }

        public abstract void Process();

        public void Start()
        {
            _timer.Start();
        }

		public void Stop()
		{
			_timer.Stop();
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
			if (_timer != null)
			{
				_timer.Stop();
				_timer.Elapsed -= Timer_Elapsed;
				_timer.Dispose();
				_timer = null;
			}
			_disposed = true;
		}

		~TimedBaseTask()
		{
			Dispose(false);
		}

		#endregion

	}
}
