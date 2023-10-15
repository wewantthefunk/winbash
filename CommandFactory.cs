namespace bash.dotnet
{
    class CommandFactory : ICommandFactory
    {
        private IView _view;

        private IView _nullView;

        private IInput _inputDevice;

        public CommandFactory(IView view, IInput inputDevice) {
            _view = view;
            _nullView = new NullView();
            _inputDevice = inputDevice;
        }
        public ICommand Create(string input, ConfigOptions configOptions) {
            input = input.ToLower().Trim();
            if (input == "ls") {
                return new LSBash(configOptions, _view);
            } else if (input == "pwd") {
                return new PWDDBash(configOptions, _view);
            } else if (input == "cd") {
                return new CDBash(configOptions, _view);
            } else if (input == "cat") {
                return new CATBash(configOptions, _view);
            } else if (input.StartsWith("./")) {
                return new EXECBash(configOptions, _view, input[2..]);
            } else if (input == "clear") {
                return new CLEARBash(configOptions, _view);
            } else if (input == "echo") {
                return new ECHOBash(_view, configOptions);
            } else if (input == "config") {
                return new CONFIGBash(configOptions, _view, this, _inputDevice);
            }

            if (input == "./") {
                input = input[2..];
            }

            string[] y = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (string p in configOptions.getPath()) {
                ConfigOptions co = new(p);
                co.setStartingDir(p);
                string exe = FindCommandInPath(y[0], co);
                if (!string.IsNullOrEmpty(exe)) {
                    return new EXECBash(configOptions, _view, exe);
                }
            }

            return new UnknownCommand(_view, configOptions);
        }

        private string FindCommandInPath(string command, ConfigOptions configOptions) {
            string result = string.Empty;

            CDBash cd = new(configOptions, _nullView);
            string currentDirectory = Environment.CurrentDirectory;
            cd.Go(new string[] { configOptions.getStartingDir() });
            LSBash c = new(configOptions, _nullView);

            c.Go(new string[] { "-q" });

            string strippedCmd = command.Split(".")[0];

            foreach(string name in c.getNames()) {
                string n = name.ToLower().Trim();
                if (n.Equals(strippedCmd + ".exe") ||
                      n.Equals(strippedCmd + ".bat") ||
                      n.Equals(strippedCmd + ".ps1") ||
                      n.Equals(strippedCmd + ".cmd") ) {
                    result = n;
                    break;
                }
            }

            cd.Go(new string[] { currentDirectory});
            return result;
        }
    }
}