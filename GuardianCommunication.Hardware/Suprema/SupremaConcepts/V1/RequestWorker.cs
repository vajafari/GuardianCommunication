using System;
using System.Collections;

namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1
{

	public class RequestWorker
	{
		Queue _requestQueue;
		object _requestLock;
		public void DoWork()
		{
			_requestQueue = new Queue();
			_requestLock = new object();
			while (!_shouldStop)
			{
				RequestToken token = null;
				var found = 0;
				lock (_requestLock)
				{
					if (_requestQueue.Count > 0)
					{
						var temp = _requestQueue.Dequeue();
						token = temp as RequestToken;
						found = 1;
					}
				}

				if (found == 1)
				{
					System.Threading.Thread.Sleep(1000);
					BSSDK.BS_StartRequest(token.Handle, token.DeviceType, token.Port);
					Console.WriteLine(token.Handle);
				}
				System.Threading.Thread.Sleep(10);
			}
		}

		public void RequestStop()
		{
			_shouldStop = true;
		}

		public void AddRequest(int handle, int type, int port)
		{
			var token = new RequestToken {Handle = handle, DeviceType = type, Port = port};
			lock (_requestLock)
			{
				_requestQueue.Enqueue(token);
			}
		}
		private volatile bool _shouldStop;
	}

}
