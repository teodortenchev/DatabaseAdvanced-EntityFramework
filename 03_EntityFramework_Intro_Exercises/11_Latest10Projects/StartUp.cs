namespace SoftUni
{
    using System;
    using SoftUni.Data;
    using SoftUni.Models;
    using System.Linq;
    using System.Text;
    using System.Globalization;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new SoftUniContext())
            {
                string result = GetLatestProjects(context);
                Console.WriteLine(result);
            }
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
                .Select(p => new
                {
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate
                })
                .ToList()
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .ToList()
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var project in projects)
            {
                sb.AppendLine(project.Name);
                sb.AppendLine(project.Description);
                sb.AppendLine(project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            return sb.ToString().TrimEnd();
            
        }
    }
}
