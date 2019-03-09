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

            var employe147 = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(e => new
                {
                    FullName = e.FirstName + ' ' + e.LastName,
                    JobTitle = e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(p => new
                        {
                            ProjectName = p.Project.Name
                        })
                        .OrderBy(project => project.ProjectName)
                })
                .FirstOrDefault();

            if (employe147 != null)
            {
                sb.AppendLine($"{employe147.FullName} - {employe147.JobTitle}");

                foreach (var project in employe147.Projects)
                {
                    sb.AppendLine(project.ProjectName);
                }
            }
           
            return sb.ToString().TrimEnd();

    }
}
}
