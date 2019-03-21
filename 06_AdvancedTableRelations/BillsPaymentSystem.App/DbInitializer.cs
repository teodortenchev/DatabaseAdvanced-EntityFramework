using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.App
{
    public class DBInitializer
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            //SeedUsers(context);
            SeedCreditCards(context);
        }

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            decimal[] limits = { 5000000, 5000001, -1, 500, 250, 0 };
            decimal[] moneyowed = { 1, 2, 3, 4, 5, 6 };
            DateTime[] expirationDates = { DateTime.Now.AddDays(50), DateTime.Now.AddDays(500), DateTime.Now.AddDays(-500), DateTime.Now.AddDays(5), DateTime.Now.AddDays(43), DateTime.Now.AddDays(36436) };

            var creditCards = new List<CreditCard>();

            for (int i = 0; i < limits.Length; i++)
            {
                var creditCard = new CreditCard
                {
                    Limit = limits[i],
                    MoneyOwed = moneyowed[i],
                    ExpirationDate = expirationDates[i]
                };

                if (!IsValid(creditCard))
                {
                    continue;
                }

                creditCards.Add(creditCard);

            }

            context.AddRange(creditCards);
            context.SaveChanges();

        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = { "Goshko", "Mancho", "Grozdan", "a", "ERROR" };
            string[] lastNames = { "Goshkov", "Petrov", "Pesho", null, "ERROR" };
            string[] emails = { "genadi@abv.bg", "petio@gmail.com", "giri@ggaz.com", "p.com", "ERROR" };
            string[] passwords = { "password", "parolatammi123", "1112s1", null, "ERROR" };

            List<User> users = new List<User>();

            for (int i = 0; i < firstNames.Length; i++)
            {
                var user = new User
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i],
                    Password = passwords[i]
                };

                if (!IsValid(user))
                {
                    continue;
                }

                users.Add(user);
            }

            context.AddRange(users);
            context.SaveChanges();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}
