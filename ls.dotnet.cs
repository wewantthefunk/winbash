using System.Collections;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace bash.dotnet
{
    class LSBash : ICommand
    {
        private ConfigOptions _configOptions;
        private const string ENTRY_SPACER = "  ";

        private SortedList _entries;

        private IView _view;

        private string _directory;

        public LSBash(ConfigOptions configOptions, IView view) {
            _configOptions = configOptions;
            _entries = new();
            _view = view;
            String currentDirectory = Environment.CurrentDirectory;
            if (!currentDirectory.EndsWith("\\"))
            {
                currentDirectory += "\\";
            }

            _directory = currentDirectory;
        }

        public string getCurrentDirectory()
        {
            return _directory;
        }

        public void SetProperty(string key, string value) {
            key = key.ToLower().Trim();
            if (key == "directory") {
                value = value.Replace("/", "\\");
                _directory = value;
            }
        }

        public string[] getNames() {
            List<string> result = new();

            for(int x = 0; x < _entries.Count; x++) {
                object? e = _entries.GetByIndex(x);
                if (e == null) {
                    continue;
                }
                LSEntry entry = (LSEntry)e;
                result.Add(entry.getName());
            }

            return result.ToArray();
        }

        public string[] getNames(string name)
        {
            return new string[0];
        }

        public EntryType GetEntryType(string name)
        {
            for (int x = 0; x < _entries.Count; x++)
            {
                object? o = _entries.GetByIndex(x);
                if (o == null)
                {
                    continue;
                }

                LSEntry entry = (LSEntry)o;
                if (entry.getName() == name)
                {
                    return entry.getEntryType();
                }
            }

            return EntryType.UNKNOWN;
        }
        
        public ConfigOptions Go(string[] args)
        {
            bool showHidden = false;
            bool longForm = false;

            _entries.Clear();

            foreach (string arg in args) {
                if (arg.StartsWith("-")) {
                    for(int x = 1; x < arg.Length; x++) {
                        if (arg[x] =='l') {
                            longForm = true;
                        } else if (arg[x] == 'a') {
                            showHidden = true;
                        }
                    }
                }
            }
            int max_columns = 6;

            if (longForm) {
                max_columns = 1;
            }
            
            
            string[] files = Directory.GetFiles(_directory);
            foreach (string file in files) {
                string name = file.Replace(_directory, string.Empty);
                if ((name.StartsWith(".") && showHidden) || !name.StartsWith(".")) {
                    _entries.Add(name, new LSEntry(name, 0, EntryType.FILE));
                }
            }
            string[] dirs = Directory.GetDirectories(_directory);
            foreach (string dir in dirs) {
                string name = dir.Replace(_directory, string.Empty);
                if ((name.StartsWith(".") && showHidden) || !name.StartsWith(".")) {
                    _entries.Add(name, new LSEntry(name, 0, EntryType.DIR));
                }
            }

            int colCount = 0;
            int[] colCountMaxLength = new int[max_columns];
            int[] colSizeMaxLength = new int[max_columns];
            for (int x = 0; x < _entries.Count; x++) {
                object? o = _entries.GetByIndex(x);
                if (o == null) {
                    continue;
                }
                LSEntry entry = (LSEntry)o;
                if (entry.getName().Length > colCountMaxLength[colCount]) {
                    colCountMaxLength[colCount] = entry.getName().Length;
                }
                if (entry.getFileSize().Length > colSizeMaxLength[colCount]) {
                    colSizeMaxLength[colCount] = entry.getFileSize().Length;
                }
                entry.setCol(colCount);
                _entries.SetByIndex(x, entry);
                colCount++;
                if (colCount >= max_columns) {
                    colCount = 0;
                }
            }

            if (!args.Contains("-q")) {
                // final console write
                colCount = 0;
                if (longForm) {
                    _view.DisplayInfo("total " + _entries.Count.ToString()  + Environment.NewLine);
                }
                for (int x = 0; x < _entries.Count; x++) {
                    object? o = _entries.GetByIndex(x);
                    if (o == null) {
                        continue;
                    }
                    LSEntry entry = (LSEntry)o;
                    if (longForm) {
                        _view.Display(entry.ToFullString(_configOptions, colSizeMaxLength[0]), entry.getEntryType());
                    } else {
                        _view.Display(entry.ToString(colCountMaxLength[colCount], ENTRY_SPACER), entry.getEntryType());
                    }
                    colCount++;
                    if (colCount >= max_columns) {
                        colCount = 0;
                        _view.Display(Environment.NewLine);
                    }
                }

                if (colCount > 0) {
                    _view.Display(Environment.NewLine);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            return _configOptions;
        }
    }

    class LSEntry {
        private string _name;
        private int _col;

        private EntryType _type;

        private string _fileSize;

        private string _fileDate;

        public LSEntry(string name, int col, EntryType type) {
            _name = name;
            _col = col;
            if (name.ToLower().Trim().EndsWith(".exe") ||
                name.ToLower().Trim().EndsWith(".bat") ||
                name.ToLower().Trim().EndsWith(".ps1") ||
                name.ToLower().Trim().EndsWith(".cmd")
                ) {
                _type = EntryType.EXE;
            } else if (name.ToLower().Trim().EndsWith(".lnk")) {
                _type = EntryType.LINK; 
            } else {
                _type = type;
            }

            FileInfo fi = new(name);

            if (!getEntryType().Equals(EntryType.DIR)) {
                try {
                long l = fi.Length;
                _fileSize = Convert.ToString(l);
                } catch {
                    _fileSize = "???";
                }
            } else {
                _fileSize = "512";
            }

            _fileDate = FormatCustomDateTime(fi.LastWriteTime);
        }

        public void setName(string value) {
            _name = value;
        }

        public string getName() {
            return _name;
        }

        public void setCol(int value) {
            _col = value;
        }

        public int getCol() {
            return _col;
        }

        public EntryType getEntryType() {
            return _type;
        }

        public string getFileSize() {
            return _fileSize;
        }

        public string getFileDate()
        {
            return _fileDate;
        }

        public string ToString(int colCountMaxLength, String spacer)
        {
            return getName().PadRight(colCountMaxLength) + spacer;
        }
        public string ToFullString(ConfigOptions configOptions, int maxFileSizeLength) {
            StringBuilder result = new();

            if (getEntryType().Equals(EntryType.DIR)) {
                result.Append("d");
            } else {
                result.Append("-");
            }

            result.Append("rw");

            if (getEntryType().Equals(EntryType.DIR) || 
                getEntryType().Equals(EntryType.EXE)) {
                result.Append("x");
            } else {
                result.Append("-");
            }

            result.Append("r-");

            if (getEntryType().Equals(EntryType.DIR) || 
                getEntryType().Equals(EntryType.EXE)) {
                result.Append("x");
            } else {
                result.Append("-");
            }

            result.Append("r-");

            if (getEntryType().Equals(EntryType.DIR) || 
                getEntryType().Equals(EntryType.EXE)) {
                result.Append("x");
            } else {
                result.Append("-");
            }

            result.Append(" 1 ");

            result.Append(configOptions.getUserName());

            result.Append(" ");

            result.Append(configOptions.getUserName());

            result.Append(" ");

            result.Append(getFileSize().PadLeft(maxFileSizeLength));

            result.Append(" ");

            result.Append(getFileDate());

            result.Append(" ");

            result.Append(getName());

            return result.ToString();
        }

        private string FormatCustomDateTime(DateTime dt)
        {
            string month = dt.ToString("MMM", CultureInfo.InvariantCulture);  // First 3 characters of the month name
            string day = dt.ToString("dd");  // Two-digit day

            // Check if the year is the current year
            if (dt.Year == DateTime.Now.Year)
            {
                string time = dt.ToString("HH:mm");  // 24-hour format time
                return $"{month} {day} {time}";
            }
            else
            {
                return $"{month} {day}  {dt.Year}";
            }
        }
    }

    public enum EntryType {
        DIR,
        FILE, 
        EXE,
        LINK,
        UNKNOWN
    }
}
