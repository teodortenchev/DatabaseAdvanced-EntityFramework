namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                // DbInitializer.ResetDatabase(db);
                Console.WriteLine(GetGoldenBooks(db));
            }
        }

        //Task 1
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            List<string> books = context.Books.
                Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(x => x.Title)
                .Select(b => b.Title).ToList();

            string result = String.Join(Environment.NewLine, books);

            return result;
        }

        //Task 2
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            string result = String.Join(Environment.NewLine, goldenBooks);

            return result;
        }
    }
}
