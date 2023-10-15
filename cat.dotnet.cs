namespace bash.dotnet
{
    class CATBash : ICommand
    {
        private ConfigOptions _configOptions;

        private IView _view;

        public CATBash(ConfigOptions configOptions, IView view)
        {
            _configOptions = configOptions;
            _view = view;
        }
        
        public ConfigOptions Go(string[] args)
        {
            string filename = args[0];

            if (File.Exists(filename)) {
                _view.DisplayInfo(Environment.NewLine);
                var lines = File.ReadLines(filename);
                foreach (var line in lines){
                    _view.DisplayInfo(line + Environment.NewLine);
                }
                _view.DisplayInfo(Environment.NewLine);
            }

            return _configOptions;
        }
    }
}