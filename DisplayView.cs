using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace bash.dotnet
{
    class DisplayView : IView
    {
        private IView _debugView;
        private ConfigOptions? _configOptions;
        private ConsoleColor _foregroundColor;
        private ConsoleColor _backgroundColor;

        private const string COLOR_CHANGE_START = "[c\\";
        private const string COLOR_CHANGE_END = "[/c]";

        public DisplayView(IView debugView) {
            _debugView = debugView;
            _configOptions = null;
            _foregroundColor = ConsoleColor.White;
            _backgroundColor = ConsoleColor.Black;  
        }

        public void Display(string msg) {
            List<TextInfo> list = new List<TextInfo>();
            int startIndex = 0;
            int index = 0;
            int end = 0;
            int lastKnownStart = 0;

            if (msg.IndexOf(COLOR_CHANGE_START, startIndex) >= 0) {
                list.Add(new(msg.Substring(index, msg.IndexOf(COLOR_CHANGE_START, startIndex)), "default"));
                while ((startIndex = msg.IndexOf(COLOR_CHANGE_START, startIndex)) != -1) {
                    if (startIndex > lastKnownStart) {
                        list.Add(new(msg.Substring(lastKnownStart, startIndex - lastKnownStart), _foregroundColor.ToString().ToLower()));
                    }
                    int start = msg.IndexOf(COLOR_CHANGE_START, startIndex);
                    int start_1 = msg.IndexOf("]", start);
                    string color = msg.Substring(start + COLOR_CHANGE_START.Length, start_1 - COLOR_CHANGE_START.Length - start);
                    end = msg.IndexOf(COLOR_CHANGE_END, start + COLOR_CHANGE_START.Length + color.Length + 1);
                    list.Add(new(msg.Substring(start_1 + 1, end - start_1 - 1), color));
                    end = end + COLOR_CHANGE_END.Length;
                    startIndex = end;
                    lastKnownStart = startIndex;
                }

                list.Add(new(msg.Substring(end), "default"));

                foreach (TextInfo m in list) {
                    string color = m.Color;
                    ConsoleColor c = _foregroundColor;
                    if (color == "blue")
                        c = ConsoleColor.Blue;
                    else if (color == "red")
                        c = ConsoleColor.Red;
                    else if (color == "green")
                        c = ConsoleColor.Green;
                    else if (color == "gray")
                        c = ConsoleColor.Gray;
                    else if (color == "yellow")
                        c = ConsoleColor.Yellow;
                    else if (color == "white")
                        c = ConsoleColor.White;
                    else if (color == "black")
                        c = ConsoleColor.Black;

                    Console.ForegroundColor = c;

                    Console.Write(m.Text);

                    Console.ForegroundColor = _foregroundColor;
                }
            } else 
                Console.Write(msg);
        }

        public void Display(string msg, EntryType type) {
            switch (type) {
                case EntryType.EXE: DisplayExe(msg); break;
                case EntryType.LINK: DisplayLink(msg); break;
                case EntryType.DIR: DisplayDir(msg); break;
                default: Display(msg); break;
            }
        }

        public void SetConfigOptions(ConfigOptions? configOptions) {
            _configOptions = configOptions;

            if (_configOptions == null)
                return;

            string fcolor = _configOptions.getForegroundColor();

            _foregroundColor = ConsoleColor.White;

            if (fcolor == "blue")
                _foregroundColor = ConsoleColor.Blue;
            else if (fcolor == "red")
                _foregroundColor = ConsoleColor.Red;
            else if (fcolor == "green")
                _foregroundColor = ConsoleColor.Green;
            else if (fcolor == "gray")
                _foregroundColor = ConsoleColor.Gray;
            else if (fcolor == "yellow")
                _foregroundColor = ConsoleColor.Yellow;
            else if (fcolor == "white")
                _foregroundColor = ConsoleColor.White;
            else if (fcolor == "black")
                _foregroundColor = ConsoleColor.Black;

            string bcolor = _configOptions.getBackgroundColor();

            _backgroundColor = ConsoleColor.Black;

            if (bcolor == "blue")
                _backgroundColor = ConsoleColor.Blue;
            else if (bcolor == "red")
                _backgroundColor = ConsoleColor.Red;
            else if (bcolor == "green")
                _backgroundColor = ConsoleColor.Green;
            else if (bcolor == "gray")
                _backgroundColor = ConsoleColor.Gray;
            else if (bcolor == "yellow")
                _backgroundColor = ConsoleColor.Yellow;
            else if (bcolor == "white")
                _backgroundColor = ConsoleColor.White;
            else if (bcolor == "black")
                _backgroundColor = ConsoleColor.Black;

            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Clear();
        }

        public void DisplayDebug(string msg) {
            _debugView.Display(msg);
        }

        public void DisplayError(string msg) {
            Console.ForegroundColor = ConsoleColor.Red;
            Display(msg);
            Console.ForegroundColor = _foregroundColor;
        }


        public void DisplayInfo(string msg) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Display(msg);
            Console.ForegroundColor = _foregroundColor;
        }

        public void DisplayCloseOut() {
            Display(Environment.NewLine);
        }

        public void DisplayDir(string msg) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Display(msg);
            Console.ForegroundColor = _foregroundColor;
        }

        public void DisplayExe(string msg) {
            Console.ForegroundColor = ConsoleColor.Green;
            Display(msg);
            Console.ForegroundColor = _foregroundColor;
        }

        public void DisplayLink(string msg) {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Display(msg);
            Console.ForegroundColor = _foregroundColor;
        }

        public void Clear() {
            // Clear the console window
            Console.Clear();

            // Set the cursor position to the top right corner
            int topRightRow = 0;
            int topRightColumn = 0;
            Console.SetCursorPosition(topRightColumn, topRightRow);
        }

        public ViewType getViewType() {
            return ViewType.Console;
        }
    }

    internal class TextInfo {
        public string Text;
        public string Color;
        public TextInfo(string text, string color) {
            Text = text;
            Color = color;
        }
    }
}