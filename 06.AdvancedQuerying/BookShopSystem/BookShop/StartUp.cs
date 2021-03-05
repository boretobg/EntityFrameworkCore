namespace BookShop
{
    using Data;
    using Initializer;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
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
