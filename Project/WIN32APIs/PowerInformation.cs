using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KeepDisplayOn.WIN32APIs
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/powerbase/nf-powerbase-callntpowerinformation
    /// </summary>
    public class PowerInformation
    {
        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a SYSTEM_POWER_INFORMATION structure.
        /// Applications can use this level to retrieve information about the idleness of the system.
        /// </summary>
        const uint SystemPowerInformation = 12;

        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a SYSTEM_POWER_POLICY structure containing the current system power policy used while the system is running on AC (utility) power.
        /// </summary>
        const uint SystemPowerPolicyCurrent = 8;

        const uint STATUS_SUCCESS = 0;
        const uint STATUS_BUFFER_TOO_SMALL = 0xC0000023;
        const uint STATUS_ACCESS_DENIED = 0xC0000022;

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_POWER_INFORMATION
        {
            public uint MaxIdlenessAllowed;
            public uint Idleness;
            public uint TimeRemaining;
            public byte CoolingMode;
        }

        public enum POWER_ACTION : uint
        {
            PowerActionNone = 0,       // No system power action.
            PowerActionReserved,       // Reserved; do not use.
            PowerActionSleep,      // Sleep.
            PowerActionHibernate,      // Hibernate.
            PowerActionShutdown,       // Shutdown.
            PowerActionShutdownReset,  // Shutdown and reset.
            PowerActionShutdownOff,    // Shutdown and power off.
            PowerActionWarmEject,      // Warm eject.
        }

        [Flags]
        public enum PowerActionFlags : uint
        {
            POWER_ACTION_QUERY_ALLOWED = 0x00000001, // Broadcasts a PBT_APMQUERYSUSPEND event to each application to request permission to suspend operation.
            POWER_ACTION_UI_ALLOWED = 0x00000002, // Applications can prompt the user for directions on how to prepare for suspension. Sets bit 0 in the Flags parameter passed in the lParam parameter of
            POWER_ACTION_OVERRIDE_APPS = 0x00000004, // Ignores applications that do not respond to the PBT_APMQUERYSUSPEND event broadcast in the WM_POWERBROADCAST message.
            POWER_ACTION_LIGHTEST_FIRST = 0x10000000, // Uses the first lightest available sleep state.
            POWER_ACTION_LOCK_CONSOLE = 0x20000000, // Requires entry of the system password upon resume from one of the system standby states.
            POWER_ACTION_DISABLE_WAKES = 0x40000000, // Disables all wake events.
            POWER_ACTION_CRITICAL = 0x80000000, // Forces a critical suspension.
        }

        [Flags]
        public enum PowerActionEventCode : uint
        {
            POWER_LEVEL_USER_NOTIFY_TEXT = 0x00000001, // User notified using the UI.
            POWER_LEVEL_USER_NOTIFY_SOUND = 0x00000002, // User notified using sound.
            POWER_LEVEL_USER_NOTIFY_EXEC = 0x00000004, // Specifies a program to be executed.
            POWER_USER_NOTIFY_BUTTON = 0x00000008, // Indicates that the power action is in response to a user power button press.
            POWER_USER_NOTIFY_SHUTDOWN = 0x00000010, // Indicates a power action of shutdown/off.
            POWER_FORCE_TRIGGER_RESET = 0x80000000, // Clears a user power button press.
        }

        public enum SYSTEM_POWER_STATE : UInt32
        {
            PowerSystemUnspecified = 0,
            PowerSystemWorking = 1,
            PowerSystemSleeping1 = 2,
            PowerSystemSleeping2 = 3,
            PowerSystemSleeping3 = 4,
            PowerSystemHibernate = 5,
            PowerSystemShutdown = 6,
            PowerSystemMaximum = 7
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct POWER_ACTION_POLICY
        {
            public POWER_ACTION Action;
            public PowerActionFlags Flags;
            public PowerActionEventCode EventCode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SYSTEM_POWER_LEVEL // SIZE MUST BE 24 bytes
        {
            public byte Enable;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Spare;
            public uint BatteryLevel;
            public POWER_ACTION_POLICY PowerPolicy;
            public SYSTEM_POWER_STATE MinSystemState;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct SYSTEM_POWER_POLICY // SIZE MUST BE 232 bytes
        {
            public UInt32 Revision;
            public POWER_ACTION_POLICY PowerButton;
            public POWER_ACTION_POLICY SleepButton;
            public POWER_ACTION_POLICY LidClose;
            public SYSTEM_POWER_STATE LidOpenWake;
            public UInt32 Reserved;
            public POWER_ACTION_POLICY Idle;
            public UInt32 IdleTimeout;
            public byte IdleSensitivity;
            public byte DynamicThrottle;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Spare2;
            public SYSTEM_POWER_STATE MinSleep;
            public SYSTEM_POWER_STATE MaxSleep;
            public SYSTEM_POWER_STATE ReducedLatencySleep;
            public UInt32 WinLogonFlags;
            public UInt32 Spare3;
            public UInt32 DozeS4Timeout;
            public UInt32 BroadcastCapacityResolution;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public SYSTEM_POWER_LEVEL[] DischargePolicy;
            public UInt32 VideoTimeout;
            public byte VideoDimDisplay;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public UInt32[] VideoReserved;
            public UInt32 SpindownTimeout;
            public byte OptimizeForPower;
            public byte FanThrottleTolerance;
            public byte ForcedThrottle;
            public byte MinThrottle;
            public POWER_ACTION_POLICY OverThrottled;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/powerbase/nf-powerbase-callntpowerinformation
        /// </summary>
        /// <param name="InformationLevel"></param>
        /// <param name="lpInputBuffer"></param>
        /// <param name="nInputBufferSize"></param>
        /// <param name="structure"></param>
        /// <param name="nOutputBufferSize"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            uint InformationLevel,
            IntPtr lpInputBuffer,
            uint nInputBufferSize,
            IntPtr structure,
            uint nOutputBufferSize
        );

        public static bool GetPowerInformation(out SYSTEM_POWER_INFORMATION value)
        {
            SYSTEM_POWER_INFORMATION spi = new SYSTEM_POWER_INFORMATION();
            value = spi;

            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(spi));
            Marshal.StructureToPtr(spi, pnt, false);

            uint retval = CallNtPowerInformation(
                SystemPowerInformation,
                IntPtr.Zero,
                0,
                pnt,
                (uint)Marshal.SizeOf(spi)
            );

            var ret = retval == STATUS_SUCCESS;
            if (ret)
            {
                spi = Marshal.PtrToStructure<SYSTEM_POWER_INFORMATION>(pnt);
                value = spi;
            }
            return ret;
        }

        public static bool GetPowerPolicy(out SYSTEM_POWER_POLICY value)
        {
            SYSTEM_POWER_POLICY spp = new SYSTEM_POWER_POLICY();
            value = spp;

            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(spp));
            Marshal.StructureToPtr(spp, pnt, false);

            uint retval = CallNtPowerInformation(
                SystemPowerPolicyCurrent,
                IntPtr.Zero,
                0,
                pnt,
                (uint)Marshal.SizeOf(spp)
            );

            var ret = retval == STATUS_SUCCESS;
            if (ret)
            {
                spp = Marshal.PtrToStructure<SYSTEM_POWER_POLICY>(pnt);
                value = spp;
            }
            return ret;
        }
    }
}
