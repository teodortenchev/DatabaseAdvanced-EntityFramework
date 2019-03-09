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
                string result = GetDepartmentsWithMoreThan5Employees(context);
                Console.WriteLine(result);
            }
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(x => new
                {
                    DepartmentName = x.Name,
                    ManagerFullName = x.Manager.FirstName + ' ' + x.Manager.LastName,
                    Employees = x.Employees
                        .Select(e => new
                        {
                            EmployeeFullName = e.FirstName + ' ' + e.LastName,
                            EmployeeJobTitle = e.JobTitle
                        })
                        .OrderBy(e => e.EmployeeFullName)
                        .ToList()
                }).ToList();


            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepartmentName} - {department.ManagerFullName}");

                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.EmployeeFullName} - {employee.EmployeeJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();

        }
    }
}
