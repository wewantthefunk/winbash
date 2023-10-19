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

            List<string> s = new();

            if (args[0] == "-a" || args[0] == "-c") {
                string[] s1 = _inputDevice.getAliases();                
                
                foreach(string s11 in s1) {
                    s.Add(s11);
                }
            } 
            
            if (args[0] == "-b" || args[0] == "-c") {
                    string[] s1 = _factory.getBuiltInCommands();

                foreach (string s11 in s1) {
                    s.Add(s11);
                }
            }

            _view.DisplayInfo("Total " + s.Count.ToString() + Environment.NewLine);
            foreach (string s1 in s) {
                _view.DisplayInfo(s1 + Environment.NewLine);
            }

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}
