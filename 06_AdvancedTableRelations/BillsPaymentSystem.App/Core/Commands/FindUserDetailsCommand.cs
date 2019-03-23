using BillsPaymentSystem.Data;
using System.Linq;
using System;
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace BillsPaymentSystem.App.Core.Commands
{
    public class FindUserDetailsCommand : Command
    {


        public FindUserDetailsCommand(string[] data, BillsPaymentSystemContext context) : base(data, context)
        {

        }

        public override void Execute()
        {
            int id = int.Parse(Data[1]);

            var user = Context.Users.Find(id);

            if (user == null)
            {
                Console.WriteLine($"User with {id} doesn't exist!");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"User: {user.FirstName} {user.LastName}");

            var bankAccounts = Context.BankAccounts.Where(p => p.PaymentMethod.UserId == id).ToList();

            if (bankAccounts.Count > 0)
            {
                sb.AppendLine("Bank Accounts:");

                foreach (var account in bankAccounts.OrderBy(a => a.BankAccountId))
                {

                    sb.AppendLine($"-- ID: {account.BankAccountId}");
                    sb.AppendLine($"--- Balance: {account.Balance:F2}");
                    sb.AppendLine($"--- Bank: {account.BankName}");
                    sb.AppendLine($"--- SWIFT: {account.SWIFT}");

                }
            }


            var creditCards = Context.CreditCards.Where(p => p.PaymentMethod.UserId == id).ToList();

            if (creditCards.Count > 0)
            {
                sb.AppendLine("Credit Cards:");

                foreach (var cc in creditCards.OrderBy(cc => cc.CreditCardId))
                {
                    var expirationDate = cc.ExpirationDate;
                    sb.AppendLine($"-- ID: {cc.CreditCardId}");
                    sb.AppendLine($"--- Limit: {cc.Limit:F2}");
                    sb.AppendLine($"--- Money Owed: {cc.MoneyOwed:F2}");
                    sb.AppendLine($"--- Limit Left: {cc.LimitLeft:F2}");
                    sb.AppendLine($"--- Expiration Date: {expirationDate.ToString("yyyy-MM", CultureInfo.InvariantCulture)}");
                }
            }


            Console.WriteLine(sb.ToString().TrimEnd());

        }
    }
}
