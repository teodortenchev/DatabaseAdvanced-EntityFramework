namespace P03_FootballBetting
{
    using System;
    using P03_FootballBetting.Data;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using (var context = new FootballBettingContext())
            {
                context.Database.EnsureCreated();
                Console.WriteLine("Db created successfully.");


                Console.WriteLine("Press any key to continue. DB will be deleted.");
                Console.ReadKey();

                context.Database.EnsureDeleted();
                Console.WriteLine("Db Deleted.");
            }
        }
    }
}
