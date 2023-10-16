using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bash.dotnet
{
    internal class Alias
    {
        private string _name;
        private string _command;

        public Alias(string name, string command) {
            _name = name;
            _command = command;
        }

        public string getName() { return _name; }
        public string getCommand() { return _command;}
        public void setCommand(string command) { _command = command; }
    }
}
