using ArkaneSystems.MouseJiggle;
using KeepDisplayOn.WIN32APIs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KeepDisplayOn
{
    public class KeepDisplayOnCore
    {
        public const string RemoteDesktopDisplayAdapterName = "Microsoft Remote Display Adapter";

        public static uint uintNULL = 0;
        public static readonly TimeSpan ApiGuardInterval = TimeSpan.FromSeconds(5);

        public static Random RandomAtStart = new Random();

        public uint m_ScreensaverTimeout;
        public bool m_ScreensaverTimeoutIsRefreshed;
        public DateTime m_ScreensaverTimeoutRefreshedAt;

        public uint m_ScreensaverActiveState;
        public bool m_ScreensaverActiveStateIsRefreshed;
        public DateTime m_ScreensaverActiveStateRefreshedAt;

        public HashSet<string> m_CurrentDisplayAdapterNames;
        public bool m_CurrentDisplayAdapterNamesIsRefreshed;
        public DateTime m_CurrentDisplayAdapterNamesRefreshedAt;

        public uint m_SetRequired;
        public bool m_SetRequiredIsSuccessful;
        public DateTime m_SetRequiredCalledAt;

        protected bool m_jiggled = false;
        protected int m_lastMovedDistance = 1;

        public void InitialPullSystemSettings()
        {
            m_ScreensaverTimeoutIsRefreshed = false;
            {
                m_ScreensaverTimeoutRefreshedAt = DateTime.Now;
                var getTimeoutCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVETIMEOUT, 0, ref m_ScreensaverTimeout, 0);
                m_ScreensaverTimeoutIsRefreshed = getTimeoutCallRet > 0;
            }

            m_ScreensaverActiveStateIsRefreshed = false;
            {
                m_ScreensaverActiveStateRefreshedAt = DateTime.Now;
                var getScreenSaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVEACTIVE, 0, ref m_ScreensaverActiveState, 0);
                m_ScreensaverActiveStateIsRefreshed = getScreenSaverActiveCallRet > 0;
            }
        }

        public void DisableScreenSaver()
        {
            if (m_ScreensaverActiveStateIsRefreshed && m_ScreensaverActiveState != 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, 0, ref uintNULL, 0);
            }
        }

        public void RestoreSystem()
        {
            if (m_ScreensaverActiveStateIsRefreshed && m_ScreensaverActiveState != 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, m_ScreensaverActiveState, ref uintNULL, 0);
            }
        }

        public int GetRecommendedKeepAliveIntervalMilliseconds()
        {
            var ret = 30000;
            if (m_ScreensaverTimeoutIsRefreshed)
            {
                ret = (int)m_ScreensaverTimeout * 1000 / 2;
            }
            if (ret < 10000)
            {
                ret = 10000;
            }
            return ret;
        }

        public void PullConnectedDisplayAdapterInfo()
        {
            // WMI Pull Guard
            if (m_CurrentDisplayAdapterNamesIsRefreshed)
            {
                var timePassed = DateTime.Now - m_CurrentDisplayAdapterNamesRefreshedAt;
                if (timePassed < ApiGuardInterval)
                {
                    // If last refresh too close, ignore refresh.
                    return;
                }
            }
            // Actual refresh
            m_CurrentDisplayAdapterNamesIsRefreshed = false;
            try
            {
                m_CurrentDisplayAdapterNamesRefreshedAt = DateTime.Now;
                var displayAdapterNames = WMI.GetActiveDisplayAdapterNames();
                m_CurrentDisplayAdapterNames = new HashSet<string>(displayAdapterNames, StringComparer.InvariantCultureIgnoreCase);
                m_CurrentDisplayAdapterNamesIsRefreshed = true;
            }
            catch (Exception ex)
            {
                // Left blank for debug.
            }
        }

        public bool IsInRemoteSession()
        {
            return m_CurrentDisplayAdapterNames.Contains(RemoteDesktopDisplayAdapterName);
        }

        public void RunSetAliveWithKeepDisplay()
        {
            // Guard
            if (m_SetRequiredIsSuccessful)
            {
                var timePassed = DateTime.Now - m_SetRequiredCalledAt;
                if (timePassed < ApiGuardInterval)
                {
                    return;
                }
            }
            m_SetRequiredIsSuccessful = false;
            m_SetRequiredCalledAt = DateTime.Now;
            m_SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_DISPLAY_REQUIRED | ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            m_SetRequiredIsSuccessful = m_SetRequired != 0;
        }

        public void RunSetAliveWithoutKeepDisplay()
        {
            // Guard
            if (m_SetRequiredIsSuccessful)
            {
                var timePassed = DateTime.Now - m_SetRequiredCalledAt;
                if (timePassed < ApiGuardInterval)
                {
                    return;
                }
            }
            m_SetRequiredIsSuccessful = false;
            m_SetRequiredCalledAt = DateTime.Now;
            m_SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            m_SetRequiredIsSuccessful = m_SetRequired != 0;
        }

        public void KeepAlive(bool keepDisplay, bool mimicInput)
        {
            m_SetRequiredIsSuccessful = false;
            if (keepDisplay)
            {
                RunSetAliveWithKeepDisplay();
            }
            if (!m_SetRequiredIsSuccessful)
            {
                if (keepDisplay)
                {
                    // Reset guard so we won't be ignored
                    m_SetRequiredCalledAt = DateTime.MinValue;
                }
                RunSetAliveWithoutKeepDisplay();
            }

            if (mimicInput)
            {
                var lastIdle = IdleTimeFinder.GetIdleTimeMilliseconds();
                Debugger.Log(2, "Info", $"Last idle: {lastIdle}\n");
                if (lastIdle > 10000)
                {
                    if (m_jiggled)
                    {
                        Jiggler.Jiggle(-m_lastMovedDistance, -m_lastMovedDistance);
                    }
                    else
                    {
                        m_lastMovedDistance = RandomAtStart.Next(1, 4);
                        Jiggler.Jiggle(m_lastMovedDistance, m_lastMovedDistance);
                        m_jiggled = !m_jiggled;
                    }
                    //SendKeys.Send("{NUMLOCK}{NUMLOCK}");
                }
            }
        }
    }
}
