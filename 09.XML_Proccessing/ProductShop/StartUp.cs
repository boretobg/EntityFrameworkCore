﻿using ProductShop.Data;
using ProductShop.Dtos.Export;
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
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            System.Console.WriteLine(GetProductsInRange(context));
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(p => new ProductOutputModel
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ProductOutputModel[]), new XmlRootAttribute("Products"));

            var textWriter = new StringWriter();

            var nameSpace = new XmlSerializerNamespaces();
            nameSpace.Add("", "");

            xmlSerializer.Serialize(textWriter, products, nameSpace);

            return textWriter.ToString();
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(CategoryProductInputModel[]), new XmlRootAttribute("CategoryProducts"));

            var textRead = new StringReader(inputXml);

            var categoryProductsDto = xmlSerialized.Deserialize(textRead) as CategoryProductInputModel[];

            var categoriesProducts = categoryProductsDto
                .Where(x => x.CategoryId != null || x.ProductId != null)
                .Select(x => new CategoryProduct
                {
                    CategoryId = (int)x.CategoryId,
                    ProductId = (int)x.ProductId
                })
                .ToList();

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerialized = new XmlSerializer(typeof(CategoryInputModel[]), new XmlRootAttribute("Categories"));

            var textRead = new StringReader(inputXml);

            var categoriesDto = xmlSerialized.Deserialize(textRead) as CategoryInputModel[];

            var categories = categoriesDto
                .Where(x => x.Name != null)
                .Select(x => new Category
                {
                    Name = x.Name
                })
                .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
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