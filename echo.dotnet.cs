namespace bash.dotnet
{
    class ECHOBash : ICommand
    {
        private IView _view;
        private ConfigOptions _configOptions;
        public ECHOBash(IView view, ConfigOptions configOptions) {
            _view = view;
            _configOptions = configOptions;
        }
        public ConfigOptions Go(string[] args) {
            foreach(string arg in args) {
                string e = arg.Replace("\\n", Environment.NewLine) + " ";
                _view.DisplayInfo(e);
            }
            _view.DisplayInfo(Environment.NewLine);

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}