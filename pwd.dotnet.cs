namespace bash.dotnet
{
    class PWDDBash : ICommand
    {
        private ConfigOptions _configOptions;

        private IView _view;

        public PWDDBash(ConfigOptions configOptions, IView view)
        {
            _configOptions = configOptions;
            _view = view;
        }

        public ConfigOptions Go(string[] args)
        {
            _view.DisplayInfo(Directory.GetCurrentDirectory() + Environment.NewLine);
            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}