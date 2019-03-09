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
                string result = GetAddressesByTown(context);
                Console.WriteLine(result);
            }
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addressesWithCounts = context.Addresses
                .Select(a => new
                {
                    EmployeesCount = a.Employees.Count,
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                }).ToList()
                .OrderByDescending(a => a.EmployeesCount).ThenBy(a => a.TownName).ThenBy(a => a.AddressText)
                .Take(10)
                .ToList();

            foreach (var address in addressesWithCounts)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }
           

            return sb.ToString().TrimEnd();
            
        }
    }
}
