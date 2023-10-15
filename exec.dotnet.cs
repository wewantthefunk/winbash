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
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"/c {_command} {string.Join(" ", args)}"
            };

            var process = new Process { StartInfo = processStartInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            _view.Display(output);

            if (!string.IsNullOrEmpty(error))
            {
                _view.DisplayError(error);
            }

            process.WaitForExit();

            return _configOptions;
        }
    }
}