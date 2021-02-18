using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace KeepDisplayOn
{
    public static class WMI
    {
        public static ManagementScope DefaultScope = new ManagementScope("root\\CIMV2");
        private static SelectQuery QueryVideoController = new SelectQuery("Win32_VideoController");
        private static EnumerationOptions QueryVideoControllerOptions = new EnumerationOptions(
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
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(DefaultScope, QueryVideoController, QueryVideoControllerOptions))
            using (var moCollection = searcher.Get())
            {
                foreach (ManagementObject mo in moCollection)
                {
                    using (mo)
                    {
                        PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                        PropertyData description = mo.Properties["Description"];
                        if (currentBitsPerPixel != null && description != null)
                        {
                            if (currentBitsPerPixel.Value != null && description.Value is string)
                            {
                                output.Add(description.Value as string);
                            }
                        }
                    }
                }
            }
            return output;
        }
    }
}
