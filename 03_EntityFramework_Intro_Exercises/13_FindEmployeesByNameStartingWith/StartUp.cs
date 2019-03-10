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
                string result = GetEmployeesByFirstNameStartingWithSa(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                    .Select(e => new
                    {
                        FullName = e.FirstName + " " + e.LastName,
                        e.JobTitle,
                        e.Salary
                    })
                .Where(e => e.FullName.StartsWith("Sa"))
                .OrderBy(e => e.FullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FullName} - {employee.JobTitle} - (${employee.Salary:F2})");
            }


            return sb.ToString().TrimEnd();
        }
    }
}
