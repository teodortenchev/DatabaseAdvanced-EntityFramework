using BillsPaymentSystem.App.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BillsPaymentSystem.App.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {

        private BillsPaymentSystemContext context;

        public CommandInterpreter(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public void Interpret(string[] data)
        {
            string commandName = data[0];

            Type commandType = Assembly.GetExecutingAssembly().GetTypes().
                FirstOrDefault(t => t.Name.StartsWith(commandName));

            if (commandType == null)
            {
                throw new InvalidOperationException($"Command {commandName} is not recognized!");
            }

            IExecutable commandInstance = (IExecutable)Activator.CreateInstance(commandType, data, context);

            commandInstance.Execute();
        }
    }
}
