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
            int count = 0;
            foreach(string arg in args) {
                if (string.IsNullOrEmpty(arg)) {
                    continue;
                }
                string suffix = " ";
                count++;
                if (count >= args.Length) {
                    suffix = "";
                }
                string e = arg.Replace("\\n", Environment.NewLine) + suffix;
                _view.DisplayInfo(e);
            }

            _view.DisplayCloseOut();

            return _configOptions;
        }

        public void SetProperty(string key, string value) { }
    }
}