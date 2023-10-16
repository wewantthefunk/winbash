using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bash.dotnet
{
    internal class RMBash : ICommand
    {
        private ConfigOptions _configOptions;
        private IView _view;

        public RMBash(IView view, ConfigOptions configOptions)
        {
            _view = view;
            _configOptions = configOptions;
        }

        public ConfigOptions Go(string[] args)
        {
            if (args.Length < 1)
            {
                _view.DisplayError("mv command requires a file to delete\n");
                return _configOptions;
            }

            if (!File.Exists(args[0]))
            {
                _view.DisplayError($"Source file '{args[0]}' does not exists.\n");
                return _configOptions;
            }

            try
            {
                File.Delete(args[0]);
            }
            catch (Exception e)
            {
                _view.DisplayError(e.Message + Environment.NewLine);
            }

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
