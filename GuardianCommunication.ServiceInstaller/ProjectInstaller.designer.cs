namespace GuardianCommunication.ServiceInstaller
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.hardwareCommunicationServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.HardwareCommunicationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// hardwareCommunicationServiceProcessInstaller
			// 
			this.hardwareCommunicationServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.hardwareCommunicationServiceProcessInstaller.Password = null;
			this.hardwareCommunicationServiceProcessInstaller.Username = null;
			// 
			// HardwareCommunicationServiceInstaller
			// 
			this.HardwareCommunicationServiceInstaller.ServiceName = "HardwareCommunicationService";
			this.HardwareCommunicationServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.hardwareCommunicationServiceProcessInstaller,
            this.HardwareCommunicationServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller hardwareCommunicationServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller HardwareCommunicationServiceInstaller;
	}
}