namespace BillsPaymentSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CreditCard
    {
        public int CreditCardId { get; set; }

        [Range(typeof(decimal), "0.00", "5000000")]
        public decimal Limit { get; set; }
        [Range(typeof(decimal), "0.00", "50000000")]
        public decimal MoneyOwed { get; set; }

        public DateTime ExpirationDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal LimitLeft => this.Limit - this.MoneyOwed;
    }
}
