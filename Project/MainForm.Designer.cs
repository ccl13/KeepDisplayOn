namespace KeepDisplayOn
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TimerMaintainer = new System.Windows.Forms.Timer(components);
            TimerDisplayDetector = new System.Windows.Forms.Timer(components);
            ButtonMinimize = new System.Windows.Forms.Button();
            CheckBoxOnlyInRDP = new System.Windows.Forms.CheckBox();
            CheckBoxWakeScreen = new System.Windows.Forms.CheckBox();
            SplitMain = new System.Windows.Forms.SplitContainer();
            CheckBoxAggressive = new System.Windows.Forms.CheckBox();
            CheckBoxPowerOverlay = new System.Windows.Forms.CheckBox();
            NotifyIconMain = new System.Windows.Forms.NotifyIcon(components);
            ((System.ComponentModel.ISupportInitialize)SplitMain).BeginInit();
            SplitMain.Panel1.SuspendLayout();
            SplitMain.Panel2.SuspendLayout();
            SplitMain.SuspendLayout();
            SuspendLayout();
            // 
            // TimerMaintainer
            // 
            TimerMaintainer.Interval = 30000;
            TimerMaintainer.Tick += TimerMaintainer_Tick;
            // 
            // TimerDisplayDetector
            // 
            TimerDisplayDetector.Interval = 30000;
            TimerDisplayDetector.Tick += TimerDisplayDetector_Tick;
            // 
            // ButtonMinimize
            // 
            ButtonMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            ButtonMinimize.Location = new System.Drawing.Point(15, 15);
            ButtonMinimize.Name = "ButtonMinimize";
            ButtonMinimize.Size = new System.Drawing.Size(190, 127);
            ButtonMinimize.TabIndex = 0;
            ButtonMinimize.Text = "Minimize";
            ButtonMinimize.UseVisualStyleBackColor = true;
            ButtonMinimize.Click += ButtonMinimize_Click;
            // 
            // CheckBoxOnlyInRDP
            // 
            CheckBoxOnlyInRDP.AutoSize = true;
            CheckBoxOnlyInRDP.Checked = true;
            CheckBoxOnlyInRDP.CheckState = System.Windows.Forms.CheckState.Checked;
            CheckBoxOnlyInRDP.Dock = System.Windows.Forms.DockStyle.Top;
            CheckBoxOnlyInRDP.Location = new System.Drawing.Point(2, 39);
            CheckBoxOnlyInRDP.Margin = new System.Windows.Forms.Padding(10);
            CheckBoxOnlyInRDP.Name = "CheckBoxOnlyInRDP";
            CheckBoxOnlyInRDP.Padding = new System.Windows.Forms.Padding(8);
            CheckBoxOnlyInRDP.Size = new System.Drawing.Size(164, 37);
            CheckBoxOnlyInRDP.TabIndex = 1;
            CheckBoxOnlyInRDP.Text = "Only in RDP Session";
            CheckBoxOnlyInRDP.UseVisualStyleBackColor = true;
            CheckBoxOnlyInRDP.CheckedChanged += CheckBoxOnlyInRDP_CheckedChanged;
            // 
            // CheckBoxWakeScreen
            // 
            CheckBoxWakeScreen.AutoSize = true;
            CheckBoxWakeScreen.Dock = System.Windows.Forms.DockStyle.Top;
            CheckBoxWakeScreen.Location = new System.Drawing.Point(2, 76);
            CheckBoxWakeScreen.Margin = new System.Windows.Forms.Padding(10);
            CheckBoxWakeScreen.Name = "CheckBoxWakeScreen";
            CheckBoxWakeScreen.Padding = new System.Windows.Forms.Padding(8);
            CheckBoxWakeScreen.Size = new System.Drawing.Size(164, 37);
            CheckBoxWakeScreen.TabIndex = 2;
            CheckBoxWakeScreen.Text = "Keep Screen Light";
            CheckBoxWakeScreen.UseVisualStyleBackColor = true;
            CheckBoxWakeScreen.CheckedChanged += CheckBoxWakeScreen_CheckedChanged;
            // 
            // SplitMain
            // 
            SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            SplitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            SplitMain.IsSplitterFixed = true;
            SplitMain.Location = new System.Drawing.Point(0, 0);
            SplitMain.Name = "SplitMain";
            // 
            // SplitMain.Panel1
            // 
            SplitMain.Panel1.Controls.Add(ButtonMinimize);
            SplitMain.Panel1.Padding = new System.Windows.Forms.Padding(15);
            // 
            // SplitMain.Panel2
            // 
            SplitMain.Panel2.Controls.Add(CheckBoxAggressive);
            SplitMain.Panel2.Controls.Add(CheckBoxWakeScreen);
            SplitMain.Panel2.Controls.Add(CheckBoxOnlyInRDP);
            SplitMain.Panel2.Controls.Add(CheckBoxPowerOverlay);
            SplitMain.Panel2.Margin = new System.Windows.Forms.Padding(10);
            SplitMain.Panel2.Padding = new System.Windows.Forms.Padding(2);
            SplitMain.Size = new System.Drawing.Size(392, 157);
            SplitMain.SplitterDistance = 220;
            SplitMain.TabIndex = 3;
            // 
            // CheckBoxAggressive
            // 
            CheckBoxAggressive.AutoSize = true;
            CheckBoxAggressive.Dock = System.Windows.Forms.DockStyle.Top;
            CheckBoxAggressive.Location = new System.Drawing.Point(2, 113);
            CheckBoxAggressive.Margin = new System.Windows.Forms.Padding(10);
            CheckBoxAggressive.Name = "CheckBoxAggressive";
            CheckBoxAggressive.Padding = new System.Windows.Forms.Padding(8);
            CheckBoxAggressive.Size = new System.Drawing.Size(164, 37);
            CheckBoxAggressive.TabIndex = 3;
            CheckBoxAggressive.Text = "Aggressive";
            CheckBoxAggressive.UseVisualStyleBackColor = true;
            // 
            // CheckBoxPowerOverlay
            // 
            CheckBoxPowerOverlay.AutoSize = true;
            CheckBoxPowerOverlay.Dock = System.Windows.Forms.DockStyle.Top;
            CheckBoxPowerOverlay.Location = new System.Drawing.Point(2, 2);
            CheckBoxPowerOverlay.Margin = new System.Windows.Forms.Padding(10);
            CheckBoxPowerOverlay.Name = "CheckBoxPowerOverlay";
            CheckBoxPowerOverlay.Padding = new System.Windows.Forms.Padding(8);
            CheckBoxPowerOverlay.Size = new System.Drawing.Size(164, 37);
            CheckBoxPowerOverlay.TabIndex = 4;
            CheckBoxPowerOverlay.Text = "RDP Power Saving";
            CheckBoxPowerOverlay.UseVisualStyleBackColor = true;
            CheckBoxPowerOverlay.CheckedChanged += CheckBoxPowerOverlay_CheckedChanged;
            // 
            // NotifyIconMain
            // 
            NotifyIconMain.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            NotifyIconMain.Text = "KeepDisplayOn";
            NotifyIconMain.Visible = true;
            NotifyIconMain.Click += NotifyIconMain_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            CausesValidation = false;
            ClientSize = new System.Drawing.Size(392, 157);
            Controls.Add(SplitMain);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            ShowIcon = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "KeepDisplayOn";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            SplitMain.Panel1.ResumeLayout(false);
            SplitMain.Panel2.ResumeLayout(false);
            SplitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)SplitMain).EndInit();
            SplitMain.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer TimerMaintainer;
        private System.Windows.Forms.Timer TimerDisplayDetector;
        private System.Windows.Forms.Button ButtonMinimize;
        private System.Windows.Forms.CheckBox CheckBoxOnlyInRDP;
        private System.Windows.Forms.CheckBox CheckBoxWakeScreen;
        private System.Windows.Forms.SplitContainer SplitMain;
        private System.Windows.Forms.NotifyIcon NotifyIconMain;
        private System.Windows.Forms.CheckBox CheckBoxAggressive;
        private System.Windows.Forms.CheckBox CheckBoxPowerOverlay;
    }
}

