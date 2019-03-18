namespace BillsPaymentSystem.Data.EntityConfigurations
{
    using BillsPaymentSystem.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PaymentMethodConfig : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.HasOne(pm => pm.User).WithMany(pm => pm.PaymentMethods);

            builder.HasOne(pm => pm.BankAccount)
                .WithOne(ba => ba.PaymentMethod)
                .HasForeignKey<BankAccount>(pm => pm.BankAccountId);

            builder.HasOne(pm => pm.CreditCard)
                .WithOne(cc => cc.PaymentMethod)
                .HasForeignKey<CreditCard>(pm => pm.CreditCardId);
        }
    }
}
