using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace bash.dotnet
{
    class EXECBash : ICommand {
        private string _command;

        private IView _view;

        private ConfigOptions _configOptions;

        public EXECBash(ConfigOptions configOptions, IView view, string command) {
            _command = command;    
            _view = view;
            _configOptions = configOptions;
        }
        public ConfigOptions Go(string[] args) {
            string currentDirectory = Environment.CurrentDirectory;
            string cmdWithArgs = "/c \"" + _configOptions.getLaunchDir() + "\\" + _command + "\" " + string.Join(" ", args).Replace("\"", "'");
            
            var processStartInfo = new ProcessStartInfo {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = cmdWithArgs

            };

            // Create and start the process
            using (Process process = new Process()) {
                process.StartInfo = processStartInfo;

                // Define event handlers for real-time output
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data)) {
                        _view.Display(e.Data + Environment.NewLine);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data)) {
                        _view.DisplayError(e.Data + Environment.NewLine);
                    }
                };

                // Start the process
                process.Start();

                // Start reading from the redirected streams
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to exit
                process.WaitForExit();
            }

            Environment.CurrentDirectory = currentDirectory;

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}