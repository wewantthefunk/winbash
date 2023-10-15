namespace bash.dotnet
{
    interface IView
    {
        void Display(string msg);

        void Display(string msg, EntryType type);

        void DisplayDebug(string msg);

        void DisplayError(string msg);

        void DisplayInfo(string msg);

        void Clear();
    }
}