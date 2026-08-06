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
            this.tpCamera = new System.Windows.Forms.TabPage();
            this.pbCameraLastImage = new System.Windows.Forms.PictureBox();
            this.btnShowLatestImage = new System.Windows.Forms.Button();
            this.lblCamera = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tpSelfTools = new System.Windows.Forms.TabPage();
            this.gbSelfBillInfoPrint = new System.Windows.Forms.GroupBox();
            this.chkSelfPrintInfoClearTitle3 = new System.Windows.Forms.CheckBox();
            this.chkSelfPrintInfoClearTitle4 = new System.Windows.Forms.CheckBox();
            this.chkSelfPrintInfoClearFood2 = new System.Windows.Forms.CheckBox();
            this.chkSelfPrintInfoClearFood3 = new System.Windows.Forms.CheckBox();
            this.chkSelfPrintInfoClearManual = new System.Windows.Forms.CheckBox();
            this.chkSelfPrintInfoClearTitle2 = new System.Windows.Forms.CheckBox();
            this.btnSelfPrintInfoClear = new System.Windows.Forms.Button();
            this.txtSelfPrintInfoFoodSeparator = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodSeparator = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSelfPrintInfoHeaderTitle1 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoHeaderTitle1 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoHeaderTitle2 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoHeaderTitle2 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoHeaderTitle3 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoHeaderTitle4 = new System.Windows.Forms.TextBox();
            this.txtSelfPrintInfoHeaderTitle3 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoHeaderTitle4 = new System.Windows.Forms.Label();
            this.gbSelfPrintInfoFood3 = new System.Windows.Forms.GroupBox();
            this.lblSelfPrintInfoFoodType3 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodTitle3 = new System.Windows.Forms.TextBox();
            this.txtSelfPrintInfoFoodType3 = new System.Windows.Forms.TextBox();
            this.chkSelfPrintInfoFoodIsAllowed3 = new System.Windows.Forms.CheckBox();
            this.lblSelfPrintInfoFishCount3 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFishCount3 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodDescription3 = new System.Windows.Forms.Label();
            this.lblSelfPrintInfoFoodPrice3 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodDescription3 = new System.Windows.Forms.TextBox();
            this.txtSelfPrintInfoFoodPrice3 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodTitle3 = new System.Windows.Forms.Label();
            this.gbSelfPrintInfoFood2 = new System.Windows.Forms.GroupBox();
            this.lblSelfPrintInfoFoodType2 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodTitle2 = new System.Windows.Forms.TextBox();
            this.chkSelfPrintInfoFoodIsAllowed2 = new System.Windows.Forms.CheckBox();
            this.txtSelfPrintInfoFoodType2 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFishCount2 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFishCount2 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodPrice2 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodPrice2 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodTitle2 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodDescription2 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodDescription2 = new System.Windows.Forms.Label();
            this.gbSelfPrintInfoFood1 = new System.Windows.Forms.GroupBox();
            this.lblSelfPrintInfoFoodType1 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodTitle1 = new System.Windows.Forms.TextBox();
            this.txtSelfPrintInfoFoodType1 = new System.Windows.Forms.TextBox();
            this.chkSelfPrintInfoFoodIsAllowed1 = new System.Windows.Forms.CheckBox();
            this.lblSelfPrintInfoFishCount1 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFishCount1 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodPrice1 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodPrice1 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodTitle1 = new System.Windows.Forms.Label();
            this.txtSelfPrintInfoFoodDescription1 = new System.Windows.Forms.TextBox();
            this.lblSelfPrintInfoFoodDescription1 = new System.Windows.Forms.Label();
            this.chkSelfPrintInfoDirect = new System.Windows.Forms.CheckBox();
            this.txtEmployeeTitle = new System.Windows.Forms.TextBox();
            this.lblEmployeeTitle = new System.Windows.Forms.Label();
            this.txtPrinterIp = new System.Windows.Forms.TextBox();
            this.lvlPrinterIp = new System.Windows.Forms.Label();
            this.txtPrinterName = new System.Windows.Forms.TextBox();
            this.lblPrinterName = new System.Windows.Forms.Label();
            this.txtFishNumber = new System.Windows.Forms.TextBox();
            this.lblFishNumber = new System.Windows.Forms.Label();
            this.txtManualFishString = new System.Windows.Forms.TextBox();
            this.lblManualFishString = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.gbSelfToolsFoodTitles = new System.Windows.Forms.GroupBox();
            this.lblSendFoodTitles = new System.Windows.Forms.Label();
            this.btnDisableFoodTitles = new System.Windows.Forms.Button();
            this.btnSendFoodTitles = new System.Windows.Forms.Button();
            this.txtSendFoodTitlesDeviceNumber = new System.Windows.Forms.TextBox();
            this.txtSendFoodTitles = new System.Windows.Forms.TextBox();
            this.tpAttendance = new System.Windows.Forms.TabPage();
            this.btnAttendanceToolsSaveAttendanceByPublisher = new System.Windows.Forms.Button();
            this.txtAttendanceToolsDelay = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsDelay = new System.Windows.Forms.Label();
            this.txtAttendanceToolsAttendanceCount = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsAttendanceCount = new System.Windows.Forms.Label();
            this.txtAttendanceToolsDeviceCount = new System.Windows.Forms.TextBox();
            this.lblAttendanceToolsDeviceCount = new System.Windows.Forms.Label();
            this.btnAttendanceToolsSaveAttendanceByComponent = new System.Windows.Forms.Button();
            this.tpTimyAccess = new System.Windows.Forms.TabPage();
            this.tcTimyInner = new System.Windows.Forms.TabControl();
            this.tpTimyDay = new System.Windows.Forms.TabPage();
            this.pnlTimyDayRight = new System.Windows.Forms.Panel();
            this.dgvTimyDayTimezones = new System.Windows.Forms.DataGridView();
            this.colDayTimezoneId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDayStartHourMinute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDayEndHourMinute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTimyDayHeader = new System.Windows.Forms.Panel();
            this.lblTimyDayDeviceIndex = new System.Windows.Forms.Label();
            this.numTimyDayDeviceIndex = new System.Windows.Forms.NumericUpDown();
            this.lblTimyDayTitle = new System.Windows.Forms.Label();
            this.txtTimyDayTitle = new System.Windows.Forms.TextBox();
            this.pnlTimyDayLeft = new System.Windows.Forms.Panel();
            this.lstTimyDayGroups = new System.Windows.Forms.ListBox();
            this.pnlTimyDayButtons = new System.Windows.Forms.Panel();
            this.btnTimyDayAdd = new System.Windows.Forms.Button();
            this.btnTimyDayRemove = new System.Windows.Forms.Button();
            this.tpTimyWeek = new System.Windows.Forms.TabPage();
            this.pnlTimyWeekRight = new System.Windows.Forms.Panel();
            this.dgvTimyWeekTimezones = new System.Windows.Forms.DataGridView();
            this.colWeekDay = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colWeekDayTimezoneIndex = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.pnlTimyWeekHeader = new System.Windows.Forms.Panel();
            this.lblTimyWeekDeviceIndex = new System.Windows.Forms.Label();
            this.numTimyWeekDeviceIndex = new System.Windows.Forms.NumericUpDown();
            this.lblTimyWeekTitle = new System.Windows.Forms.Label();
            this.txtTimyWeekTitle = new System.Windows.Forms.TextBox();
            this.pnlTimyWeekLeft = new System.Windows.Forms.Panel();
            this.lstTimyWeekGroups = new System.Windows.Forms.ListBox();
            this.pnlTimyWeekButtons = new System.Windows.Forms.Panel();
            this.btnTimyWeekAdd = new System.Windows.Forms.Button();
            this.btnTimyWeekRemove = new System.Windows.Forms.Button();
            this.tpTimySetForUser = new System.Windows.Forms.TabPage();
            this.dtUserEndTime = new System.Windows.Forms.DateTimePicker();
            this.lblUserEndTime = new System.Windows.Forms.Label();
            this.dtUserStartTime = new System.Windows.Forms.DateTimePicker();
            this.lblUserStartTime = new System.Windows.Forms.Label();
            this.cmbUserWeekzone = new System.Windows.Forms.ComboBox();
            this.lblUserWeekzone = new System.Windows.Forms.Label();
            this.txtEnrollId = new System.Windows.Forms.TextBox();
            this.lblEnrollId = new System.Windows.Forms.Label();
            this.tpTimyHoliday = new System.Windows.Forms.TabPage();
            this.pnlTimyHolidayRight = new System.Windows.Forms.Panel();
            this.cmbTimyHolidayDayTimezone = new System.Windows.Forms.ComboBox();
            this.lblTimyHolidayDayTimezone = new System.Windows.Forms.Label();
            this.dtTimyHolidayEndDate = new System.Windows.Forms.DateTimePicker();
            this.lblTimyHolidayEndDate = new System.Windows.Forms.Label();
            this.dtTimyHolidayStartDate = new System.Windows.Forms.DateTimePicker();
            this.lblTimyHolidayStartDate = new System.Windows.Forms.Label();
            this.txtTimyHolidayTitle = new System.Windows.Forms.TextBox();
            this.lblTimyHolidayTitle = new System.Windows.Forms.Label();
            this.pnlTimyHolidayLeft = new System.Windows.Forms.Panel();
            this.lstTimyHolidays = new System.Windows.Forms.ListBox();
            this.pnlTimyHolidayButtons = new System.Windows.Forms.Panel();
            this.btnTimyHolidayAdd = new System.Windows.Forms.Button();
            this.btnTimyHolidayRemove = new System.Windows.Forms.Button();
            this.tpTimyJsonOutput = new System.Windows.Forms.TabPage();
            this.txtTimyJson = new System.Windows.Forms.TextBox();
            this.pnlTimyBottom = new System.Windows.Forms.Panel();
            this.btnTimySetPersTimezone = new System.Windows.Forms.Button();
            this.btnAddWeekTimezone = new System.Windows.Forms.Button();
            this.lblTimyDeviceNumber = new System.Windows.Forms.Label();
            this.txtTimyDeviceNumber = new System.Windows.Forms.TextBox();
            this.btnAddDayTimezone = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnTimyGenerateJson = new System.Windows.Forms.Button();
            this.btnTimyHoliday = new System.Windows.Forms.Button();
            this.tcMain.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tpCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCameraLastImage)).BeginInit();
            this.tpSelfTools.SuspendLayout();
            this.gbSelfBillInfoPrint.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSelfPrintInfoFood3.SuspendLayout();
            this.gbSelfPrintInfoFood2.SuspendLayout();
            this.gbSelfPrintInfoFood1.SuspendLayout();
            this.gbSelfToolsFoodTitles.SuspendLayout();
            this.tpAttendance.SuspendLayout();
            this.tpTimyAccess.SuspendLayout();
            this.tcTimyInner.SuspendLayout();
            this.tpTimyDay.SuspendLayout();
            this.pnlTimyDayRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimyDayTimezones)).BeginInit();
            this.pnlTimyDayHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimyDayDeviceIndex)).BeginInit();
            this.pnlTimyDayLeft.SuspendLayout();
            this.pnlTimyDayButtons.SuspendLayout();
            this.tpTimyWeek.SuspendLayout();
            this.pnlTimyWeekRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimyWeekTimezones)).BeginInit();
            this.pnlTimyWeekHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimyWeekDeviceIndex)).BeginInit();
            this.pnlTimyWeekLeft.SuspendLayout();
            this.pnlTimyWeekButtons.SuspendLayout();
            this.tpTimySetForUser.SuspendLayout();
            this.tpTimyHoliday.SuspendLayout();
            this.pnlTimyHolidayRight.SuspendLayout();
            this.pnlTimyHolidayLeft.SuspendLayout();
            this.pnlTimyHolidayButtons.SuspendLayout();
            this.tpTimyJsonOutput.SuspendLayout();
            this.pnlTimyBottom.SuspendLayout();
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
            "PW Auto Collect",
            "ZK Auto Collect",
            "Timy Auto Collect",
            "ElmoSanat Auto Collect ",
            "AttendanceHook",
            "AttendanceSendToKarnama",
            "OnlineDevice",
            "RaiseHardwareEventLog",
            "Suprema SDK 1 Check Connection",
            "Send Meals",
            "Take Image",
            "Send Valid List To Karabin Cemera",
            "Auto Collect Karabin Cemera",
            "Delete Failed Command",
            "AutoCollectPadisController"});
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
            this.tcMain.Controls.Add(this.tpCamera);
            this.tcMain.Controls.Add(this.tpSelfTools);
            this.tcMain.Controls.Add(this.tpAttendance);
            this.tcMain.Controls.Add(this.tpTimyAccess);
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
            // tpCamera
            // 
            this.tpCamera.Controls.Add(this.pbCameraLastImage);
            this.tpCamera.Controls.Add(this.btnShowLatestImage);
            this.tpCamera.Controls.Add(this.lblCamera);
            this.tpCamera.Controls.Add(this.comboBox1);
            this.tpCamera.Location = new System.Drawing.Point(4, 22);
            this.tpCamera.Name = "tpCamera";
            this.tpCamera.Padding = new System.Windows.Forms.Padding(3);
            this.tpCamera.Size = new System.Drawing.Size(523, 538);
            this.tpCamera.TabIndex = 2;
            this.tpCamera.Text = "Camera";
            this.tpCamera.UseVisualStyleBackColor = true;
            // 
            // pbCameraLastImage
            // 
            this.pbCameraLastImage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbCameraLastImage.Location = new System.Drawing.Point(3, 313);
            this.pbCameraLastImage.Name = "pbCameraLastImage";
            this.pbCameraLastImage.Size = new System.Drawing.Size(517, 222);
            this.pbCameraLastImage.TabIndex = 42;
            this.pbCameraLastImage.TabStop = false;
            // 
            // btnShowLatestImage
            // 
            this.btnShowLatestImage.Location = new System.Drawing.Point(216, 3);
            this.btnShowLatestImage.Name = "btnShowLatestImage";
            this.btnShowLatestImage.Size = new System.Drawing.Size(120, 23);
            this.btnShowLatestImage.TabIndex = 41;
            this.btnShowLatestImage.Text = "Show last image";
            this.btnShowLatestImage.UseVisualStyleBackColor = true;
            this.btnShowLatestImage.Click += new System.EventHandler(this.btnShowLatestImage_Click);
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.Location = new System.Drawing.Point(7, 8);
            this.lblCamera.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(46, 13);
            this.lblCamera.TabIndex = 38;
            this.lblCamera.Text = "Camera:";
            // 
            // comboBox1
            // 
            this.comboBox1.DisplayMember = "Title";
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(57, 4);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(153, 21);
            this.comboBox1.TabIndex = 28;
            this.comboBox1.ValueMember = "Id";
            // 
            // tpSelfTools
            // 
            this.tpSelfTools.Controls.Add(this.gbSelfBillInfoPrint);
            this.tpSelfTools.Controls.Add(this.gbSelfToolsFoodTitles);
            this.tpSelfTools.Location = new System.Drawing.Point(4, 22);
            this.tpSelfTools.Name = "tpSelfTools";
            this.tpSelfTools.Padding = new System.Windows.Forms.Padding(3);
            this.tpSelfTools.Size = new System.Drawing.Size(523, 538);
            this.tpSelfTools.TabIndex = 3;
            this.tpSelfTools.Text = "Self Tools";
            this.tpSelfTools.UseVisualStyleBackColor = true;
            // 
            // gbSelfBillInfoPrint
            // 
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearTitle3);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearTitle4);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearFood2);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearFood3);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearManual);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoClearTitle2);
            this.gbSelfBillInfoPrint.Controls.Add(this.btnSelfPrintInfoClear);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtSelfPrintInfoFoodSeparator);
            this.gbSelfBillInfoPrint.Controls.Add(this.lblSelfPrintInfoFoodSeparator);
            this.gbSelfBillInfoPrint.Controls.Add(this.groupBox1);
            this.gbSelfBillInfoPrint.Controls.Add(this.gbSelfPrintInfoFood3);
            this.gbSelfBillInfoPrint.Controls.Add(this.gbSelfPrintInfoFood2);
            this.gbSelfBillInfoPrint.Controls.Add(this.gbSelfPrintInfoFood1);
            this.gbSelfBillInfoPrint.Controls.Add(this.chkSelfPrintInfoDirect);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtEmployeeTitle);
            this.gbSelfBillInfoPrint.Controls.Add(this.lblEmployeeTitle);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtPrinterIp);
            this.gbSelfBillInfoPrint.Controls.Add(this.lvlPrinterIp);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtPrinterName);
            this.gbSelfBillInfoPrint.Controls.Add(this.lblPrinterName);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtFishNumber);
            this.gbSelfBillInfoPrint.Controls.Add(this.lblFishNumber);
            this.gbSelfBillInfoPrint.Controls.Add(this.txtManualFishString);
            this.gbSelfBillInfoPrint.Controls.Add(this.lblManualFishString);
            this.gbSelfBillInfoPrint.Controls.Add(this.btnPrint);
            this.gbSelfBillInfoPrint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSelfBillInfoPrint.Location = new System.Drawing.Point(3, 112);
            this.gbSelfBillInfoPrint.Name = "gbSelfBillInfoPrint";
            this.gbSelfBillInfoPrint.Size = new System.Drawing.Size(517, 423);
            this.gbSelfBillInfoPrint.TabIndex = 1;
            this.gbSelfBillInfoPrint.TabStop = false;
            this.gbSelfBillInfoPrint.Text = "PringBillInfo";
            // 
            // chkSelfPrintInfoClearTitle3
            // 
            this.chkSelfPrintInfoClearTitle3.AutoSize = true;
            this.chkSelfPrintInfoClearTitle3.Checked = true;
            this.chkSelfPrintInfoClearTitle3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearTitle3.Location = new System.Drawing.Point(174, 393);
            this.chkSelfPrintInfoClearTitle3.Name = "chkSelfPrintInfoClearTitle3";
            this.chkSelfPrintInfoClearTitle3.Size = new System.Drawing.Size(39, 17);
            this.chkSelfPrintInfoClearTitle3.TabIndex = 66;
            this.chkSelfPrintInfoClearTitle3.Text = "T3";
            this.chkSelfPrintInfoClearTitle3.UseVisualStyleBackColor = true;
            // 
            // chkSelfPrintInfoClearTitle4
            // 
            this.chkSelfPrintInfoClearTitle4.AutoSize = true;
            this.chkSelfPrintInfoClearTitle4.Checked = true;
            this.chkSelfPrintInfoClearTitle4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearTitle4.Location = new System.Drawing.Point(234, 393);
            this.chkSelfPrintInfoClearTitle4.Name = "chkSelfPrintInfoClearTitle4";
            this.chkSelfPrintInfoClearTitle4.Size = new System.Drawing.Size(39, 17);
            this.chkSelfPrintInfoClearTitle4.TabIndex = 65;
            this.chkSelfPrintInfoClearTitle4.Text = "T3";
            this.chkSelfPrintInfoClearTitle4.UseVisualStyleBackColor = true;
            // 
            // chkSelfPrintInfoClearFood2
            // 
            this.chkSelfPrintInfoClearFood2.AutoSize = true;
            this.chkSelfPrintInfoClearFood2.Checked = true;
            this.chkSelfPrintInfoClearFood2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearFood2.Location = new System.Drawing.Point(294, 393);
            this.chkSelfPrintInfoClearFood2.Name = "chkSelfPrintInfoClearFood2";
            this.chkSelfPrintInfoClearFood2.Size = new System.Drawing.Size(38, 17);
            this.chkSelfPrintInfoClearFood2.TabIndex = 64;
            this.chkSelfPrintInfoClearFood2.Text = "F2";
            this.chkSelfPrintInfoClearFood2.UseVisualStyleBackColor = true;
            // 
            // chkSelfPrintInfoClearFood3
            // 
            this.chkSelfPrintInfoClearFood3.AutoSize = true;
            this.chkSelfPrintInfoClearFood3.Checked = true;
            this.chkSelfPrintInfoClearFood3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearFood3.Location = new System.Drawing.Point(354, 393);
            this.chkSelfPrintInfoClearFood3.Name = "chkSelfPrintInfoClearFood3";
            this.chkSelfPrintInfoClearFood3.Size = new System.Drawing.Size(38, 17);
            this.chkSelfPrintInfoClearFood3.TabIndex = 63;
            this.chkSelfPrintInfoClearFood3.Text = "F3";
            this.chkSelfPrintInfoClearFood3.UseVisualStyleBackColor = true;
            // 
            // chkSelfPrintInfoClearManual
            // 
            this.chkSelfPrintInfoClearManual.AutoSize = true;
            this.chkSelfPrintInfoClearManual.Checked = true;
            this.chkSelfPrintInfoClearManual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearManual.Location = new System.Drawing.Point(414, 393);
            this.chkSelfPrintInfoClearManual.Name = "chkSelfPrintInfoClearManual";
            this.chkSelfPrintInfoClearManual.Size = new System.Drawing.Size(61, 17);
            this.chkSelfPrintInfoClearManual.TabIndex = 62;
            this.chkSelfPrintInfoClearManual.Text = "Manual";
            this.chkSelfPrintInfoClearManual.UseVisualStyleBackColor = true;
            // 
            // chkSelfPrintInfoClearTitle2
            // 
            this.chkSelfPrintInfoClearTitle2.AutoSize = true;
            this.chkSelfPrintInfoClearTitle2.Checked = true;
            this.chkSelfPrintInfoClearTitle2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoClearTitle2.Location = new System.Drawing.Point(111, 393);
            this.chkSelfPrintInfoClearTitle2.Name = "chkSelfPrintInfoClearTitle2";
            this.chkSelfPrintInfoClearTitle2.Size = new System.Drawing.Size(39, 17);
            this.chkSelfPrintInfoClearTitle2.TabIndex = 61;
            this.chkSelfPrintInfoClearTitle2.Text = "T2";
            this.chkSelfPrintInfoClearTitle2.UseVisualStyleBackColor = true;
            // 
            // btnSelfPrintInfoClear
            // 
            this.btnSelfPrintInfoClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelfPrintInfoClear.Location = new System.Drawing.Point(6, 381);
            this.btnSelfPrintInfoClear.Name = "btnSelfPrintInfoClear";
            this.btnSelfPrintInfoClear.Size = new System.Drawing.Size(82, 37);
            this.btnSelfPrintInfoClear.TabIndex = 60;
            this.btnSelfPrintInfoClear.Text = "Clear";
            this.btnSelfPrintInfoClear.UseVisualStyleBackColor = true;
            this.btnSelfPrintInfoClear.Click += new System.EventHandler(this.btnSelfPrintInfoClear_Click);
            // 
            // txtSelfPrintInfoFoodSeparator
            // 
            this.txtSelfPrintInfoFoodSeparator.Location = new System.Drawing.Point(90, 357);
            this.txtSelfPrintInfoFoodSeparator.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodSeparator.Name = "txtSelfPrintInfoFoodSeparator";
            this.txtSelfPrintInfoFoodSeparator.Size = new System.Drawing.Size(158, 20);
            this.txtSelfPrintInfoFoodSeparator.TabIndex = 58;
            this.txtSelfPrintInfoFoodSeparator.Text = "   ";
            // 
            // lblSelfPrintInfoFoodSeparator
            // 
            this.lblSelfPrintInfoFoodSeparator.AutoSize = true;
            this.lblSelfPrintInfoFoodSeparator.Location = new System.Drawing.Point(2, 361);
            this.lblSelfPrintInfoFoodSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodSeparator.Name = "lblSelfPrintInfoFoodSeparator";
            this.lblSelfPrintInfoFoodSeparator.Size = new System.Drawing.Size(86, 13);
            this.lblSelfPrintInfoFoodSeparator.TabIndex = 59;
            this.lblSelfPrintInfoFoodSeparator.Text = "Food Separator :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblSelfPrintInfoHeaderTitle1);
            this.groupBox1.Controls.Add(this.txtSelfPrintInfoHeaderTitle1);
            this.groupBox1.Controls.Add(this.lblSelfPrintInfoHeaderTitle2);
            this.groupBox1.Controls.Add(this.txtSelfPrintInfoHeaderTitle2);
            this.groupBox1.Controls.Add(this.lblSelfPrintInfoHeaderTitle3);
            this.groupBox1.Controls.Add(this.txtSelfPrintInfoHeaderTitle4);
            this.groupBox1.Controls.Add(this.txtSelfPrintInfoHeaderTitle3);
            this.groupBox1.Controls.Add(this.lblSelfPrintInfoHeaderTitle4);
            this.groupBox1.Location = new System.Drawing.Point(4, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 64);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            // 
            // lblSelfPrintInfoHeaderTitle1
            // 
            this.lblSelfPrintInfoHeaderTitle1.AutoSize = true;
            this.lblSelfPrintInfoHeaderTitle1.Location = new System.Drawing.Point(5, 16);
            this.lblSelfPrintInfoHeaderTitle1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoHeaderTitle1.Name = "lblSelfPrintInfoHeaderTitle1";
            this.lblSelfPrintInfoHeaderTitle1.Size = new System.Drawing.Size(77, 13);
            this.lblSelfPrintInfoHeaderTitle1.TabIndex = 5;
            this.lblSelfPrintInfoHeaderTitle1.Text = "Header Title 1:";
            // 
            // txtSelfPrintInfoHeaderTitle1
            // 
            this.txtSelfPrintInfoHeaderTitle1.Location = new System.Drawing.Point(83, 12);
            this.txtSelfPrintInfoHeaderTitle1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoHeaderTitle1.Name = "txtSelfPrintInfoHeaderTitle1";
            this.txtSelfPrintInfoHeaderTitle1.Size = new System.Drawing.Size(158, 20);
            this.txtSelfPrintInfoHeaderTitle1.TabIndex = 6;
            this.txtSelfPrintInfoHeaderTitle1.Text = "سلف شرکت دخانیات ایران";
            // 
            // lblSelfPrintInfoHeaderTitle2
            // 
            this.lblSelfPrintInfoHeaderTitle2.AutoSize = true;
            this.lblSelfPrintInfoHeaderTitle2.Location = new System.Drawing.Point(245, 16);
            this.lblSelfPrintInfoHeaderTitle2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoHeaderTitle2.Name = "lblSelfPrintInfoHeaderTitle2";
            this.lblSelfPrintInfoHeaderTitle2.Size = new System.Drawing.Size(77, 13);
            this.lblSelfPrintInfoHeaderTitle2.TabIndex = 7;
            this.lblSelfPrintInfoHeaderTitle2.Text = "Header Title 2:";
            // 
            // txtSelfPrintInfoHeaderTitle2
            // 
            this.txtSelfPrintInfoHeaderTitle2.Location = new System.Drawing.Point(322, 12);
            this.txtSelfPrintInfoHeaderTitle2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoHeaderTitle2.Name = "txtSelfPrintInfoHeaderTitle2";
            this.txtSelfPrintInfoHeaderTitle2.Size = new System.Drawing.Size(158, 20);
            this.txtSelfPrintInfoHeaderTitle2.TabIndex = 8;
            this.txtSelfPrintInfoHeaderTitle2.Text = "رستوران 1";
            // 
            // lblSelfPrintInfoHeaderTitle3
            // 
            this.lblSelfPrintInfoHeaderTitle3.AutoSize = true;
            this.lblSelfPrintInfoHeaderTitle3.Location = new System.Drawing.Point(5, 40);
            this.lblSelfPrintInfoHeaderTitle3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoHeaderTitle3.Name = "lblSelfPrintInfoHeaderTitle3";
            this.lblSelfPrintInfoHeaderTitle3.Size = new System.Drawing.Size(77, 13);
            this.lblSelfPrintInfoHeaderTitle3.TabIndex = 9;
            this.lblSelfPrintInfoHeaderTitle3.Text = "Header Title 3:";
            // 
            // txtSelfPrintInfoHeaderTitle4
            // 
            this.txtSelfPrintInfoHeaderTitle4.Location = new System.Drawing.Point(322, 36);
            this.txtSelfPrintInfoHeaderTitle4.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoHeaderTitle4.Name = "txtSelfPrintInfoHeaderTitle4";
            this.txtSelfPrintInfoHeaderTitle4.Size = new System.Drawing.Size(158, 20);
            this.txtSelfPrintInfoHeaderTitle4.TabIndex = 12;
            // 
            // txtSelfPrintInfoHeaderTitle3
            // 
            this.txtSelfPrintInfoHeaderTitle3.Location = new System.Drawing.Point(82, 36);
            this.txtSelfPrintInfoHeaderTitle3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoHeaderTitle3.Name = "txtSelfPrintInfoHeaderTitle3";
            this.txtSelfPrintInfoHeaderTitle3.Size = new System.Drawing.Size(158, 20);
            this.txtSelfPrintInfoHeaderTitle3.TabIndex = 10;
            this.txtSelfPrintInfoHeaderTitle3.Text = "صف مردانه";
            // 
            // lblSelfPrintInfoHeaderTitle4
            // 
            this.lblSelfPrintInfoHeaderTitle4.AutoSize = true;
            this.lblSelfPrintInfoHeaderTitle4.Location = new System.Drawing.Point(245, 40);
            this.lblSelfPrintInfoHeaderTitle4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoHeaderTitle4.Name = "lblSelfPrintInfoHeaderTitle4";
            this.lblSelfPrintInfoHeaderTitle4.Size = new System.Drawing.Size(77, 13);
            this.lblSelfPrintInfoHeaderTitle4.TabIndex = 11;
            this.lblSelfPrintInfoHeaderTitle4.Text = "Header Title 4:";
            // 
            // gbSelfPrintInfoFood3
            // 
            this.gbSelfPrintInfoFood3.Controls.Add(this.lblSelfPrintInfoFoodType3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.txtSelfPrintInfoFoodTitle3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.txtSelfPrintInfoFoodType3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.chkSelfPrintInfoFoodIsAllowed3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.lblSelfPrintInfoFishCount3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.txtSelfPrintInfoFishCount3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.lblSelfPrintInfoFoodDescription3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.lblSelfPrintInfoFoodPrice3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.txtSelfPrintInfoFoodDescription3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.txtSelfPrintInfoFoodPrice3);
            this.gbSelfPrintInfoFood3.Controls.Add(this.lblSelfPrintInfoFoodTitle3);
            this.gbSelfPrintInfoFood3.Location = new System.Drawing.Point(3, 256);
            this.gbSelfPrintInfoFood3.Name = "gbSelfPrintInfoFood3";
            this.gbSelfPrintInfoFood3.Size = new System.Drawing.Size(508, 64);
            this.gbSelfPrintInfoFood3.TabIndex = 56;
            this.gbSelfPrintInfoFood3.TabStop = false;
            // 
            // lblSelfPrintInfoFoodType3
            // 
            this.lblSelfPrintInfoFoodType3.AutoSize = true;
            this.lblSelfPrintInfoFoodType3.Location = new System.Drawing.Point(10, 14);
            this.lblSelfPrintInfoFoodType3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodType3.Name = "lblSelfPrintInfoFoodType3";
            this.lblSelfPrintInfoFoodType3.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodType3.TabIndex = 39;
            this.lblSelfPrintInfoFoodType3.Text = "Food Type 3:";
            // 
            // txtSelfPrintInfoFoodTitle3
            // 
            this.txtSelfPrintInfoFoodTitle3.Location = new System.Drawing.Point(80, 36);
            this.txtSelfPrintInfoFoodTitle3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodTitle3.Name = "txtSelfPrintInfoFoodTitle3";
            this.txtSelfPrintInfoFoodTitle3.Size = new System.Drawing.Size(148, 20);
            this.txtSelfPrintInfoFoodTitle3.TabIndex = 46;
            this.txtSelfPrintInfoFoodTitle3.Text = "ماست و خیار";
            // 
            // txtSelfPrintInfoFoodType3
            // 
            this.txtSelfPrintInfoFoodType3.Location = new System.Drawing.Point(80, 10);
            this.txtSelfPrintInfoFoodType3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodType3.Name = "txtSelfPrintInfoFoodType3";
            this.txtSelfPrintInfoFoodType3.Size = new System.Drawing.Size(82, 20);
            this.txtSelfPrintInfoFoodType3.TabIndex = 40;
            this.txtSelfPrintInfoFoodType3.Text = "مخلفه";
            // 
            // chkSelfPrintInfoFoodIsAllowed3
            // 
            this.chkSelfPrintInfoFoodIsAllowed3.AutoSize = true;
            this.chkSelfPrintInfoFoodIsAllowed3.Checked = true;
            this.chkSelfPrintInfoFoodIsAllowed3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoFoodIsAllowed3.Location = new System.Drawing.Point(399, 38);
            this.chkSelfPrintInfoFoodIsAllowed3.Name = "chkSelfPrintInfoFoodIsAllowed3";
            this.chkSelfPrintInfoFoodIsAllowed3.Size = new System.Drawing.Size(83, 17);
            this.chkSelfPrintInfoFoodIsAllowed3.TabIndex = 53;
            this.chkSelfPrintInfoFoodIsAllowed3.Text = "Is Allowed 3";
            this.chkSelfPrintInfoFoodIsAllowed3.UseVisualStyleBackColor = true;
            // 
            // lblSelfPrintInfoFishCount3
            // 
            this.lblSelfPrintInfoFishCount3.AutoSize = true;
            this.lblSelfPrintInfoFishCount3.Location = new System.Drawing.Point(166, 14);
            this.lblSelfPrintInfoFishCount3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFishCount3.Name = "lblSelfPrintInfoFishCount3";
            this.lblSelfPrintInfoFishCount3.Size = new System.Drawing.Size(74, 13);
            this.lblSelfPrintInfoFishCount3.TabIndex = 41;
            this.lblSelfPrintInfoFishCount3.Text = "Food Count 3:";
            // 
            // txtSelfPrintInfoFishCount3
            // 
            this.txtSelfPrintInfoFishCount3.Location = new System.Drawing.Point(240, 10);
            this.txtSelfPrintInfoFishCount3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFishCount3.Name = "txtSelfPrintInfoFishCount3";
            this.txtSelfPrintInfoFishCount3.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFishCount3.TabIndex = 42;
            this.txtSelfPrintInfoFishCount3.Text = "تعداد-13";
            // 
            // lblSelfPrintInfoFoodDescription3
            // 
            this.lblSelfPrintInfoFoodDescription3.AutoSize = true;
            this.lblSelfPrintInfoFoodDescription3.Location = new System.Drawing.Point(237, 40);
            this.lblSelfPrintInfoFoodDescription3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodDescription3.Name = "lblSelfPrintInfoFoodDescription3";
            this.lblSelfPrintInfoFoodDescription3.Size = new System.Drawing.Size(71, 13);
            this.lblSelfPrintInfoFoodDescription3.TabIndex = 47;
            this.lblSelfPrintInfoFoodDescription3.Text = "Food Desc 3:";
            // 
            // lblSelfPrintInfoFoodPrice3
            // 
            this.lblSelfPrintInfoFoodPrice3.AutoSize = true;
            this.lblSelfPrintInfoFoodPrice3.Location = new System.Drawing.Point(324, 14);
            this.lblSelfPrintInfoFoodPrice3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodPrice3.Name = "lblSelfPrintInfoFoodPrice3";
            this.lblSelfPrintInfoFoodPrice3.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodPrice3.TabIndex = 43;
            this.lblSelfPrintInfoFoodPrice3.Text = "Food Price 3:";
            // 
            // txtSelfPrintInfoFoodDescription3
            // 
            this.txtSelfPrintInfoFoodDescription3.Location = new System.Drawing.Point(308, 36);
            this.txtSelfPrintInfoFoodDescription3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodDescription3.Name = "txtSelfPrintInfoFoodDescription3";
            this.txtSelfPrintInfoFoodDescription3.Size = new System.Drawing.Size(86, 20);
            this.txtSelfPrintInfoFoodDescription3.TabIndex = 48;
            this.txtSelfPrintInfoFoodDescription3.Text = "فیش نامجاز 3";
            // 
            // txtSelfPrintInfoFoodPrice3
            // 
            this.txtSelfPrintInfoFoodPrice3.Location = new System.Drawing.Point(397, 10);
            this.txtSelfPrintInfoFoodPrice3.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodPrice3.Name = "txtSelfPrintInfoFoodPrice3";
            this.txtSelfPrintInfoFoodPrice3.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFoodPrice3.TabIndex = 44;
            this.txtSelfPrintInfoFoodPrice3.Text = "800,000 ت";
            // 
            // lblSelfPrintInfoFoodTitle3
            // 
            this.lblSelfPrintInfoFoodTitle3.AutoSize = true;
            this.lblSelfPrintInfoFoodTitle3.Location = new System.Drawing.Point(10, 40);
            this.lblSelfPrintInfoFoodTitle3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodTitle3.Name = "lblSelfPrintInfoFoodTitle3";
            this.lblSelfPrintInfoFoodTitle3.Size = new System.Drawing.Size(66, 13);
            this.lblSelfPrintInfoFoodTitle3.TabIndex = 45;
            this.lblSelfPrintInfoFoodTitle3.Text = "Food Title 3:";
            // 
            // gbSelfPrintInfoFood2
            // 
            this.gbSelfPrintInfoFood2.Controls.Add(this.lblSelfPrintInfoFoodType2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.txtSelfPrintInfoFoodTitle2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.chkSelfPrintInfoFoodIsAllowed2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.txtSelfPrintInfoFoodType2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.lblSelfPrintInfoFishCount2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.txtSelfPrintInfoFishCount2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.lblSelfPrintInfoFoodPrice2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.txtSelfPrintInfoFoodPrice2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.lblSelfPrintInfoFoodTitle2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.txtSelfPrintInfoFoodDescription2);
            this.gbSelfPrintInfoFood2.Controls.Add(this.lblSelfPrintInfoFoodDescription2);
            this.gbSelfPrintInfoFood2.Location = new System.Drawing.Point(3, 191);
            this.gbSelfPrintInfoFood2.Name = "gbSelfPrintInfoFood2";
            this.gbSelfPrintInfoFood2.Size = new System.Drawing.Size(508, 64);
            this.gbSelfPrintInfoFood2.TabIndex = 55;
            this.gbSelfPrintInfoFood2.TabStop = false;
            // 
            // lblSelfPrintInfoFoodType2
            // 
            this.lblSelfPrintInfoFoodType2.AutoSize = true;
            this.lblSelfPrintInfoFoodType2.Location = new System.Drawing.Point(5, 15);
            this.lblSelfPrintInfoFoodType2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodType2.Name = "lblSelfPrintInfoFoodType2";
            this.lblSelfPrintInfoFoodType2.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodType2.TabIndex = 28;
            this.lblSelfPrintInfoFoodType2.Text = "Food Type 2:";
            // 
            // txtSelfPrintInfoFoodTitle2
            // 
            this.txtSelfPrintInfoFoodTitle2.Location = new System.Drawing.Point(75, 37);
            this.txtSelfPrintInfoFoodTitle2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodTitle2.Name = "txtSelfPrintInfoFoodTitle2";
            this.txtSelfPrintInfoFoodTitle2.Size = new System.Drawing.Size(148, 20);
            this.txtSelfPrintInfoFoodTitle2.TabIndex = 36;
            this.txtSelfPrintInfoFoodTitle2.Text = "نوشابه یا دوغ";
            // 
            // chkSelfPrintInfoFoodIsAllowed2
            // 
            this.chkSelfPrintInfoFoodIsAllowed2.AutoSize = true;
            this.chkSelfPrintInfoFoodIsAllowed2.Checked = true;
            this.chkSelfPrintInfoFoodIsAllowed2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoFoodIsAllowed2.Location = new System.Drawing.Point(394, 39);
            this.chkSelfPrintInfoFoodIsAllowed2.Name = "chkSelfPrintInfoFoodIsAllowed2";
            this.chkSelfPrintInfoFoodIsAllowed2.Size = new System.Drawing.Size(83, 17);
            this.chkSelfPrintInfoFoodIsAllowed2.TabIndex = 52;
            this.chkSelfPrintInfoFoodIsAllowed2.Text = "Is Allowed 2";
            this.chkSelfPrintInfoFoodIsAllowed2.UseVisualStyleBackColor = true;
            // 
            // txtSelfPrintInfoFoodType2
            // 
            this.txtSelfPrintInfoFoodType2.Location = new System.Drawing.Point(75, 11);
            this.txtSelfPrintInfoFoodType2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodType2.Name = "txtSelfPrintInfoFoodType2";
            this.txtSelfPrintInfoFoodType2.Size = new System.Drawing.Size(82, 20);
            this.txtSelfPrintInfoFoodType2.TabIndex = 29;
            this.txtSelfPrintInfoFoodType2.Text = "دسر";
            // 
            // lblSelfPrintInfoFishCount2
            // 
            this.lblSelfPrintInfoFishCount2.AutoSize = true;
            this.lblSelfPrintInfoFishCount2.Location = new System.Drawing.Point(161, 15);
            this.lblSelfPrintInfoFishCount2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFishCount2.Name = "lblSelfPrintInfoFishCount2";
            this.lblSelfPrintInfoFishCount2.Size = new System.Drawing.Size(74, 13);
            this.lblSelfPrintInfoFishCount2.TabIndex = 30;
            this.lblSelfPrintInfoFishCount2.Text = "Food Count 2:";
            // 
            // txtSelfPrintInfoFishCount2
            // 
            this.txtSelfPrintInfoFishCount2.Location = new System.Drawing.Point(235, 11);
            this.txtSelfPrintInfoFishCount2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFishCount2.Name = "txtSelfPrintInfoFishCount2";
            this.txtSelfPrintInfoFishCount2.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFishCount2.TabIndex = 31;
            this.txtSelfPrintInfoFishCount2.Text = "تعداد-20";
            // 
            // lblSelfPrintInfoFoodPrice2
            // 
            this.lblSelfPrintInfoFoodPrice2.AutoSize = true;
            this.lblSelfPrintInfoFoodPrice2.Location = new System.Drawing.Point(319, 15);
            this.lblSelfPrintInfoFoodPrice2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodPrice2.Name = "lblSelfPrintInfoFoodPrice2";
            this.lblSelfPrintInfoFoodPrice2.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodPrice2.TabIndex = 32;
            this.lblSelfPrintInfoFoodPrice2.Text = "Food Price 2:";
            // 
            // txtSelfPrintInfoFoodPrice2
            // 
            this.txtSelfPrintInfoFoodPrice2.Location = new System.Drawing.Point(392, 11);
            this.txtSelfPrintInfoFoodPrice2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodPrice2.Name = "txtSelfPrintInfoFoodPrice2";
            this.txtSelfPrintInfoFoodPrice2.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFoodPrice2.TabIndex = 33;
            this.txtSelfPrintInfoFoodPrice2.Text = "600,000 ت";
            // 
            // lblSelfPrintInfoFoodTitle2
            // 
            this.lblSelfPrintInfoFoodTitle2.AutoSize = true;
            this.lblSelfPrintInfoFoodTitle2.Location = new System.Drawing.Point(5, 41);
            this.lblSelfPrintInfoFoodTitle2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodTitle2.Name = "lblSelfPrintInfoFoodTitle2";
            this.lblSelfPrintInfoFoodTitle2.Size = new System.Drawing.Size(66, 13);
            this.lblSelfPrintInfoFoodTitle2.TabIndex = 34;
            this.lblSelfPrintInfoFoodTitle2.Text = "Food Title 2:";
            // 
            // txtSelfPrintInfoFoodDescription2
            // 
            this.txtSelfPrintInfoFoodDescription2.Location = new System.Drawing.Point(303, 37);
            this.txtSelfPrintInfoFoodDescription2.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodDescription2.Name = "txtSelfPrintInfoFoodDescription2";
            this.txtSelfPrintInfoFoodDescription2.Size = new System.Drawing.Size(86, 20);
            this.txtSelfPrintInfoFoodDescription2.TabIndex = 38;
            // 
            // lblSelfPrintInfoFoodDescription2
            // 
            this.lblSelfPrintInfoFoodDescription2.AutoSize = true;
            this.lblSelfPrintInfoFoodDescription2.Location = new System.Drawing.Point(232, 41);
            this.lblSelfPrintInfoFoodDescription2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodDescription2.Name = "lblSelfPrintInfoFoodDescription2";
            this.lblSelfPrintInfoFoodDescription2.Size = new System.Drawing.Size(71, 13);
            this.lblSelfPrintInfoFoodDescription2.TabIndex = 37;
            this.lblSelfPrintInfoFoodDescription2.Text = "Food Desc 2:";
            // 
            // gbSelfPrintInfoFood1
            // 
            this.gbSelfPrintInfoFood1.Controls.Add(this.lblSelfPrintInfoFoodType1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.txtSelfPrintInfoFoodTitle1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.txtSelfPrintInfoFoodType1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.chkSelfPrintInfoFoodIsAllowed1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.lblSelfPrintInfoFishCount1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.txtSelfPrintInfoFishCount1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.lblSelfPrintInfoFoodPrice1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.txtSelfPrintInfoFoodPrice1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.lblSelfPrintInfoFoodTitle1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.txtSelfPrintInfoFoodDescription1);
            this.gbSelfPrintInfoFood1.Controls.Add(this.lblSelfPrintInfoFoodDescription1);
            this.gbSelfPrintInfoFood1.Location = new System.Drawing.Point(3, 126);
            this.gbSelfPrintInfoFood1.Name = "gbSelfPrintInfoFood1";
            this.gbSelfPrintInfoFood1.Size = new System.Drawing.Size(508, 64);
            this.gbSelfPrintInfoFood1.TabIndex = 54;
            this.gbSelfPrintInfoFood1.TabStop = false;
            // 
            // lblSelfPrintInfoFoodType1
            // 
            this.lblSelfPrintInfoFoodType1.AutoSize = true;
            this.lblSelfPrintInfoFoodType1.Location = new System.Drawing.Point(7, 17);
            this.lblSelfPrintInfoFoodType1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodType1.Name = "lblSelfPrintInfoFoodType1";
            this.lblSelfPrintInfoFoodType1.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodType1.TabIndex = 17;
            this.lblSelfPrintInfoFoodType1.Text = "Food Type 1:";
            // 
            // txtSelfPrintInfoFoodTitle1
            // 
            this.txtSelfPrintInfoFoodTitle1.Location = new System.Drawing.Point(77, 39);
            this.txtSelfPrintInfoFoodTitle1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodTitle1.Name = "txtSelfPrintInfoFoodTitle1";
            this.txtSelfPrintInfoFoodTitle1.Size = new System.Drawing.Size(148, 20);
            this.txtSelfPrintInfoFoodTitle1.TabIndex = 25;
            this.txtSelfPrintInfoFoodTitle1.Text = " چلو جوجه کباب";
            // 
            // txtSelfPrintInfoFoodType1
            // 
            this.txtSelfPrintInfoFoodType1.Location = new System.Drawing.Point(77, 13);
            this.txtSelfPrintInfoFoodType1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodType1.Name = "txtSelfPrintInfoFoodType1";
            this.txtSelfPrintInfoFoodType1.Size = new System.Drawing.Size(82, 20);
            this.txtSelfPrintInfoFoodType1.TabIndex = 18;
            this.txtSelfPrintInfoFoodType1.Text = "ناهار";
            // 
            // chkSelfPrintInfoFoodIsAllowed1
            // 
            this.chkSelfPrintInfoFoodIsAllowed1.AutoSize = true;
            this.chkSelfPrintInfoFoodIsAllowed1.Checked = true;
            this.chkSelfPrintInfoFoodIsAllowed1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelfPrintInfoFoodIsAllowed1.Location = new System.Drawing.Point(396, 41);
            this.chkSelfPrintInfoFoodIsAllowed1.Name = "chkSelfPrintInfoFoodIsAllowed1";
            this.chkSelfPrintInfoFoodIsAllowed1.Size = new System.Drawing.Size(83, 17);
            this.chkSelfPrintInfoFoodIsAllowed1.TabIndex = 51;
            this.chkSelfPrintInfoFoodIsAllowed1.Text = "Is Allowed 1";
            this.chkSelfPrintInfoFoodIsAllowed1.UseVisualStyleBackColor = true;
            // 
            // lblSelfPrintInfoFishCount1
            // 
            this.lblSelfPrintInfoFishCount1.AutoSize = true;
            this.lblSelfPrintInfoFishCount1.Location = new System.Drawing.Point(163, 17);
            this.lblSelfPrintInfoFishCount1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFishCount1.Name = "lblSelfPrintInfoFishCount1";
            this.lblSelfPrintInfoFishCount1.Size = new System.Drawing.Size(74, 13);
            this.lblSelfPrintInfoFishCount1.TabIndex = 19;
            this.lblSelfPrintInfoFishCount1.Text = "Food Count 1:";
            // 
            // txtSelfPrintInfoFishCount1
            // 
            this.txtSelfPrintInfoFishCount1.Location = new System.Drawing.Point(237, 13);
            this.txtSelfPrintInfoFishCount1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFishCount1.Name = "txtSelfPrintInfoFishCount1";
            this.txtSelfPrintInfoFishCount1.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFishCount1.TabIndex = 20;
            this.txtSelfPrintInfoFishCount1.Text = "تعداد-10";
            // 
            // lblSelfPrintInfoFoodPrice1
            // 
            this.lblSelfPrintInfoFoodPrice1.AutoSize = true;
            this.lblSelfPrintInfoFoodPrice1.Location = new System.Drawing.Point(321, 17);
            this.lblSelfPrintInfoFoodPrice1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodPrice1.Name = "lblSelfPrintInfoFoodPrice1";
            this.lblSelfPrintInfoFoodPrice1.Size = new System.Drawing.Size(70, 13);
            this.lblSelfPrintInfoFoodPrice1.TabIndex = 21;
            this.lblSelfPrintInfoFoodPrice1.Text = "Food Price 1:";
            // 
            // txtSelfPrintInfoFoodPrice1
            // 
            this.txtSelfPrintInfoFoodPrice1.Location = new System.Drawing.Point(394, 13);
            this.txtSelfPrintInfoFoodPrice1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodPrice1.Name = "txtSelfPrintInfoFoodPrice1";
            this.txtSelfPrintInfoFoodPrice1.Size = new System.Drawing.Size(73, 20);
            this.txtSelfPrintInfoFoodPrice1.TabIndex = 22;
            this.txtSelfPrintInfoFoodPrice1.Text = "1,200,000 ت";
            // 
            // lblSelfPrintInfoFoodTitle1
            // 
            this.lblSelfPrintInfoFoodTitle1.AutoSize = true;
            this.lblSelfPrintInfoFoodTitle1.Location = new System.Drawing.Point(7, 43);
            this.lblSelfPrintInfoFoodTitle1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodTitle1.Name = "lblSelfPrintInfoFoodTitle1";
            this.lblSelfPrintInfoFoodTitle1.Size = new System.Drawing.Size(66, 13);
            this.lblSelfPrintInfoFoodTitle1.TabIndex = 23;
            this.lblSelfPrintInfoFoodTitle1.Text = "Food Title 1:";
            // 
            // txtSelfPrintInfoFoodDescription1
            // 
            this.txtSelfPrintInfoFoodDescription1.Location = new System.Drawing.Point(305, 39);
            this.txtSelfPrintInfoFoodDescription1.Margin = new System.Windows.Forms.Padding(2);
            this.txtSelfPrintInfoFoodDescription1.Name = "txtSelfPrintInfoFoodDescription1";
            this.txtSelfPrintInfoFoodDescription1.Size = new System.Drawing.Size(86, 20);
            this.txtSelfPrintInfoFoodDescription1.TabIndex = 27;
            this.txtSelfPrintInfoFoodDescription1.Text = "فیش نامجاز 1";
            // 
            // lblSelfPrintInfoFoodDescription1
            // 
            this.lblSelfPrintInfoFoodDescription1.AutoSize = true;
            this.lblSelfPrintInfoFoodDescription1.Location = new System.Drawing.Point(234, 43);
            this.lblSelfPrintInfoFoodDescription1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSelfPrintInfoFoodDescription1.Name = "lblSelfPrintInfoFoodDescription1";
            this.lblSelfPrintInfoFoodDescription1.Size = new System.Drawing.Size(71, 13);
            this.lblSelfPrintInfoFoodDescription1.TabIndex = 26;
            this.lblSelfPrintInfoFoodDescription1.Text = "Food Desc 1:";
            // 
            // chkSelfPrintInfoDirect
            // 
            this.chkSelfPrintInfoDirect.AutoSize = true;
            this.chkSelfPrintInfoDirect.Location = new System.Drawing.Point(306, 344);
            this.chkSelfPrintInfoDirect.Name = "chkSelfPrintInfoDirect";
            this.chkSelfPrintInfoDirect.Size = new System.Drawing.Size(54, 17);
            this.chkSelfPrintInfoDirect.TabIndex = 2;
            this.chkSelfPrintInfoDirect.Text = "Direct";
            this.chkSelfPrintInfoDirect.UseVisualStyleBackColor = true;
            // 
            // txtEmployeeTitle
            // 
            this.txtEmployeeTitle.Location = new System.Drawing.Point(87, 42);
            this.txtEmployeeTitle.Margin = new System.Windows.Forms.Padding(2);
            this.txtEmployeeTitle.Name = "txtEmployeeTitle";
            this.txtEmployeeTitle.Size = new System.Drawing.Size(158, 20);
            this.txtEmployeeTitle.TabIndex = 14;
            this.txtEmployeeTitle.Text = "محمد سبحانی راد";
            // 
            // lblEmployeeTitle
            // 
            this.lblEmployeeTitle.AutoSize = true;
            this.lblEmployeeTitle.Location = new System.Drawing.Point(6, 46);
            this.lblEmployeeTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEmployeeTitle.Name = "lblEmployeeTitle";
            this.lblEmployeeTitle.Size = new System.Drawing.Size(79, 13);
            this.lblEmployeeTitle.TabIndex = 13;
            this.lblEmployeeTitle.Text = "Employee Title:";
            // 
            // txtPrinterIp
            // 
            this.txtPrinterIp.Location = new System.Drawing.Point(323, 18);
            this.txtPrinterIp.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrinterIp.Name = "txtPrinterIp";
            this.txtPrinterIp.Size = new System.Drawing.Size(158, 20);
            this.txtPrinterIp.TabIndex = 4;
            this.txtPrinterIp.Text = "192.168.40.204";
            // 
            // lvlPrinterIp
            // 
            this.lvlPrinterIp.AutoSize = true;
            this.lvlPrinterIp.Location = new System.Drawing.Point(246, 22);
            this.lvlPrinterIp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lvlPrinterIp.Name = "lvlPrinterIp";
            this.lvlPrinterIp.Size = new System.Drawing.Size(53, 13);
            this.lvlPrinterIp.TabIndex = 2;
            this.lvlPrinterIp.Text = "Printer IP:";
            // 
            // txtPrinterName
            // 
            this.txtPrinterName.Location = new System.Drawing.Point(84, 18);
            this.txtPrinterName.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrinterName.Name = "txtPrinterName";
            this.txtPrinterName.Size = new System.Drawing.Size(158, 20);
            this.txtPrinterName.TabIndex = 1;
            this.txtPrinterName.Text = "OSCAR 204";
            // 
            // lblPrinterName
            // 
            this.lblPrinterName.AutoSize = true;
            this.lblPrinterName.Location = new System.Drawing.Point(6, 22);
            this.lblPrinterName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPrinterName.Name = "lblPrinterName";
            this.lblPrinterName.Size = new System.Drawing.Size(74, 13);
            this.lblPrinterName.TabIndex = 0;
            this.lblPrinterName.Text = "Printer Name :";
            // 
            // txtFishNumber
            // 
            this.txtFishNumber.Location = new System.Drawing.Point(91, 328);
            this.txtFishNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txtFishNumber.Name = "txtFishNumber";
            this.txtFishNumber.Size = new System.Drawing.Size(158, 20);
            this.txtFishNumber.TabIndex = 0;
            this.txtFishNumber.Text = "شماره فیش: از 19 تا 21\r\n";
            // 
            // lblFishNumber
            // 
            this.lblFishNumber.AutoSize = true;
            this.lblFishNumber.Location = new System.Drawing.Point(2, 332);
            this.lblFishNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFishNumber.Name = "lblFishNumber";
            this.lblFishNumber.Size = new System.Drawing.Size(72, 13);
            this.lblFishNumber.TabIndex = 49;
            this.lblFishNumber.Text = "Fish Number :";
            // 
            // txtManualFishString
            // 
            this.txtManualFishString.Location = new System.Drawing.Point(323, 42);
            this.txtManualFishString.Margin = new System.Windows.Forms.Padding(2);
            this.txtManualFishString.Name = "txtManualFishString";
            this.txtManualFishString.Size = new System.Drawing.Size(158, 20);
            this.txtManualFishString.TabIndex = 16;
            this.txtManualFishString.Text = "فیش دستی";
            // 
            // lblManualFishString
            // 
            this.lblManualFishString.AutoSize = true;
            this.lblManualFishString.Location = new System.Drawing.Point(246, 46);
            this.lblManualFishString.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManualFishString.Name = "lblManualFishString";
            this.lblManualFishString.Size = new System.Drawing.Size(70, 13);
            this.lblManualFishString.TabIndex = 15;
            this.lblManualFishString.Text = "Manual Fish :";
            // 
            // btnPrint
            // 
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(388, 332);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(94, 37);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print Self";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // gbSelfToolsFoodTitles
            // 
            this.gbSelfToolsFoodTitles.Controls.Add(this.lblSendFoodTitles);
            this.gbSelfToolsFoodTitles.Controls.Add(this.btnDisableFoodTitles);
            this.gbSelfToolsFoodTitles.Controls.Add(this.btnSendFoodTitles);
            this.gbSelfToolsFoodTitles.Controls.Add(this.txtSendFoodTitlesDeviceNumber);
            this.gbSelfToolsFoodTitles.Controls.Add(this.txtSendFoodTitles);
            this.gbSelfToolsFoodTitles.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbSelfToolsFoodTitles.Location = new System.Drawing.Point(3, 3);
            this.gbSelfToolsFoodTitles.Name = "gbSelfToolsFoodTitles";
            this.gbSelfToolsFoodTitles.Size = new System.Drawing.Size(517, 100);
            this.gbSelfToolsFoodTitles.TabIndex = 0;
            this.gbSelfToolsFoodTitles.TabStop = false;
            this.gbSelfToolsFoodTitles.Text = "Send Food Titles";
            // 
            // lblSendFoodTitles
            // 
            this.lblSendFoodTitles.AutoSize = true;
            this.lblSendFoodTitles.Location = new System.Drawing.Point(6, 33);
            this.lblSendFoodTitles.Name = "lblSendFoodTitles";
            this.lblSendFoodTitles.Size = new System.Drawing.Size(59, 13);
            this.lblSendFoodTitles.TabIndex = 0;
            this.lblSendFoodTitles.Text = "FoodTitles:";
            // 
            // btnDisableFoodTitles
            // 
            this.btnDisableFoodTitles.Location = new System.Drawing.Point(263, 69);
            this.btnDisableFoodTitles.Name = "btnDisableFoodTitles";
            this.btnDisableFoodTitles.Size = new System.Drawing.Size(75, 23);
            this.btnDisableFoodTitles.TabIndex = 4;
            this.btnDisableFoodTitles.Text = "Disable";
            this.btnDisableFoodTitles.UseVisualStyleBackColor = true;
            this.btnDisableFoodTitles.Click += new System.EventHandler(this.btnDisableFoodTitles_Click);
            // 
            // btnSendFoodTitles
            // 
            this.btnSendFoodTitles.Location = new System.Drawing.Point(263, 16);
            this.btnSendFoodTitles.Name = "btnSendFoodTitles";
            this.btnSendFoodTitles.Size = new System.Drawing.Size(75, 23);
            this.btnSendFoodTitles.TabIndex = 2;
            this.btnSendFoodTitles.Text = "Send";
            this.btnSendFoodTitles.UseVisualStyleBackColor = true;
            this.btnSendFoodTitles.Click += new System.EventHandler(this.btnSendFoodTitles_Click);
            // 
            // txtSendFoodTitlesDeviceNumber
            // 
            this.txtSendFoodTitlesDeviceNumber.Location = new System.Drawing.Point(263, 43);
            this.txtSendFoodTitlesDeviceNumber.Name = "txtSendFoodTitlesDeviceNumber";
            this.txtSendFoodTitlesDeviceNumber.Size = new System.Drawing.Size(75, 20);
            this.txtSendFoodTitlesDeviceNumber.TabIndex = 3;
            // 
            // txtSendFoodTitles
            // 
            this.txtSendFoodTitles.Location = new System.Drawing.Point(68, 16);
            this.txtSendFoodTitles.Multiline = true;
            this.txtSendFoodTitles.Name = "txtSendFoodTitles";
            this.txtSendFoodTitles.Size = new System.Drawing.Size(189, 76);
            this.txtSendFoodTitles.TabIndex = 1;
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
            // tpTimyAccess
            // 
            this.tpTimyAccess.Controls.Add(this.tcTimyInner);
            this.tpTimyAccess.Controls.Add(this.pnlTimyBottom);
            this.tpTimyAccess.Location = new System.Drawing.Point(4, 22);
            this.tpTimyAccess.Name = "tpTimyAccess";
            this.tpTimyAccess.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimyAccess.Size = new System.Drawing.Size(523, 538);
            this.tpTimyAccess.TabIndex = 5;
            this.tpTimyAccess.Text = "Timy Access";
            this.tpTimyAccess.UseVisualStyleBackColor = true;
            // 
            // tcTimyInner
            // 
            this.tcTimyInner.Controls.Add(this.tpTimyDay);
            this.tcTimyInner.Controls.Add(this.tpTimyWeek);
            this.tcTimyInner.Controls.Add(this.tpTimySetForUser);
            this.tcTimyInner.Controls.Add(this.tpTimyHoliday);
            this.tcTimyInner.Controls.Add(this.tpTimyJsonOutput);
            this.tcTimyInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcTimyInner.Location = new System.Drawing.Point(3, 3);
            this.tcTimyInner.Name = "tcTimyInner";
            this.tcTimyInner.SelectedIndex = 0;
            this.tcTimyInner.Size = new System.Drawing.Size(517, 492);
            this.tcTimyInner.TabIndex = 0;
            // 
            // tpTimyDay
            // 
            this.tpTimyDay.Controls.Add(this.pnlTimyDayRight);
            this.tpTimyDay.Controls.Add(this.pnlTimyDayLeft);
            this.tpTimyDay.Location = new System.Drawing.Point(4, 22);
            this.tpTimyDay.Name = "tpTimyDay";
            this.tpTimyDay.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimyDay.Size = new System.Drawing.Size(509, 466);
            this.tpTimyDay.TabIndex = 0;
            this.tpTimyDay.Text = "Day Timezone Groups";
            this.tpTimyDay.UseVisualStyleBackColor = true;
            // 
            // pnlTimyDayRight
            // 
            this.pnlTimyDayRight.Controls.Add(this.dgvTimyDayTimezones);
            this.pnlTimyDayRight.Controls.Add(this.pnlTimyDayHeader);
            this.pnlTimyDayRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTimyDayRight.Location = new System.Drawing.Point(153, 3);
            this.pnlTimyDayRight.Name = "pnlTimyDayRight";
            this.pnlTimyDayRight.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyDayRight.Size = new System.Drawing.Size(353, 460);
            this.pnlTimyDayRight.TabIndex = 1;
            // 
            // dgvTimyDayTimezones
            // 
            this.dgvTimyDayTimezones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimyDayTimezones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDayTimezoneId,
            this.colDayStartHourMinute,
            this.colDayEndHourMinute});
            this.dgvTimyDayTimezones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTimyDayTimezones.Location = new System.Drawing.Point(3, 69);
            this.dgvTimyDayTimezones.Name = "dgvTimyDayTimezones";
            this.dgvTimyDayTimezones.Size = new System.Drawing.Size(347, 388);
            this.dgvTimyDayTimezones.TabIndex = 1;
            this.dgvTimyDayTimezones.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Timy_DataError);
            // 
            // colDayTimezoneId
            // 
            this.colDayTimezoneId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDayTimezoneId.DataPropertyName = "DayTimezoneId";
            this.colDayTimezoneId.HeaderText = "DayTimezoneId";
            this.colDayTimezoneId.Name = "colDayTimezoneId";
            // 
            // colDayStartHourMinute
            // 
            this.colDayStartHourMinute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDayStartHourMinute.DataPropertyName = "StartHourMinute";
            this.colDayStartHourMinute.HeaderText = "StartHourMinute";
            this.colDayStartHourMinute.Name = "colDayStartHourMinute";
            // 
            // colDayEndHourMinute
            // 
            this.colDayEndHourMinute.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDayEndHourMinute.DataPropertyName = "EndHourMinute";
            this.colDayEndHourMinute.HeaderText = "EndHourMinute";
            this.colDayEndHourMinute.Name = "colDayEndHourMinute";
            // 
            // pnlTimyDayHeader
            // 
            this.pnlTimyDayHeader.Controls.Add(this.lblTimyDayDeviceIndex);
            this.pnlTimyDayHeader.Controls.Add(this.numTimyDayDeviceIndex);
            this.pnlTimyDayHeader.Controls.Add(this.lblTimyDayTitle);
            this.pnlTimyDayHeader.Controls.Add(this.txtTimyDayTitle);
            this.pnlTimyDayHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTimyDayHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlTimyDayHeader.Name = "pnlTimyDayHeader";
            this.pnlTimyDayHeader.Size = new System.Drawing.Size(347, 66);
            this.pnlTimyDayHeader.TabIndex = 0;
            // 
            // lblTimyDayDeviceIndex
            // 
            this.lblTimyDayDeviceIndex.AutoSize = true;
            this.lblTimyDayDeviceIndex.Location = new System.Drawing.Point(3, 10);
            this.lblTimyDayDeviceIndex.Name = "lblTimyDayDeviceIndex";
            this.lblTimyDayDeviceIndex.Size = new System.Drawing.Size(70, 13);
            this.lblTimyDayDeviceIndex.TabIndex = 0;
            this.lblTimyDayDeviceIndex.Text = "DeviceIndex:";
            // 
            // numTimyDayDeviceIndex
            // 
            this.numTimyDayDeviceIndex.Location = new System.Drawing.Point(85, 7);
            this.numTimyDayDeviceIndex.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTimyDayDeviceIndex.Name = "numTimyDayDeviceIndex";
            this.numTimyDayDeviceIndex.Size = new System.Drawing.Size(90, 20);
            this.numTimyDayDeviceIndex.TabIndex = 1;
            this.numTimyDayDeviceIndex.ValueChanged += new System.EventHandler(this.NumTimyDayDeviceIndex_ValueChanged);
            // 
            // lblTimyDayTitle
            // 
            this.lblTimyDayTitle.AutoSize = true;
            this.lblTimyDayTitle.Location = new System.Drawing.Point(3, 40);
            this.lblTimyDayTitle.Name = "lblTimyDayTitle";
            this.lblTimyDayTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTimyDayTitle.TabIndex = 2;
            this.lblTimyDayTitle.Text = "Title:";
            // 
            // txtTimyDayTitle
            // 
            this.txtTimyDayTitle.Location = new System.Drawing.Point(85, 37);
            this.txtTimyDayTitle.Name = "txtTimyDayTitle";
            this.txtTimyDayTitle.Size = new System.Drawing.Size(255, 20);
            this.txtTimyDayTitle.TabIndex = 3;
            this.txtTimyDayTitle.TextChanged += new System.EventHandler(this.TxtTimyDayTitle_TextChanged);
            // 
            // pnlTimyDayLeft
            // 
            this.pnlTimyDayLeft.Controls.Add(this.lstTimyDayGroups);
            this.pnlTimyDayLeft.Controls.Add(this.pnlTimyDayButtons);
            this.pnlTimyDayLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTimyDayLeft.Location = new System.Drawing.Point(3, 3);
            this.pnlTimyDayLeft.Name = "pnlTimyDayLeft";
            this.pnlTimyDayLeft.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyDayLeft.Size = new System.Drawing.Size(150, 460);
            this.pnlTimyDayLeft.TabIndex = 0;
            // 
            // lstTimyDayGroups
            // 
            this.lstTimyDayGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTimyDayGroups.FormattingEnabled = true;
            this.lstTimyDayGroups.IntegralHeight = false;
            this.lstTimyDayGroups.Location = new System.Drawing.Point(3, 3);
            this.lstTimyDayGroups.Name = "lstTimyDayGroups";
            this.lstTimyDayGroups.Size = new System.Drawing.Size(144, 424);
            this.lstTimyDayGroups.TabIndex = 0;
            this.lstTimyDayGroups.SelectedIndexChanged += new System.EventHandler(this.LstTimyDayGroups_SelectedIndexChanged);
            // 
            // pnlTimyDayButtons
            // 
            this.pnlTimyDayButtons.Controls.Add(this.btnTimyDayAdd);
            this.pnlTimyDayButtons.Controls.Add(this.btnTimyDayRemove);
            this.pnlTimyDayButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTimyDayButtons.Location = new System.Drawing.Point(3, 427);
            this.pnlTimyDayButtons.Name = "pnlTimyDayButtons";
            this.pnlTimyDayButtons.Size = new System.Drawing.Size(144, 30);
            this.pnlTimyDayButtons.TabIndex = 1;
            // 
            // btnTimyDayAdd
            // 
            this.btnTimyDayAdd.Location = new System.Drawing.Point(0, 3);
            this.btnTimyDayAdd.Name = "btnTimyDayAdd";
            this.btnTimyDayAdd.Size = new System.Drawing.Size(68, 24);
            this.btnTimyDayAdd.TabIndex = 0;
            this.btnTimyDayAdd.Text = "Add";
            this.btnTimyDayAdd.UseVisualStyleBackColor = true;
            this.btnTimyDayAdd.Click += new System.EventHandler(this.BtnTimyDayAdd_Click);
            // 
            // btnTimyDayRemove
            // 
            this.btnTimyDayRemove.Location = new System.Drawing.Point(74, 3);
            this.btnTimyDayRemove.Name = "btnTimyDayRemove";
            this.btnTimyDayRemove.Size = new System.Drawing.Size(68, 24);
            this.btnTimyDayRemove.TabIndex = 1;
            this.btnTimyDayRemove.Text = "Remove";
            this.btnTimyDayRemove.UseVisualStyleBackColor = true;
            this.btnTimyDayRemove.Click += new System.EventHandler(this.BtnTimyDayRemove_Click);
            // 
            // tpTimyWeek
            // 
            this.tpTimyWeek.Controls.Add(this.pnlTimyWeekRight);
            this.tpTimyWeek.Controls.Add(this.pnlTimyWeekLeft);
            this.tpTimyWeek.Location = new System.Drawing.Point(4, 22);
            this.tpTimyWeek.Name = "tpTimyWeek";
            this.tpTimyWeek.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimyWeek.Size = new System.Drawing.Size(509, 466);
            this.tpTimyWeek.TabIndex = 1;
            this.tpTimyWeek.Text = "Week Timezone Groups";
            this.tpTimyWeek.UseVisualStyleBackColor = true;
            // 
            // pnlTimyWeekRight
            // 
            this.pnlTimyWeekRight.Controls.Add(this.dgvTimyWeekTimezones);
            this.pnlTimyWeekRight.Controls.Add(this.pnlTimyWeekHeader);
            this.pnlTimyWeekRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTimyWeekRight.Location = new System.Drawing.Point(153, 3);
            this.pnlTimyWeekRight.Name = "pnlTimyWeekRight";
            this.pnlTimyWeekRight.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyWeekRight.Size = new System.Drawing.Size(353, 460);
            this.pnlTimyWeekRight.TabIndex = 1;
            // 
            // dgvTimyWeekTimezones
            // 
            this.dgvTimyWeekTimezones.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimyWeekTimezones.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWeekDay,
            this.colWeekDayTimezoneIndex});
            this.dgvTimyWeekTimezones.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTimyWeekTimezones.Location = new System.Drawing.Point(3, 69);
            this.dgvTimyWeekTimezones.Name = "dgvTimyWeekTimezones";
            this.dgvTimyWeekTimezones.Size = new System.Drawing.Size(347, 388);
            this.dgvTimyWeekTimezones.TabIndex = 1;
            this.dgvTimyWeekTimezones.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.Timy_DataError);
            // 
            // colWeekDay
            // 
            this.colWeekDay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colWeekDay.DataPropertyName = "WeekDay";
            this.colWeekDay.HeaderText = "WeekDay";
            this.colWeekDay.Name = "colWeekDay";
            // 
            // colWeekDayTimezoneIndex
            // 
            this.colWeekDayTimezoneIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colWeekDayTimezoneIndex.DataPropertyName = "DayTimezoneIndex";
            this.colWeekDayTimezoneIndex.HeaderText = "DayTimezone";
            this.colWeekDayTimezoneIndex.Name = "colWeekDayTimezoneIndex";
            // 
            // pnlTimyWeekHeader
            // 
            this.pnlTimyWeekHeader.Controls.Add(this.lblTimyWeekDeviceIndex);
            this.pnlTimyWeekHeader.Controls.Add(this.numTimyWeekDeviceIndex);
            this.pnlTimyWeekHeader.Controls.Add(this.lblTimyWeekTitle);
            this.pnlTimyWeekHeader.Controls.Add(this.txtTimyWeekTitle);
            this.pnlTimyWeekHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTimyWeekHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlTimyWeekHeader.Name = "pnlTimyWeekHeader";
            this.pnlTimyWeekHeader.Size = new System.Drawing.Size(347, 66);
            this.pnlTimyWeekHeader.TabIndex = 0;
            // 
            // lblTimyWeekDeviceIndex
            // 
            this.lblTimyWeekDeviceIndex.AutoSize = true;
            this.lblTimyWeekDeviceIndex.Location = new System.Drawing.Point(3, 10);
            this.lblTimyWeekDeviceIndex.Name = "lblTimyWeekDeviceIndex";
            this.lblTimyWeekDeviceIndex.Size = new System.Drawing.Size(70, 13);
            this.lblTimyWeekDeviceIndex.TabIndex = 0;
            this.lblTimyWeekDeviceIndex.Text = "DeviceIndex:";
            // 
            // numTimyWeekDeviceIndex
            // 
            this.numTimyWeekDeviceIndex.Location = new System.Drawing.Point(85, 7);
            this.numTimyWeekDeviceIndex.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTimyWeekDeviceIndex.Name = "numTimyWeekDeviceIndex";
            this.numTimyWeekDeviceIndex.Size = new System.Drawing.Size(90, 20);
            this.numTimyWeekDeviceIndex.TabIndex = 1;
            this.numTimyWeekDeviceIndex.ValueChanged += new System.EventHandler(this.NumTimyWeekDeviceIndex_ValueChanged);
            // 
            // lblTimyWeekTitle
            // 
            this.lblTimyWeekTitle.AutoSize = true;
            this.lblTimyWeekTitle.Location = new System.Drawing.Point(3, 40);
            this.lblTimyWeekTitle.Name = "lblTimyWeekTitle";
            this.lblTimyWeekTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTimyWeekTitle.TabIndex = 2;
            this.lblTimyWeekTitle.Text = "Title:";
            // 
            // txtTimyWeekTitle
            // 
            this.txtTimyWeekTitle.Location = new System.Drawing.Point(85, 37);
            this.txtTimyWeekTitle.Name = "txtTimyWeekTitle";
            this.txtTimyWeekTitle.Size = new System.Drawing.Size(255, 20);
            this.txtTimyWeekTitle.TabIndex = 3;
            this.txtTimyWeekTitle.TextChanged += new System.EventHandler(this.TxtTimyWeekTitle_TextChanged);
            // 
            // pnlTimyWeekLeft
            // 
            this.pnlTimyWeekLeft.Controls.Add(this.lstTimyWeekGroups);
            this.pnlTimyWeekLeft.Controls.Add(this.pnlTimyWeekButtons);
            this.pnlTimyWeekLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTimyWeekLeft.Location = new System.Drawing.Point(3, 3);
            this.pnlTimyWeekLeft.Name = "pnlTimyWeekLeft";
            this.pnlTimyWeekLeft.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyWeekLeft.Size = new System.Drawing.Size(150, 460);
            this.pnlTimyWeekLeft.TabIndex = 0;
            // 
            // lstTimyWeekGroups
            // 
            this.lstTimyWeekGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTimyWeekGroups.FormattingEnabled = true;
            this.lstTimyWeekGroups.IntegralHeight = false;
            this.lstTimyWeekGroups.Location = new System.Drawing.Point(3, 3);
            this.lstTimyWeekGroups.Name = "lstTimyWeekGroups";
            this.lstTimyWeekGroups.Size = new System.Drawing.Size(144, 424);
            this.lstTimyWeekGroups.TabIndex = 0;
            this.lstTimyWeekGroups.SelectedIndexChanged += new System.EventHandler(this.LstTimyWeekGroups_SelectedIndexChanged);
            // 
            // pnlTimyWeekButtons
            // 
            this.pnlTimyWeekButtons.Controls.Add(this.btnTimyWeekAdd);
            this.pnlTimyWeekButtons.Controls.Add(this.btnTimyWeekRemove);
            this.pnlTimyWeekButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTimyWeekButtons.Location = new System.Drawing.Point(3, 427);
            this.pnlTimyWeekButtons.Name = "pnlTimyWeekButtons";
            this.pnlTimyWeekButtons.Size = new System.Drawing.Size(144, 30);
            this.pnlTimyWeekButtons.TabIndex = 1;
            // 
            // btnTimyWeekAdd
            // 
            this.btnTimyWeekAdd.Location = new System.Drawing.Point(0, 3);
            this.btnTimyWeekAdd.Name = "btnTimyWeekAdd";
            this.btnTimyWeekAdd.Size = new System.Drawing.Size(68, 24);
            this.btnTimyWeekAdd.TabIndex = 0;
            this.btnTimyWeekAdd.Text = "Add";
            this.btnTimyWeekAdd.UseVisualStyleBackColor = true;
            this.btnTimyWeekAdd.Click += new System.EventHandler(this.BtnTimyWeekAdd_Click);
            // 
            // btnTimyWeekRemove
            // 
            this.btnTimyWeekRemove.Location = new System.Drawing.Point(74, 3);
            this.btnTimyWeekRemove.Name = "btnTimyWeekRemove";
            this.btnTimyWeekRemove.Size = new System.Drawing.Size(68, 24);
            this.btnTimyWeekRemove.TabIndex = 1;
            this.btnTimyWeekRemove.Text = "Remove";
            this.btnTimyWeekRemove.UseVisualStyleBackColor = true;
            this.btnTimyWeekRemove.Click += new System.EventHandler(this.BtnTimyWeekRemove_Click);
            // 
            // tpTimySetForUser
            // 
            this.tpTimySetForUser.Controls.Add(this.dtUserEndTime);
            this.tpTimySetForUser.Controls.Add(this.lblUserEndTime);
            this.tpTimySetForUser.Controls.Add(this.dtUserStartTime);
            this.tpTimySetForUser.Controls.Add(this.lblUserStartTime);
            this.tpTimySetForUser.Controls.Add(this.cmbUserWeekzone);
            this.tpTimySetForUser.Controls.Add(this.lblUserWeekzone);
            this.tpTimySetForUser.Controls.Add(this.txtEnrollId);
            this.tpTimySetForUser.Controls.Add(this.lblEnrollId);
            this.tpTimySetForUser.Location = new System.Drawing.Point(4, 22);
            this.tpTimySetForUser.Name = "tpTimySetForUser";
            this.tpTimySetForUser.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimySetForUser.Size = new System.Drawing.Size(509, 466);
            this.tpTimySetForUser.TabIndex = 3;
            this.tpTimySetForUser.Text = "Set For User";
            this.tpTimySetForUser.UseVisualStyleBackColor = true;
            // 
            // dtUserEndTime
            // 
            this.dtUserEndTime.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtUserEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtUserEndTime.Location = new System.Drawing.Point(120, 123);
            this.dtUserEndTime.Name = "dtUserEndTime";
            this.dtUserEndTime.Size = new System.Drawing.Size(200, 20);
            this.dtUserEndTime.TabIndex = 7;
            // 
            // lblUserEndTime
            // 
            this.lblUserEndTime.AutoSize = true;
            this.lblUserEndTime.Location = new System.Drawing.Point(15, 128);
            this.lblUserEndTime.Name = "lblUserEndTime";
            this.lblUserEndTime.Size = new System.Drawing.Size(52, 13);
            this.lblUserEndTime.TabIndex = 6;
            this.lblUserEndTime.Text = "End Time";
            // 
            // dtUserStartTime
            // 
            this.dtUserStartTime.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtUserStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtUserStartTime.Location = new System.Drawing.Point(120, 88);
            this.dtUserStartTime.Name = "dtUserStartTime";
            this.dtUserStartTime.Size = new System.Drawing.Size(200, 20);
            this.dtUserStartTime.TabIndex = 5;
            // 
            // lblUserStartTime
            // 
            this.lblUserStartTime.AutoSize = true;
            this.lblUserStartTime.Location = new System.Drawing.Point(15, 93);
            this.lblUserStartTime.Name = "lblUserStartTime";
            this.lblUserStartTime.Size = new System.Drawing.Size(55, 13);
            this.lblUserStartTime.TabIndex = 4;
            this.lblUserStartTime.Text = "Start Time";
            // 
            // cmbUserWeekzone
            // 
            this.cmbUserWeekzone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserWeekzone.FormattingEnabled = true;
            this.cmbUserWeekzone.Location = new System.Drawing.Point(120, 55);
            this.cmbUserWeekzone.Name = "cmbUserWeekzone";
            this.cmbUserWeekzone.Size = new System.Drawing.Size(200, 21);
            this.cmbUserWeekzone.TabIndex = 3;
            // 
            // lblUserWeekzone
            // 
            this.lblUserWeekzone.AutoSize = true;
            this.lblUserWeekzone.Location = new System.Drawing.Point(15, 58);
            this.lblUserWeekzone.Name = "lblUserWeekzone";
            this.lblUserWeekzone.Size = new System.Drawing.Size(59, 13);
            this.lblUserWeekzone.TabIndex = 2;
            this.lblUserWeekzone.Text = "Weekzone";
            // 
            // txtEnrollId
            // 
            this.txtEnrollId.Location = new System.Drawing.Point(120, 20);
            this.txtEnrollId.Name = "txtEnrollId";
            this.txtEnrollId.Size = new System.Drawing.Size(200, 20);
            this.txtEnrollId.TabIndex = 1;
            // 
            // lblEnrollId
            // 
            this.lblEnrollId.AutoSize = true;
            this.lblEnrollId.Location = new System.Drawing.Point(15, 23);
            this.lblEnrollId.Name = "lblEnrollId";
            this.lblEnrollId.Size = new System.Drawing.Size(47, 13);
            this.lblEnrollId.TabIndex = 0;
            this.lblEnrollId.Text = "Enroll ID";
            // 
            // tpTimyHoliday
            // 
            this.tpTimyHoliday.Controls.Add(this.pnlTimyHolidayRight);
            this.tpTimyHoliday.Controls.Add(this.pnlTimyHolidayLeft);
            this.tpTimyHoliday.Location = new System.Drawing.Point(4, 22);
            this.tpTimyHoliday.Name = "tpTimyHoliday";
            this.tpTimyHoliday.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimyHoliday.Size = new System.Drawing.Size(509, 466);
            this.tpTimyHoliday.TabIndex = 4;
            this.tpTimyHoliday.Text = "Holiday List";
            this.tpTimyHoliday.UseVisualStyleBackColor = true;
            // 
            // pnlTimyHolidayRight
            // 
            this.pnlTimyHolidayRight.Controls.Add(this.cmbTimyHolidayDayTimezone);
            this.pnlTimyHolidayRight.Controls.Add(this.lblTimyHolidayDayTimezone);
            this.pnlTimyHolidayRight.Controls.Add(this.dtTimyHolidayEndDate);
            this.pnlTimyHolidayRight.Controls.Add(this.lblTimyHolidayEndDate);
            this.pnlTimyHolidayRight.Controls.Add(this.dtTimyHolidayStartDate);
            this.pnlTimyHolidayRight.Controls.Add(this.lblTimyHolidayStartDate);
            this.pnlTimyHolidayRight.Controls.Add(this.txtTimyHolidayTitle);
            this.pnlTimyHolidayRight.Controls.Add(this.lblTimyHolidayTitle);
            this.pnlTimyHolidayRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTimyHolidayRight.Location = new System.Drawing.Point(153, 3);
            this.pnlTimyHolidayRight.Name = "pnlTimyHolidayRight";
            this.pnlTimyHolidayRight.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyHolidayRight.Size = new System.Drawing.Size(353, 460);
            this.pnlTimyHolidayRight.TabIndex = 1;
            // 
            // cmbTimyHolidayDayTimezone
            // 
            this.cmbTimyHolidayDayTimezone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimyHolidayDayTimezone.FormattingEnabled = true;
            this.cmbTimyHolidayDayTimezone.Location = new System.Drawing.Point(110, 122);
            this.cmbTimyHolidayDayTimezone.Name = "cmbTimyHolidayDayTimezone";
            this.cmbTimyHolidayDayTimezone.Size = new System.Drawing.Size(220, 21);
            this.cmbTimyHolidayDayTimezone.TabIndex = 7;
            this.cmbTimyHolidayDayTimezone.SelectedIndexChanged += new System.EventHandler(this.CmbTimyHolidayDayTimezone_SelectedIndexChanged);
            // 
            // lblTimyHolidayDayTimezone
            // 
            this.lblTimyHolidayDayTimezone.AutoSize = true;
            this.lblTimyHolidayDayTimezone.Location = new System.Drawing.Point(12, 125);
            this.lblTimyHolidayDayTimezone.Name = "lblTimyHolidayDayTimezone";
            this.lblTimyHolidayDayTimezone.Size = new System.Drawing.Size(72, 13);
            this.lblTimyHolidayDayTimezone.TabIndex = 6;
            this.lblTimyHolidayDayTimezone.Text = "DayTimezone";
            // 
            // dtTimyHolidayEndDate
            // 
            this.dtTimyHolidayEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTimyHolidayEndDate.Location = new System.Drawing.Point(110, 86);
            this.dtTimyHolidayEndDate.Name = "dtTimyHolidayEndDate";
            this.dtTimyHolidayEndDate.Size = new System.Drawing.Size(150, 20);
            this.dtTimyHolidayEndDate.TabIndex = 5;
            this.dtTimyHolidayEndDate.ValueChanged += new System.EventHandler(this.DtTimyHolidayEndDate_ValueChanged);
            // 
            // lblTimyHolidayEndDate
            // 
            this.lblTimyHolidayEndDate.AutoSize = true;
            this.lblTimyHolidayEndDate.Location = new System.Drawing.Point(12, 90);
            this.lblTimyHolidayEndDate.Name = "lblTimyHolidayEndDate";
            this.lblTimyHolidayEndDate.Size = new System.Drawing.Size(52, 13);
            this.lblTimyHolidayEndDate.TabIndex = 4;
            this.lblTimyHolidayEndDate.Text = "End Date";
            // 
            // dtTimyHolidayStartDate
            // 
            this.dtTimyHolidayStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTimyHolidayStartDate.Location = new System.Drawing.Point(110, 51);
            this.dtTimyHolidayStartDate.Name = "dtTimyHolidayStartDate";
            this.dtTimyHolidayStartDate.Size = new System.Drawing.Size(150, 20);
            this.dtTimyHolidayStartDate.TabIndex = 3;
            this.dtTimyHolidayStartDate.ValueChanged += new System.EventHandler(this.DtTimyHolidayStartDate_ValueChanged);
            // 
            // lblTimyHolidayStartDate
            // 
            this.lblTimyHolidayStartDate.AutoSize = true;
            this.lblTimyHolidayStartDate.Location = new System.Drawing.Point(12, 55);
            this.lblTimyHolidayStartDate.Name = "lblTimyHolidayStartDate";
            this.lblTimyHolidayStartDate.Size = new System.Drawing.Size(55, 13);
            this.lblTimyHolidayStartDate.TabIndex = 2;
            this.lblTimyHolidayStartDate.Text = "Start Date";
            // 
            // txtTimyHolidayTitle
            // 
            this.txtTimyHolidayTitle.Location = new System.Drawing.Point(110, 17);
            this.txtTimyHolidayTitle.Name = "txtTimyHolidayTitle";
            this.txtTimyHolidayTitle.Size = new System.Drawing.Size(220, 20);
            this.txtTimyHolidayTitle.TabIndex = 1;
            this.txtTimyHolidayTitle.TextChanged += new System.EventHandler(this.TxtTimyHolidayTitle_TextChanged);
            // 
            // lblTimyHolidayTitle
            // 
            this.lblTimyHolidayTitle.AutoSize = true;
            this.lblTimyHolidayTitle.Location = new System.Drawing.Point(12, 20);
            this.lblTimyHolidayTitle.Name = "lblTimyHolidayTitle";
            this.lblTimyHolidayTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTimyHolidayTitle.TabIndex = 0;
            this.lblTimyHolidayTitle.Text = "Title";
            // 
            // pnlTimyHolidayLeft
            // 
            this.pnlTimyHolidayLeft.Controls.Add(this.lstTimyHolidays);
            this.pnlTimyHolidayLeft.Controls.Add(this.pnlTimyHolidayButtons);
            this.pnlTimyHolidayLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTimyHolidayLeft.Location = new System.Drawing.Point(3, 3);
            this.pnlTimyHolidayLeft.Name = "pnlTimyHolidayLeft";
            this.pnlTimyHolidayLeft.Padding = new System.Windows.Forms.Padding(3);
            this.pnlTimyHolidayLeft.Size = new System.Drawing.Size(150, 460);
            this.pnlTimyHolidayLeft.TabIndex = 0;
            // 
            // lstTimyHolidays
            // 
            this.lstTimyHolidays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTimyHolidays.FormattingEnabled = true;
            this.lstTimyHolidays.IntegralHeight = false;
            this.lstTimyHolidays.Location = new System.Drawing.Point(3, 3);
            this.lstTimyHolidays.Name = "lstTimyHolidays";
            this.lstTimyHolidays.Size = new System.Drawing.Size(144, 424);
            this.lstTimyHolidays.TabIndex = 0;
            this.lstTimyHolidays.SelectedIndexChanged += new System.EventHandler(this.LstTimyHolidays_SelectedIndexChanged);
            // 
            // pnlTimyHolidayButtons
            // 
            this.pnlTimyHolidayButtons.Controls.Add(this.btnTimyHolidayAdd);
            this.pnlTimyHolidayButtons.Controls.Add(this.btnTimyHolidayRemove);
            this.pnlTimyHolidayButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTimyHolidayButtons.Location = new System.Drawing.Point(3, 427);
            this.pnlTimyHolidayButtons.Name = "pnlTimyHolidayButtons";
            this.pnlTimyHolidayButtons.Size = new System.Drawing.Size(144, 30);
            this.pnlTimyHolidayButtons.TabIndex = 1;
            // 
            // btnTimyHolidayAdd
            // 
            this.btnTimyHolidayAdd.Location = new System.Drawing.Point(0, 3);
            this.btnTimyHolidayAdd.Name = "btnTimyHolidayAdd";
            this.btnTimyHolidayAdd.Size = new System.Drawing.Size(68, 24);
            this.btnTimyHolidayAdd.TabIndex = 0;
            this.btnTimyHolidayAdd.Text = "Add";
            this.btnTimyHolidayAdd.UseVisualStyleBackColor = true;
            this.btnTimyHolidayAdd.Click += new System.EventHandler(this.BtnTimyHolidayAdd_Click);
            // 
            // btnTimyHolidayRemove
            // 
            this.btnTimyHolidayRemove.Location = new System.Drawing.Point(74, 3);
            this.btnTimyHolidayRemove.Name = "btnTimyHolidayRemove";
            this.btnTimyHolidayRemove.Size = new System.Drawing.Size(68, 24);
            this.btnTimyHolidayRemove.TabIndex = 1;
            this.btnTimyHolidayRemove.Text = "Remove";
            this.btnTimyHolidayRemove.UseVisualStyleBackColor = true;
            this.btnTimyHolidayRemove.Click += new System.EventHandler(this.BtnTimyHolidayRemove_Click);
            // 
            // tpTimyJsonOutput
            // 
            this.tpTimyJsonOutput.Controls.Add(this.txtTimyJson);
            this.tpTimyJsonOutput.Location = new System.Drawing.Point(4, 22);
            this.tpTimyJsonOutput.Name = "tpTimyJsonOutput";
            this.tpTimyJsonOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpTimyJsonOutput.Size = new System.Drawing.Size(509, 466);
            this.tpTimyJsonOutput.TabIndex = 2;
            this.tpTimyJsonOutput.Text = "JSON Output";
            this.tpTimyJsonOutput.UseVisualStyleBackColor = true;
            // 
            // txtTimyJson
            // 
            this.txtTimyJson.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTimyJson.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtTimyJson.Location = new System.Drawing.Point(3, 3);
            this.txtTimyJson.Multiline = true;
            this.txtTimyJson.Name = "txtTimyJson";
            this.txtTimyJson.ReadOnly = true;
            this.txtTimyJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTimyJson.Size = new System.Drawing.Size(503, 460);
            this.txtTimyJson.TabIndex = 0;
            this.txtTimyJson.WordWrap = false;
            // 
            // pnlTimyBottom
            // 
            this.pnlTimyBottom.Controls.Add(this.btnTimyHoliday);
            this.pnlTimyBottom.Controls.Add(this.btnTimySetPersTimezone);
            this.pnlTimyBottom.Controls.Add(this.btnAddWeekTimezone);
            this.pnlTimyBottom.Controls.Add(this.lblTimyDeviceNumber);
            this.pnlTimyBottom.Controls.Add(this.txtTimyDeviceNumber);
            this.pnlTimyBottom.Controls.Add(this.btnAddDayTimezone);
            this.pnlTimyBottom.Controls.Add(this.btnSave);
            this.pnlTimyBottom.Controls.Add(this.btnTimyGenerateJson);
            this.pnlTimyBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTimyBottom.Location = new System.Drawing.Point(3, 495);
            this.pnlTimyBottom.Name = "pnlTimyBottom";
            this.pnlTimyBottom.Size = new System.Drawing.Size(517, 40);
            this.pnlTimyBottom.TabIndex = 1;
            // 
            // btnTimySetPersTimezone
            // 
            this.btnTimySetPersTimezone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimySetPersTimezone.Location = new System.Drawing.Point(229, 7);
            this.btnTimySetPersTimezone.Name = "btnTimySetPersTimezone";
            this.btnTimySetPersTimezone.Size = new System.Drawing.Size(56, 26);
            this.btnTimySetPersTimezone.TabIndex = 6;
            this.btnTimySetPersTimezone.Text = "Pest TZ";
            this.btnTimySetPersTimezone.UseVisualStyleBackColor = true;
            this.btnTimySetPersTimezone.Click += new System.EventHandler(this.btnTimySetPersTimezone_Click);
            // 
            // btnAddWeekTimezone
            // 
            this.btnAddWeekTimezone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddWeekTimezone.Location = new System.Drawing.Point(374, 7);
            this.btnAddWeekTimezone.Name = "btnAddWeekTimezone";
            this.btnAddWeekTimezone.Size = new System.Drawing.Size(44, 26);
            this.btnAddWeekTimezone.TabIndex = 4;
            this.btnAddWeekTimezone.Text = "Week";
            this.btnAddWeekTimezone.UseVisualStyleBackColor = true;
            this.btnAddWeekTimezone.Click += new System.EventHandler(this.btnAddWeekTimezone_Click);
            // 
            // lblTimyDeviceNumber
            // 
            this.lblTimyDeviceNumber.AutoSize = true;
            this.lblTimyDeviceNumber.Location = new System.Drawing.Point(3, 13);
            this.lblTimyDeviceNumber.Name = "lblTimyDeviceNumber";
            this.lblTimyDeviceNumber.Size = new System.Drawing.Size(84, 13);
            this.lblTimyDeviceNumber.TabIndex = 0;
            this.lblTimyDeviceNumber.Text = "Device Number:";
            // 
            // txtTimyDeviceNumber
            // 
            this.txtTimyDeviceNumber.Location = new System.Drawing.Point(91, 10);
            this.txtTimyDeviceNumber.Name = "txtTimyDeviceNumber";
            this.txtTimyDeviceNumber.Size = new System.Drawing.Size(63, 20);
            this.txtTimyDeviceNumber.TabIndex = 1;
            // 
            // btnAddDayTimezone
            // 
            this.btnAddDayTimezone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDayTimezone.Location = new System.Drawing.Point(419, 7);
            this.btnAddDayTimezone.Name = "btnAddDayTimezone";
            this.btnAddDayTimezone.Size = new System.Drawing.Size(43, 26);
            this.btnAddDayTimezone.TabIndex = 2;
            this.btnAddDayTimezone.Text = "Day";
            this.btnAddDayTimezone.UseVisualStyleBackColor = true;
            this.btnAddDayTimezone.Click += new System.EventHandler(this.btnAddDayTimezone_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(177, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 26);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnTimyGenerateJson
            // 
            this.btnTimyGenerateJson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimyGenerateJson.Location = new System.Drawing.Point(465, 7);
            this.btnTimyGenerateJson.Name = "btnTimyGenerateJson";
            this.btnTimyGenerateJson.Size = new System.Drawing.Size(47, 26);
            this.btnTimyGenerateJson.TabIndex = 3;
            this.btnTimyGenerateJson.Text = "JSON";
            this.btnTimyGenerateJson.UseVisualStyleBackColor = true;
            this.btnTimyGenerateJson.Click += new System.EventHandler(this.BtnTimyGenerateJson_Click);
            // 
            // btnTimyHoliday
            // 
            this.btnTimyHoliday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimyHoliday.Location = new System.Drawing.Point(319, 7);
            this.btnTimyHoliday.Name = "btnTimyHoliday";
            this.btnTimyHoliday.Size = new System.Drawing.Size(54, 26);
            this.btnTimyHoliday.TabIndex = 7;
            this.btnTimyHoliday.Text = "Holiday";
            this.btnTimyHoliday.UseVisualStyleBackColor = true;
            this.btnTimyHoliday.Click += new System.EventHandler(this.btnTimyHoliday_Click);
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
            this.tpCamera.ResumeLayout(false);
            this.tpCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCameraLastImage)).EndInit();
            this.tpSelfTools.ResumeLayout(false);
            this.gbSelfBillInfoPrint.ResumeLayout(false);
            this.gbSelfBillInfoPrint.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSelfPrintInfoFood3.ResumeLayout(false);
            this.gbSelfPrintInfoFood3.PerformLayout();
            this.gbSelfPrintInfoFood2.ResumeLayout(false);
            this.gbSelfPrintInfoFood2.PerformLayout();
            this.gbSelfPrintInfoFood1.ResumeLayout(false);
            this.gbSelfPrintInfoFood1.PerformLayout();
            this.gbSelfToolsFoodTitles.ResumeLayout(false);
            this.gbSelfToolsFoodTitles.PerformLayout();
            this.tpAttendance.ResumeLayout(false);
            this.tpAttendance.PerformLayout();
            this.tpTimyAccess.ResumeLayout(false);
            this.tcTimyInner.ResumeLayout(false);
            this.tpTimyDay.ResumeLayout(false);
            this.pnlTimyDayRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimyDayTimezones)).EndInit();
            this.pnlTimyDayHeader.ResumeLayout(false);
            this.pnlTimyDayHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimyDayDeviceIndex)).EndInit();
            this.pnlTimyDayLeft.ResumeLayout(false);
            this.pnlTimyDayButtons.ResumeLayout(false);
            this.tpTimyWeek.ResumeLayout(false);
            this.pnlTimyWeekRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimyWeekTimezones)).EndInit();
            this.pnlTimyWeekHeader.ResumeLayout(false);
            this.pnlTimyWeekHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimyWeekDeviceIndex)).EndInit();
            this.pnlTimyWeekLeft.ResumeLayout(false);
            this.pnlTimyWeekButtons.ResumeLayout(false);
            this.tpTimySetForUser.ResumeLayout(false);
            this.tpTimySetForUser.PerformLayout();
            this.tpTimyHoliday.ResumeLayout(false);
            this.pnlTimyHolidayRight.ResumeLayout(false);
            this.pnlTimyHolidayRight.PerformLayout();
            this.pnlTimyHolidayLeft.ResumeLayout(false);
            this.pnlTimyHolidayButtons.ResumeLayout(false);
            this.tpTimyJsonOutput.ResumeLayout(false);
            this.tpTimyJsonOutput.PerformLayout();
            this.pnlTimyBottom.ResumeLayout(false);
            this.pnlTimyBottom.PerformLayout();
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
        private System.Windows.Forms.TabPage tpCamera;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnShowLatestImage;
        private System.Windows.Forms.PictureBox pbCameraLastImage;
        private System.Windows.Forms.Button btnDoStopProcess;
        private System.Windows.Forms.TabPage tpSelfTools;
        private System.Windows.Forms.Button btnSendFoodTitles;
        private System.Windows.Forms.Label lblSendFoodTitles;
        private System.Windows.Forms.TextBox txtSendFoodTitles;
        private System.Windows.Forms.TextBox txtSendFoodTitlesDeviceNumber;
        private System.Windows.Forms.Button btnDisableFoodTitles;
        private System.Windows.Forms.GroupBox gbSelfToolsFoodTitles;
        private System.Windows.Forms.GroupBox gbSelfBillInfoPrint;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoDirect;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodTitle1;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodType1;
        private System.Windows.Forms.TextBox txtEmployeeTitle;
        private System.Windows.Forms.Label lblEmployeeTitle;
        private System.Windows.Forms.TextBox txtPrinterIp;
        private System.Windows.Forms.Label lvlPrinterIp;
        private System.Windows.Forms.TextBox txtPrinterName;
        private System.Windows.Forms.Label lblPrinterName;
        private System.Windows.Forms.TextBox txtFishNumber;
        private System.Windows.Forms.Label lblFishNumber;
        private System.Windows.Forms.TextBox txtManualFishString;
        private System.Windows.Forms.Label lblManualFishString;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.TextBox txtSelfPrintInfoHeaderTitle1;
        private System.Windows.Forms.Label lblSelfPrintInfoHeaderTitle1;
        private System.Windows.Forms.TextBox txtSelfPrintInfoHeaderTitle2;
        private System.Windows.Forms.Label lblSelfPrintInfoHeaderTitle2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoHeaderTitle4;
        private System.Windows.Forms.Label lblSelfPrintInfoHeaderTitle4;
        private System.Windows.Forms.TextBox txtSelfPrintInfoHeaderTitle3;
        private System.Windows.Forms.Label lblSelfPrintInfoHeaderTitle3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodType1;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFishCount1;
        private System.Windows.Forms.Label lblSelfPrintInfoFishCount1;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodPrice1;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodPrice1;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodTitle1;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodDescription1;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodDescription1;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodDescription3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodDescription3;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodTitle3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodPrice3;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodPrice3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFishCount3;
        private System.Windows.Forms.Label lblSelfPrintInfoFishCount3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodType3;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodTitle3;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodType3;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodDescription2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodDescription2;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodTitle2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodPrice2;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodPrice2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFishCount2;
        private System.Windows.Forms.Label lblSelfPrintInfoFishCount2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodType2;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodTitle2;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodType2;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoFoodIsAllowed1;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoFoodIsAllowed3;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoFoodIsAllowed2;
        private System.Windows.Forms.GroupBox gbSelfPrintInfoFood3;
        private System.Windows.Forms.GroupBox gbSelfPrintInfoFood2;
        private System.Windows.Forms.GroupBox gbSelfPrintInfoFood1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtSelfPrintInfoFoodSeparator;
        private System.Windows.Forms.Label lblSelfPrintInfoFoodSeparator;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearTitle3;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearTitle4;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearFood2;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearFood3;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearManual;
        private System.Windows.Forms.CheckBox chkSelfPrintInfoClearTitle2;
        private System.Windows.Forms.Button btnSelfPrintInfoClear;
        private System.Windows.Forms.TabPage tpAttendance;
        private System.Windows.Forms.Label lblAttendanceToolsDeviceCount;
        private System.Windows.Forms.Button btnAttendanceToolsSaveAttendanceByComponent;
        private System.Windows.Forms.TextBox txtAttendanceToolsDeviceCount;
        private System.Windows.Forms.TextBox txtAttendanceToolsAttendanceCount;
        private System.Windows.Forms.Label lblAttendanceToolsAttendanceCount;
        private System.Windows.Forms.TextBox txtAttendanceToolsDelay;
        private System.Windows.Forms.Label lblAttendanceToolsDelay;
        private System.Windows.Forms.Button btnAttendanceToolsSaveAttendanceByPublisher;
        private System.Windows.Forms.TabPage tpTimyAccess;
        private System.Windows.Forms.TabControl tcTimyInner;
        private System.Windows.Forms.TabPage tpTimyDay;
        private System.Windows.Forms.Panel pnlTimyDayRight;
        private System.Windows.Forms.DataGridView dgvTimyDayTimezones;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDayTimezoneId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDayStartHourMinute;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDayEndHourMinute;
        private System.Windows.Forms.Panel pnlTimyDayHeader;
        private System.Windows.Forms.Label lblTimyDayDeviceIndex;
        private System.Windows.Forms.NumericUpDown numTimyDayDeviceIndex;
        private System.Windows.Forms.Label lblTimyDayTitle;
        private System.Windows.Forms.TextBox txtTimyDayTitle;
        private System.Windows.Forms.Panel pnlTimyDayLeft;
        private System.Windows.Forms.ListBox lstTimyDayGroups;
        private System.Windows.Forms.Panel pnlTimyDayButtons;
        private System.Windows.Forms.Button btnTimyDayAdd;
        private System.Windows.Forms.Button btnTimyDayRemove;
        private System.Windows.Forms.TabPage tpTimyWeek;
        private System.Windows.Forms.Panel pnlTimyWeekRight;
        private System.Windows.Forms.DataGridView dgvTimyWeekTimezones;
        private System.Windows.Forms.DataGridViewComboBoxColumn colWeekDay;
        private System.Windows.Forms.DataGridViewComboBoxColumn colWeekDayTimezoneIndex;
        private System.Windows.Forms.Panel pnlTimyWeekHeader;
        private System.Windows.Forms.Label lblTimyWeekDeviceIndex;
        private System.Windows.Forms.NumericUpDown numTimyWeekDeviceIndex;
        private System.Windows.Forms.Label lblTimyWeekTitle;
        private System.Windows.Forms.TextBox txtTimyWeekTitle;
        private System.Windows.Forms.Panel pnlTimyWeekLeft;
        private System.Windows.Forms.ListBox lstTimyWeekGroups;
        private System.Windows.Forms.Panel pnlTimyWeekButtons;
        private System.Windows.Forms.Button btnTimyWeekAdd;
        private System.Windows.Forms.Button btnTimyWeekRemove;
        private System.Windows.Forms.TabPage tpTimyJsonOutput;
        private System.Windows.Forms.TextBox txtTimyJson;
        private System.Windows.Forms.TabPage tpTimySetForUser;
        private System.Windows.Forms.Label lblEnrollId;
        private System.Windows.Forms.TextBox txtEnrollId;
        private System.Windows.Forms.Label lblUserWeekzone;
        private System.Windows.Forms.ComboBox cmbUserWeekzone;
        private System.Windows.Forms.Label lblUserStartTime;
        private System.Windows.Forms.DateTimePicker dtUserStartTime;
        private System.Windows.Forms.Label lblUserEndTime;
        private System.Windows.Forms.DateTimePicker dtUserEndTime;
        private System.Windows.Forms.TabPage tpTimyHoliday;
        private System.Windows.Forms.Panel pnlTimyHolidayRight;
        private System.Windows.Forms.Label lblTimyHolidayTitle;
        private System.Windows.Forms.TextBox txtTimyHolidayTitle;
        private System.Windows.Forms.Label lblTimyHolidayStartDate;
        private System.Windows.Forms.DateTimePicker dtTimyHolidayStartDate;
        private System.Windows.Forms.Label lblTimyHolidayEndDate;
        private System.Windows.Forms.DateTimePicker dtTimyHolidayEndDate;
        private System.Windows.Forms.Label lblTimyHolidayDayTimezone;
        private System.Windows.Forms.ComboBox cmbTimyHolidayDayTimezone;
        private System.Windows.Forms.Panel pnlTimyHolidayLeft;
        private System.Windows.Forms.ListBox lstTimyHolidays;
        private System.Windows.Forms.Panel pnlTimyHolidayButtons;
        private System.Windows.Forms.Button btnTimyHolidayAdd;
        private System.Windows.Forms.Button btnTimyHolidayRemove;
        private System.Windows.Forms.Panel pnlTimyBottom;
        private System.Windows.Forms.Label lblTimyDeviceNumber;
        private System.Windows.Forms.TextBox txtTimyDeviceNumber;
        private System.Windows.Forms.Button btnAddDayTimezone;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTimyGenerateJson;
        private System.Windows.Forms.Button btnAddWeekTimezone;
        private System.Windows.Forms.Button btnTimySetPersTimezone;
        private System.Windows.Forms.Button btnTimyHoliday;
    }
}

