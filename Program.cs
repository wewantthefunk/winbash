using System.Diagnostics;
using System.Text;
using bash.dotnet;

class Program
{
    static void Main(string[] args)
    {
        IView debugView = new FileView("debug.txt");
        IView outputView = new NullView();
        if (args.Length < 2)
            outputView = new DisplayView(debugView);
        else if (args.Length == 2)
            outputView = new FileView(args[1]);

        IView nullView = new NullView();
        
        IInput input = new KeyboardInput();
        if (args.Length > 0) 
            input = new FileInput(args[0]);

        CommandFactory factory = new(outputView, input);

        input.AcceptInput(nullView, factory);
    }
}
