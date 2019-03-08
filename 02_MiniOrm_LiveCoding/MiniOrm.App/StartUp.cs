namespace MiniOrm.App
{
    using System.Linq;
    using Data;
    using Data.Entities;
    class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=localhost\SQLEXPRESS;Database=MinionsDB;Integrated Security=true";

            var context = new SoftUniDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Gosho",
                LastName = "Inserted",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true,
            });

            var employee = context.Employees.Last();
            employee.FirstName = "Modified";

            context.SaveChanges();
        }
    }
}
