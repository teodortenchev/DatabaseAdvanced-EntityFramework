using MyApp.Core.Contracts;
using System;

namespace MyApp.Core.Commands
{
    public class ExitCommand : IExecutable
    {
        public string Execute(string[] inputArgs)
        {
            Environment.Exit(0);

            //Unreachable
            return "Thank you for using this simple app.";
        }
    }
}
