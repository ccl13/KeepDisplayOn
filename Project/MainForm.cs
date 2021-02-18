using ArkaneSystems.MouseJiggle;
using KeepDisplayOn.WIN32APIs;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace KeepDisplayOn
{
    public partial class MainForm : Form
    {
        public static Mutex SingleInstanceMutex = new Mutex(false, "Global\\Carlton's_KeepDisplayOn");
        protected static object LockObject = new object();
        public static uint uintNULL = 0;

        public uint m_LastThreadExecState;
        public int m_ScreensaverWasActive;
        public bool m_ScreensaverActiveOk;
        public uint m_ScreensaverTimeout;

        public bool m_IsDisplayHoldingActive = false;

        public static Random RandomAtStart = new Random();

        public MainForm()
        {
            InitializeComponent();
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

                //--Step 1 set SetThreadExecutionState
                //--Step 2 Get screensaver information with SystemParametersInfo.
                //--Step 3 Turn off screen saver with SystemParametersInfo
                //--Step 4 Timer call every 30 seconds to do step 1 and 3 (minimal time of screen saver is 1 minute, so every 30 seconds will do)

                m_LastThreadExecState = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_DISPLAY_REQUIRED);
                if (m_LastThreadExecState == 0U)
                {
                    MessageBox.Show("Error setting display required.\r\nClosing application.", "Keep Display On", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
                if (ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVETIMEOUT, 0, ref m_ScreensaverTimeout, 0) > 0)
                {
                }
                uint b = 0;
                if (ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVEACTIVE, 0, ref b, 0) > 0)
                {
                    m_ScreensaverWasActive = (int)b;
                    m_ScreensaverActiveOk = true;
                }
                else
                {
                    m_ScreensaverWasActive = 0;
                    m_ScreensaverActiveOk = false;
                }
                ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, 0, ref uintNULL, 0);

                TimerMaintainer.Start();

                SetActive();
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
                if (m_LastThreadExecState != 0)
                {
                    ScreenSaverInteractions.SetThreadExecutionState(m_LastThreadExecState);
                }
                if (m_ScreensaverActiveOk)
                {
                    ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, m_ScreensaverWasActive, ref uintNULL, 0);
                }

                SetInactive();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopDisplayHolding();
            SingleInstanceMutex?.ReleaseMutex();
        }


        protected bool _jiggled = false;
        protected int _lastMoved = 1;
        private void TimerMaintainer_Tick(object sender, EventArgs e)
        {
            if (!m_IsDisplayHoldingActive)
            {
                return;
            }

            if (CheckBoxWakeScreen.Checked)
            {
                ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_DISPLAY_REQUIRED | ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            }
            else
            {
                ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            }

            var lastIdle = IdleTimeFinder.GetIdleTimeMilliseconds();
            Debugger.Log(2, "Info", $"Last idle: {lastIdle}\n");
            if (CheckBoxAggressive.Checked && lastIdle > 10000)
            {
                if (_jiggled)
                {
                    Jiggler.Jiggle(-_lastMoved, -_lastMoved);
                }
                else
                {
                    _lastMoved = RandomAtStart.Next(1, 4);
                    Jiggler.Jiggle(_lastMoved, _lastMoved);
                    _jiggled = !_jiggled;
                }
                //SendKeys.Send("{NUMLOCK}{NUMLOCK}");
            }

            Application.DoEvents();
        }

        private void TimerDisplayDetector_Tick(object sender, EventArgs e)
        {
            try
            {
                var displayAdapterNames = WMI.GetActiveDisplayAdapterNames();
                var isInRDP = displayAdapterNames.Contains("Microsoft Remote Display Adapter");
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
            this.Visible = shouldBeVisible;
            this.FormBorderStyle = shouldBeVisible ? FormBorderStyle.FixedSingle : FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = shouldBeVisible;
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
