namespace SoftUni
{
    using System;
    using SoftUni.Data;
    using SoftUni.Models;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                string result = GetEmployee147(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4

            };

            //Entity Framework can add the below automatically if it does not exist, so not needed
            context.Addresses.Add(address);

            var nakov = context.Employees
                .FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = address;

            context.SaveChanges();
        
            var employeeAddresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Select(x => x.Address.AddressText)
                .Take(10)
                .ToList();

            foreach (var addr in employeeAddresses)
            {
                sb.AppendLine(addr);
            }

            return sb.ToString().TrimEnd();
            
        }
    }
}
