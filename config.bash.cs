using bash.dotnet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bash.dotnet
{
    internal class CONFIGBash : ICommand
    {
        private ConfigOptions _configOptions;
        private ICommandFactory _factory;
        private IView _view;
        private IInput _inputDevice;

        public CONFIGBash(ConfigOptions configOptions, IView view, ICommandFactory factory, IInput inputDevice)
        {
            _configOptions = configOptions;
            _view = view;
            _factory = factory;
            _inputDevice = inputDevice;
        }

        public ConfigOptions Go(string[] args)
        {
            ICommand command = _factory.Create("notepad", _configOptions);
            command.Go(new string[] { _configOptions.getLaunchDir().Replace("/", "\\") + "\\keyboard.cfg" });
            _configOptions.setPromptTmeplate(ConfigOptions.DEFAULT_PROMPT);
            _configOptions = _inputDevice.ReadConfig(_configOptions);
            _configOptions.resetPrompt();
            _configOptions.resetEnvironmentPath();
            _view.SetConfigOptions(_configOptions);
            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
