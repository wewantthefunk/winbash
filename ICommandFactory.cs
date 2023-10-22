namespace bash.dotnet
{
    internal interface ICommandFactory
    {
        ICommand Create(string input, ConfigOptions configOptions);

        ICommand Create(string input, ConfigOptions configOptions, IView view);
        string[] getBuiltInCommands();
    }
}
