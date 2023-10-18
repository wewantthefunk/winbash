using System.IO;

namespace bash.dotnet
{
    class RMDIRBash : ICommand {
        private ConfigOptions _configOptions;

        private IView _view;

        public RMDIRBash(ConfigOptions configOptions, IView view) {
            _configOptions = configOptions;
            _view = view;
        }

        public ConfigOptions Go(string[] args) {
            // Determine whether the directory exists
            if (!Directory.Exists(args[0])) {
                return _configOptions;
            }

            try {
                // Try to create the directory
                Directory.Delete(args[0]);
            }
            catch {
                _view.DisplayError("Unable to delete directory: " + args[0]);
            }
            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
