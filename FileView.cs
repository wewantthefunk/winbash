namespace bash.dotnet
{
    class FileView : IView
    {
        private string _filename;

        public FileView(string filename) {
            _filename = filename;
            Clear();
        }
        public void Display(string msg) {
            File.AppendAllLines(_filename, new string[] { msg });
        }

        public void Display(string msg, EntryType type)
        {
            Display(msg);
        }

        public void DisplayDebug(string msg) {
            Display(msg);
        }

        public void DisplayError(string msg) {
            Display("error: " + msg);
        }

        public void DisplayInfo(string msg) {
            Display(msg);
        }

        public void Clear() {
            if (File.Exists(_filename))
            {
                // If it exists, delete it
                File.Delete(_filename);
            }

            // Create a new file
            using (FileStream fs = File.Create(_filename))
            {                
            }
        }
    }
}