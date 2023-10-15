namespace bash.dotnet
{
    interface IInput
    {
        void AcceptInput(IView nullView, CommandFactory factory);
        ConfigOptions ReadConfig(ConfigOptions configOptions);
    }
}