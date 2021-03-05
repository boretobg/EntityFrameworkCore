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

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var bookTitles = context.Books
                .OrderBy(x => x.Title)
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => new { Title = x.Title})
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
