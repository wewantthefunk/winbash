namespace bash.dotnet
{
    interface ICommand
    {
        ConfigOptions Go(string[] args);
        void SetProperty(string key, string value);
    }
}