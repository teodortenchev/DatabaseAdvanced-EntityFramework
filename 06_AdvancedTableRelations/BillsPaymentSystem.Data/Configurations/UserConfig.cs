using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillsPaymentSystem.Data.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            builder.Property(u => u.Email)
                .HasMaxLength(80)
                .IsUnicode(false)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasMaxLength(25)
                .IsUnicode()
                .IsRequired();
        }
    }
}
