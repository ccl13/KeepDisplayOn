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
        public DateTime m_ScreensaverTimeoutRefreshedAt = DateTime.MinValue;

        public uint m_ScreensaverActiveState;
        public bool m_ScreensaverActiveStateIsRefreshed;
        public DateTime m_ScreensaverActiveStateRefreshedAt = DateTime.MinValue;

        public HashSet<string> m_CurrentDisplayAdapterNames;
        public bool m_CurrentDisplayAdapterNamesIsRefreshed;
        public DateTime m_CurrentDisplayAdapterNamesRefreshedAt = DateTime.MinValue;

        public bool m_ShouldSetScreenSaverOnEnd = false;
        public DateTime m_ShouldSetScreenSaverOnEndRefreshedAt = DateTime.MinValue;

        public uint m_SetRequired;
        public bool m_SetRequiredIsSuccessful;
        public DateTime m_SetRequiredCalledAt = DateTime.MinValue;

        protected bool m_Jiggled = false;
        protected int m_LastJiggleMovedDistance = 1;

        public void PullSystemSettings()
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

        public void InvestigateScreenSaverSetting()
        {
            PullSystemSettings();
            if (m_ScreensaverActiveStateIsRefreshed)
            {
                m_ShouldSetScreenSaverOnEndRefreshedAt = DateTime.Now;
                m_ShouldSetScreenSaverOnEnd = m_ScreensaverActiveState > 0;
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
            if (m_ScreensaverActiveStateIsRefreshed && m_ShouldSetScreenSaverOnEnd)
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
            if (ret > 60000)
            {
                ret = 60000;
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
                    if (m_Jiggled)
                    {
                        Jiggler.Jiggle(-m_LastJiggleMovedDistance, -m_LastJiggleMovedDistance);
                    }
                    else
                    {
                        m_LastJiggleMovedDistance = RandomAtStart.Next(1, 4);
                        Jiggler.Jiggle(m_LastJiggleMovedDistance, m_LastJiggleMovedDistance);
                        m_Jiggled = !m_Jiggled;
                    }
                    //SendKeys.Send("{NUMLOCK}{NUMLOCK}");
                }
            }
        }
    }
}
