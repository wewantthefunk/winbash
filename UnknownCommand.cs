namespace bash.dotnet
{
    class UnknownCommand : ICommand
    {
        private IView _view;
        private ConfigOptions _configOptions;
        public UnknownCommand(IView view, ConfigOptions configOptions) {
            _view = view;
            _configOptions = configOptions;
        }

        public ConfigOptions Go(string[] args) {
            _view.DisplayError("Invalid command\n");
            return _configOptions; 
        }

        public void SetProperty(string key, string value) { }
    }
}