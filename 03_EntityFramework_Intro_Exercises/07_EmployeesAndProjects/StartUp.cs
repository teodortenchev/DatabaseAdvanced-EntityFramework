namespace SoftUni
{
    using SoftUni.Data;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                string result = GetEmployeesInPeriod(context);
                Console.WriteLine(result);
            }
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesInPeriod = context.Employees
                .Where(p => p.EmployeesProjects.Any(start => start.Project.StartDate.Year >= 2001
                        && start.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    EmployeeFullName = e.FirstName + ' ' + e.LastName,
                    ManagerFullName = e.Manager.FirstName + ' ' + e.Manager.LastName,
                    Projects = e.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    }).ToList()
                })
                .Take(10)
                .ToList();

            foreach (var employee in employeesInPeriod)
            {
                var projects = employee.Projects;

                sb.AppendLine($"{employee.EmployeeFullName} - Manager: {employee.ManagerFullName}");

                foreach (var project in projects)
                {
                    string format = "M/d/yyyy h:mm:ss tt";
                    string startDate = project.StartDate.ToString(format, CultureInfo.InvariantCulture);
                    string endDate = project.EndDate != null ? project.EndDate.Value.ToString(format, CultureInfo.InvariantCulture) : "not finished";

                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
