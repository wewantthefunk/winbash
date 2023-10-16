namespace bash.dotnet
{
    class FileInput : IInput
    {
        private string _filename;

        public FileInput(string filename) {
            _filename = filename;
        }
        public void AcceptInput(IView nullView, CommandFactory factory) {
            ConfigOptions configOptions = new(Directory.GetCurrentDirectory().Replace("\\", "/")[2..]);
            configOptions = ReadConfig(configOptions);
            if (File.Exists(_filename)) {
                var lines = File.ReadLines(_filename);
                foreach (var line in lines){
                    string[] tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    string[] args = new string[tokens.Length - 1];

                    for (int x = 1; x < tokens.Length; x++) {
                        args[x - 1] = tokens[x];
                    }

                    ICommand command = factory.Create(tokens[0], configOptions);

                    command.Go(args);
                }
            }
        }

        public ConfigOptions ReadConfig(ConfigOptions configOptions) {
            if (!File.Exists("config.cfg")) {
                return configOptions;
            }
            var lines = File.ReadAllLines("config.cfg");
            for (var i = 0; i < lines.Length; i += 1) {
                var line = lines[i];
                string[] tokens = line.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2) {
                    break;
                }

                if (tokens[0] == "dir") {
                    configOptions.setStartingDir(tokens[1]);
                } else if (tokens[0] == "prompt") {
                    configOptions.setPromptTmeplate(tokens[1]);
                }
            }

            return configOptions;
        }

        public void SetCurrentDirectory(string currentDirectory) { }
    }
}