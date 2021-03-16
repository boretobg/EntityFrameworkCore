using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
           // context.Database.EnsureCreated();

            var text = File.ReadAllText("../../../Datasets/products.xml");
            var xml = XDocument.Parse(text);
            System.Console.WriteLine(ImportProducts(context, text));
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(ProductInputModel[]), new XmlRootAttribute("Products"));

            var textRead = new StringReader(inputXml);

            var productsDto = xmlSerialized.Deserialize(textRead) as ProductInputModel[];

            var products = productsDto.Select(x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                SellerId = x.SellerId,
                BuyerId = x.BuyerId
            })
                .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var users = XDocument.Parse(inputXml);

            return $"Successfully imported {users.Root.Elements().Count()}";
        }
    }
}