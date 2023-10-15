namespace bash.dotnet
{
    internal interface ICommandFactory
    {
        ICommand Create(string input, ConfigOptions configOptions);
    }
}
