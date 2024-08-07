namespace KeepDisplayOn.WIN32APIs
{
    using Microsoft.Win32;

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Main article: https://learn.microsoft.com/en-us/windows/win32/termserv/detecting-the-terminal-services-environment
    /// </summary>
    class RemoteDesktopDetector
    {
        const string TERMINAL_SERVER_KEY = @"SYSTEM\CurrentControlSet\Control\Terminal Server\";
        const string GLASS_SESSION_ID = "GlassSessionId";

        const int SM_REMOTESESSION = 0x1000;

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        public static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

        public static bool IsCurrentSessionRemote()
        {
            if (GetSystemMetrics(SM_REMOTESESSION) != 0)
            {
                return true;
            }

            bool isRemoteable = false;

            RegistryKey? regKey = null;

            try
            {
                regKey = Registry.LocalMachine.OpenSubKey(TERMINAL_SERVER_KEY, false);
                if (regKey == null)
                {
                    return isRemoteable;
                }

                object? value = regKey.GetValue(GLASS_SESSION_ID);
                if (value == null || !(value is int glassSessionId))
                {
                    return isRemoteable;
                }

                if (ProcessIdToSessionId(GetCurrentProcessId(), out uint currentSessionId))
                {
                    isRemoteable = (currentSessionId != glassSessionId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                regKey?.Close();
            }


            return isRemoteable;
        }

    }

}
