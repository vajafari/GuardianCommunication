using System.ServiceProcess;

namespace GuardianCommunication.ServiceInstaller
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] servicesToRun = { new AccessControlProjectServices() };
			ServiceBase.Run(servicesToRun);
		}
	}
}
