namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                // DbInitializer.ResetDatabase(db);
                Console.WriteLine(GetBooksByAuthor(db, "po"));
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

        //Task 3
        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksOver40 = context.Books
                .Where(b => b.Price > 40).OrderByDescending(b => b.Price)
                .Select(x => new Book
                {
                    Title = x.Title,
                    Price = x.Price
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var book in booksOver40)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Task 4
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var booksNotReleasedInYear = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            string result = String.Join(Environment.NewLine, booksNotReleasedInYear);

            return result;
        }

        //Task 5
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();

            var booksWithCategories = context.Books
                .Where(b => b.BookCategories.Any(bk => categories.Contains(bk.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            string result = String.Join(Environment.NewLine, booksWithCategories);

            return result;

        }

        //Task 6
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime lookupDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var booksBeforeDate = context.Books.Where(b => b.ReleaseDate < lookupDate).OrderByDescending(b => b.ReleaseDate);

            string result = String.Join(Environment.NewLine, booksBeforeDate.Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}"));

            return result;
        }

        //Task 7
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authorsEndingIn = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = String.Concat(a.FirstName, " ", a.LastName)
                })
                .Select(a => a.FullName)
                .OrderBy(a => a)
                .ToList();

            string result = String.Join(Environment.NewLine, authorsEndingIn);

            return result;
        }

        /// <summary>
        /// Task 8. Return the titles of book, which contain a given string. Ignore casing.
        ///Return all titles in a single string, each on a new row, ordered alphabetically.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string comparisonString = input.ToLower();

            var bookTitlesContainingX = context.Books
                .Where(b => b.Title.ToLower().Contains(comparisonString))
                .Select(b => b.Title).OrderBy(b => b);

            string result = String.Join(Environment.NewLine, bookTitlesContainingX);

            return result;
        }

        /// <summary>
        /// Task 9. Return all titles of books and their authors’ names for books, which are written by authors whose last names start with the 
        /// given string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            string nameStart = input.ToLower();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(nameStart))
                .OrderBy(b => b.BookId)
                .Select(b => new { Title = b.Title, AuthorName = b.Author.FirstName + " " + b.Author.LastName })
                .ToList();

            string result = String.Join(Environment.NewLine, books.Select(b => $"{b.Title} ({b.AuthorName})"));

            return result;
        }
    }
}
