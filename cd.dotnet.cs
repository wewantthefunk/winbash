using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace bash.dotnet {
    class CDBash : ICommand {
        private readonly ConfigOptions _configOptions;

        private IView _view;

        private IInput _inputDevice;
        public CDBash(ConfigOptions configOptions, IView view, IInput inputDevice) {
            _configOptions = configOptions;
            _view = view;
            _inputDevice = inputDevice;
        }
        
        public ConfigOptions Go(string[] args) {
            string path = string.Empty;

            for (int x = 0; x < args.Length; x++) {
                if (args[x] == "~home") {
                    args[x] = _configOptions.getStartingDir();
                }
                path += args[x];
            }

            path = path.Replace("/", "\\");
            path = path.Replace("\"", "");

            try {
                Directory.SetCurrentDirectory(path);
            } catch {
                _view.DisplayError("Invalid directory\n");
            }
            _configOptions.resetPrompt();
            _inputDevice.SetCurrentDirectory(Directory.GetCurrentDirectory() + "\\");
            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}