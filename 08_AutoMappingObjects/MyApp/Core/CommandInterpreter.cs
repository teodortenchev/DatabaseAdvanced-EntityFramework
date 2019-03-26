using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Contracts;
using MyApp.Data;
using System;
using System.Linq;
using System.Reflection;


namespace MyApp.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private readonly IServiceProvider serviceProvider;

        public CommandInterpreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Interpret(string[] data)
        {
            string commandName = data[0];

            Type commandType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.StartsWith(commandName));

            if (commandType == null)
            {
                throw new InvalidOperationException($"Command {commandName} is not recognized.");
            }

            var constructor = commandType.GetConstructors().FirstOrDefault();

            var ctorParams = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

            var services = ctorParams.Select(this.serviceProvider.GetService).ToArray();

            var command = (IExecutable)Activator.CreateInstance(commandType, services);

            string result = command.Execute(data.Skip(1).ToArray());

            return result;
        }
    }
}
