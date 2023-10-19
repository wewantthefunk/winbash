namespace bash.dotnet {
    interface IInput {
        void AcceptInput(IView nullView, ICommandFactory factory);
        ConfigOptions ReadConfig(ConfigOptions configOptions);
        void SetCurrentDirectory(string currentDirectory);
        string[] getAliases();
    }
}