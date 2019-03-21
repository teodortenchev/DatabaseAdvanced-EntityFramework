using System;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExpirationCheckAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime expirationDate = (DateTime)value;
            DateTime currentDate = DateTime.Now;

            if (expirationDate > currentDate)
            {
                return ValidationResult.Success;
            }

            string errorMessage = "Your card is expired.";

            return new ValidationResult(errorMessage);

        }
    }
}
