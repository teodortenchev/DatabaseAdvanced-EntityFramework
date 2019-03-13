namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Models;

    public class SalesContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCustomerEntity(modelBuilder);
            ConfigureProductEntity(modelBuilder);
            ConfigureSaleEntity(modelBuilder);
            ConfigureStoreEntity(modelBuilder);
        }

        private void ConfigureProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(product =>
            {
                product.HasKey(p => p.ProductId);

                product.HasMany(p => p.Sales).WithOne(s => s.Product);

                product.Property(p => p.Name).IsRequired().HasMaxLength(50).IsUnicode();
            });
        }

        private void ConfigureCustomerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(customer =>
            {
                customer.HasKey(c => c.CustomerId);

                customer.HasMany(c => c.Sales).WithOne(s => s.Customer);

                customer.Property(c => c.Name).IsRequired().HasMaxLength(100).IsUnicode();

                customer.Property(c => c.Email).IsRequired().HasMaxLength(80);
            });
        }

        private void ConfigureSaleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(sale =>
            {
                sale.HasKey(s => s.SaleId);
            });
        }

        private void ConfigureStoreEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>(store =>
            {
                store.HasKey(st => st.StoreId);

                store.HasMany(st => st.Sales).WithOne(s => s.Store);

                store.Property(st => st.Name).IsRequired().HasMaxLength(80).IsUnicode();
            });
        }

    }
}
