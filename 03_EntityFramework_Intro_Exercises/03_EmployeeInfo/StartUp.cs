﻿namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                string result = GetEmployeesFullInformation(context);

                Console.WriteLine(result);
            }

        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                               .Select(e => new
                               {
                                   e.FirstName,
                                   e.LastName,
                                   e.MiddleName,
                                   e.JobTitle,
                                   e.Salary,
                                   e.EmployeeId
                               })
                                 .OrderBy(x => x.EmployeeId)
                                 .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }


    }
}
