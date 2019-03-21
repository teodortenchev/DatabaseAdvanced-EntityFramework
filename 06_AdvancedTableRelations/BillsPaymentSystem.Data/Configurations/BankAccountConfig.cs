using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillsPaymentSystem.Data.Configurations
{
    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            

            builder.Property(ba => ba.Balance).HasColumnType("MONEY");

            builder.Property(ba => ba.BankName)
                .HasMaxLength(50)
                .IsUnicode()
                .IsRequired();

            builder.Property(ba => ba.SWIFT)
                .HasMaxLength(20)
                .HasColumnName("SwiftCode")
                .IsUnicode(false)
                .IsRequired();
        }
    }
}
