namespace bash.dotnet
{
    class CLEARBash : ICommand
    {
        private ConfigOptions _configOptions;

        private IView _view;

        public CLEARBash(ConfigOptions configOptions, IView view)
        {
            _configOptions = configOptions;
            _view = view;
        }

        public ConfigOptions Go(string[] args)
        {
            _view.Clear();
            return _configOptions;
        }
    }
}