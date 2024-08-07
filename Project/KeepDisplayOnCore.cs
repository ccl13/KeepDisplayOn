using ArkaneSystems.MouseJiggle;

using KeepDisplayOn.WIN32APIs;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KeepDisplayOn
{
    public class KeepDisplayOnCore
    {
        public const string RemoteDesktopDisplayAdapterName = "Microsoft Remote Display Adapter";

        private static uint uintNULL = 0;
        public static readonly TimeSpan ApiGuardInterval = TimeSpan.FromSeconds(5);

        public static readonly Random RandomAtStart = new Random();

        public uint m_LastPulledScreensaverTimeout;
        public bool m_LastPulledScreensaverTimeoutIsRefreshed;
        public DateTime m_LastPulledScreensaverTimeoutRefreshedAt = DateTime.MinValue;

        public uint m_LastPulledScreensaverActiveState;
        public bool m_LastPulledScreensaverActiveStateIsRefreshed;
        public DateTime m_LastPulledScreensaverActiveStateRefreshedAt = DateTime.MinValue;

        public HashSet<string> m_LastPulledDisplayAdapterNames;
        public bool m_LastPulledDisplayAdapterNamesIsRefreshed;
        public DateTime m_LastPulledDisplayAdapterNamesRefreshedAt = DateTime.MinValue;

        public bool m_ShouldSetScreenSaverOnEnd = false;
        public DateTime m_ShouldSetScreenSaverOnEndRefreshedAt = DateTime.MinValue;

        public uint m_SetRequired;
        public bool m_SetRequiredIsSuccessful;
        public DateTime m_SetRequiredCalledAt = DateTime.MinValue;

        protected bool m_Jiggled = false;
        protected int m_LastJiggleMovedDistance = 1;

        protected bool m_LastRemoteSessionIndicator = false;
        public DateTime m_LastRemoteSessionIndicatorRefreshedAt = DateTime.MinValue;

        const int MaxKeepAliveInternal = 60000;
        const int MinKeepAliveInternal = 10000;
        const int DefaultKeepAliveInternal = 30000;

        public void PullSystemSettings()
        {
            m_LastPulledScreensaverTimeoutIsRefreshed = false;
            {
                m_LastPulledScreensaverTimeoutRefreshedAt = DateTime.Now;
                var getTimeoutCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVETIMEOUT, 0, ref m_LastPulledScreensaverTimeout, 0);
                m_LastPulledScreensaverTimeoutIsRefreshed = getTimeoutCallRet > 0;
            }

            m_LastPulledScreensaverActiveStateIsRefreshed = false;
            {
                m_LastPulledScreensaverActiveStateRefreshedAt = DateTime.Now;
                var getScreenSaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVEACTIVE, 0, ref m_LastPulledScreensaverActiveState, 0);
                m_LastPulledScreensaverActiveStateIsRefreshed = getScreenSaverActiveCallRet > 0;
            }
        }

        public void Initialize()
        {
            PullSystemSettings();
            if (m_LastPulledScreensaverActiveStateIsRefreshed)
            {
                m_ShouldSetScreenSaverOnEndRefreshedAt = DateTime.Now;
                m_ShouldSetScreenSaverOnEnd = m_LastPulledScreensaverActiveState != 0;
            }
        }

        public void DisableScreenSaver()
        {
            if (m_LastPulledScreensaverActiveStateIsRefreshed && m_LastPulledScreensaverActiveState != 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, 0, ref uintNULL, 0);
            }
        }

        public void RestoreSystem()
        {
            if (m_LastPulledScreensaverActiveStateIsRefreshed && m_ShouldSetScreenSaverOnEnd)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, m_LastPulledScreensaverActiveState, ref uintNULL, 0);
            }
        }

        public int GetRecommendedKeepAliveIntervalMilliseconds()
        {
            var ret = DefaultKeepAliveInternal;
            if (m_LastPulledScreensaverTimeoutIsRefreshed)
            {
                ret = (int)m_LastPulledScreensaverTimeout * 1000 / 2;
            }
            if (ret < MinKeepAliveInternal)
            {
                ret = MinKeepAliveInternal;
            }
            if (ret > MaxKeepAliveInternal)
            {
                ret = MaxKeepAliveInternal;
            }
            return ret;
        }

        public void RefreshRemoteSessionStatus()
        {
            PullRemoteSessionInfoStandard();
            //PullConnectedDisplayAdapterInfo();
        }

        public void PullRemoteSessionInfoStandard()
        {
            TimeSpan timePassed = DateTime.Now - m_LastRemoteSessionIndicatorRefreshedAt;
            if (timePassed < ApiGuardInterval)
            {
                return;
            }
            m_LastRemoteSessionIndicator = RemoteDesktopDetector.IsCurrentSessionRemote();
            m_LastRemoteSessionIndicatorRefreshedAt = DateTime.Now;
        }

        public void PullConnectedDisplayAdapterInfo()
        {
            // WMI Pull Guard
            if (m_LastPulledDisplayAdapterNamesIsRefreshed)
            {
                var timePassed = DateTime.Now - m_LastPulledDisplayAdapterNamesRefreshedAt;
                if (timePassed < ApiGuardInterval)
                {
                    // If last refresh too close, ignore refresh.
                    return;
                }
            }
            // Actual refresh
            m_LastPulledDisplayAdapterNamesIsRefreshed = false;
            try
            {
                m_LastPulledDisplayAdapterNamesRefreshedAt = DateTime.Now;
                var displayAdapterNames = WMI.GetActiveDisplayAdapterNames();
                m_LastPulledDisplayAdapterNames = new HashSet<string>(displayAdapterNames, StringComparer.InvariantCultureIgnoreCase);
                m_LastPulledDisplayAdapterNamesIsRefreshed = true;
            }
            catch (Exception ex)
            {
                // Left blank for debug.
            }
        }

        public bool IsInRemoteSession()
        {
            return m_LastRemoteSessionIndicator;
            //return m_LastPulledDisplayAdapterNames.Contains(RemoteDesktopDisplayAdapterName);
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

        public void KeepDisplayOn(bool keepScreenWake, bool mimicInput)
        {
            m_SetRequiredIsSuccessful = false;
            // Maintain internal assumption
            if (m_LastPulledScreensaverActiveStateIsRefreshed && DateTime.Now - m_LastPulledScreensaverActiveStateRefreshedAt > TimeSpan.FromMinutes(2))
            {
                PullSystemSettings();
                if (m_ShouldSetScreenSaverOnEnd == (m_LastPulledScreensaverActiveState != 0))
                {
                    DisableScreenSaver();
                }
            }
            // Keep Display On
            if (keepScreenWake)
            {
                RunSetAliveWithKeepDisplay();
            }
            if (!m_SetRequiredIsSuccessful)
            {
                if (keepScreenWake)
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
