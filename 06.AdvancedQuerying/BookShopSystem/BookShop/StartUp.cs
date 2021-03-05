namespace BookShop
{
    using BookShop.Models;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.Split(" ").Select(x => x.ToLower()).ToArray();
            var titles = new List<string>();


            var temp = context.Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
                .ToList()
                .Where(x => x.BookCategories.Any(x => categories.Contains(x.Category.Name.ToLower())))
                .Select(x => x.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, temp);

            return result;
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .OrderBy(b => b.BookId)
                .Where(b => b.ReleaseDate.Value.Year != year)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var titlePrices = context.Books
                .OrderByDescending(b => b.Price)
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var titlePrice in titlePrices)
            {
                sb.AppendLine($"{titlePrice.Title} - ${titlePrice.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var titles = context.Books
                .OrderBy(b => b.BookId)
                .Where(b => b.EditionType.ToString() == "Gold")
                .Where(b => b.Copies < 5000)
                .Select(b => new { Title = b.Title })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var title in titles)
            {
                sb.AppendLine(title.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var bookTitles = context.Books
                .OrderBy(x => x.Title)
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => new { Title = x.Title })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var tittle in bookTitles)
            {
                sb.AppendLine(tittle.Title);
            }

            return sb.ToString().TrimEnd();
        }
    }
}
