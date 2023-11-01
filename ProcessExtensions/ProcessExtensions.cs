using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessExtensions
{
    public static class ProcessExtensions
    {
        // Define the PROCESS_BASIC_INFORMATION struct
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2_0;
            public IntPtr Reserved2_1;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        // Import the NtQueryInformationProcess function from ntdll.dll
        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass,
                                                            ref PROCESS_BASIC_INFORMATION processInformation,
                                                            uint processInformationLength,
                                                            out uint returnLength);

        public static Process? GetParentProcess(this Process process)
        {
            // Use the handle from the Process object directly
            IntPtr processHandle = process.Handle;

            // Create a PROCESS_BASIC_INFORMATION object and fill it
            PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
            uint bytesWritten;
            int status = NtQueryInformationProcess(processHandle, 0, ref pbi, (uint)Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)), out bytesWritten);

            if (status != 0)
            {
                throw new ApplicationException("NtQueryInformationProcess failed", new System.ComponentModel.Win32Exception(status));
            }

            // Get the parent process ID
            int parentProcessId = pbi.InheritedFromUniqueProcessId.ToInt32();

            // Return the parent process as a Process object if it's running
            try
            {
                return Process.GetProcessById(parentProcessId);
            }
            catch (ArgumentException)
            {
                // If the parent process is not running, GetProcessById will throw an ArgumentException
                return null;
            }
            catch (InvalidOperationException)
            {
                // If there is any issue in getting the parent process, return null
                return null;
            }
        }
    }
}