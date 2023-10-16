namespace bash.dotnet
{
    internal class MVBash : ICommand
    {
        private ConfigOptions _configOptions;
        private IView _view;

        public MVBash(IView view, ConfigOptions configOptions)
        {
            _view = view;
            _configOptions = configOptions;
        }

        public ConfigOptions Go(string[] args)
        {
            if (args.Length < 2) {
                _view.DisplayError("mv command requires a source and a destination\n");
                return _configOptions;
            }
            
            if (!File.Exists(args[0])) {
                _view.DisplayError($"Source file '{args[0]}' does not exists.\n");
                return _configOptions;
            }

            if (File.Exists(args[1])) {
                _view.DisplayError($"Destination file '{args[1]}' already exists.\n");
                return _configOptions;
            }

            try {
                File.Move(args[0], args[1]);
            } catch (Exception e) {
                _view.DisplayError(e.Message + Environment.NewLine);
            }

            return _configOptions;
        }
    }
}
