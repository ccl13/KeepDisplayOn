using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace KeepDisplayOn.WIN32APIs
{
    public static class PowerOverlay
    {
        private const string PowerSchemesRegistryPath = @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes";
        private const string ActiveOverlayAcKey = "ActiveOverlayAcPowerScheme";
        private const string ActiveOverlayDcKey = "ActiveOverlayDcPowerScheme";

        public static readonly Guid LowPowerOverlay = new Guid("961cc777-2547-4f9d-8174-7d86181b8a7a");
        public static readonly Guid DefaultPowerOverlay = Guid.Empty;


        // Import undocumented API from powrprof.dll
        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern uint PowerSetActiveOverlayScheme(ref Guid overlayGuid);

        /// <summary>
        /// Gets the currently active power overlay GUIDs for both AC and DC power
        /// </summary>
        /// <returns>Tuple containing AC and DC power overlay GUIDs</returns>
        public static (Guid acOverlay, Guid dcOverlay) GetActiveOverlays()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(PowerSchemesRegistryPath))
            {
                if (key == null)
                {
                    throw new Exception("Unable to open power schemes registry key");
                }

                var acValue = key.GetValue(ActiveOverlayAcKey);
                var dcValue = key.GetValue(ActiveOverlayDcKey);

                Guid acGuid = acValue != null ? new Guid(acValue.ToString()) : Guid.Empty;
                Guid dcGuid = dcValue != null ? new Guid(dcValue.ToString()) : Guid.Empty;

                return (acGuid, dcGuid);
            }
        }

        /// <summary>
        /// Gets all available power overlay schemes from the registry
        /// </summary>
        /// <returns>Dictionary of overlay GUIDs and their friendly names</returns>
        public static Dictionary<Guid, string> GetAllOverlays()
        {
            var overlays = new Dictionary<Guid, string>();

            using (var key = Registry.LocalMachine.OpenSubKey(PowerSchemesRegistryPath))
            {
                if (key == null)
                {
                    throw new Exception("Unable to open power schemes registry key");
                }

                foreach (string subKeyName in key.GetSubKeyNames())
                {
                    if (Guid.TryParse(subKeyName, out Guid overlayGuid))
                    {
                        using (var subKey = key.OpenSubKey(subKeyName))
                        {
                            if (subKey != null)
                            {
                                string friendlyName = subKey.GetValue("FriendlyName")?.ToString() ?? subKeyName;
                                overlays.Add(overlayGuid, friendlyName);
                            }
                        }
                    }
                }
            }

            return overlays;
        }

        /// <summary>
        /// Sets the active power overlay scheme
        /// </summary>
        /// <param name="overlayGuid">The GUID of the overlay to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetActiveOverlay(Guid overlayGuid)
        {
            uint result = PowerSetActiveOverlayScheme(ref overlayGuid);
            return result == 0;
        }

        /// <summary>
        /// Sets the power overlay to default (empty GUID)
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetDefaultOverlay()
        {
            Guid defaultOverlay = Guid.Empty;
            return SetActiveOverlay(defaultOverlay);
        }
    }
}
