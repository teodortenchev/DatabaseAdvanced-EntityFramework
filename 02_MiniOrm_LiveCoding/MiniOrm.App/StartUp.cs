namespace MiniOrm.App
{
    using System.Linq;
    using Data;
    using Data.Entities;
    class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = "Sever=.;Database=MiniORM;Integrated Security=True";

            var context = new SoftUniDbContext(connectionString);
        }
    }
}
