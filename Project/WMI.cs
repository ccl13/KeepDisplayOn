using System;
using System.Collections.Generic;
using System.Management;
using System.Diagnostics;

namespace KeepDisplayOn
{
    public static class WMI
    {
        public static readonly ManagementScope DefaultScope = new ManagementScope("root\\CIMV2");
        private static readonly SelectQuery QueryVideoController = new SelectQuery("Win32_VideoController");
        private static readonly EnumerationOptions QueryVideoControllerOptions = new EnumerationOptions(
            context: null,
            timeout: TimeSpan.FromSeconds(5),
            blockSize: 1,
            rewindable: true,
            returnImmediatley: true,
            useAmendedQualifiers: false,
            ensureLocatable: false,
            prototypeOnly: false,
            directRead: true,
            enumerateDeep: false);

        public static List<string> GetActiveDisplayAdapterNames()
        {
            var output = new List<string>();

            try
            {
                // Connect to WMI before creating searcher
                if (!DefaultScope.IsConnected)
                {
                    DefaultScope.Connect();
                }

                using var searcher = new ManagementObjectSearcher(DefaultScope, QueryVideoController, QueryVideoControllerOptions);
                using var collection = searcher.Get();

                foreach (ManagementBaseObject obj in collection)
                {
                    // Ensure proper disposal of each ManagementBaseObject
                    using var mo = (ManagementObject)obj;
                    try
                    {
                        var currentBitsPerPixel = mo.GetPropertyValue("CurrentBitsPerPixel");
                        var description = mo.GetPropertyValue("Description");

                        if (currentBitsPerPixel != null && description != null)
                        {
                            output.Add(description.ToString());
                        }
                    }
                    catch (ManagementException ex)
                    {
                        // Log property access error but continue processing
                        Debug.WriteLine($"Error accessing WMI property: {ex.Message}");
                    }
                }
            }
            catch (ManagementException ex)
            {
                // Log WMI access error
                Debug.WriteLine($"WMI Error: {ex.Message}");
                throw; // Rethrow if you want calling code to handle it
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle access permission issues
                Debug.WriteLine($"Access denied to WMI: {ex.Message}");
                throw;
            }

            return output;
        }
    }
}
