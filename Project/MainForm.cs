using System;
using System.Threading;
using System.Windows.Forms;

using KeepDisplayOn.WIN32APIs;

namespace KeepDisplayOn
{
    public partial class MainForm : Form
    {
        public static Mutex SingleInstanceMutex = new Mutex(false, "Global\\Carlton's_KeepDisplayOn");
        protected static object LockObject = new object();

        public bool m_IsDisplayHoldingActive = false;
        public DateTime m_IsDisplayHoldingLastChecked = DateTime.MinValue;

        protected KeepDisplayOnCore _core = new KeepDisplayOnCore();

        private bool m_IsInRDP = false;

        public MainForm()
        {
            InitializeComponent();

            NotifyIconMain.Icon = Properties.Resources.DefaultIcon;
            this.Icon = Properties.Resources.DefaultIcon;
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

            Initialize();
            SetInactive();

            if (CheckBoxOnlyInRDP.Checked)
            {
                CheckBoxOnlyInRDP_CheckedChanged(sender, e);
            }
            else
            {
                StartDisplayHolding();
            }

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
        }

        public void SetRDPState(bool isInRDP)
        {
            m_IsInRDP = isInRDP;
        }

        public void SetActive()
        {
            m_IsDisplayHoldingActive = true;
            UpdateStateMessage();
        }

        public void SetInactive()
        {
            m_IsDisplayHoldingActive = false;
            UpdateStateMessage();
        }

        private void UpdateStateMessage()
        {
            var stateMessage = $"KeepDisplayOn - [{(m_IsDisplayHoldingActive ? "Active" : "Inactive")}]{(m_IsInRDP ? " (RDP)" : "")}";
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

                _core.Initialize();
                _core.RefreshRemoteSessionStatus();

                TimerMaintainer.Interval = _core.GetRecommendedKeepAliveIntervalMilliseconds();

                SetActive();

                TimerMaintainer_Tick(this, null);
                TimerMaintainer.Start();

                _core.DisableScreenSaver();
            }
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

                _core.RestoreSystem();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            HandleExitingRDP();
            SingleInstanceMutex?.ReleaseMutex();
        }

        private void TimerMaintainer_Tick(object sender, EventArgs e)
        {
            if (!m_IsDisplayHoldingActive)
            {
                return;
            }

            _core.KeepDisplayOn(CheckBoxWakeScreen.Checked, CheckBoxAggressive.Checked);

            Application.DoEvents();
        }

        private void HandleEnteringRDP()
        {
            if (_core.m_SavedPowerOverlay != null)
                return;

            if (CheckBoxPowerOverlay.Checked)
            {
                try
                {
                    _core.EnableBatterySavingOverlay();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to handle power overlay when entering RDP: {ex.Message}");
                }
            }
        }

        private void HandleInRDP()
        {
            try
            {
                StartDisplayHolding();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start display holding in RDP: {ex.Message}");
            }
        }

        private void HandleExitingRDP()
        {
            try
            {
                StopDisplayHolding();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to stop display holding when exiting RDP: {ex.Message}");
            }

            if (CheckBoxPowerOverlay.Checked)
            {
                try
                {
                    _core.RestoreSavedBatteryOverlay();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to restore power overlay when exiting RDP: {ex.Message}");
                }
            }
        }

        private void TimerDisplayDetector_Tick(object sender, EventArgs e)
        {
            bool isInRDP = false;

            try
            {
                _core.RefreshRemoteSessionStatus();
                isInRDP = _core.IsInRemoteSession();
                SetRDPState(isInRDP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to detect RDP status: {ex.Message}");
                return;
            }

            // State transition handling
            if (isInRDP && !m_IsDisplayHoldingActive)
            {
                // Entering RDP
                HandleEnteringRDP();
                HandleInRDP();
            }
            else if (!isInRDP && m_IsDisplayHoldingActive)
            {
                // Exiting RDP
                HandleExitingRDP();
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

        public void Initialize()
        {
            _core.Initialize();
            _core.RefreshRemoteSessionStatus();
        }

        private void CheckBoxPowerOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxPowerOverlay.Checked)
            {
                if (TimerDisplayDetector.Enabled)
                {
                    _core.EnableBatterySavingOverlay();
                }
            }
            else
            {
                _core.RestoreSavedBatteryOverlay();
            }
            ButtonMinimize.Focus();
        }
    }
}
