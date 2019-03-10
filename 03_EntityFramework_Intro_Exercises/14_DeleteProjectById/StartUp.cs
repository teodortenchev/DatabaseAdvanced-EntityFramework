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
                string result = DeleteProjectById(context);
                Console.WriteLine(result);
            }
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            int projectId = 2;

            Project project = context.Projects.Find(projectId);

            int employeeId = context.EmployeesProjects
                .Where(p => p.ProjectId == projectId)
                .Select(e => e.EmployeeId).First();

            EmployeeProject[] employeeProject = context.EmployeesProjects.Where(ep => ep.ProjectId == projectId).ToArray();

            foreach (var ep in employeeProject)
            {
                context.Remove(ep);
            }

            context.Remove(project);
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            foreach (var p in context.Projects.Take(10).ToList())
            {
                sb.AppendLine(p.Name);
            }
            return sb.ToString().TrimEnd();

        }
    }
}
