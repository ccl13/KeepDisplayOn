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

        private uint _lastPulledScreensaverTimeout;
        public uint LastPulledScreensaverTimeout
        {
            get => _lastPulledScreensaverTimeout;
            protected set => _lastPulledScreensaverTimeout = value;
        }


        public bool LastPulledScreensaverTimeoutIsRefreshed { get; protected set; }
        public DateTime LastPulledScreensaverTimeoutRefreshedAt { get; protected set; } = DateTime.MinValue;

        private uint _lastPulledScreensaverActiveState;
        public uint LastPulledScreensaverActiveState
        {
            get => _lastPulledScreensaverActiveState;
            protected set => _lastPulledScreensaverActiveState = value;
        }

        public bool LastPulledScreensaverActiveStateIsRefreshed { get; protected set; }
        public DateTime LastPulledScreensaverActiveStateRefreshedAt { get; protected set; } = DateTime.MinValue;

        public HashSet<string> LastPulledDisplayAdapterNames { get; protected set; }
        public bool LastPulledDisplayAdapterNamesIsRefreshed { get; protected set; }
        public DateTime LastPulledDisplayAdapterNamesRefreshedAt { get; protected set; } = DateTime.MinValue;

        public bool ShouldSetScreenSaverOnEnd { get; protected set; }
        public DateTime ShouldSetScreenSaverOnEndRefreshedAt { get; protected set; } = DateTime.MinValue;

        public uint SetRequired { get; protected set; }
        public bool SetRequiredIsSuccessful { get; protected set; }
        public DateTime SetRequiredCalledAt { get; protected set; } = DateTime.MinValue;

        protected bool Jiggled { get; set; }
        protected int LastJiggleMovedDistance { get; set; } = 1;

        protected bool LastRemoteSessionIndicator { get; set; }
        public DateTime LastRemoteSessionIndicatorRefreshedAt { get; protected set; } = DateTime.MinValue;

        private static readonly TimeSpan MINIMUM_KEEP_ALIVE_INTERVAL = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan MAXIMUM_KEEP_ALIVE_INTERVAL = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan DEFAULT_KEEP_ALIVE_INTERVAL = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan SETTINGS_REFRESH_INTERVAL = TimeSpan.FromMinutes(2);

        public void PullSystemSettings()
        {
            LastPulledScreensaverTimeoutIsRefreshed = false;
            {
                LastPulledScreensaverTimeoutRefreshedAt = DateTime.Now;
                var getTimeoutCallRet = ScreenSaverInteractions.SystemParametersInfo(
                    ScreenSaverInteractions.SPI_GETSCREENSAVETIMEOUT,
                    0,
                    ref _lastPulledScreensaverTimeout,
                    0);
                LastPulledScreensaverTimeoutIsRefreshed = getTimeoutCallRet > 0;
            }

            LastPulledScreensaverActiveStateIsRefreshed = false;
            {
                LastPulledScreensaverActiveStateRefreshedAt = DateTime.Now;
                var getScreenSaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(
                    ScreenSaverInteractions.SPI_GETSCREENSAVEACTIVE,
                    0,
                    ref _lastPulledScreensaverActiveState,
                    0);
                LastPulledScreensaverActiveStateIsRefreshed = getScreenSaverActiveCallRet > 0;
            }
        }

        public void Initialize()
        {
            PullSystemSettings();
            if (LastPulledScreensaverActiveStateIsRefreshed)
            {
                ShouldSetScreenSaverOnEndRefreshedAt = DateTime.Now;
                ShouldSetScreenSaverOnEnd = LastPulledScreensaverActiveState != 0;
            }
        }

        public void DisableScreenSaver()
        {
            if (LastPulledScreensaverActiveStateIsRefreshed && LastPulledScreensaverActiveState != 0)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, 0, ref uintNULL, 0);
            }
        }

        public void RestoreSystem()
        {
            if (LastPulledScreensaverActiveStateIsRefreshed && ShouldSetScreenSaverOnEnd)
            {
                var setScreensaverActiveCallRet = ScreenSaverInteractions.SystemParametersInfo(ScreenSaverInteractions.SPI_SETSCREENSAVEACTIVE, LastPulledScreensaverActiveState, ref uintNULL, 0);
            }
        }

        public int GetRecommendedKeepAliveIntervalMilliseconds()
        {
            var ret = DEFAULT_KEEP_ALIVE_INTERVAL.TotalMilliseconds;
            if (LastPulledScreensaverTimeoutIsRefreshed)
            {
                ret = (int)LastPulledScreensaverTimeout * 1000 / 2;
            }
            if (ret < MINIMUM_KEEP_ALIVE_INTERVAL.TotalMilliseconds)
            {
                ret = MINIMUM_KEEP_ALIVE_INTERVAL.TotalMilliseconds;
            }
            if (ret > MAXIMUM_KEEP_ALIVE_INTERVAL.TotalMilliseconds)
            {
                ret = MAXIMUM_KEEP_ALIVE_INTERVAL.TotalMilliseconds;
            }
            return (int)ret;
        }

        public void RefreshRemoteSessionStatus()
        {
            PullRemoteSessionInfoStandard();
            //PullConnectedDisplayAdapterInfo();
        }

        public void PullRemoteSessionInfoStandard()
        {
            TimeSpan timePassed = DateTime.Now - LastRemoteSessionIndicatorRefreshedAt;
            if (timePassed < ApiGuardInterval)
            {
                return;
            }
            LastRemoteSessionIndicator = RemoteDesktopDetector.IsCurrentSessionRemote();
            LastRemoteSessionIndicatorRefreshedAt = DateTime.Now;
        }

        public void PullConnectedDisplayAdapterInfo()
        {
            // WMI Pull Guard
            if (LastPulledDisplayAdapterNamesIsRefreshed)
            {
                var timePassed = DateTime.Now - LastPulledDisplayAdapterNamesRefreshedAt;
                if (timePassed < ApiGuardInterval)
                {
                    // If last refresh too close, ignore refresh.
                    return;
                }
            }
            // Actual refresh
            LastPulledDisplayAdapterNamesIsRefreshed = false;
            try
            {
                LastPulledDisplayAdapterNamesRefreshedAt = DateTime.Now;
                var displayAdapterNames = WMI.GetActiveDisplayAdapterNames();
                LastPulledDisplayAdapterNames = new HashSet<string>(displayAdapterNames, StringComparer.InvariantCultureIgnoreCase);
                LastPulledDisplayAdapterNamesIsRefreshed = true;
            }
            catch (Exception ex)
            {
                // Left blank for debug.
            }
        }

        public bool IsInRemoteSession()
        {
            return LastRemoteSessionIndicator;
            //return LastPulledDisplayAdapterNames.Contains(RemoteDesktopDisplayAdapterName);
        }

        public void RunSetAliveWithKeepDisplay()
        {
            // Guard
            if (SetRequiredIsSuccessful)
            {
                var timePassed = DateTime.Now - SetRequiredCalledAt;
                if (timePassed < ApiGuardInterval)
                {
                    return;
                }
            }
            SetRequiredIsSuccessful = false;
            SetRequiredCalledAt = DateTime.Now;
            SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_DISPLAY_REQUIRED | ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            SetRequiredIsSuccessful = SetRequired != 0;
        }

        public void RunSetAliveWithoutKeepDisplay()
        {
            // Guard
            if (SetRequiredIsSuccessful)
            {
                var timePassed = DateTime.Now - SetRequiredCalledAt;
                if (timePassed < ApiGuardInterval)
                {
                    return;
                }
            }
            SetRequiredIsSuccessful = false;
            SetRequiredCalledAt = DateTime.Now;
            SetRequired = ScreenSaverInteractions.SetThreadExecutionState(ScreenSaverInteractions.ES_SYSTEM_REQUIRED);
            SetRequiredIsSuccessful = SetRequired != 0;
        }

        public void KeepDisplayOn(bool keepScreenWake, bool mimicInput)
        {
            // Refresh system settings if needed
            if (ShouldRefreshSettings())
            {
                PullSystemSettings();
                EnsureScreenSaverState();
            }

            // Try to keep display on using primary method
            bool displayKeepSuccess = false;
            if (keepScreenWake)
            {
                displayKeepSuccess = TryKeepDisplayOn();
            }

            // Fall back to system-only method if display keep failed
            if (!displayKeepSuccess)
            {
                TryKeepSystemOn();
            }

            // Optionally simulate user input
            if (mimicInput)
            {
                SimulateUserActivity();
            }
        }

        private bool ShouldRefreshSettings()
        {
            return !LastPulledScreensaverActiveStateIsRefreshed ||
                   DateTime.Now - LastPulledScreensaverActiveStateRefreshedAt > SETTINGS_REFRESH_INTERVAL;
        }

        private void EnsureScreenSaverState()
        {
            if (ShouldSetScreenSaverOnEnd == (LastPulledScreensaverActiveState != 0))
            {
                DisableScreenSaver();
            }
        }

        private bool TryKeepDisplayOn()
        {
            if (CanCallSetRequired())
            {
                RunSetAliveWithKeepDisplay();
                return SetRequiredIsSuccessful;
            }
            return false;
        }

        private bool TryKeepSystemOn()
        {
            if (CanCallSetRequired())
            {
                RunSetAliveWithoutKeepDisplay();
                return SetRequiredIsSuccessful;
            }
            return false;
        }

        private void SimulateUserActivity()
        {
            var idleTime = IdleTimeFinder.GetIdleTimeMilliseconds();
            if (idleTime > 10000) // Consider making this threshold configurable
            {
                JiggleMouse();
            }
        }

        private void JiggleMouse()
        {
            if (Jiggled)
            {
                Jiggler.Jiggle(-LastJiggleMovedDistance, -LastJiggleMovedDistance);
            }
            else
            {
                LastJiggleMovedDistance = RandomAtStart.Next(1, 4);
                Jiggler.Jiggle(LastJiggleMovedDistance, LastJiggleMovedDistance);
            }
            Jiggled = !Jiggled;
        }

        private bool CanCallSetRequired()
        {
            var timePassed = DateTime.Now - SetRequiredCalledAt;
            if (timePassed < ApiGuardInterval)
            {
                return false;
            }
            SetRequiredIsSuccessful = false;
            SetRequiredCalledAt = DateTime.Now;
            return true;
        }
    }
}
