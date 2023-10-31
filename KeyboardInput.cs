using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Diagnostics;

namespace bash.dotnet {
    partial class KeyboardInput : IInput {
        private List<Alias> _aliases;

        private LSBash? _tabLSBash;

        private bool _isInsertMode;

        private IView _view;

        public KeyboardInput(IView view) {
            _aliases = new();
            _isInsertMode = true;
            _view = view;
        }

        public void SetCurrentDirectory(string currentDirectory) {
            if (_tabLSBash == null) return;
            _tabLSBash.SetProperty("directory", currentDirectory);
        }

        public void AcceptInput(IView nullView, ICommandFactory factory) {
            List<string> commandList = new();
            
            int lastCommandIndex = 0;
            StringBuilder commandBuilder = new();
            ConfigOptions configOptions = new(Directory.GetCurrentDirectory().Replace("\\", "/")[2..]);
            configOptions = ReadConfig(configOptions);

            _view.SetConfigOptions(configOptions);

            _tabLSBash = new(configOptions, nullView);

            Console.Title = configOptions.getTitle() + " [INS]";

            CDBash bash = new(configOptions, nullView, this);
            bash.Go(new string[] { configOptions.getStartingDir() });

            int cursorPosition;
            int promptLength;

            while (true) {
                commandBuilder.Clear();
                string prompt = configOptions.getPrompt();
                
                _view.Display(prompt);
                string[] p = prompt.Split(Environment.NewLine);
                promptLength = p[^1].Length;
                cursorPosition = promptLength;
                while (true) {
                    var keyInfo = Console.ReadKey(intercept: true);

                    switch (keyInfo.Key) {
                        case ConsoleKey.Tab:
                            string[] one = commandBuilder.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                            var suggestions = GetAutocompleteSuggestions(one[^1], configOptions);
                            if (suggestions.Count > 0) {
                                for (int y = 0; y < one[^1].Length; y++) {
                                    commandBuilder = Backspace(commandBuilder, commandBuilder.Length - 1);
                                }
                                var itemName = suggestions[0].Replace("C:", "");
                                itemName = itemName.Replace("\\", "/");
                                itemName = itemName.Replace(configOptions.getStartingDir() + "/", "");
                                if (itemName.Contains(" ")) {
                                    itemName = "\"" + itemName + "\"";
                                }
                                _view.Display(itemName);
                                commandBuilder.Append(itemName);
                                cursorPosition = promptLength + commandBuilder.Length;
                            }
                            break;
                        case ConsoleKey.Escape:
                            Console.CursorLeft = promptLength + commandBuilder.Length;
                            commandBuilder = ClearCommand(commandBuilder);
                            cursorPosition = promptLength;                            
                            break;
                        case ConsoleKey.Backspace:
                            cursorPosition--;
                            if (cursorPosition < promptLength) {
                                cursorPosition= promptLength;
                                continue;
                            }
                            commandBuilder = Backspace(commandBuilder,cursorPosition - promptLength);
                            int rest = commandBuilder.Length - (cursorPosition - promptLength);
                            for (int x = 0; x < rest + 1; x++) {
                                _view.Display(" ");
                            }
                            for (int x = 0; x < rest + 1; x++) {
                                MoveCursorLeft();
                            }
                            for (int x = 0; x < rest; x++) {
                                _view.Display(commandBuilder.ToString()[(cursorPosition - promptLength) + x].ToString());
                            }
                            for (int x = 0; x < rest; x++) {
                                MoveCursorLeft();
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            lastCommandIndex--;
                            if (lastCommandIndex < 0) lastCommandIndex = 0;
                            if (commandList.Count > 0 && commandList.Count > lastCommandIndex) {
                                commandBuilder = ClearCommand(commandBuilder);
                                commandBuilder.Append(commandList[lastCommandIndex]);
                                _view.Display(commandBuilder.ToString());
                                cursorPosition = promptLength + commandBuilder.Length;
                            }
                            break;
                        case ConsoleKey.DownArrow:
                            lastCommandIndex++;
                            if (lastCommandIndex >= commandList.Count) lastCommandIndex = commandList.Count - 1;
                            if (commandList.Count > 0 && commandList.Count > lastCommandIndex) {
                                commandBuilder = ClearCommand(commandBuilder);
                                commandBuilder.Append(commandList[lastCommandIndex]);
                                _view.Display(commandBuilder.ToString());
                                cursorPosition = promptLength + commandBuilder.Length;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            if (cursorPosition > promptLength) {
                                cursorPosition--;  // Move cursor position one step back.
                                MoveCursorLeft();
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            if (cursorPosition < promptLength + commandBuilder.Length) {
                                cursorPosition++;  // Move cursor position one step forward.
                                MoveCursorRight();
                            }
                            break;
                        case ConsoleKey.Home:
                            while (cursorPosition > promptLength) {
                                if (cursorPosition > promptLength) {
                                    cursorPosition--;  // Move cursor position one step back.
                                    MoveCursorLeft();
                                }
                            }
                            break;
                        case ConsoleKey.End:
                            while (cursorPosition < promptLength + commandBuilder.Length) {
                                if (cursorPosition < promptLength + commandBuilder.Length) {
                                    cursorPosition++;  // Move cursor position one step forward.
                                    MoveCursorRight();
                                }
                            }
                            break;
                        case ConsoleKey.Insert:
                            _isInsertMode = !_isInsertMode;
                            if (_isInsertMode) {
                                Console.Title = configOptions.getTitle() + " [INS]";
                            } else {
                                Console.Title = configOptions.getTitle();
                            }
                            break;
                        case ConsoleKey.Delete:
                            string therest1 = "";
                            if (cursorPosition < promptLength + commandBuilder.Length) {
                                int i = cursorPosition - promptLength;
                                if (i < 0) i = 0;
                                therest1 = commandBuilder.ToString().Substring(i + 1);
                                commandBuilder.Remove(i, 1);
                                cursorPosition = promptLength + commandBuilder.Length - therest1.Length;
                                _view.Display(therest1 + " ");
                                _view.Display("\b \b");  // Erase last character on the screen
                                Console.CursorLeft = cursorPosition;
                            }
                            break;
                        default:
                            if (!IsValidCharacter(keyInfo.Key)) {
                                break;
                            }
                            string therest = "";
                            if (cursorPosition < promptLength + commandBuilder.Length) {
                                int i = cursorPosition - promptLength;
                                if (i < 0) i = 0;
                                if (!_isInsertMode) i++;
                                therest = commandBuilder.ToString().Substring(i);
                                commandBuilder.Insert(i, keyInfo.KeyChar);
                                cursorPosition = promptLength + commandBuilder.Length - therest.Length;
                            } else {
                                commandBuilder.Append(keyInfo.KeyChar);
                                cursorPosition = promptLength + commandBuilder.Length;                                
                            }

                            _view.Display(keyInfo.KeyChar + therest);

                            for (int x = 0; x < therest.Length; x++) {
                                MoveCursorLeft();
                            }

                            break;

                    }

                    if (keyInfo.Key == ConsoleKey.Enter) {                        
                        _view.Display(Environment.NewLine);
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

        private List<string> GetAutocompleteSuggestions(string input, ConfigOptions configOptions) {
            // Split the input into a path and the current incomplete directory/file name
            string basePath, currentIncompleteName;
            int lastSlashIndex = input.LastIndexOf('/');
            if (lastSlashIndex >= 0) {
                basePath = input.Substring(0, lastSlashIndex);
                currentIncompleteName = input.Substring(lastSlashIndex + 1);
            }
            else {
                basePath = "";  // current directory
                currentIncompleteName = input;
            }

            // If no base path is provided, use the current directory
            if (string.IsNullOrEmpty(basePath)) {
                basePath = Directory.GetCurrentDirectory();

                if (input.StartsWith("/")) {
                    basePath = "C:\\";
                }
            }

            var suggestions = new List<string>();
            try {
                // Get all directories and files in the base path
                var entries = Directory.EnumerateFileSystemEntries(basePath);

                // Filter entries to those that start with the current incomplete name
                suggestions = entries.Where(path =>
                {
                    var itemName = Path.GetFileName(path);
                    return itemName.StartsWith(currentIncompleteName, StringComparison.OrdinalIgnoreCase);
                }).ToList();
            }
            catch {
                // Handle exceptions (e.g., directory not found, no permission, etc.)
            }

            return suggestions;
        }

        private bool IsValidCharacter(ConsoleKey key) {
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
            var specialChars = new[] {
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

        private void MoveCursorLeft() {
            if (Console.CursorLeft == 0) {
                Console.CursorTop--;
                Console.CursorLeft = Console.BufferWidth - 1;
            }
            else {
                Console.CursorLeft--;
            }
        }

        private void MoveCursorRight() {
            if (Console.CursorLeft == Console.BufferWidth - 1) {
                Console.CursorTop++;
                Console.CursorLeft = 0;
            }
            else {
                Console.CursorLeft++;
            }
        }

        public ConfigOptions ReadConfig(ConfigOptions configOptions) {
            if (!File.Exists(configOptions.getLaunchDir() + "\\keyboard.cfg")) {
                return configOptions;
            }
            configOptions.setForegroundColor("white");
            configOptions.setBackgroundColor("black");
            configOptions.setShowCmd(false);
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
                } else if (tokens[0] == "title") {
                    configOptions.setTitle(tokens[1]);
                } else if (tokens[0] == "bgc") {
                    string color = tokens[1].Trim().ToLower();
                    configOptions.setBackgroundColor(color);
                } else if (tokens[0] == "fgc") {
                    string color = tokens[1].Trim().ToLower();
                    configOptions.setForegroundColor(color);
                } else if (tokens[0] == "showCmd") {
                    configOptions.setShowCmd(bool.Parse(tokens[1]));
                }
            }

            return configOptions;
        }

        private int AliasExist(string name) {
            for (int x = 0; x < _aliases.Count; x++) {
                if (_aliases[x].getName() == name) {
                    return x;
                }
            }

            return -1;
        }

        private StringBuilder Backspace(StringBuilder commandBuilder, int characterPos) {
            if (commandBuilder.Length > 0)
            {
                commandBuilder.Remove(characterPos, 1);
                _view.Display("\b \b");  // Erase last character on the screen
            }

            return commandBuilder;
        }

        private StringBuilder ClearCommand(StringBuilder commandBuilder) {
            while (commandBuilder.Length > 0) {
                commandBuilder = Backspace(commandBuilder, commandBuilder.Length -  1);
            }

            return commandBuilder;
        }

        private ConfigOptions ExecuteCommand(string cmdText, ICommandFactory factory, ConfigOptions configOptions) {
            string[] tokens_temp = cmdText.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            List<string> tokensList = new();

            string temp = "";
            bool inQuotes = false;
            foreach(string token in tokens_temp) {
                if (token.StartsWith("\"")) {
                    inQuotes = true;
                    temp = token[0..];    
                    if (token.EndsWith("\"")) {
                        inQuotes = false;
                    }
                } else if (token.EndsWith("\"")) {
                    inQuotes = false;
                    temp += string.Concat(" ", token.AsSpan(0, token.Length));
                } else if (inQuotes) {
                    temp += " " + token;
                } else {
                    temp = token;
                }

                if (!inQuotes) {
                    tokensList.Add(temp);
                }
            }

            string[] tokens = tokensList.ToArray();

            string[] args = new string[tokens.Length - 1];

            for (int x = 1; x < tokens.Length; x++) {
                args[x - 1] = tokens[x];
            }

            foreach(Alias a in _aliases) {
                if (a.getName() == tokens[0]) {
                    List<string> list = new List<string>();
                    list.Add(a.getCommand());
                    list.AddRange(tokens.Skip(1));
                    return ExecuteCommand(string.Join(' ', list.ToArray()), factory, configOptions);
                }
            }

            IView view = _view;

            if (tokens.Contains(">")) {
                int index = find(tokens, ">");
                view = new FileView(tokens[index]);
                args = new string[index - 2];

                for (int x = 1; x < index - 1; x++) {
                    args[x - 1] = tokens[x];
                }
            }

            ICommand command = factory.Create(tokens[0], configOptions, view);

            configOptions = command.Go(args);

            return configOptions;
        }

        public string[] getAliases() {
            string[] result = new string[_aliases.Count];

            for (int x = 0; x < result.Length; x++) {
                result[x] = _aliases[x].getName();
            }

            return result;
        }

        private int find(string[] s, string search) {
            int result = -1;

            int count = 0;

            foreach(string s2 in s) {
                if (s2 == search) {
                    result = count + 1;
                    break;
                }

                count++;
            }

            return result;
        }
    }
}
