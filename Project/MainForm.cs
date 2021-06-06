using System;
using System.Threading;
using System.Windows.Forms;

namespace KeepDisplayOn
{
    public partial class MainForm : Form
    {
        public static Mutex SingleInstanceMutex = new Mutex(false, "Global\\Carlton's_KeepDisplayOn");
        protected static object LockObject = new object();

        public bool m_IsDisplayHoldingActive = false;

        protected KeepDisplayOnCore _core = new KeepDisplayOnCore();

        public MainForm()
        {
            InitializeComponent();

            NotifyIconMain.Icon = ((System.Drawing.Icon)(Properties.Resources.DefaultIcon));
            this.Icon = ((System.Drawing.Icon)(Properties.Resources.DefaultIcon));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            bool isAnotherInstanceOpen = !SingleInstanceMutex.WaitOne(TimeSpan.Zero);
            if (isAnotherInstanceOpen)
            {
                Console.WriteLine("Only one instance of this app is allowed.");
                SingleInstanceMutex = null;
                Application.Exit();
                return;
            }
            else
            {
                StartDisplayHolding();
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
            }
        }

        public void SetActive()
        {
            m_IsDisplayHoldingActive = true;
            var stateMessage = "KeepDisplayOn - [Active]";
            this.Text = stateMessage;
            this.NotifyIconMain.Text = stateMessage;
        }

        public void StartDisplayHolding()
        {
            lock (LockObject)
            {
                if (m_IsDisplayHoldingActive)
                {
                    return;
                }

                _core.InitialPullSystemSettings();
                _core.PullConnectedDisplayAdapterInfo();

                TimerMaintainer.Interval = _core.GetRecommendedKeepAliveIntervalMilliseconds();

                SetActive();

                TimerMaintainer_Tick(this, null);
                TimerMaintainer.Start();
            }
        }

        public void SetInactive()
        {
            m_IsDisplayHoldingActive = false;
            var stateMessage = "KeepDisplayOn - [Inactive]";
            this.Text = stateMessage;
            this.NotifyIconMain.Text = stateMessage;
        }

        public void StopDisplayHolding()
        {
            lock (LockObject)
            {
                if (!m_IsDisplayHoldingActive)
                {
                    return;
                }

                TimerMaintainer.Stop();

                SetInactive();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDisplayHolding();
            SingleInstanceMutex?.ReleaseMutex();
        }

        private void TimerMaintainer_Tick(object sender, EventArgs e)
        {
            if (!m_IsDisplayHoldingActive)
            {
                return;
            }

            _core.KeepAlive(CheckBoxWakeScreen.Checked, CheckBoxAggressive.Checked);

            Application.DoEvents();
        }

        private void TimerDisplayDetector_Tick(object sender, EventArgs e)
        {
            try
            {
                _core.PullConnectedDisplayAdapterInfo();
                var isInRDP = _core.IsInRemoteSession();
                if (isInRDP != m_IsDisplayHoldingActive)
                {
                    if (isInRDP)
                    {
                        StartDisplayHolding();
                    }
                    else
                    {
                        StopDisplayHolding();
                    }
                }
            }
            catch (Exception ex)
            {
                // Left blank for debug.
            }
        }

        private void ButtonMinimize_Click(object sender, EventArgs e)
        {
            ButtonMinimize.Focus();
            this.WindowState = FormWindowState.Minimized;
        }

        private void CheckBoxOnlyInRDP_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxOnlyInRDP.Checked)
            {
                TimerDisplayDetector_Tick(sender, e);
                TimerDisplayDetector.Start();
            }
            else
            {
                TimerDisplayDetector.Stop();
                StartDisplayHolding();
            }
            ButtonMinimize.Focus();
        }

        private void CheckBoxWakeScreen_CheckedChanged(object sender, EventArgs e)
        {
            ButtonMinimize.Focus();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            var shouldBeVisible = this.WindowState != FormWindowState.Minimized;
            this.FormBorderStyle = shouldBeVisible ? FormBorderStyle.FixedSingle : FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = shouldBeVisible;
            this.Visible = shouldBeVisible;
        }

        private void NotifyIconMain_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            Application.DoEvents();
            this.Activate();
        }
    }
}
