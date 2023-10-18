using System.IO;

namespace bash.dotnet
{
    class MKDIRBash : ICommand
    {
        private ConfigOptions _configOptions;

        private IView _view;

        public MKDIRBash(ConfigOptions configOptions, IView view) {
            _configOptions = configOptions;
            _view = view;
        }

        public ConfigOptions Go(string[] args) {
            // Determine whether the directory exists
            if (Directory.Exists(args[0])) {
                return _configOptions;
            }

            try {
                // Try to create the directory
                Directory.CreateDirectory(args[0]);
            } catch { 
                _view.DisplayError("Unable to create directory: " + args[0]);
            }
            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
