namespace bash.dotnet {
    internal class COMPGENBash : ICommand {
        private IView _view;
        private ConfigOptions _configOptions;
        private IInput _inputDevice;
        private ICommandFactory _factory;

        public COMPGENBash (IView view, ConfigOptions configOptions, IInput inputDevice, ICommandFactory factory) {
            _view = view;
            _configOptions = configOptions;
            _inputDevice = inputDevice;
            _factory = factory;
        }

        public ConfigOptions Go(string[] args) {
            if (args.Length == 0) {
                _view.DisplayError("invalid switch\n");
            }

            string[] s = new string[0];

            if (args[0] == "-a") {
                s = _inputDevice.getAliases();                
            } else if (args[0] == "-b") {
                s = _factory.getBuiltInCommands();
            }

            _view.DisplayInfo("Total " + s.Length.ToString() + Environment.NewLine);
            foreach (string s1 in s) {
                _view.DisplayInfo(s1 + Environment.NewLine);
            }

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
