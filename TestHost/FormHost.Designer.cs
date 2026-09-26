namespace TestHost
{
	partial class FormHost
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
            DoStopProcess();
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.chkShowMessage = new System.Windows.Forms.CheckBox();
            this.btnHostAll = new System.Windows.Forms.Button();
            this.btnCloseAll = new System.Windows.Forms.Button();
            this.btnCloseHardwareService = new System.Windows.Forms.Button();
            this.btnHostHardwareService = new System.Windows.Forms.Button();
            this.btnRaiseEvent = new System.Windows.Forms.Button();
            this.cmbEvent = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.btnDoStopProcess = new System.Windows.Forms.Button();
            this.tpAttendance = new System.Windows.Forms.TabPage();
            this.btnAttendanceToolsSaveAttendanceByPublisher = new System.Windows.Forms.Button();
            this.txtAttendanceToolsDelay = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsDelay = new System.Windows.Forms.Label();
            this.txtAttendanceToolsAttendanceCount = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsAttendanceCount = new System.Windows.Forms.Label();
            this.txtAttendanceToolsDeviceCount = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsDeviceCount = new System.Windows.Forms.Label();
            this.btnAttendanceToolsSaveAttendanceByComponent = new System.Windows.Forms.Button();
            this.tcMain.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tpAttendance.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkShowMessage
            // 
            this.chkShowMessage.AutoSize = true;
            this.chkShowMessage.Location = new System.Drawing.Point(5, 242);
            this.chkShowMessage.Name = "chkShowMessage";
            this.chkShowMessage.Size = new System.Drawing.Size(99, 17);
            this.chkShowMessage.TabIndex = 21;
            this.chkShowMessage.Text = "Show Message";
            this.chkShowMessage.UseVisualStyleBackColor = true;
            // 
            // btnHostAll
            // 
            this.btnHostAll.Location = new System.Drawing.Point(5, 187);
            this.btnHostAll.Name = "btnHostAll";
            this.btnHostAll.Size = new System.Drawing.Size(141, 44);
            this.btnHostAll.TabIndex = 23;
            this.btnHostAll.Text = "Host All";
            this.btnHostAll.UseVisualStyleBackColor = true;
            this.btnHostAll.Click += new System.EventHandler(this.btnHostAll_Click);
            // 
            // btnCloseAll
            // 
            this.btnCloseAll.BackColor = System.Drawing.Color.IndianRed;
            this.btnCloseAll.Location = new System.Drawing.Point(194, 187);
            this.btnCloseAll.Name = "btnCloseAll";
            this.btnCloseAll.Size = new System.Drawing.Size(141, 44);
            this.btnCloseAll.TabIndex = 22;
            this.btnCloseAll.Text = "Close All";
            this.btnCloseAll.UseVisualStyleBackColor = false;
            this.btnCloseAll.Click += new System.EventHandler(this.btnCloseAll_Click);
            // 
            // btnCloseHardwareService
            // 
            this.btnCloseHardwareService.BackColor = System.Drawing.Color.IndianRed;
            this.btnCloseHardwareService.Location = new System.Drawing.Point(194, 6);
            this.btnCloseHardwareService.Name = "btnCloseHardwareService";
            this.btnCloseHardwareService.Size = new System.Drawing.Size(141, 44);
            this.btnCloseHardwareService.TabIndex = 25;
            this.btnCloseHardwareService.Text = "Close Hardware";
            this.btnCloseHardwareService.UseVisualStyleBackColor = false;
            this.btnCloseHardwareService.Click += new System.EventHandler(this.btnCloseHardwareService_Click);
            // 
            // btnHostHardwareService
            // 
            this.btnHostHardwareService.Location = new System.Drawing.Point(5, 6);
            this.btnHostHardwareService.Name = "btnHostHardwareService";
            this.btnHostHardwareService.Size = new System.Drawing.Size(141, 44);
            this.btnHostHardwareService.TabIndex = 24;
            this.btnHostHardwareService.Text = "Hardware";
            this.btnHostHardwareService.UseVisualStyleBackColor = true;
            this.btnHostHardwareService.Click += new System.EventHandler(this.btnHostHardwareService_Click);
            // 
            // btnRaiseEvent
            // 
            this.btnRaiseEvent.Location = new System.Drawing.Point(5, 71);
            this.btnRaiseEvent.Name = "btnRaiseEvent";
            this.btnRaiseEvent.Size = new System.Drawing.Size(141, 44);
            this.btnRaiseEvent.TabIndex = 26;
            this.btnRaiseEvent.Text = "Raise Event";
            this.btnRaiseEvent.UseVisualStyleBackColor = true;
            this.btnRaiseEvent.Click += new System.EventHandler(this.btnRaiseEvent_Click);
            // 
            // cmbEvent
            // 
            this.cmbEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEvent.FormattingEnabled = true;
            this.cmbEvent.Items.AddRange(new object[] {
            "Suprema SDK 1 Auto Collect",
            "Suprema SDK 2 Auto Collect",
            "Virdi Auto Collect",
            "ZK Auto Collect",
            "Timy Auto Collect",
            "AttendanceHook",
            "AttendanceSendToGuardian",
            "OnlineDevice"});
            this.cmbEvent.Location = new System.Drawing.Point(194, 87);
            this.cmbEvent.Margin = new System.Windows.Forms.Padding(2);
            this.cmbEvent.Name = "cmbEvent";
            this.cmbEvent.Size = new System.Drawing.Size(142, 21);
            this.cmbEvent.TabIndex = 27;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(5, 121);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(141, 44);
            this.btnTest.TabIndex = 28;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabPageMain);
            this.tcMain.Controls.Add(this.tpAttendance);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Margin = new System.Windows.Forms.Padding(2);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(531, 564);
            this.tcMain.TabIndex = 0;
            // 
            // tabPageMain
            // 
            this.tabPageMain.Controls.Add(this.btnDoStopProcess);
            this.tabPageMain.Controls.Add(this.btnHostHardwareService);
            this.tabPageMain.Controls.Add(this.btnTest);
            this.tabPageMain.Controls.Add(this.btnRaiseEvent);
            this.tabPageMain.Controls.Add(this.btnCloseAll);
            this.tabPageMain.Controls.Add(this.chkShowMessage);
            this.tabPageMain.Controls.Add(this.btnHostAll);
            this.tabPageMain.Controls.Add(this.btnCloseHardwareService);
            this.tabPageMain.Controls.Add(this.cmbEvent);
            this.tabPageMain.Location = new System.Drawing.Point(4, 22);
            this.tabPageMain.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageMain.Size = new System.Drawing.Size(523, 538);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = "Main";
            this.tabPageMain.UseVisualStyleBackColor = true;
            // 
            // btnDoStopProcess
            // 
            this.btnDoStopProcess.BackColor = System.Drawing.Color.IndianRed;
            this.btnDoStopProcess.Location = new System.Drawing.Point(194, 137);
            this.btnDoStopProcess.Name = "btnDoStopProcess";
            this.btnDoStopProcess.Size = new System.Drawing.Size(141, 44);
            this.btnDoStopProcess.TabIndex = 29;
            this.btnDoStopProcess.Text = "Do Stop Process";
            this.btnDoStopProcess.UseVisualStyleBackColor = false;
            this.btnDoStopProcess.Click += new System.EventHandler(this.btnDoStopProcess_Click);
            // 
            // tpAttendance
            // 
            this.tpAttendance.Controls.Add(this.btnAttendanceToolsSaveAttendanceByPublisher);
            this.tpAttendance.Controls.Add(this.txtAttendanceToolsDelay);
            this.tpAttendance.Controls.Add(this.lblAttendanceToolsDelay);
            this.tpAttendance.Controls.Add(this.txtAttendanceToolsAttendanceCount);
            this.tpAttendance.Controls.Add(this.lblAttendanceToolsAttendanceCount);
            this.tpAttendance.Controls.Add(this.txtAttendanceToolsDeviceCount);
            this.tpAttendance.Controls.Add(this.lblAttendanceToolsDeviceCount);
            this.tpAttendance.Controls.Add(this.btnAttendanceToolsSaveAttendanceByComponent);
            this.tpAttendance.Location = new System.Drawing.Point(4, 22);
            this.tpAttendance.Name = "tpAttendance";
            this.tpAttendance.Padding = new System.Windows.Forms.Padding(3);
            this.tpAttendance.Size = new System.Drawing.Size(523, 538);
            this.tpAttendance.TabIndex = 4;
            this.tpAttendance.Text = "Attendance";
            this.tpAttendance.UseVisualStyleBackColor = true;
            // 
            // btnAttendanceToolsSaveAttendanceByPublisher
            // 
            this.btnAttendanceToolsSaveAttendanceByPublisher.Location = new System.Drawing.Point(333, 139);
            this.btnAttendanceToolsSaveAttendanceByPublisher.Name = "btnAttendanceToolsSaveAttendanceByPublisher";
            this.btnAttendanceToolsSaveAttendanceByPublisher.Size = new System.Drawing.Size(151, 71);
            this.btnAttendanceToolsSaveAttendanceByPublisher.TabIndex = 8;
            this.btnAttendanceToolsSaveAttendanceByPublisher.Text = "Event Publisher";
            this.btnAttendanceToolsSaveAttendanceByPublisher.UseVisualStyleBackColor = true;
            this.btnAttendanceToolsSaveAttendanceByPublisher.Click += new System.EventHandler(this.btnAttendanceToolsSaveAttendanceByPublisher_Click);
            // 
            // txtAttendanceToolsDelay
            // 
            this.txtAttendanceToolsDelay.Location = new System.Drawing.Point(166, 133);
            this.txtAttendanceToolsDelay.Name = "txtAttendanceToolsDelay";
            this.txtAttendanceToolsDelay.Size = new System.Drawing.Size(100, 20);
            this.txtAttendanceToolsDelay.TabIndex = 6;
            this.txtAttendanceToolsDelay.Text = "50";
            // 
            // lblAttendanceToolsDelay
            // 
            this.lblAttendanceToolsDelay.AutoSize = true;
            this.lblAttendanceToolsDelay.Location = new System.Drawing.Point(70, 137);
            this.lblAttendanceToolsDelay.Name = "lblAttendanceToolsDelay";
            this.lblAttendanceToolsDelay.Size = new System.Drawing.Size(34, 13);
            this.lblAttendanceToolsDelay.TabIndex = 5;
            this.lblAttendanceToolsDelay.Text = "Delay";
            // 
            // txtAttendanceToolsAttendanceCount
            // 
            this.txtAttendanceToolsAttendanceCount.Location = new System.Drawing.Point(166, 95);
            this.txtAttendanceToolsAttendanceCount.Name = "txtAttendanceToolsAttendanceCount";
            this.txtAttendanceToolsAttendanceCount.Size = new System.Drawing.Size(100, 20);
            this.txtAttendanceToolsAttendanceCount.TabIndex = 4;
            this.txtAttendanceToolsAttendanceCount.Text = "2000";
            // 
            // lblAttendanceToolsAttendanceCount
            // 
            this.lblAttendanceToolsAttendanceCount.AutoSize = true;
            this.lblAttendanceToolsAttendanceCount.Location = new System.Drawing.Point(70, 99);
            this.lblAttendanceToolsAttendanceCount.Name = "lblAttendanceToolsAttendanceCount";
            this.lblAttendanceToolsAttendanceCount.Size = new System.Drawing.Size(92, 13);
            this.lblAttendanceToolsAttendanceCount.TabIndex = 3;
            this.lblAttendanceToolsAttendanceCount.Text = "Attendance count";
            // 
            // txtAttendanceToolsDeviceCount
            // 
            this.txtAttendanceToolsDeviceCount.Location = new System.Drawing.Point(166, 62);
            this.txtAttendanceToolsDeviceCount.Name = "txtAttendanceToolsDeviceCount";
            this.txtAttendanceToolsDeviceCount.Size = new System.Drawing.Size(100, 20);
            this.txtAttendanceToolsDeviceCount.TabIndex = 2;
            this.txtAttendanceToolsDeviceCount.Text = "10";
            // 
            // lblAttendanceToolsDeviceCount
            // 
            this.lblAttendanceToolsDeviceCount.AutoSize = true;
            this.lblAttendanceToolsDeviceCount.Location = new System.Drawing.Point(70, 66);
            this.lblAttendanceToolsDeviceCount.Name = "lblAttendanceToolsDeviceCount";
            this.lblAttendanceToolsDeviceCount.Size = new System.Drawing.Size(71, 13);
            this.lblAttendanceToolsDeviceCount.TabIndex = 1;
            this.lblAttendanceToolsDeviceCount.Text = "Device count";
            // 
            // btnAttendanceToolsSaveAttendanceByComponent
            // 
            this.btnAttendanceToolsSaveAttendanceByComponent.Location = new System.Drawing.Point(333, 62);
            this.btnAttendanceToolsSaveAttendanceByComponent.Name = "btnAttendanceToolsSaveAttendanceByComponent";
            this.btnAttendanceToolsSaveAttendanceByComponent.Size = new System.Drawing.Size(151, 71);
            this.btnAttendanceToolsSaveAttendanceByComponent.TabIndex = 0;
            this.btnAttendanceToolsSaveAttendanceByComponent.Text = "Component";
            this.btnAttendanceToolsSaveAttendanceByComponent.UseVisualStyleBackColor = true;
            this.btnAttendanceToolsSaveAttendanceByComponent.Click += new System.EventHandler(this.btnAttendanceToolsSaveAttendanceByComponent_Click);
            // 
            // FormHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 564);
            this.Controls.Add(this.tcMain);
            this.Name = "FormHost";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormTest_Load);
            this.tcMain.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.tpAttendance.ResumeLayout(false);
            this.tpAttendance.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.CheckBox chkShowMessage;
		private System.Windows.Forms.Button btnHostAll;
		private System.Windows.Forms.Button btnCloseAll;
		private System.Windows.Forms.Button btnCloseHardwareService;
		private System.Windows.Forms.Button btnHostHardwareService;
		private System.Windows.Forms.Button btnRaiseEvent;
		private System.Windows.Forms.ComboBox cmbEvent;
		private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.Button btnDoStopProcess;
        private System.Windows.Forms.TabPage tpAttendance;
        private System.Windows.Forms.Label lblAttendanceToolsDeviceCount;
        private System.Windows.Forms.Button btnAttendanceToolsSaveAttendanceByComponent;
        private System.Windows.Forms.TextBox txtAttendanceToolsDeviceCount;
        private System.Windows.Forms.TextBox txtAttendanceToolsAttendanceCount;
        private System.Windows.Forms.Label lblAttendanceToolsAttendanceCount;
        private System.Windows.Forms.TextBox txtAttendanceToolsDelay;
        private System.Windows.Forms.Label lblAttendanceToolsDelay;
        private System.Windows.Forms.Button btnAttendanceToolsSaveAttendanceByPublisher;
    }
}

