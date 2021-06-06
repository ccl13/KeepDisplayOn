
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
            this.components = new System.ComponentModel.Container();
            this.TimerMaintainer = new System.Windows.Forms.Timer(this.components);
            this.TimerDisplayDetector = new System.Windows.Forms.Timer(this.components);
            this.ButtonMinimize = new System.Windows.Forms.Button();
            this.CheckBoxOnlyInRDP = new System.Windows.Forms.CheckBox();
            this.CheckBoxWakeScreen = new System.Windows.Forms.CheckBox();
            this.SplitMain = new System.Windows.Forms.SplitContainer();
            this.CheckBoxAggressive = new System.Windows.Forms.CheckBox();
            this.NotifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.SplitMain)).BeginInit();
            this.SplitMain.Panel1.SuspendLayout();
            this.SplitMain.Panel2.SuspendLayout();
            this.SplitMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimerMaintainer
            // 
            this.TimerMaintainer.Interval = 30000;
            this.TimerMaintainer.Tick += new System.EventHandler(this.TimerMaintainer_Tick);
            // 
            // TimerDisplayDetector
            // 
            this.TimerDisplayDetector.Interval = 30000;
            this.TimerDisplayDetector.Tick += new System.EventHandler(this.TimerDisplayDetector_Tick);
            // 
            // ButtonMinimize
            // 
            this.ButtonMinimize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonMinimize.Location = new System.Drawing.Point(15, 15);
            this.ButtonMinimize.Name = "ButtonMinimize";
            this.ButtonMinimize.Size = new System.Drawing.Size(190, 87);
            this.ButtonMinimize.TabIndex = 0;
            this.ButtonMinimize.Text = "Minimize";
            this.ButtonMinimize.UseVisualStyleBackColor = true;
            this.ButtonMinimize.Click += new System.EventHandler(this.ButtonMinimize_Click);
            // 
            // CheckBoxOnlyInRDP
            // 
            this.CheckBoxOnlyInRDP.AutoSize = true;
            this.CheckBoxOnlyInRDP.Dock = System.Windows.Forms.DockStyle.Top;
            this.CheckBoxOnlyInRDP.Location = new System.Drawing.Point(2, 2);
            this.CheckBoxOnlyInRDP.Margin = new System.Windows.Forms.Padding(10);
            this.CheckBoxOnlyInRDP.Name = "CheckBoxOnlyInRDP";
            this.CheckBoxOnlyInRDP.Padding = new System.Windows.Forms.Padding(8);
            this.CheckBoxOnlyInRDP.Size = new System.Drawing.Size(170, 37);
            this.CheckBoxOnlyInRDP.TabIndex = 1;
            this.CheckBoxOnlyInRDP.Text = "Only in RDP Session";
            this.CheckBoxOnlyInRDP.UseVisualStyleBackColor = true;
            this.CheckBoxOnlyInRDP.CheckedChanged += new System.EventHandler(this.CheckBoxOnlyInRDP_CheckedChanged);
            // 
            // CheckBoxWakeScreen
            // 
            this.CheckBoxWakeScreen.AutoSize = true;
            this.CheckBoxWakeScreen.Dock = System.Windows.Forms.DockStyle.Top;
            this.CheckBoxWakeScreen.Location = new System.Drawing.Point(2, 39);
            this.CheckBoxWakeScreen.Margin = new System.Windows.Forms.Padding(10);
            this.CheckBoxWakeScreen.Name = "CheckBoxWakeScreen";
            this.CheckBoxWakeScreen.Padding = new System.Windows.Forms.Padding(8);
            this.CheckBoxWakeScreen.Size = new System.Drawing.Size(170, 37);
            this.CheckBoxWakeScreen.TabIndex = 2;
            this.CheckBoxWakeScreen.Text = "Keep Screen Light";
            this.CheckBoxWakeScreen.UseVisualStyleBackColor = true;
            this.CheckBoxWakeScreen.CheckedChanged += new System.EventHandler(this.CheckBoxWakeScreen_CheckedChanged);
            // 
            // SplitMain
            // 
            this.SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitMain.IsSplitterFixed = true;
            this.SplitMain.Location = new System.Drawing.Point(0, 0);
            this.SplitMain.Name = "SplitMain";
            // 
            // SplitMain.Panel1
            // 
            this.SplitMain.Panel1.Controls.Add(this.ButtonMinimize);
            this.SplitMain.Panel1.Padding = new System.Windows.Forms.Padding(15);
            // 
            // SplitMain.Panel2
            // 
            this.SplitMain.Panel2.Controls.Add(this.CheckBoxAggressive);
            this.SplitMain.Panel2.Controls.Add(this.CheckBoxWakeScreen);
            this.SplitMain.Panel2.Controls.Add(this.CheckBoxOnlyInRDP);
            this.SplitMain.Panel2.Margin = new System.Windows.Forms.Padding(10);
            this.SplitMain.Panel2.Padding = new System.Windows.Forms.Padding(2);
            this.SplitMain.Size = new System.Drawing.Size(398, 117);
            this.SplitMain.SplitterDistance = 220;
            this.SplitMain.TabIndex = 3;
            // 
            // CheckBoxAggressive
            // 
            this.CheckBoxAggressive.AutoSize = true;
            this.CheckBoxAggressive.Dock = System.Windows.Forms.DockStyle.Top;
            this.CheckBoxAggressive.Location = new System.Drawing.Point(2, 76);
            this.CheckBoxAggressive.Margin = new System.Windows.Forms.Padding(10);
            this.CheckBoxAggressive.Name = "CheckBoxAggressive";
            this.CheckBoxAggressive.Padding = new System.Windows.Forms.Padding(8);
            this.CheckBoxAggressive.Size = new System.Drawing.Size(170, 37);
            this.CheckBoxAggressive.TabIndex = 3;
            this.CheckBoxAggressive.Text = "Aggressive";
            this.CheckBoxAggressive.UseVisualStyleBackColor = true;
            // 
            // NotifyIconMain
            // 
            this.NotifyIconMain.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.NotifyIconMain.Text = "KeepDisplayOn";
            this.NotifyIconMain.Visible = true;
            this.NotifyIconMain.Click += new System.EventHandler(this.NotifyIconMain_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(398, 117);
            this.Controls.Add(this.SplitMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeepDisplayOn";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.SplitMain.Panel1.ResumeLayout(false);
            this.SplitMain.Panel2.ResumeLayout(false);
            this.SplitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitMain)).EndInit();
            this.SplitMain.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}

