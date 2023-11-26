namespace bash.dotnet
{
    class ConfigOptions
    {
        private string _startingDir;
        private string _launchDir;
        private string _promptTemplate;
        private string _username;
        private string _prompt;
        private string _environmentPath;
        private string _title;
        private string _backgroundColor;
        private string _foregroundColor;
        private bool _showCmd;
        private string _temp;

        public const string DEFAULT_PROMPT = "[dir] $ ";

        public ConfigOptions(string launchDir) {
            _launchDir = launchDir;
            _prompt = string.Empty;
            _startingDir = string.Empty;
            _environmentPath = string.Empty;
            _promptTemplate = DEFAULT_PROMPT;
            _username = Environment.UserName;
            _title = "WinBash Command Line";
            _backgroundColor = "black";
            _foregroundColor = "white";
            resetPrompt();
            resetEnvironmentPath();
            _showCmd = false;
            _temp = string.Empty;
        }

        public string getLaunchDir() {
            return _launchDir;
        }

        public string getStartingDir() {
            return _startingDir;
        }

        public void setStartingDir(string value) {
            _startingDir = value;
        }

        public string getPromptTemplate() {
            return _promptTemplate;
        }

        public void setPromptTmeplate(string value) {
            _promptTemplate = value;
        }

        public string getTitle() {
            return _title;
        }

        public void setTitle(string value) {
            _title = value;
        }

        public string getUserName() {
            return _username;
        }

        public string getPrompt() {
            return _prompt;
        }

        public string[] getPath() {
            return _environmentPath.Split(";", StringSplitOptions.RemoveEmptyEntries);
        }

        public string getBackgroundColor() {
            return _backgroundColor;
        }

        public void setBackgroundColor(string value) {
            _backgroundColor = value;
        }

        public string getForegroundColor() {
            return _foregroundColor;
        }

        public void setForegroundColor(string value) {
            _foregroundColor = value;
        }

        public void setShowCmd(bool value) { 
            _showCmd= value;
        }

        public bool getShowCmd() {
            return _showCmd;
        }

        public string getTemp() {
            return _temp;
        }

        public void setTemp(string value) {
            _temp = value;
        }

        public void resetPrompt() {
            string prompt = getPromptTemplate();

            string dir = Directory.GetCurrentDirectory().Substring(2).Replace("\\", "/");

            dir = dir.Replace("/Users/" + getUserName(), "~");

            prompt = prompt.Replace("[dir]", dir);

            prompt = prompt.Replace("[br]", Environment.NewLine);

            prompt = prompt.Replace("[sp]", " ");

            _prompt = prompt;
        }

        public void resetEnvironmentPath() {
            string? p = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (p == null) { p = string.Empty; }
            p += ";" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(p)) {
                _environmentPath = string.Empty;
            } else {
                _environmentPath = p;
            }
        }
    }
}
