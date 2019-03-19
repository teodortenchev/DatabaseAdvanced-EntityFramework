namespace BillsPaymentSystem.App
{
    using BillsPaymentSystem.Data;
    using System;
    using Microsoft.EntityFrameworkCore.Design;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new BillsPaymentSystemContext())
            {
                DbInitializer.Seed(context);

                //context.Database.EnsureCreated();
                //Console.WriteLine("Db created successfully.");


                //Console.WriteLine("Press any key to continue. DB will be deleted." + Environment.NewLine);
                //Console.ReadKey();

                //Console.WriteLine();

                //context.Database.EnsureDeleted();
                //Console.WriteLine("Database Successfully Deleted.");
            }
        }
    }
}
