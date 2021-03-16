using ProductShop.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var text = File.ReadAllText("../../../Datasets/users.xml");
            var xml = XDocument.Parse(text);
            System.Console.WriteLine(ImportUsers(context, text));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlDoc = XDocument.Parse(inputXml);

            return $"Successfully imported {xmlDoc.Root.Elements().Count()}";
        }
    }
}