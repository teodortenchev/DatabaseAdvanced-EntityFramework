using MyApp.Core.Contracts;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Core
{
    public class Engine : IEngine
    {
        private readonly IServiceProvider serviceProvider;

        public Engine(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        public void Run()
        {
            string input = Console.ReadLine();

            var commandInterpreter = this.serviceProvider.GetService<ICommandInterpreter>();

            while (true)
            {
                string[] data = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                string result = commandInterpreter.Interpret(data);

                Console.WriteLine(result);
                
                //TODO Try catch

                input = Console.ReadLine();
            }
        }
    }
}
