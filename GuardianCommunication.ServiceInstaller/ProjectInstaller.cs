using System.ComponentModel;

namespace GuardianCommunication.ServiceInstaller
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}
		
	}
}
