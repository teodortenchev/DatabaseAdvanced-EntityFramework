using BillsPaymentSystem.Data;
using System;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new BillsPaymentSystemContext())
            {
                context.Database.EnsureCreated();
                Console.WriteLine("Db created successfully.");


                Console.WriteLine("Press any key to continue. DB will be deleted." + Environment.NewLine);
                Console.ReadKey();

                context.Database.EnsureDeleted();
                Console.WriteLine("Database Successfully Deleted.");
            }
        }
    }
}
