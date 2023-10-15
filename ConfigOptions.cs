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

        public const string DEFAULT_PROMPT = "[dir] $ ";

        public ConfigOptions(string launchDir) {
            _launchDir = launchDir;
            _prompt = string.Empty;
            _startingDir = string.Empty;
            _environmentPath = string.Empty;
            _promptTemplate = DEFAULT_PROMPT;
            _username = Environment.UserName;
            resetPrompt();
            resetEnvironmentPath();
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

        public string getUserName() {
            return _username;
        }

        public string getPrompt() {
            return _prompt;
        }

        public string[] getPath() {
            return _environmentPath.Split(";", StringSplitOptions.RemoveEmptyEntries);
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
            string? p = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(p)) {
                _environmentPath = string.Empty;
            } else {
                _environmentPath = p;
            }
        }
    }
}