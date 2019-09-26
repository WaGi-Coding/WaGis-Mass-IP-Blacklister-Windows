namespace WaGis_IP_Blacklister
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.richtbList = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolstripMenuItemCOPY1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolstripMenuItemPASTE = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolstripMenuItemCUT = new System.Windows.Forms.ToolStripMenuItem();
            this.btnADD = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.btnLoadSettings = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.pnlDirection = new System.Windows.Forms.Panel();
            this.cbOutbound = new System.Windows.Forms.CheckBox();
            this.cbInbound = new System.Windows.Forms.CheckBox();
            this.lblDirection = new System.Windows.Forms.Label();
            this.pnlAddDelDelallLog = new System.Windows.Forms.Panel();
            this.btnDEL = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.pnlExcludeIPs = new System.Windows.Forms.Panel();
            this.listBoxExcludeIPs = new System.Windows.Forms.ListBox();
            this.lblExcludeIPs = new System.Windows.Forms.Label();
            this.pnlListURL = new System.Windows.Forms.Panel();
            this.tbListURL = new System.Windows.Forms.TextBox();
            this.btnGetList = new System.Windows.Forms.Button();
            this.lblListURL = new System.Windows.Forms.Label();
            this.pnlAutoMode = new System.Windows.Forms.Panel();
            this.cbAutoOnDoOnce = new System.Windows.Forms.CheckBox();
            this.numericMinutes = new System.Windows.Forms.NumericUpDown();
            this.lblMinutes = new System.Windows.Forms.Label();
            this.numericHours = new System.Windows.Forms.NumericUpDown();
            this.lblDoEvery = new System.Windows.Forms.Label();
            this.lblAutoMode = new System.Windows.Forms.Label();
            this.cbAutoOnOff = new System.Windows.Forms.CheckBox();
            this.lblHours = new System.Windows.Forms.Label();
            this.lblDots = new System.Windows.Forms.Label();
            this.pnlProtocol = new System.Windows.Forms.Panel();
            this.lblProtocol = new System.Windows.Forms.Label();
            this.rbUDP = new System.Windows.Forms.RadioButton();
            this.rbTCP = new System.Windows.Forms.RadioButton();
            this.rbALL = new System.Windows.Forms.RadioButton();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolstripMenuItemCOPY2 = new System.Windows.Forms.ToolStripMenuItem();
            this.lblLines = new System.Windows.Forms.Label();
            this.IntervalTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolstripMenuItemADD = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStriptbListURL = new System.Windows.Forms.ToolStripTextBox();
            this.ToolstripMenuItemOK = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolstripMenuItemREMOVE = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimizeToSystemTrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerUpdateBlink = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerCheckForUpdate = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.pnlDirection.SuspendLayout();
            this.pnlAddDelDelallLog.SuspendLayout();
            this.pnlExcludeIPs.SuspendLayout();
            this.pnlListURL.SuspendLayout();
            this.pnlAutoMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHours)).BeginInit();
            this.pnlProtocol.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.trayIconContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // richtbList
            // 
            this.richtbList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richtbList.ContextMenuStrip = this.contextMenuStrip1;
            this.richtbList.Location = new System.Drawing.Point(12, 27);
            this.richtbList.Name = "richtbList";
            this.richtbList.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richtbList.Size = new System.Drawing.Size(235, 527);
            this.richtbList.TabIndex = 0;
            this.richtbList.Text = "";
            this.richtbList.TextChanged += new System.EventHandler(this.richtbList_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolstripMenuItemCOPY1,
            this.ToolstripMenuItemPASTE,
            this.ToolstripMenuItemCUT});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(103, 70);
            // 
            // ToolstripMenuItemCOPY1
            // 
            this.ToolstripMenuItemCOPY1.Image = global::WaGis_IP_Blacklister.Properties.Resources.copyPic;
            this.ToolstripMenuItemCOPY1.Name = "ToolstripMenuItemCOPY1";
            this.ToolstripMenuItemCOPY1.Size = new System.Drawing.Size(102, 22);
            this.ToolstripMenuItemCOPY1.Text = "Copy";
            this.ToolstripMenuItemCOPY1.Click += new System.EventHandler(this.CopyAction);
            // 
            // ToolstripMenuItemPASTE
            // 
            this.ToolstripMenuItemPASTE.Image = global::WaGis_IP_Blacklister.Properties.Resources.pastePic;
            this.ToolstripMenuItemPASTE.Name = "ToolstripMenuItemPASTE";
            this.ToolstripMenuItemPASTE.Size = new System.Drawing.Size(102, 22);
            this.ToolstripMenuItemPASTE.Text = "Paste";
            this.ToolstripMenuItemPASTE.Click += new System.EventHandler(this.PasteAction);
            // 
            // ToolstripMenuItemCUT
            // 
            this.ToolstripMenuItemCUT.Image = global::WaGis_IP_Blacklister.Properties.Resources.cutPic;
            this.ToolstripMenuItemCUT.Name = "ToolstripMenuItemCUT";
            this.ToolstripMenuItemCUT.Size = new System.Drawing.Size(102, 22);
            this.ToolstripMenuItemCUT.Text = "Cut";
            this.ToolstripMenuItemCUT.Click += new System.EventHandler(this.CutAction);
            // 
            // btnADD
            // 
            this.btnADD.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnADD.BackColor = System.Drawing.Color.Green;
            this.btnADD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnADD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnADD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnADD.Location = new System.Drawing.Point(1, 7);
            this.btnADD.Name = "btnADD";
            this.btnADD.Size = new System.Drawing.Size(90, 25);
            this.btnADD.TabIndex = 3;
            this.btnADD.Text = "ADD";
            this.btnADD.UseVisualStyleBackColor = false;
            this.btnADD.Click += new System.EventHandler(this.btnADD_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteAll.BackColor = System.Drawing.Color.Red;
            this.btnDeleteAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteAll.ForeColor = System.Drawing.Color.Black;
            this.btnDeleteAll.Location = new System.Drawing.Point(1, 35);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(187, 25);
            this.btnDeleteAll.TabIndex = 5;
            this.btnDeleteAll.Text = "DELETE ALL";
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            this.btnDeleteAll.MouseEnter += new System.EventHandler(this.btnDeleteAll_MouseEnter);
            this.btnDeleteAll.MouseLeave += new System.EventHandler(this.btnDeleteAll_MouseLeave);
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.btnLoadSettings);
            this.pnlOptions.Controls.Add(this.btnSaveSettings);
            this.pnlOptions.Controls.Add(this.pnlDirection);
            this.pnlOptions.Controls.Add(this.pnlAddDelDelallLog);
            this.pnlOptions.Controls.Add(this.lblInfo);
            this.pnlOptions.Controls.Add(this.pnlExcludeIPs);
            this.pnlOptions.Controls.Add(this.pnlListURL);
            this.pnlOptions.Controls.Add(this.pnlAutoMode);
            this.pnlOptions.Controls.Add(this.pnlProtocol);
            this.pnlOptions.Location = new System.Drawing.Point(253, 12);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(202, 551);
            this.pnlOptions.TabIndex = 9999;
            // 
            // btnLoadSettings
            // 
            this.btnLoadSettings.Location = new System.Drawing.Point(3, 120);
            this.btnLoadSettings.Name = "btnLoadSettings";
            this.btnLoadSettings.Size = new System.Drawing.Size(189, 23);
            this.btnLoadSettings.TabIndex = 8;
            this.btnLoadSettings.Text = "LOAD SETTINGS";
            this.btnLoadSettings.UseVisualStyleBackColor = true;
            this.btnLoadSettings.Click += new System.EventHandler(this.btnLoadSettings_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(3, 87);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(189, 23);
            this.btnSaveSettings.TabIndex = 7;
            this.btnSaveSettings.Text = "SAVE SETTINGS";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // pnlDirection
            // 
            this.pnlDirection.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlDirection.Controls.Add(this.cbOutbound);
            this.pnlDirection.Controls.Add(this.cbInbound);
            this.pnlDirection.Controls.Add(this.lblDirection);
            this.pnlDirection.Location = new System.Drawing.Point(107, 158);
            this.pnlDirection.Name = "pnlDirection";
            this.pnlDirection.Size = new System.Drawing.Size(85, 68);
            this.pnlDirection.TabIndex = 9999;
            this.pnlDirection.Tag = "";
            // 
            // cbOutbound
            // 
            this.cbOutbound.AutoSize = true;
            this.cbOutbound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbOutbound.Location = new System.Drawing.Point(6, 48);
            this.cbOutbound.Name = "cbOutbound";
            this.cbOutbound.Size = new System.Drawing.Size(73, 17);
            this.cbOutbound.TabIndex = 13;
            this.cbOutbound.Text = "Outbound";
            this.cbOutbound.UseVisualStyleBackColor = true;
            this.cbOutbound.CheckedChanged += new System.EventHandler(this.cbOutbound_CheckedChanged);
            // 
            // cbInbound
            // 
            this.cbInbound.AutoSize = true;
            this.cbInbound.Checked = true;
            this.cbInbound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbInbound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbInbound.Location = new System.Drawing.Point(6, 26);
            this.cbInbound.Name = "cbInbound";
            this.cbInbound.Size = new System.Drawing.Size(65, 17);
            this.cbInbound.TabIndex = 12;
            this.cbInbound.Text = "Inbound";
            this.cbInbound.UseVisualStyleBackColor = true;
            this.cbInbound.CheckedChanged += new System.EventHandler(this.cbInbound_CheckedChanged);
            // 
            // lblDirection
            // 
            this.lblDirection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDirection.AutoSize = true;
            this.lblDirection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDirection.Location = new System.Drawing.Point(3, 2);
            this.lblDirection.Name = "lblDirection";
            this.lblDirection.Size = new System.Drawing.Size(62, 13);
            this.lblDirection.TabIndex = 9999;
            this.lblDirection.Text = "Direction:";
            // 
            // pnlAddDelDelallLog
            // 
            this.pnlAddDelDelallLog.Controls.Add(this.btnDEL);
            this.pnlAddDelDelallLog.Controls.Add(this.btnADD);
            this.pnlAddDelDelallLog.Controls.Add(this.btnDeleteAll);
            this.pnlAddDelDelallLog.Location = new System.Drawing.Point(3, 8);
            this.pnlAddDelDelallLog.Name = "pnlAddDelDelallLog";
            this.pnlAddDelDelallLog.Size = new System.Drawing.Size(196, 63);
            this.pnlAddDelDelallLog.TabIndex = 2;
            // 
            // btnDEL
            // 
            this.btnDEL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDEL.BackColor = System.Drawing.Color.DarkOrange;
            this.btnDEL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDEL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDEL.Location = new System.Drawing.Point(99, 7);
            this.btnDEL.Name = "btnDEL";
            this.btnDEL.Size = new System.Drawing.Size(90, 25);
            this.btnDEL.TabIndex = 4;
            this.btnDEL.Text = "DEL";
            this.btnDEL.UseVisualStyleBackColor = false;
            this.btnDEL.Click += new System.EventHandler(this.btnDEL_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(0, 71);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(191, 13);
            this.lblInfo.TabIndex = 9999;
            this.lblInfo.Text = "XXXXXXXXXXXXXXXXXXXXXXX";
            // 
            // pnlExcludeIPs
            // 
            this.pnlExcludeIPs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlExcludeIPs.Controls.Add(this.listBoxExcludeIPs);
            this.pnlExcludeIPs.Controls.Add(this.lblExcludeIPs);
            this.pnlExcludeIPs.Location = new System.Drawing.Point(3, 240);
            this.pnlExcludeIPs.Name = "pnlExcludeIPs";
            this.pnlExcludeIPs.Size = new System.Drawing.Size(189, 92);
            this.pnlExcludeIPs.TabIndex = 9999;
            // 
            // listBoxExcludeIPs
            // 
            this.listBoxExcludeIPs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listBoxExcludeIPs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxExcludeIPs.FormattingEnabled = true;
            this.listBoxExcludeIPs.ItemHeight = 20;
            this.listBoxExcludeIPs.Location = new System.Drawing.Point(6, 19);
            this.listBoxExcludeIPs.Name = "listBoxExcludeIPs";
            this.listBoxExcludeIPs.ScrollAlwaysVisible = true;
            this.listBoxExcludeIPs.Size = new System.Drawing.Size(174, 64);
            this.listBoxExcludeIPs.TabIndex = 14;
            this.listBoxExcludeIPs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxExcludeIPs_MouseDown);
            // 
            // lblExcludeIPs
            // 
            this.lblExcludeIPs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExcludeIPs.AutoSize = true;
            this.lblExcludeIPs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExcludeIPs.Location = new System.Drawing.Point(4, 3);
            this.lblExcludeIPs.Name = "lblExcludeIPs";
            this.lblExcludeIPs.Size = new System.Drawing.Size(78, 13);
            this.lblExcludeIPs.TabIndex = 9999;
            this.lblExcludeIPs.Text = "Exclude IPs:";
            // 
            // pnlListURL
            // 
            this.pnlListURL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlListURL.Controls.Add(this.tbListURL);
            this.pnlListURL.Controls.Add(this.btnGetList);
            this.pnlListURL.Controls.Add(this.lblListURL);
            this.pnlListURL.Location = new System.Drawing.Point(3, 461);
            this.pnlListURL.Name = "pnlListURL";
            this.pnlListURL.Size = new System.Drawing.Size(189, 81);
            this.pnlListURL.TabIndex = 9999;
            // 
            // tbListURL
            // 
            this.tbListURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbListURL.Location = new System.Drawing.Point(8, 20);
            this.tbListURL.Name = "tbListURL";
            this.tbListURL.Size = new System.Drawing.Size(174, 20);
            this.tbListURL.TabIndex = 19;
            this.tbListURL.Enter += new System.EventHandler(this.tbListURL_Enter);
            // 
            // btnGetList
            // 
            this.btnGetList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGetList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetList.Location = new System.Drawing.Point(8, 46);
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.Size = new System.Drawing.Size(73, 23);
            this.btnGetList.TabIndex = 20;
            this.btnGetList.Text = "GET LIST";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
            // 
            // lblListURL
            // 
            this.lblListURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblListURL.AutoSize = true;
            this.lblListURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblListURL.Location = new System.Drawing.Point(5, 3);
            this.lblListURL.Name = "lblListURL";
            this.lblListURL.Size = new System.Drawing.Size(76, 13);
            this.lblListURL.TabIndex = 9999;
            this.lblListURL.Text = "IP-List URL:";
            // 
            // pnlAutoMode
            // 
            this.pnlAutoMode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlAutoMode.Controls.Add(this.cbAutoOnDoOnce);
            this.pnlAutoMode.Controls.Add(this.numericMinutes);
            this.pnlAutoMode.Controls.Add(this.lblMinutes);
            this.pnlAutoMode.Controls.Add(this.numericHours);
            this.pnlAutoMode.Controls.Add(this.lblDoEvery);
            this.pnlAutoMode.Controls.Add(this.lblAutoMode);
            this.pnlAutoMode.Controls.Add(this.cbAutoOnOff);
            this.pnlAutoMode.Controls.Add(this.lblHours);
            this.pnlAutoMode.Controls.Add(this.lblDots);
            this.pnlAutoMode.Location = new System.Drawing.Point(4, 341);
            this.pnlAutoMode.Name = "pnlAutoMode";
            this.pnlAutoMode.Size = new System.Drawing.Size(188, 109);
            this.pnlAutoMode.TabIndex = 9999;
            // 
            // cbAutoOnDoOnce
            // 
            this.cbAutoOnDoOnce.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAutoOnDoOnce.AutoSize = true;
            this.cbAutoOnDoOnce.Checked = true;
            this.cbAutoOnDoOnce.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoOnDoOnce.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbAutoOnDoOnce.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoOnDoOnce.Location = new System.Drawing.Point(6, 61);
            this.cbAutoOnDoOnce.Name = "cbAutoOnDoOnce";
            this.cbAutoOnDoOnce.Size = new System.Drawing.Size(154, 17);
            this.cbAutoOnDoOnce.TabIndex = 17;
            this.cbAutoOnDoOnce.Text = "Do once when starting";
            this.cbAutoOnDoOnce.UseVisualStyleBackColor = true;
            // 
            // numericMinutes
            // 
            this.numericMinutes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericMinutes.Location = new System.Drawing.Point(119, 22);
            this.numericMinutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numericMinutes.Name = "numericMinutes";
            this.numericMinutes.Size = new System.Drawing.Size(36, 22);
            this.numericMinutes.TabIndex = 16;
            this.numericMinutes.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericMinutes.ValueChanged += new System.EventHandler(this.numericMinutes_ValueChanged);
            // 
            // lblMinutes
            // 
            this.lblMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinutes.AutoSize = true;
            this.lblMinutes.Location = new System.Drawing.Point(115, 44);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(44, 13);
            this.lblMinutes.TabIndex = 9999;
            this.lblMinutes.Text = "Minutes";
            // 
            // numericHours
            // 
            this.numericHours.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericHours.Location = new System.Drawing.Point(77, 22);
            this.numericHours.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numericHours.Name = "numericHours";
            this.numericHours.Size = new System.Drawing.Size(36, 22);
            this.numericHours.TabIndex = 15;
            this.numericHours.ValueChanged += new System.EventHandler(this.numericHours_ValueChanged);
            // 
            // lblDoEvery
            // 
            this.lblDoEvery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDoEvery.AutoSize = true;
            this.lblDoEvery.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDoEvery.Location = new System.Drawing.Point(6, 24);
            this.lblDoEvery.Name = "lblDoEvery";
            this.lblDoEvery.Size = new System.Drawing.Size(66, 16);
            this.lblDoEvery.TabIndex = 9999;
            this.lblDoEvery.Text = "Do every:";
            // 
            // lblAutoMode
            // 
            this.lblAutoMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAutoMode.AutoSize = true;
            this.lblAutoMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoMode.Location = new System.Drawing.Point(5, 3);
            this.lblAutoMode.Name = "lblAutoMode";
            this.lblAutoMode.Size = new System.Drawing.Size(102, 13);
            this.lblAutoMode.TabIndex = 9999;
            this.lblAutoMode.Text = "Automatic Mode:";
            // 
            // cbAutoOnOff
            // 
            this.cbAutoOnOff.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAutoOnOff.AutoSize = true;
            this.cbAutoOnOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbAutoOnOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbAutoOnOff.Location = new System.Drawing.Point(6, 84);
            this.cbAutoOnOff.Name = "cbAutoOnOff";
            this.cbAutoOnOff.Size = new System.Drawing.Size(91, 20);
            this.cbAutoOnOff.TabIndex = 18;
            this.cbAutoOnOff.Text = "ON / OFF";
            this.cbAutoOnOff.UseVisualStyleBackColor = true;
            this.cbAutoOnOff.CheckedChanged += new System.EventHandler(this.cbAutoOnOff_CheckedChanged);
            // 
            // lblHours
            // 
            this.lblHours.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHours.AutoSize = true;
            this.lblHours.Location = new System.Drawing.Point(74, 44);
            this.lblHours.Name = "lblHours";
            this.lblHours.Size = new System.Drawing.Size(35, 13);
            this.lblHours.TabIndex = 9999;
            this.lblHours.Text = "Hours";
            // 
            // lblDots
            // 
            this.lblDots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDots.AutoSize = true;
            this.lblDots.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDots.Location = new System.Drawing.Point(107, 19);
            this.lblDots.Name = "lblDots";
            this.lblDots.Size = new System.Drawing.Size(19, 25);
            this.lblDots.TabIndex = 9999;
            this.lblDots.Text = ":";
            // 
            // pnlProtocol
            // 
            this.pnlProtocol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlProtocol.Controls.Add(this.lblProtocol);
            this.pnlProtocol.Controls.Add(this.rbUDP);
            this.pnlProtocol.Controls.Add(this.rbTCP);
            this.pnlProtocol.Controls.Add(this.rbALL);
            this.pnlProtocol.Location = new System.Drawing.Point(3, 158);
            this.pnlProtocol.Name = "pnlProtocol";
            this.pnlProtocol.Size = new System.Drawing.Size(85, 68);
            this.pnlProtocol.TabIndex = 9999;
            this.pnlProtocol.Tag = "";
            // 
            // lblProtocol
            // 
            this.lblProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProtocol.AutoSize = true;
            this.lblProtocol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProtocol.Location = new System.Drawing.Point(3, 2);
            this.lblProtocol.Name = "lblProtocol";
            this.lblProtocol.Size = new System.Drawing.Size(58, 13);
            this.lblProtocol.TabIndex = 9999;
            this.lblProtocol.Text = "Protocol:";
            // 
            // rbUDP
            // 
            this.rbUDP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbUDP.AutoSize = true;
            this.rbUDP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbUDP.Location = new System.Drawing.Point(7, 32);
            this.rbUDP.Name = "rbUDP";
            this.rbUDP.Size = new System.Drawing.Size(48, 17);
            this.rbUDP.TabIndex = 10;
            this.rbUDP.Text = "UDP";
            this.rbUDP.UseVisualStyleBackColor = true;
            this.rbUDP.CheckedChanged += new System.EventHandler(this.ProtocolSwitch);
            // 
            // rbTCP
            // 
            this.rbTCP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbTCP.AutoSize = true;
            this.rbTCP.Checked = true;
            this.rbTCP.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbTCP.Location = new System.Drawing.Point(7, 15);
            this.rbTCP.Name = "rbTCP";
            this.rbTCP.Size = new System.Drawing.Size(46, 17);
            this.rbTCP.TabIndex = 9;
            this.rbTCP.TabStop = true;
            this.rbTCP.Text = "TCP";
            this.rbTCP.UseVisualStyleBackColor = true;
            this.rbTCP.CheckedChanged += new System.EventHandler(this.ProtocolSwitch);
            // 
            // rbALL
            // 
            this.rbALL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbALL.AutoSize = true;
            this.rbALL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbALL.Location = new System.Drawing.Point(7, 47);
            this.rbALL.Name = "rbALL";
            this.rbALL.Size = new System.Drawing.Size(71, 17);
            this.rbALL.TabIndex = 11;
            this.rbALL.Text = "ALL (15+)";
            this.rbALL.UseVisualStyleBackColor = true;
            this.rbALL.CheckedChanged += new System.EventHandler(this.ProtocolSwitch);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolstripMenuItemCOPY2});
            this.contextMenuStrip3.Name = "contextMenuStrip1";
            this.contextMenuStrip3.Size = new System.Drawing.Size(103, 26);
            // 
            // ToolstripMenuItemCOPY2
            // 
            this.ToolstripMenuItemCOPY2.Image = global::WaGis_IP_Blacklister.Properties.Resources.copyPic;
            this.ToolstripMenuItemCOPY2.Name = "ToolstripMenuItemCOPY2";
            this.ToolstripMenuItemCOPY2.Size = new System.Drawing.Size(102, 22);
            this.ToolstripMenuItemCOPY2.Text = "Copy";
            // 
            // lblLines
            // 
            this.lblLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLines.AutoSize = true;
            this.lblLines.Location = new System.Drawing.Point(12, 556);
            this.lblLines.Name = "lblLines";
            this.lblLines.Size = new System.Drawing.Size(44, 13);
            this.lblLines.TabIndex = 9999;
            this.lblLines.Text = "Lines: 0";
            // 
            // IntervalTimer
            // 
            this.IntervalTimer.Tick += new System.EventHandler(this.IntervalTimer_Tick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolstripMenuItemADD,
            this.ToolstripMenuItemREMOVE});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(121, 48);
            // 
            // ToolstripMenuItemADD
            // 
            this.ToolstripMenuItemADD.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStriptbListURL,
            this.ToolstripMenuItemOK});
            this.ToolstripMenuItemADD.Image = global::WaGis_IP_Blacklister.Properties.Resources.addPic;
            this.ToolstripMenuItemADD.Name = "ToolstripMenuItemADD";
            this.ToolstripMenuItemADD.Size = new System.Drawing.Size(120, 22);
            this.ToolstripMenuItemADD.Text = "ADD";
            // 
            // toolStriptbListURL
            // 
            this.toolStriptbListURL.BackColor = System.Drawing.Color.Gainsboro;
            this.toolStriptbListURL.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStriptbListURL.Name = "toolStriptbListURL";
            this.toolStriptbListURL.Size = new System.Drawing.Size(100, 23);
            this.toolStriptbListURL.Text = "----ENTER IP----";
            this.toolStriptbListURL.Click += new System.EventHandler(this.toolStriptbListURL_Click);
            // 
            // ToolstripMenuItemOK
            // 
            this.ToolstripMenuItemOK.Image = global::WaGis_IP_Blacklister.Properties.Resources.checkedPic;
            this.ToolstripMenuItemOK.Name = "ToolstripMenuItemOK";
            this.ToolstripMenuItemOK.Size = new System.Drawing.Size(160, 22);
            this.ToolstripMenuItemOK.Text = "OK";
            this.ToolstripMenuItemOK.Click += new System.EventHandler(this.ToolstripMenuItemOK_Click);
            // 
            // ToolstripMenuItemREMOVE
            // 
            this.ToolstripMenuItemREMOVE.Image = global::WaGis_IP_Blacklister.Properties.Resources.minusPic;
            this.ToolstripMenuItemREMOVE.Name = "ToolstripMenuItemREMOVE";
            this.ToolstripMenuItemREMOVE.Size = new System.Drawing.Size(120, 22);
            this.ToolstripMenuItemREMOVE.Text = "REMOVE";
            this.ToolstripMenuItemREMOVE.Click += new System.EventHandler(this.ToolstripMenuItemREMOVE_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infoToolStripMenuItem,
            this.minimizeToSystemTrayToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(458, 24);
            this.menuStrip1.TabIndex = 10000;
            this.menuStrip1.Text = "Menu";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // minimizeToSystemTrayToolStripMenuItem
            // 
            this.minimizeToSystemTrayToolStripMenuItem.Name = "minimizeToSystemTrayToolStripMenuItem";
            this.minimizeToSystemTrayToolStripMenuItem.Size = new System.Drawing.Size(147, 20);
            this.minimizeToSystemTrayToolStripMenuItem.Text = "Minimize to System Tray";
            this.minimizeToSystemTrayToolStripMenuItem.Click += new System.EventHandler(this.minimizeToSystemTrayToolStripMenuItem_Click);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.BackColor = System.Drawing.Color.Crimson;
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(109, 20);
            this.updateToolStripMenuItem.Text = "Update available!";
            this.updateToolStripMenuItem.Visible = false;
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // timerUpdateBlink
            // 
            this.timerUpdateBlink.Interval = 1000;
            this.timerUpdateBlink.Tick += new System.EventHandler(this.timerUpdateBlink_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Testi testi teeeest";
            this.notifyIcon1.BalloonTipTitle = "Test";
            this.notifyIcon1.ContextMenuStrip = this.trayIconContextMenuStrip;
            this.notifyIcon1.Text = "WaGi\'s IP-Blacklister";
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_Click);
            // 
            // trayIconContextMenuStrip
            // 
            this.trayIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.trayIconContextMenuStrip.Name = "trayIconContextMenuStrip";
            this.trayIconContextMenuStrip.Size = new System.Drawing.Size(104, 48);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // timerCheckForUpdate
            // 
            this.timerCheckForUpdate.Interval = 600000;
            this.timerCheckForUpdate.Tick += new System.EventHandler(this.timerCheckForUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 575);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lblLines);
            this.Controls.Add(this.pnlOptions);
            this.Controls.Add(this.richtbList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(370, 590);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WaGi\'s IP-Blacklister";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            this.pnlDirection.ResumeLayout(false);
            this.pnlDirection.PerformLayout();
            this.pnlAddDelDelallLog.ResumeLayout(false);
            this.pnlExcludeIPs.ResumeLayout(false);
            this.pnlExcludeIPs.PerformLayout();
            this.pnlListURL.ResumeLayout(false);
            this.pnlListURL.PerformLayout();
            this.pnlAutoMode.ResumeLayout(false);
            this.pnlAutoMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericHours)).EndInit();
            this.pnlProtocol.ResumeLayout(false);
            this.pnlProtocol.PerformLayout();
            this.contextMenuStrip3.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.trayIconContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richtbList;
        private System.Windows.Forms.Button btnADD;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Panel pnlProtocol;
        private System.Windows.Forms.Label lblProtocol;
        private System.Windows.Forms.RadioButton rbUDP;
        private System.Windows.Forms.RadioButton rbTCP;
        private System.Windows.Forms.RadioButton rbALL;
        private System.Windows.Forms.Panel pnlAutoMode;
        private System.Windows.Forms.Label lblAutoMode;
        private System.Windows.Forms.CheckBox cbAutoOnOff;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemCOPY1;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemPASTE;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemCUT;
        private System.Windows.Forms.Label lblLines;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Panel pnlListURL;
        private System.Windows.Forms.Label lblListURL;
        private System.Windows.Forms.Timer IntervalTimer;
        private System.Windows.Forms.Button btnGetList;
        private System.Windows.Forms.Label lblDoEvery;
        private System.Windows.Forms.Panel pnlAddDelDelallLog;
        private System.Windows.Forms.Panel pnlExcludeIPs;
        private System.Windows.Forms.Label lblExcludeIPs;
        private System.Windows.Forms.ListBox listBoxExcludeIPs;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemADD;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemREMOVE;
        private System.Windows.Forms.Panel pnlDirection;
        private System.Windows.Forms.Label lblDirection;
        private System.Windows.Forms.ToolStripTextBox toolStriptbListURL;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemOK;
        private System.Windows.Forms.TextBox tbListURL;
        private System.Windows.Forms.NumericUpDown numericHours;
        private System.Windows.Forms.NumericUpDown numericMinutes;
        private System.Windows.Forms.Label lblMinutes;
        private System.Windows.Forms.Label lblHours;
        private System.Windows.Forms.Label lblDots;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem ToolstripMenuItemCOPY2;
        private System.Windows.Forms.CheckBox cbAutoOnDoOnce;
        private System.Windows.Forms.CheckBox cbOutbound;
        private System.Windows.Forms.CheckBox cbInbound;
        private System.Windows.Forms.Button btnDEL;
        private System.Windows.Forms.Button btnLoadSettings;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.Timer timerUpdateBlink;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem minimizeToSystemTrayToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip trayIconContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Timer timerCheckForUpdate;
    }
}

