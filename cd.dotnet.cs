using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace bash.dotnet
{
    class CDBash : ICommand
    {
        private ConfigOptions _configOptions;

        private IView _view;
        public CDBash(ConfigOptions configOptions, IView view) {
            _configOptions = configOptions;
            _view = view;
        }
        
        public ConfigOptions Go(string[] args)
        {
            string path = string.Empty;

            for (int x = 0; x < args.Length; x++) {
                path += args[x];
            }

            path = path.Replace("/", "\\");

            try {
                Directory.SetCurrentDirectory(path);
            } catch {
                _view.DisplayError("Invalid directory\n");
            }
            _configOptions.resetPrompt();
            return _configOptions;
        }
    }
}