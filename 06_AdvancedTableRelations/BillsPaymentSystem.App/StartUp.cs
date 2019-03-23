using BillsPaymentSystem.App.Contracts;
using BillsPaymentSystem.App.Core;
using BillsPaymentSystem.Data;
using System.Linq;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new BillsPaymentSystemContext())
            {
                ICommandInterpreter commandInterpreter = new CommandInterpreter(context);

                IEngine engine = new Engine(commandInterpreter);

                engine.Run();
            }
        }
    }
}
