namespace P03_FootballBetting.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_FootballBetting.Data.Models;

    public class FootballBettingContext : DbContext
    {

        //public FootballBettingContext(DbContextOptions options)
        //      : base(options)
        //{

        //}
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureTeams(modelBuilder);
            ConfigureColors(modelBuilder);
            ConfigureTowns(modelBuilder);
            ConfigureCountries(modelBuilder);
            ConfigurePlayers(modelBuilder);
            ConfigurePositions(modelBuilder);
            ConfigurePlayerStatistics(modelBuilder);
            ConfigureGames(modelBuilder);
            ConfigureBets(modelBuilder);
            ConfigureUsers(modelBuilder);
        }

        private void ConfigureUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasMany(u => u.Bets).WithOne(b => b.User);

                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();

                entity.Property(u => u.Balance).HasColumnType("MONEY");
            });
        }

        private void ConfigureBets(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(b => b.BetId);

                entity.Property(b => b.Prediction).IsRequired();

                entity.Property(b => b.Amount).HasColumnType("MONEY").IsRequired();
            });
        }

        private void ConfigureGames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.GameId);

                entity.HasMany(g => g.PlayerStatistics).WithOne(ps => ps.Game);

                entity.HasMany(g => g.Bets).WithOne(b => b.Game);
            });

        }

        private void ConfigurePlayerStatistics(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>(entity =>
           {
               entity.HasKey(e => new { e.PlayerId, e.GameId });

               entity.HasOne(e => e.Game).WithMany(g => g.PlayerStatistics).HasForeignKey(e => e.GameId);
           });
        }

        private void ConfigurePositions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);

                entity.HasMany(p => p.Players).WithOne(p => p.Position);

                entity.Property(p => p.Name).IsRequired().HasMaxLength(30);
            });
        }

        private void ConfigurePlayers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.Property(p => p.IsInjured).HasDefaultValue(false);

                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            });
        }

        private void ConfigureColors(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);
                entity.HasMany(c => c.PrimaryKitTeams).WithOne(t => t.PrimaryKitColor).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(c => c.SecondaryKitTeams).WithOne(t => t.SecondaryKitColor).OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureTowns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity.HasMany(t => t.Teams).WithOne(tm => tm.Town);
            });
        }

        private void ConfigureCountries(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(c => c.CountryId);

                entity.HasMany(c => c.Towns).WithOne(t => t.Country);
            });
        }

        private void ConfigureTeams(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.TeamId);

                entity.HasMany(t => t.HomeGames).WithOne(hg => hg.HomeTeam).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(t => t.AwayGames).WithOne(ag => ag.AwayTeam).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(t => t.Players).WithOne(p => p.Team);
            });
        }
    }
}
