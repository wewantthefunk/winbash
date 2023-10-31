namespace bash.dotnet
{
    class NullView : IView
    {
        public void Display(string msg) {
        }

        public void Display(string msg, EntryType type) {
            Display(msg);
        }

        public void DisplayDebug(string msg) {
            Display(msg);
        }

        public void DisplayError(string msg) {
            Display(msg);
        }

        public void DisplayInfo(string msg) {
            Display(msg);
        }

        public void DisplayCloseOut() { }

        public void Clear(){}

        public void SetConfigOptions(ConfigOptions? options) { }

        public ViewType getViewType() {
            return ViewType.Null;
        }
    }
}