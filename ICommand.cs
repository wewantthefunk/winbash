namespace bash.dotnet
{
    interface ICommand
    {
        ConfigOptions Go(string[] args);
    }
}