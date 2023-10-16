using System.ComponentModel;
using System.Text;

namespace bash.dotnet
{
    class KeyboardInput : IInput
    {
        private List<Alias> _aliases;

        public KeyboardInput() {
            _aliases = new();
        }

        public void AcceptInput(IView nullView, CommandFactory factory) {
            List<string> commandList = new();
            
            int lastCommandIndex = 0;
            StringBuilder commandBuilder = new();
            ConfigOptions configOptions = new(Directory.GetCurrentDirectory().Replace("\\", "/")[2..]);
            configOptions = ReadConfig(configOptions);

            CDBash bash = new(configOptions, nullView);
            bash.Go(new string[] { configOptions.getStartingDir() });
            while (true)
            {
                commandBuilder.Clear();
                string prompt = configOptions.getPrompt();
                
                Console.Write(prompt);
                while (true) {
                    var keyInfo = Console.ReadKey(intercept: true);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Tab:
                            LSBash lSBash = new(configOptions, nullView);
                            lSBash.Go(new string[2] { "-q", "-a" });
                            string[] entries = lSBash.getNames();

                            if (commandBuilder.Length > 0 && commandBuilder.ToString().Contains("/"))
                            {
                                string c = commandBuilder.ToString();

                            }

                            string[] one = commandBuilder.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            if (one.Length < 2) {
                                continue;
                            }
                            string[] tokens = one[one.Length - 1].Split("/", StringSplitOptions.RemoveEmptyEntries);
                            if (tokens.Length > 0) {
                                for (int x = 0; x < entries.Length; x++) {
                                    if (entries[x].StartsWith(tokens[tokens.Length - 1])) {
                                        for (int y = 0; y < tokens[tokens.Length - 1].Length; y++) {
                                            commandBuilder = Backspace(commandBuilder);
                                        }
                                        commandBuilder.Append(entries[x]);
                                        Console.Write(entries[x]);
                                        if (lSBash.GetEntryType(entries[x]) == EntryType.DIR)
                                            Console.Write("/");
                                        break;
                                    }
                                }
                            }
                            break;
                        case ConsoleKey.Escape:
                            commandBuilder = ClearCommand(commandBuilder);
                            break;
                        case ConsoleKey.Backspace:
                            commandBuilder = Backspace(commandBuilder);
                            break;
                        case ConsoleKey.UpArrow:
                            lastCommandIndex--;
                            if (lastCommandIndex < 0) lastCommandIndex = 0;
                            if (commandList.Count > 0 && commandList.Count > lastCommandIndex) {
                                commandBuilder = ClearCommand(commandBuilder);
                                commandBuilder.Append(commandList[lastCommandIndex]);
                                Console.Write(commandBuilder.ToString());
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            lastCommandIndex++;
                            if (lastCommandIndex >= commandList.Count) lastCommandIndex = commandList.Count - 1;
                            if (commandList.Count > 0 && commandList.Count > lastCommandIndex)
                            {
                                commandBuilder = ClearCommand(commandBuilder);
                                commandBuilder.Append(commandList[lastCommandIndex]);
                                Console.Write(commandBuilder.ToString());
                            }
                            break;
                        default:
                            if (!IsValidCharacter(keyInfo.Key)){
                                break;
                            }
                            commandBuilder.Append(keyInfo.KeyChar);
                            Console.Write(keyInfo.KeyChar);
                            break;
                    }

                    if (keyInfo.Key == ConsoleKey.Enter) {                        
                        Console.Write(Environment.NewLine);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(commandBuilder.ToString())) continue;
                string input = commandBuilder.ToString().Trim();
                if (input.ToLower().StartsWith("exit")) break;

                if ((commandList.Count > 0 && input != commandList[commandList.Count - 1]) || commandList.Count == 0)
                    commandList.Add(input);
                lastCommandIndex = commandList.Count;

                configOptions = ExecuteCommand(input, factory, configOptions);
            }
        }

        private bool IsValidCharacter(ConsoleKey key)
        {
            // Check for letter (A to Z, both cases)
            if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
                return true;

            // Check for numbers (0 to 9)
            if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
                return true;

            // Check for numpad numbers (0 to 9)
            if (key >= ConsoleKey.NumPad0 && key <= ConsoleKey.NumPad9)
                return true;

            if (key == ConsoleKey.Spacebar) 
                return true;

            // Check for special characters (this list can be expanded as needed)
            var specialChars = new[]
            {
                ConsoleKey.Oem1,       // e.g., ';' or ':' on US keyboards
                ConsoleKey.OemPlus,    // e.g., '=' or '+' on US keyboards
                ConsoleKey.OemComma,   // e.g., ',' or '<' on US keyboards
                ConsoleKey.OemMinus,   // e.g., '-' or '_' on US keyboards
                ConsoleKey.OemPeriod,  // e.g., '.' or '>' on US keyboards
                ConsoleKey.Oem2,       // e.g., '/' or '?' on US keyboards
                ConsoleKey.Oem3,       // e.g., '`' or '~' on US keyboards
                ConsoleKey.Oem7,       // e.g., ''' or '"' on US keyboards
                ConsoleKey.Oem4,       // e.g., '/' or '?' on US keyboards
                ConsoleKey.Oem5,       // e.g., '`' or '~' on US keyboards
                ConsoleKey.Oem6,       // e.g., ''' or '"' on US keyboards
                // ... add other special characters as needed
            };

            return Array.Exists(specialChars, ch => ch == key);
        }

        public ConfigOptions ReadConfig(ConfigOptions configOptions) {
            if (!File.Exists(configOptions.getLaunchDir() + "\\keyboard.cfg")) {
                return configOptions;
            }
            var lines = File.ReadAllLines(configOptions.getLaunchDir() + "\\keyboard.cfg");
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
                } else if (tokens[0] == "alias") {
                    string[] ai = tokens[1].Split("~~~");
                    int x = AliasExist(ai[0]);
                    if (x < 0) {
                        _aliases.Add(new Alias(ai[0], ai[1]));
                    } else {
                        _aliases[x].setCommand(ai[1]);
                    }
                }
            }

            return configOptions;
        }

        private int AliasExist(string name)
        {
            for (int x = 0; x < _aliases.Count; x++) {
                if (_aliases[x].getName() == name) {
                    return x;
                }
            }

            return -1;
        }

        private StringBuilder Backspace(StringBuilder commandBuilder) {
            if (commandBuilder.Length > 0)
            {
                commandBuilder.Remove(commandBuilder.Length - 1, 1);
                Console.Write("\b \b");  // Erase last character on the screen
            }

            return commandBuilder;
        }

        private StringBuilder ClearCommand(StringBuilder commandBuilder)
        {
            while (commandBuilder.Length > 0)
            {
                commandBuilder = Backspace(commandBuilder);
            }

            return commandBuilder;
        }

        private ConfigOptions ExecuteCommand(string cmdText, CommandFactory factory, ConfigOptions configOptions) {
            string[] tokens = cmdText.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            string[] args = new string[tokens.Length - 1];

            for (int x = 1; x < tokens.Length; x++) {
                args[x - 1] = tokens[x];
            }

            foreach(Alias a in _aliases) {
                if (a.getName() == tokens[0]) {
                    return ExecuteCommand(a.getCommand(), factory, configOptions);
                }
            }

            ICommand command = factory.Create(tokens[0], configOptions);

            configOptions = command.Go(args);

            return configOptions;
        }
    }
}