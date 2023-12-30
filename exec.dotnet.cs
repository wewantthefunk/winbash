using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace bash.dotnet
{

    public struct PROCESS_INFORMATION {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    public struct STARTUPINFO {
        public uint cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public uint dwX;
        public uint dwY;
        public uint dwXSize;
        public uint dwYSize;
        public uint dwXCountChars;
        public uint dwYCountChars;
        public uint dwFillAttribute;
        public uint dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    class EXECBash : ICommand {

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(
            string? lpApplicationName,
            StringBuilder lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string? lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);


        private string _command;

        private IView _view;

        private ConfigOptions _configOptions;

        public EXECBash(ConfigOptions configOptions, IView view, string command) {
            _command = command;    
            _view = view;
            _configOptions = configOptions;
        }
        public ConfigOptions Go(string[] args) {

            startProcessShell(args);

            return _configOptions;
        }

        private void startProcessShell(string[] args) {
            STARTUPINFO si = new();

            // Create the process
            bool success = CreateProcess(
                null, // Use command line
                new StringBuilder(_command + " " + string.Join(" ", args)), // Command line
                IntPtr.Zero, // Process handle not inheritable
                IntPtr.Zero, // Thread handle not inheritable
                false, // Set handle inheritance to FALSE
                0, // No creation flags
                IntPtr.Zero, // Use parent's environment block
                null, // Use parent's starting directory 
                ref si, // Pointer to STARTUPINFO structure
                out PROCESS_INFORMATION pi // Pointer to PROCESS_INFORMATION structure
            );

            // Check to see if CreateProcess succeeded
            if (success) {
                // Here, you might want to do something with the process, such as waiting for it to complete

                _ = WaitForSingleObject(pi.hProcess, uint.MaxValue);
                // Close process and thread handles. 
                CloseHandle(pi.hProcess);
                CloseHandle(pi.hThread);
            } else {
                // Handle the error
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"CreateProcess failed with error {error}");
            }
        }

        public void SetProperty(string key, string value) { }
    }
}