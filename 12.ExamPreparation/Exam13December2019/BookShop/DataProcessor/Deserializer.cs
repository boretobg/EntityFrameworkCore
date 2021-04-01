namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(BookXmlImportModel[]), new XmlRootAttribute("Books"));

            var bookList = new List<Book>();

            using (var reader = new StringReader(xmlString))
            {
                var xmlBooks = (BookXmlImportModel[])xmlSerializer.Deserialize(reader);

                foreach (var xmlBook in xmlBooks)
                {
                    if (!IsValid(xmlBook))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    var isValidDate = DateTime.TryParseExact(
                        xmlBook.PublishedOn,
                        "MM/dd/yyyy"
                        ,CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime publishedOn
                        );

                    if (!isValidDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Book book = new Book()
                    {
                        Genre = (Genre)xmlBook.Genre,
                        Name = xmlBook.Name,
                        Pages = xmlBook.Pages,
                        Price = xmlBook.Price,
                        PublishedOn = publishedOn
                    };

                    bookList.Add(book);
                    output.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
                }
            }

            context.Books.AddRange(bookList);
            context.SaveChanges();

            return output.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}