using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace bash.dotnet
{
    class DisplayView : IView
    {
        private IView _debugView;

        public DisplayView(IView debugView) {
            _debugView = debugView;
        }

        public void Display(string msg) {
            Console.Write(msg);
        }

        public void Display(string msg, EntryType type) {
            switch (type)
            {
                case EntryType.EXE: DisplayExe(msg); break;
                case EntryType.LINK: DisplayLink(msg); break;
                case EntryType.DIR: DisplayDir(msg); break;
                default: Display(msg); break;
            }
        }

        public void DisplayDebug(string msg) {
            _debugView.Display(msg);
        }

        public void DisplayError(string msg) {
            Console.ForegroundColor = ConsoleColor.Red;
            Display(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }


        public void DisplayInfo(string msg) {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Display(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DisplayDir(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Display(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DisplayExe(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Display(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DisplayLink(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Display(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Clear() {
            // Clear the console window
            Console.Clear();

            // Set the cursor position to the top right corner
            int topRightRow = 0;
            int topRightColumn = 0;
            Console.SetCursorPosition(topRightColumn, topRightRow);
        }
    }
}