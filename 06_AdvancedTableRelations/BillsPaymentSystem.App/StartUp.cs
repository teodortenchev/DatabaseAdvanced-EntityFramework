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
                var creditCards = context.CreditCards.ToList();

                DBInitializer.Seed(context);
            }
        }
    }
}
