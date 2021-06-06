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

        public static Random RandomAtStart = new Random();

        public uint m_ScreensaverTimeout;
        public bool m_ScreensaverTimeoutIsRefreshed;
        public DateTime m_ScreensaverTimeoutRefreshedAt;

        public int m_ScreensaverWasActive;
        public bool m_ScreensaverWasActiveIsRefreshed;
        public DateTime m_ScreensaverWasActiveRefreshedAt;

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

            m_ScreensaverWasActiveIsRefreshed = false;
            {
                uint b = 0;
                m_ScreensaverWasActiveRefreshedAt = DateTime.Now;
                var getScreenSaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_GETSCREENSAVEACTIVE, 0, ref b, 0);
                m_ScreensaverWasActiveIsRefreshed = getScreenSaverActiveCallRet > 0;
                if (m_ScreensaverWasActiveIsRefreshed)
                {
                    m_ScreensaverWasActive = (int)b;
                }
            }
        }

        public void DisableScreenSaver()
        {
            if (m_ScreensaverWasActiveIsRefreshed && m_ScreensaverWasActive > 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, 0, ref uintNULL, 0);
            }
        }

        public void RestoreSystem()
        {
            if (m_ScreensaverWasActiveIsRefreshed && m_ScreensaverWasActive > 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, m_ScreensaverWasActive, ref uintNULL, 0);
            }
        }

        public void PullConnectedDisplayAdapterInfo()
        {
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

        public void KeepAlive(bool keepDisplay, bool mimicInput)
        {
            m_SetRequiredCalledAt = DateTime.Now;
            if (keepDisplay)
            {
                m_SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_DISPLAY_REQUIRED | ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            }
            else
            {
                m_SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            }
            m_SetRequiredIsSuccessful = m_SetRequired != 0;

            var lastIdle = IdleTimeFinder.GetIdleTimeMilliseconds();
            Debugger.Log(2, "Info", $"Last idle: {lastIdle}\n");
            if (mimicInput && lastIdle > 10000)
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
